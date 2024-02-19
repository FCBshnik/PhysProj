using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;
using Phys.Shared;
using Phys.Shared.Cache;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Files.Cache
{
    internal class FilesCache : IFilesCache
    {
        private readonly ICache cache;

        public FilesCache(ICache cache)
        {
            this.cache = cache;
        }

        public List<FileDbo> GetFiles(IEnumerable<string> codes)
        {
            var codesDistinct = codes.Distinct().ToList();

            var files = codesDistinct
                .Select(c => cache.Get<FileDbo>(CacheKeys.File(c)))
                .Where(w => w is not null)
                .Select(w => w!)
                .ToList();
            if (codesDistinct.Count != files.Count)
                throw new PhysException($"files {codesDistinct.Except(files.Select(w => w.Code)).Join(",")} not found in cache");

            return files;
        }

        public void Set(FileDbo file)
        {
            cache.Set(CacheKeys.File(file.Code), file);
        }
    }
}
