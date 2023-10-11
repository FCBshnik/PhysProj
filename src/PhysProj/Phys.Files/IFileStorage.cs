namespace Phys.Files
{
    public interface IFileStorage
    {
        string Code { get; }

        string Name { get; }

        List<StorageFileInfo> List(string? search);

        StorageFileInfo? Get(string fileId);

        Stream Download(string fileId);

        StorageFileInfo Upload(Stream data, string name);

        void Delete(string fileId);
    }
}
