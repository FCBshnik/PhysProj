﻿using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db.Authors;
using Phys.Lib.Postgres.Utils;
using SqlKata;
using System.Text.RegularExpressions;

namespace Phys.Lib.Postgres.Authors
{
    internal class AuthorsDb : PostgresTable, IAuthorsDb
    {
        private readonly NpgsqlDataSource dataSource;
        private readonly AuthorsInfosTable authorsInfos;

        public AuthorsDb(NpgsqlDataSource dataSource, string tableName, AuthorsInfosTable authorsInfos, ILogger<AuthorsDb> logger) : base(tableName, logger)
        {
            this.dataSource = dataSource;
            this.authorsInfos = authorsInfos;
        }

        public void Create(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using (var cnx = dataSource.OpenConnection())
                Insert(cnx, new AuthorInsertModel { Code = code });
        }

        public void Delete(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using var cnx = dataSource.OpenConnection();
            using var trx = cnx.BeginTransaction();
            authorsInfos.Delete(cnx, q => q.Where(AuthorModel.InfoModel.AuthorCodeColumn, code));
            Delete(cnx, q => q.Where(AuthorModel.CodeColumn, code));
            trx.Commit();
        }

        public List<AuthorDbo> Find(AuthorsDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var cmd = new Query(tableName)
                .LeftJoin(authorsInfos.TableName, AuthorModel.CodeColumn, AuthorModel.InfoModel.AuthorCodeColumn)
                .Limit(query.Limit);

            if (query.Code != null)
                cmd = cmd.Where(AuthorModel.CodeColumn, query.Code);
            if (query.Codes != null)
                cmd = cmd.WhereIn(AuthorModel.CodeColumn, query.Codes);
            if (query.Search != null)
                cmd = cmd.WhereRaw($"{AuthorModel.CodeColumn} ~* \'{Regex.Escape(query.Search)}\'");

            var authors = new Dictionary<string, AuthorModel>();
            using var cnx = dataSource.OpenConnection();
            FindJoin<AuthorModel, AuthorModel.InfoModel>(cnx, cmd, AuthorModel.InfoModel.AuthorCodeColumn, (a, i) =>
            {
                authors.TryAdd(a.Code, a);
                var author = authors[a.Code];

                if (i != null)
                    author.Infos.Add(i);
                return author;
            }).ToList();

            return authors.Values.Select(AuthorMapper.Map).ToList();
        }

        public void Update(string code, AuthorDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(update);

            using var cnx = dataSource.OpenConnection();
            using (var trx = cnx.BeginTransaction())
            {
                var updateDic = new Dictionary<string, object?>();

                if (update.Born.HasValue())
                    updateDic[AuthorModel.BornColumn] = update.Born;
                else if (update.Born.IsEmpty())
                    updateDic[AuthorModel.BornColumn] = null;
                if (update.Died.HasValue())
                    updateDic[AuthorModel.DiedColumn] = update.Died;
                else if (update.Died.IsEmpty())
                    updateDic[AuthorModel.DiedColumn] = null;

                if (updateDic.Any())
                {
                    var updateCmd = new Query(tableName)
                        .Where(AuthorModel.CodeColumn, code)
                        .AsUpdate(updateDic);

                    var res = Execute(cnx, updateCmd);
                    if (res == 0)
                        throw new ApplicationException($"author '{code}' update failed");
                }

                if (update.DeleteInfo != null)
                    authorsInfos.Delete(cnx, q => q.Where(AuthorModel.InfoModel.AuthorCodeColumn, code)
                        .Where(AuthorModel.InfoModel.LanguageColumn, update.DeleteInfo));
                if (update.AddInfo != null)
                    authorsInfos.Insert(cnx, new AuthorModel.InfoModel
                    {
                        AuthorCode = code,
                        Language = update.AddInfo.Language,
                        FullName = update.AddInfo.FullName,
                        Description = update.AddInfo.Description
                    });

                trx.Commit();
            }
        }
    }
}