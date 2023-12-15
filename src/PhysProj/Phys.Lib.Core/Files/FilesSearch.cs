using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Files
{
    internal class FilesSearch : IFilesSearch
    {
        private readonly IFilesDb db;

        public FilesSearch(IFilesDb db)
        {
            this.db = db;
        }

        public List<FileDbo> Find(string? search = null)
        {
            return db.Find(new FilesDbQuery { Search = search, Limit = 20 });
        }

        public List<FileDbo> FindByCodes(IEnumerable<string> codes)
        {
            ArgumentNullException.ThrowIfNull(codes);

            return db.Find(new FilesDbQuery { Codes = codes.ToList() });
        }

        public FileDbo? FindByCode(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            return db.Find(new FilesDbQuery { Code = code }).FirstOrDefault();
        }
    }
}
