using Phys.Lib.Files;
using Phys.Lib.Files.Local;
using System.Reflection;

namespace Phys.Lib.Core.Files
{
    internal class FileStoragesService : IFileStoragesService
    {
        private readonly Dictionary<FileStorageDbo, IFileStorage> storages = new Dictionary<FileStorageDbo, IFileStorage>
        {
            [new FileStorageDbo { Id = "local", Code = "local", Name = "File system storage" }] =
                new SystemFileStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/files"))
        };

        private readonly Dictionary<string, IFileStorage> storagesMap;

        public FileStoragesService()
        {
            storagesMap = storages.ToDictionary(s => s.Key.Code, s => s.Value, StringComparer.OrdinalIgnoreCase);
        }

        public IFileStorage Get(string code)
        {
            if (storagesMap.TryGetValue(code, out var storage))
                return storage;

            throw new ArgumentException($"invalid storage '{code}'");
        }

        public List<FileStorageDbo> List()
        {
            return storages.Keys.ToList();
        }
    }
}
