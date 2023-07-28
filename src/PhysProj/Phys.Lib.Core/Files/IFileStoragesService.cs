using Phys.Lib.Files;

namespace Phys.Lib.Core.Files
{
    public interface IFileStoragesService
    {
        List<FileStorageDbo> List();

        IFileStorage Get(string code);
    }
}
