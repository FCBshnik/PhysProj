﻿using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db.Works;
using Phys.Lib.Postgres.Utils;
using SqlKata;
using System.Text.RegularExpressions;

namespace Phys.Lib.Postgres.Works
{
    internal class WorksDb : PostgresTable, IWorksDb
    {
        private readonly NpgsqlDataSource dataSource;
        private readonly WorksInfosTable worksInfos;
        private readonly WorksAuthorsTable worksAuthors;
        private readonly WorksSubWorksTable worksSubWorks;
        private readonly WorksFilesTable worksFiles;

        public WorksDb(string tableName, NpgsqlDataSource dataSource,
            WorksAuthorsTable worksAuthors, WorksSubWorksTable worksSubWorks, WorksFilesTable worksFiles, WorksInfosTable worksInfos, ILogger<WorksDb> logger)
            : base(tableName, logger)
        {
            this.dataSource = dataSource;
            this.worksAuthors = worksAuthors;
            this.worksSubWorks = worksSubWorks;
            this.worksFiles = worksFiles;
            this.worksInfos = worksInfos;
        }

        public void Create(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using var cnx = dataSource.OpenConnection();
            Insert(cnx, new WorkInsertModel { Code = code });
        }

        public void Delete(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using var cnx = dataSource.OpenConnection();
            using var trx = cnx.BeginTransaction();
            worksInfos.Delete(cnx, q => q.Where(WorkModel.InfoModel.WorkCodeColumn, code));
            worksAuthors.Delete(cnx, q => q.Where(WorkModel.AuthorModel.WorkCodeColumn, code));
            worksSubWorks.Delete(cnx, q => q.Where(WorkModel.SubWorkModel.WorkCodeColumn, code));
            worksFiles.Delete(cnx, q => q.Where(WorkModel.FileModel.WorkCodeColumn, code));
            Delete(cnx, q => q.Where(WorkModel.CodeColumn, code));
            trx.Commit();
        }

        public List<WorkDbo> Find(WorksDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var cmd = new Query(tableName)
                .LeftJoin(worksInfos.TableName, TableName + "." + WorkModel.CodeColumn, worksInfos.TableName + "." + WorkModel.InfoModel.WorkCodeColumn)
                .LeftJoin(worksAuthors.TableName, TableName + "." + WorkModel.CodeColumn, worksAuthors.TableName + "." + WorkModel.AuthorModel.WorkCodeColumn)
                .LeftJoin(worksSubWorks.TableName, TableName + "." + WorkModel.CodeColumn, worksSubWorks.TableName + "." + WorkModel.SubWorkModel.WorkCodeColumn)
                .LeftJoin(worksFiles.TableName, TableName + "." + WorkModel.CodeColumn, worksFiles.TableName + "." + WorkModel.FileModel.WorkCodeColumn)
                .Limit(query.Limit);

            if (query.Code != null)
                cmd = cmd.Where(WorkModel.CodeColumn, query.Code);
            if (query.Search != null)
                cmd = cmd.WhereRaw($"{WorkModel.CodeColumn} ~* \'{Regex.Escape(query.Search)}\'");

            using var cnx = dataSource.OpenConnection();
            var sql = compiler.Compile(cmd);
            var works = new Dictionary<string, WorkModel>();

            cnx.Query<WorkModel, WorkModel.InfoModel, WorkModel.AuthorModel, WorkModel.SubWorkModel, WorkModel.FileModel, WorkModel>(sql.Sql, (w, i, a, s, f) =>
            {
                works.TryAdd(w.Code, w);
                var work = works[w.Code];

                if (i != null)
                    work.Infos.Add(i);
                if (a != null)
                    work.Authors.Add(a);
                if (s != null)
                    work.SubWorks.Add(s);
                if (f != null)
                    work.Files.Add(f);

                return work;
            }, sql.NamedBindings, splitOn: WorkModel.InfoModel.WorkCodeColumn).ToList();

            return works.Values.Select(WorksMapper.Map).ToList();
        }

        public void Update(string code, WorkDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(update);

            using var cnx = dataSource.OpenConnection();
            using (var trx = cnx.BeginTransaction())
            {
                var updateDic = new Dictionary<string, object?>();

                if (update.Language.HasValue())
                    updateDic[WorkModel.LanguageColumn] = update.Language;
                else if (update.Language.IsEmpty())
                    updateDic[WorkModel.LanguageColumn] = null;

                if (update.Publish.HasValue())
                    updateDic[WorkModel.PublishColumn] = update.Publish;
                else if (update.Publish.IsEmpty())
                    updateDic[WorkModel.PublishColumn] = null;

                if (update.Original.HasValue())
                    updateDic[WorkModel.OriginalCodeColumn] = update.Original;
                else if (update.Original.IsEmpty())
                    updateDic[WorkModel.OriginalCodeColumn] = null;

                if (updateDic.Any())
                {
                    var updateCmd = new Query(tableName)
                        .Where(WorkModel.CodeColumn, code)
                        .AsUpdate(updateDic);

                    var res = Execute(cnx, updateCmd);
                    if (res == 0)
                        throw new ApplicationException($"work '{code}' update failed");
                }

                if (update.DeleteInfo != null)
                    worksInfos.Delete(cnx, q => q.Where(WorkModel.InfoModel.WorkCodeColumn, code)
                        .Where(WorkModel.InfoModel.LanguageColumn, update.DeleteInfo));
                if (update.AddInfo != null)
                    worksInfos.Insert(cnx, new WorkModel.InfoModel
                    {
                        WorkCode = code,
                        Language = update.AddInfo.Language,
                        Name = update.AddInfo.Name,
                        Description = update.AddInfo.Description
                    });

                if (update.DeleteAuthor != null)
                    worksAuthors.Delete(cnx, q => q.Where(WorkModel.AuthorModel.WorkCodeColumn, code)
                        .Where(WorkModel.AuthorModel.AuthorCodeColumn, update.DeleteAuthor));
                if (update.AddAuthor != null)
                    worksAuthors.Insert(cnx, new WorkModel.AuthorModel
                    {
                        WorkCode = code,
                        AuthorCode = update.AddAuthor
                    });

                if (update.DeleteSubWork != null)
                    worksSubWorks.Delete(cnx, q => q.Where(WorkModel.SubWorkModel.WorkCodeColumn, code)
                        .Where(WorkModel.SubWorkModel.SubWorkCodeColumn, update.DeleteSubWork));
                if (update.AddSubWork != null)
                    worksSubWorks.Insert(cnx, new WorkModel.SubWorkModel
                    {
                        WorkCode = code,
                        SubWorkCode = update.AddSubWork
                    });

                if (update.DeleteFile != null)
                    worksFiles.Delete(cnx, q => q.Where(WorkModel.FileModel.WorkCodeColumn, code)
                        .Where(WorkModel.FileModel.FileCodeColumn, update.DeleteFile));
                if (update.AddFile != null)
                    worksFiles.Insert(cnx, new WorkModel.FileModel
                    {
                        WorkCode = code,
                        FileCode = update.AddFile
                    });

                trx.Commit();
            }
        }
    }
}