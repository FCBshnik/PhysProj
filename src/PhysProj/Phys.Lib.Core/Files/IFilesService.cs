namespace Phys.Lib.Core.Files
{
    public interface IFilesService
    {
        List<FileDbo> Find(string? search = null);

        FileDbo? FindByCode(string code);

        FileDbo CreateFromStorageFile(string storageCode, string filePath);

        void Delete(FileDbo file);
    }
}
