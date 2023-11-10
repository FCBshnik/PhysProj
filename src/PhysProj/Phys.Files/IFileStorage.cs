namespace Phys.Files
{
    /// <summary>
    /// Stores files content by id
    /// </summary>
    public interface IFileStorage
    {
        string Code { get; }

        List<StorageFileInfo> List(string? search);

        StorageFileInfo? Get(string fileId);

        Stream Download(string fileId);

        StorageFileInfo Upload(Stream data, string name);

        void Delete(string fileId);

        void Refresh();

        Uri GetDownloadLink(string fileId);
    }
}
