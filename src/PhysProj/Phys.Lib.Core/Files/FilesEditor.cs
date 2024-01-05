using FluentValidation;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Files.Storage;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Files
{
    internal class FilesEditor : IFilesEditor
    {
        private readonly ILogger<FilesEditor> log;
        private readonly IFilesDb db;
        private readonly IWorksDb worksDb;
        private readonly IFileStorages fileStorages;

        public FilesEditor(IFilesDb db, ILogger<FilesEditor> log, IWorksDb worksDb, IFileStorages fileStorages)
        {
            this.db = db;
            this.log = log;
            this.worksDb = worksDb;
            this.fileStorages = fileStorages;
        }

        public FileDbo Create(string code, long size, string? format)
        {
            code = Code.NormalizeAndValidate(code);
            db.Create(new FileDbo
            {
                Code = code,
                Format = format ?? string.Empty,
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

        public void Delete(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            var works = worksDb.Find(new WorksDbQuery { FileCode = file.Code });
            if (works.Count != 0)
                throw ValidationError($"can not delete file linked to work");

            db.Delete(file.Code);

            log.Log(LogLevel.Information, $"deleted file '{file}'");
        }

        public FileDbo CreateFileFromStorage(string storageCode, string fileId)
        {
            ArgumentNullException.ThrowIfNull(storageCode);
            ArgumentNullException.ThrowIfNull(fileId);

            var storage = fileStorages.Get(storageCode);
            var storageFile = storage.Get(fileId);
            if (storageFile == null)
                throw ValidationError($"storage '{storageCode}' file '{fileId}' not found");

            log.Log(LogLevel.Information, $"creating file from storage '{storageCode}' file {storageFile}");
            var fileName = Path.GetFileName(storageFile.Name);
            var code = Code.NormalizeAndValidate(fileName);
            var file = db.Find(new FilesDbQuery { Code = code }).FirstOrDefault();
            if (file != null)
                throw ValidationError($"file with code '{code}' already exists");

            file = Create(code, storageFile.Size, Path.GetExtension(storageFile.Name)?.Trim('.'));
            file = AddLink(file, new FileDbo.LinkDbo { StorageCode = storageCode, FileId = storageFile.Id });
            return file;
        }

        private static ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
