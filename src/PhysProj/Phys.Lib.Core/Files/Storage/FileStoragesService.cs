using FluentValidation;
using Phys.Lib.Db.Files;
using Microsoft.Extensions.Logging;
using Phys.Shared.Files;

namespace Phys.Lib.Core.Files.Storage
{
    internal class FileStoragesService : IFileStoragesService
    {
        private readonly ILogger<FileStoragesService> log;
        private readonly Dictionary<string, IFileStorage> storages;
        private readonly IFilesService filesService;

        public FileStoragesService(IFilesService filesService, IEnumerable<IFileStorage> storages, ILogger<FileStoragesService> log)
        {
            ArgumentNullException.ThrowIfNull(filesService);
            ArgumentNullException.ThrowIfNull(storages);
            ArgumentNullException.ThrowIfNull(log);

            this.filesService = filesService;
            this.storages = storages.ToDictionary(s => s.Code, s => s, StringComparer.OrdinalIgnoreCase);
            this.log = log;
        }

        public IFileStorage Get(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            if (storages.TryGetValue(code, out var storage))
                return storage;

            throw new ArgumentException($"invalid storage '{code}'");
        }

        public List<FileStorageInfo> List()
        {
            return storages.Select(s => new FileStorageInfo { Code = s.Key, Name = s.Key }).ToList();
        }

        public FileDbo CreateFileFromStorage(string storageCode, string filePath)
        {
            ArgumentNullException.ThrowIfNull(storageCode);
            ArgumentNullException.ThrowIfNull(filePath);

            var storage = Get(storageCode);
            var storageFile = storage.Get(filePath);
            if (storageFile == null)
                throw ValidationError($"storage '{storageCode}' file '{filePath}' not found");

            log.Log(LogLevel.Information, $"creating file from storage '{storageCode}' file {storageFile}");
            var fileName = Path.GetFileName(filePath);
            var code = Code.NormalizeAndValidate(fileName);
            var file = filesService.FindByCode(code);
            if (file != null)
                throw ValidationError($"file with code '{code}' already exists");

            file = filesService.Create(code, Path.GetExtension(storageFile.Path).Trim('.'), storageFile.Size);
            file = filesService.AddLink(file, new FileDbo.LinkDbo { Type = storageCode, Path = storageFile.Path });
            return file;
        }

        private static ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
