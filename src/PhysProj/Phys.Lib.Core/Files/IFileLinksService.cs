namespace Phys.Lib.Core.Files
{
    public interface IFileLinksService
    {
        List<FileLinksDbo> Find(string? search = null);

        FileLinksDbo? FindByCode(string code);

        FileLinksDbo CreateFromStorageFile(string storageCode, string filePath);

        void Delete(FileLinksDbo file);
    }
}
