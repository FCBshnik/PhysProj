﻿using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db;
using Phys.Lib.Db.Works;
using Phys.Lib.Postgres.Utils;
using SqlKata;
using System.Text.RegularExpressions;

namespace Phys.Lib.Postgres.Works
{
    internal class WorksDb : PostgresTable, IWorksDb
    {
        private readonly Lazy<NpgsqlDataSource> dataSource;
        private readonly WorksInfosTable worksInfos;
        private readonly WorksAuthorsTable worksAuthors;
        private readonly WorksSubWorksTable worksSubWorks;
        private readonly WorksSubWorksAuthorsTable worksSubWorksAuthors;
        private readonly WorksFilesTable worksFiles;

        public string Name => "postgres";

        public WorksDb(string tableName, Lazy<NpgsqlDataSource> dataSource,
            WorksAuthorsTable worksAuthors, WorksSubWorksTable worksSubWorks, WorksFilesTable worksFiles, WorksInfosTable worksInfos, ILogger<WorksDb> logger, WorksSubWorksAuthorsTable worksSubWorksAuthors)
            : base(tableName, logger)
        {
            this.dataSource = dataSource;
            this.worksAuthors = worksAuthors;
            this.worksSubWorks = worksSubWorks;
            this.worksFiles = worksFiles;
            this.worksInfos = worksInfos;
            this.worksSubWorksAuthors = worksSubWorksAuthors;
        }

        public void Create(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using var cnx = dataSource.Value.OpenConnection();
            Insert(cnx, new WorkInsertModel { Code = code });
        }

        public void Delete(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using var cnx = dataSource.Value.OpenConnection();
            using var trx = cnx.BeginTransaction();
            worksInfos.Delete(cnx, q => q.Where(WorkModel.InfoModel.WorkCodeColumn, code));
            worksAuthors.Delete(cnx, q => q.Where(WorkModel.AuthorModel.WorkCodeColumn, code));
            worksSubWorks.Delete(cnx, q => q.Where(WorkModel.SubWorkModel.WorkCodeColumn, code));
            worksSubWorksAuthors.Delete(cnx, q => q.Where(WorkModel.AuthorModel.WorkCodeColumn, code));
            worksFiles.Delete(cnx, q => q.Where(WorkModel.FileModel.WorkCodeColumn, code));
            Delete(cnx, q => q.Where(WorkModel.CodeColumn, code));
            trx.Commit();
        }

        public List<WorkDbo> Find(WorksDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            return Find(q =>
            {
                if (query.Code != null)
                    q = q.Where(tableName + "." + WorkModel.CodeColumn, query.Code);
                if (query.Codes != null)
                    q = q.WhereIn(WorkModel.CodeColumn, query.Codes);
                if (query.AuthorCode != null)
                    q = q.Where(worksAuthors.TableName + "." + WorkModel.AuthorModel.AuthorCodeColumn, query.AuthorCode);
                if (query.SubWorkCode != null)
                    q = q.Where(worksSubWorks.TableName + "." + WorkModel.SubWorkModel.SubWorkCodeColumn, query.SubWorkCode);
                if (query.FileCode != null)
                    q = q.Where(worksFiles.TableName + "." + WorkModel.FileModel.FileCodeColumn, query.FileCode);
                if (query.Search != null)
                    q = q.WhereRaw($"{WorkModel.CodeColumn} ~* \'{Regex.Escape(query.Search)}\'");
                return q.OrderByDesc(tableName + "." + WorkModel.IdColumn);
            }, query.Limit).Select(WorksMapper.Map).ToList();
        }

