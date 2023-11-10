using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files
{
    public interface IFilesEditor
    {
        FileDbo Create(string code, long size, string? format);

        FileDbo CreateFileFromStorage(string storageCode, string fileId);

        FileDbo AddLink(FileDbo file, FileDbo.LinkDbo link);

        void Delete(FileDbo file);
    }
}
