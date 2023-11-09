using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files
{
    public interface IFilesService
    {
        List<FileDbo> Find(string? search = null);

        List<FileDbo> FindByCodes(IEnumerable<string> codes);

        FileDbo? FindByCode(string code);

        FileDbo Create(string code, long size, string? format);

        FileDbo CreateFileFromStorage(string storageCode, string fileId);

        FileDbo AddLink(FileDbo file, FileDbo.LinkDbo link);

        void Delete(FileDbo file);
    }
}
