using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files
{
    internal class FilesService : IFilesService
    {
        private readonly ILogger<FilesService> log;
        private readonly IFilesDbs db;

        public FilesService(IFilesDbs db, ILogger<FilesService> log)
        {
            ArgumentNullException.ThrowIfNull(db);

            this.db = db;
            this.log = log;
        }

        public FileDbo Create(string code, string? format, long? size)
        {
            code = Code.NormalizeAndValidate(code);
            db.Create(new FileDbo
            {
                Code = code,
                Format = format,
                Size = size,
            });

            var file = db.GetByCode(code);

            log.Log(LogLevel.Information, $"created file {file}");
            return file;
        }

        public FileDbo AddLink(FileDbo file, FileDbo.LinkDbo link)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(link);
            ArgumentNullException.ThrowIfNull(link.Path);
            ArgumentNullException.ThrowIfNull(link.Type);

            var update = new FileDbUpdate { AddLink = link };
            db.Update(file.Code, update);
            file = db.GetByCode(file.Code);
            log.Log(LogLevel.Information, $"file {file} link added: {link}");
            return file;
        }

        public List<FileDbo> Find(string? search = null)
        {
            return db.Find(new FilesDbQuery { Search = search });
        }

        public FileDbo? FindByCode(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            return db.Find(new FilesDbQuery { Code = code }).FirstOrDefault();
        }

        public void Delete(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            db.Delete(file.Code);

            log.Log(LogLevel.Information, $"deletd file '{file}'");
        }
    }
}
