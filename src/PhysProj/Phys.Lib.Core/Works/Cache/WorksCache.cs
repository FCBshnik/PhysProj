using Phys.Lib.Db.Works;
using Phys.Shared;
using Phys.Shared.Cache;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Works.Cache
{
    internal class WorksCache : IWorksCache
    {
        private readonly ICache cache;

        public WorksCache(ICache cache)
        {
            this.cache = cache;
        }

        public List<WorkDbo> GetWorks(IEnumerable<string> codes)
        {
            var codesDistinct = codes.Distinct().ToList();

            var works = codesDistinct
                .Select(c => cache.Get<WorkDbo>(CacheKeys.Work(c)))
                .Where(w => w is not null)
                .Select(w => w!)
                .ToList();
            if (codesDistinct.Count != works.Count)
                throw new PhysException($"works {codesDistinct.Except(works.Select(w => w.Code)).Join(",")} not found in cache");

            return works;
        }
    }
}
