namespace Phys.Lib.Base.Files
{
    public interface IFileStorage
    {
        string Code { get; }

        string Name { get; }

        List<StorageFileInfo> List(string? search);

        StorageFileInfo? Get(string path);

        Stream Download(string path);

        StorageFileInfo Upload(string path, Stream data);

        void Delete(string path);
    }
}
