namespace Phys.Lib.Core.Files
{
    public interface IFileLinksService
    {
        List<FileLinksDbo> Find(string? search = null);

        FileLinksDbo CreateFromStorageFile(string storageCode, string filePath);
    }
}
