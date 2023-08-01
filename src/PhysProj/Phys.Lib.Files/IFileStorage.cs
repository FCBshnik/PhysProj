namespace Phys.Lib.Files
{
    public interface IFileStorage
    {
        List<FileInfo> List(string? search);

        FileInfo? Get(string path);

        Stream Download(string path);

        FileInfo Upload(string path, Stream data);

        void Delete(string path);
    }
}
