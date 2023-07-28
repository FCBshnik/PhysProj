namespace Phys.Lib.Files
{
    public interface IFileStorage
    {
        List<FileInfo> List(string? search);

        Stream Download(string path);

        FileInfo Upload(string path, Stream data);

        void Delete(string path);
    }
}
