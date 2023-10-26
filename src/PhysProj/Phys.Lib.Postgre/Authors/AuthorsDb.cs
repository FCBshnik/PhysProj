using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Postgres.Utils;
using SqlKata;
using System.Text.RegularExpressions;

namespace Phys.Lib.Postgres.Authors
{
    internal class AuthorsDb : PostgresTable, IAuthorsDb, IDbReader<AuthorDbo>
    {
        private readonly Lazy<NpgsqlDataSource> dataSource;
        private readonly AuthorsInfosTable authorsInfos;

        public AuthorsDb(Lazy<NpgsqlDataSource> dataSource, string tableName, AuthorsInfosTable authorsInfos, ILogger<AuthorsDb> logger) : base(tableName, logger)
        {
            this.dataSource = dataSource;
            this.authorsInfos = authorsInfos;
        }

        public string Name => "postgres";

        public void Create(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using (var cnx = dataSource.Value.OpenConnection())
                Insert(cnx, new AuthorInsertModel { Code = code });
        }

        public void Delete(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using var cnx = dataSource.Value.OpenConnection();
            using var trx = cnx.BeginTransaction();
            authorsInfos.Delete(cnx, q => q.Where(AuthorModel.InfoModel.AuthorCodeColumn, code));
            Delete(cnx, q => q.Where(AuthorModel.CodeColumn, code));
            trx.Commit();
        }

        public List<AuthorDbo> Find(AuthorsDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            return Find(q =>
            {
                if (query.Code != null)
                    q = q.Where(AuthorModel.CodeColumn, query.Code);
                if (query.Codes != null)
                    q = q.WhereIn(AuthorModel.CodeColumn, query.Codes);
                if (query.Search != null)
                    q = q.WhereRaw($"{AuthorModel.CodeColumn} ~* \'{Regex.Escape(query.Search)}\'");
                return q;
            }, query.Limit).Select(AuthorMapper.Map).ToList();
        }

        public void Update(string code, AuthorDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(update);

            using var cnx = dataSource.Value.OpenConnection();
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
                        throw new PhysDbException($"author '{code}' update failed");
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

        private List<AuthorModel> Find(Func<Query, Query> enrichQuery, int limit)
        {
            var cmd = new Query(tableName)
                .LeftJoin(authorsInfos.TableName, AuthorModel.CodeColumn, AuthorModel.InfoModel.AuthorCodeColumn)
                .Limit(limit);

            enrichQuery(cmd);

            var authors = new Dictionary<string, AuthorModel>();
            using var cnx = dataSource.Value.OpenConnection();
            FindJoin<AuthorModel, AuthorModel.InfoModel>(cnx, cmd, AuthorModel.InfoModel.AuthorCodeColumn, (a, i) =>
            {
                authors.TryAdd(a.Code, a);
                var author = authors[a.Code];

                if (i != null)
                    author.Infos.TryAdd(i.Language, i);
                return author;
            }).ToList();

            return authors.Values.ToList();
        }

        IDbReaderResult<AuthorDbo> IDbReader<AuthorDbo>.Read(DbReaderQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var authors = Find(q =>
            {
                q = q.OrderBy(AuthorModel.IdColumn);
                if (query.Cursor != null)
                    q = q.Where(AuthorModel.IdColumn, ">", int.Parse(query.Cursor));
                return q;
            }, query.Limit);
            return new DbReaderResult<AuthorDbo>(authors.Select(AuthorMapper.Map).ToList(), authors.LastOrDefault()?.Id.ToString());
        }
    }
}