        public void Update(string code, WorkDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(update);

            using var cnx = dataSource.Value.OpenConnection();
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

                if (update.IsPublic.HasValue)
                    updateDic[WorkModel.IsPublicColumn] = update.IsPublic.Value;

                if (updateDic.Any())
                {
                    var updateCmd = new Query(tableName)
                        .Where(WorkModel.CodeColumn, code)
                        .AsUpdate(updateDic);

                    var res = Execute(cnx, updateCmd);
                    if (res == 0)
                        throw new PhysDbException($"work '{code}' update failed");
                }

                if (update.DeleteInfo != null)
                    worksInfos.Delete(cnx, q => q.Where(WorkModel.InfoModel.WorkCodeColumn, code)
                        .Where(WorkModel.InfoModel.InfoLanguageColumn, update.DeleteInfo));
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

                if (update.DeleteSubWorkAuthor != null)
                    worksSubWorksAuthors.Delete(cnx, q => q.Where(WorkModel.AuthorModel.WorkCodeColumn, code)
                        .Where(WorkModel.AuthorModel.AuthorCodeColumn, update.DeleteSubWorkAuthor));
                if (update.AddSubWorkAuthor != null)
                    worksSubWorksAuthors.Insert(cnx, new WorkModel.AuthorModel
                    {
                        WorkCode = code,
                        AuthorCode = update.AddSubWorkAuthor
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

        private List<WorkModel> Find(Func<Query, Query> enrichQuery, int limit)
        {
            var cmd = new Query(tableName)
                .LeftJoin(worksInfos.TableName, TableName + "." + WorkModel.CodeColumn, worksInfos.TableName + "." + WorkModel.InfoModel.WorkCodeColumn)
                .LeftJoin(worksAuthors.TableName, TableName + "." + WorkModel.CodeColumn, worksAuthors.TableName + "." + WorkModel.AuthorModel.WorkCodeColumn)
                .LeftJoin(worksSubWorks.TableName, TableName + "." + WorkModel.CodeColumn, worksSubWorks.TableName + "." + WorkModel.SubWorkModel.WorkCodeColumn)
                .LeftJoin(worksFiles.TableName, TableName + "." + WorkModel.CodeColumn, worksFiles.TableName + "." + WorkModel.FileModel.WorkCodeColumn)
                .LeftJoin(worksSubWorksAuthors.TableName, TableName + "." + WorkModel.CodeColumn, worksSubWorksAuthors.TableName + "." + WorkModel.AuthorModel.WorkCodeColumn)
                .Limit(limit);

            enrichQuery(cmd);

            using var cnx = dataSource.Value.OpenConnection();
            var sql = compiler.Compile(cmd);
            var works = new Dictionary<string, WorkModel>();

            cnx.Query<WorkModel, WorkModel.InfoModel, WorkModel.AuthorModel, WorkModel.SubWorkModel, WorkModel.FileModel, WorkModel.AuthorModel, WorkModel>(sql.Sql, (w, i, a, s, f, swa) =>
            {
                works.TryAdd(w.Code, w);
                var work = works[w.Code];

                if (i != null)
                    work.Infos.TryAdd(i.Language, i);
                if (a != null)
                    work.Authors.TryAdd(a.AuthorCode, a);
                if (s != null)
                    work.SubWorks.TryAdd(s.SubWorkCode, s);
                if (f != null)
                    work.Files.TryAdd(f.FileCode, f);
                if (swa != null)
                    work.SubWorksAuthors.TryAdd(swa.AuthorCode, swa);

                return work;
            }, sql.NamedBindings, splitOn: WorkModel.InfoModel.WorkCodeColumn).ToList();

            return works.Values.ToList();
        }

        IEnumerable<List<WorkDbo>> IDbReader<WorkDbo>.Read(int limit)
        {
            string? cursor = null;
            List<WorkModel>? works = null;

            do
            {
                works = Find(q =>
                {
                    if (cursor != null)
                        q = q.Where(WorkModel.IdColumn, ">", int.Parse(cursor));
                    return q.OrderBy(WorkModel.IdColumn);
                }, limit);
                cursor = works.LastOrDefault()?.Id.ToString();
                yield return works.Select(WorksMapper.Map).ToList();
            } while (works.Count >= limit);
        }
    }
}
