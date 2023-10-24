using Microsoft.Extensions.Logging;
using Phys.Files;

namespace Phys.Lib.Core.Files.Storage
{
    internal class FileStoragesService : IFileStoragesService
    {
        private readonly ILogger<FileStoragesService> log;
        private readonly Dictionary<string, IFileStorage> storages;

        public FileStoragesService(IEnumerable<IFileStorage> storages, ILogger<FileStoragesService> log)
        {
            ArgumentNullException.ThrowIfNull(storages);
            ArgumentNullException.ThrowIfNull(log);

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
    }
}
