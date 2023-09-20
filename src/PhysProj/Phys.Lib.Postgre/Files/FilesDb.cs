using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db.Files;
using SqlKata;
using System.Text.RegularExpressions;

namespace Phys.Lib.Postgres.Files
{
    internal class FilesDb : PostgresTable, IFilesDb
    {
        private readonly NpgsqlDataSource dataSource;
        private readonly FilesLinksTable filesLinks;

        public string Name => "postres";

        public FilesDb(string tableName, NpgsqlDataSource dataSource, FilesLinksTable filesLinks, ILogger<FilesDb> logger) : base(tableName, logger)
        {
            this.dataSource = dataSource;
            this.filesLinks = filesLinks;
        }

        public void Create(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(file.Code);

            var insert = new FileModel { Code = file.Code, Format = file.Format, Size = file.Size };
            using var cnx = dataSource.OpenConnection();
            Insert(cnx, insert);
        }

        public void Delete(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            using var cnx = dataSource.OpenConnection();
            using var trx = cnx.BeginTransaction();
            filesLinks.Delete(cnx, q => q.Where(FileModel.LinkModel.FileCodeColumn, code));
            Delete(cnx, q => q.Where(FileModel.CodeColumn, code));
            trx.Commit();
        }

        public List<FileDbo> Find(FilesDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var cmd = new Query(tableName)
                .LeftJoin(filesLinks.TableName, FileModel.CodeColumn, FileModel.LinkModel.FileCodeColumn)
                .Limit(query.Limit);

            if (query.Code != null)
                cmd = cmd.Where(FileModel.CodeColumn, query.Code);
            if (query.Search != null)
                cmd = cmd.WhereRaw($"{FileModel.CodeColumn} ~* \'{Regex.Escape(query.Search)}\'");

            var files = new Dictionary<string, FileModel>();
            using var cnx = dataSource.OpenConnection();
            FindJoin<FileModel, FileModel.LinkModel>(cnx, cmd, FileModel.LinkModel.FileCodeColumn, (f, l) =>
            {
                files.TryAdd(f.Code, f);
                var file = files[f.Code];

                if (l != null)
                    file.Links.Add(l);
                return file;
            }).ToList();

            return files.Values.Select(FileMapper.Map).ToList();
        }

        public void Update(string code, FileDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(update);

            using var cnx = dataSource.OpenConnection();
            using (var trx = cnx.BeginTransaction())
            {
                if (update.DeleteLink != null)
                    filesLinks.Delete(cnx, q => q.Where(FileModel.LinkModel.FileCodeColumn, code)
                        .Where(FileModel.LinkModel.PathColumn, update.DeleteLink.Path)
                        .Where(FileModel.LinkModel.PathColumn, update.DeleteLink.Path));

                if (update.AddLink != null)
                    filesLinks.Insert(cnx, new FileModel.LinkModel
                    {
                        FileCode = code,
                        Type = update.AddLink.Type,
                        Path = update.AddLink.Path,
                    });

                trx.Commit();
            }
        }
    }
}
