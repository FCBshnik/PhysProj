using Phys.Lib.Base.Files;

namespace Phys.Lib.Core.Files.Storage
{
    public interface IFileStoragesService
    {
        List<FileStorageInfo> List();

        IFileStorage Get(string code);

        FileDbo CreateFileFromStorage(string storageCode, string filePath);
    }
}
