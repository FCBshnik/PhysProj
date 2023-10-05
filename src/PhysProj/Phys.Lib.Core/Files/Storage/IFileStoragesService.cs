using Phys.Lib.Db.Files;
using Phys.Shared.Files;

namespace Phys.Lib.Core.Files.Storage
{
    public interface IFileStoragesService
    {
        List<FileStorageInfo> List();

        IFileStorage Get(string code);

        FileDbo CreateFileFromStorage(string storageCode, string filePath);
    }
}
