using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Postgres.Authors;
using SqlKata;
using System.Text.RegularExpressions;

namespace Phys.Lib.Postgres.Files
{
    internal class FilesDb : PostgresTable, IFilesDb, IDbReader<FileDbo>
    {
        private readonly Lazy<NpgsqlDataSource> dataSource;
        private readonly FilesLinksTable filesLinks;

        public string Name => "postgres";

        public FilesDb(string tableName, Lazy<NpgsqlDataSource> dataSource, FilesLinksTable filesLinks, ILogger<FilesDb> logger) : base(tableName, logger)
        {
            this.dataSource = dataSource;
            this.filesLinks = filesLinks;
        }

        public void Create(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(file.Code);

            var insert = new FileInsertModel { Code = file.Code, Format = file.Format, Size = file.Size };
            using var cnx = dataSource.Value.OpenConnection();
            Insert(cnx, insert);
        }

        public void Delete(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using var cnx = dataSource.Value.OpenConnection();
            using var trx = cnx.BeginTransaction();
            filesLinks.Delete(cnx, q => q.Where(FileModel.LinkModel.FileCodeColumn, code));
            Delete(cnx, q => q.Where(FileModel.CodeColumn, code));
            trx.Commit();
        }

        public List<FileDbo> Find(FilesDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            return Find(q =>
            {
                if (query.Code != null)
                    q = q.Where(FileModel.CodeColumn, query.Code);
                if (query.Search != null)
                    q = q.WhereRaw($"{FileModel.CodeColumn} ~* \'{Regex.Escape(query.Search)}\'");
                return q;
            }, query.Limit).Select(FileMapper.Map).ToList();
        }

        public void Update(string code, FileDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(update);

            using var cnx = dataSource.Value.OpenConnection();
            using (var trx = cnx.BeginTransaction())
            {
                if (update.DeleteLink != null)
                    filesLinks.Delete(cnx, q => q.Where(FileModel.LinkModel.FileCodeColumn, code)
                        .Where(FileModel.LinkModel.TypeColumn, update.DeleteLink.StorageCode)
                        .Where(FileModel.LinkModel.PathColumn, update.DeleteLink.FileId));

                if (update.AddLink != null)
                    filesLinks.Insert(cnx, new FileModel.LinkModel
                    {
                        FileCode = code,
                        Type = update.AddLink.StorageCode,
                        Path = update.AddLink.FileId,
                    });

                trx.Commit();
            }
        }

        private List<FileModel> Find(Func<Query, Query> enrichQuery, int limit)
        {
            var cmd = new Query(tableName)
                .LeftJoin(filesLinks.TableName, FileModel.CodeColumn, FileModel.LinkModel.FileCodeColumn)
                .Limit(limit);

            enrichQuery(cmd);

            var files = new Dictionary<string, FileModel>();
            using var cnx = dataSource.Value.OpenConnection();
            FindJoin<FileModel, FileModel.LinkModel>(cnx, cmd, FileModel.LinkModel.FileCodeColumn, (f, l) =>
            {
                files.TryAdd(f.Code, f);
                var file = files[f.Code];

                if (l != null)
                    file.Links.Add(l);
                return file;
            }).ToList();

            return files.Values.ToList();
        }

        IDbReaderResult<FileDbo> IDbReader<FileDbo>.Read(DbReaderQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var files = Find(q =>
            {
                q = q.OrderBy(FileModel.IdColumn);
                if (query.Cursor != null)
                    q = q.Where(FileModel.IdColumn, ">", int.Parse(query.Cursor));
                return q;
            }, query.Limit);
            return new DbReaderResult<FileDbo>(files.Select(FileMapper.Map).ToList(), files.LastOrDefault()?.Id.ToString());
        }
    }
}
