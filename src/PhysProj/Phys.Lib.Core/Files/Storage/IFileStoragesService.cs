using Phys.Lib.Files;

namespace Phys.Lib.Core.Files.Storage
{
    public interface IFileStoragesService
    {
        List<FileStorageDbo> List();

        IFileStorage Get(string code);

        FileDbo CreateFileFromStorage(string storageCode, string filePath);
    }
}
