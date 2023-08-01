using FluentValidation;
using NLog;
using Phys.Lib.Files;
using Phys.Lib.Files.Local;

namespace Phys.Lib.Core.Files.Storage
{
    internal class FileStoragesService : IFileStoragesService
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<FileStorageDbo, IFileStorage> storages = new Dictionary<FileStorageDbo, IFileStorage>
        {
            [new FileStorageDbo { Id = "local", Code = "local", Name = "File system storage" }] =
                new SystemFileStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/files"))
        };

        private readonly Dictionary<string, IFileStorage> storagesMap;
        private readonly IFilesService filesService;

        public FileStoragesService(IFilesService filesService)
        {
            storagesMap = storages.ToDictionary(s => s.Key.Code, s => s.Value, StringComparer.OrdinalIgnoreCase);
            this.filesService = filesService;
        }

        public IFileStorage Get(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            if (storagesMap.TryGetValue(code, out var storage))
                return storage;

            throw new ArgumentException($"invalid storage '{code}'");
        }

        public List<FileStorageDbo> List()
        {
            return storages.Keys.ToList();
        }

        public FileDbo CreateFileFromStorage(string storageCode, string filePath)
        {
            ArgumentNullException.ThrowIfNull(storageCode);
            ArgumentNullException.ThrowIfNull(filePath);

            var storage = Get(storageCode);
            var storageFile = storage.Get(filePath);
            if (storageFile == null)
                throw ValidationError($"storage '{storageCode}' file '{filePath}' not found");

            log.Info($"creating file from storage '{storageCode}' file {storageFile}");
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
