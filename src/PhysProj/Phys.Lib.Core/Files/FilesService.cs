using FluentValidation;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Files
{
    internal class FilesService : IFilesService
    {
        private readonly ILogger<FilesService> log;
        private readonly IFilesDb db;
        private readonly IWorksDb worksDb;

        public FilesService(IFilesDb db, ILogger<FilesService> log, IWorksDb worksDb)
        {
            this.db = db;
            this.log = log;
            this.worksDb = worksDb;
        }

        public FileDbo Create(string code, long size, string? format)
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
            ArgumentNullException.ThrowIfNull(link.FileId);
            ArgumentNullException.ThrowIfNull(link.StorageCode);

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

            var works = worksDb.Find(new WorksDbQuery { FileCode = file.Code });
            if (works.Count != 0)
                throw ValidationError($"can not delete file linked to work");

            db.Delete(file.Code);

            log.Log(LogLevel.Information, $"deleted file '{file}'");
        }

        private ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
