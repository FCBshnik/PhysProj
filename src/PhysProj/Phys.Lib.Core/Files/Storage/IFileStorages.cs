using Phys.Files;

namespace Phys.Lib.Core.Files.Storage
{
    /// <summary>
    /// Aggregates all <see cref="IFileStorage"/>
    /// </summary>
    public interface IFileStorages
    {
        List<FileStorageInfo> List();

        IFileStorage Get(string code);
    }
}
