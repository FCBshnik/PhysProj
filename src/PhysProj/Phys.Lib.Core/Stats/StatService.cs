using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Works;
using Phys.Shared;
using Phys.Shared.Cache;
using Phys.Shared.EventBus;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Stats
{
    public class StatService : IStatService, IEventHandler<StatCacheInvalidatedEvent>
    {
        private readonly ILogger<StatService> log;

        private readonly IWorksDb worksDb;
        private readonly ICache cache;

        public StatService(IWorksDb worksDb, ILogger<StatService> log, ICache cache)
        {
            this.log = log;
            this.worksDb = worksDb;
            this.cache = cache;
        }

        public SystemStatsModel GetLibraryStats()
        {
            return cache.GetOrAdd(CacheKeys.Stats(), CalcLibraryStats);
        }

        public SystemStatsModel CalcLibraryStats()
        {
            log.LogInformation("calculating");

            var stats = new SystemStatsModel();

            var dbStats = new DbStatsModel { Name = worksDb.Name, Library = new LibraryStatsModel() };
            stats.Dbs.Add(dbStats);

            foreach (var works in worksDb.Read(100))
            {
                foreach (var work in works)
                {
                    var total = dbStats.Library.Works.Total;
                    var lang = dbStats.Library.Works.PerLanguage.GetOrAdd(work.Language ?? "none", _ => new WorksStatModel.StatModel());

                    total.Count++;
                    lang.Count++;
                    if (work.FilesCodes.Count > 0)
                    {
                        total.CountWithFiles++;
                        lang.CountWithFiles++;
                    }

                    if (!HasFileRefInHierarchy(worksDb, work, new HashSet<string>()))
                        dbStats.Library.Works.Unreachable.Add(work.Code);
                }
            }

            log.LogInformation("calculated");
            return stats;
        }

        private bool HasFileRefInHierarchy(IWorksDb db, WorkDbo work, HashSet<string> visited)
        {
            if (visited.Contains(work.Code))
                return work.FilesCodes.Count > 0;

            visited.Add(work.Code);

            if (visited.Count > 50)
                throw new PhysException($"too deep works tree detected for work {work}");

            foreach (var subWorkCode in work.SubWorksCodes)
            {
                var subWork = db.Find(new WorksDbQuery { Code = subWorkCode }).Single();
                if (HasFileRefInHierarchy(db, subWork, visited))
                    return true;
            }

            var collectedWorks = db.Find(new WorksDbQuery { SubWorkCode = work.Code });
            if (collectedWorks.Any(i => HasFileRefInHierarchy(db, i, visited)))
                return true;

            return work.FilesCodes.Count > 0;
        }

        string IEventHandler<StatCacheInvalidatedEvent>.EventName => EventNames.StatCacheInvalidated;

        void IEventHandler<StatCacheInvalidatedEvent>.Handle(StatCacheInvalidatedEvent data)
        {
            cache.Set(CacheKeys.Stats(), CalcLibraryStats());
        }
    }
}
