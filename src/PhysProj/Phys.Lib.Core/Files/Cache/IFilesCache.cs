using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files.Cache
{
    public interface IFilesCache
    {
        List<FileDbo> GetFiles(IEnumerable<string> codes);

        void Set(FileDbo file);
    }
}
