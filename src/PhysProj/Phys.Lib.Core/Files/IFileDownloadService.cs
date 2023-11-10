using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files
{
    public interface IFileDownloadService
    {
        FileDownloadLink GetDownloadLink(FileDbo file);
    }
}
