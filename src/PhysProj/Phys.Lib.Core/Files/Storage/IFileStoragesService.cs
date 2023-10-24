using Phys.Files;

namespace Phys.Lib.Core.Files.Storage
{
    public interface IFileStoragesService
    {
        List<FileStorageInfo> List();

        IFileStorage Get(string code);
    }
}
