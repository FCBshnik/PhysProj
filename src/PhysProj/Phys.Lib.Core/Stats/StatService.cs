using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Db.Works;
using Phys.Shared;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Stats
{
    internal class StatService : IStatService
    {
        private readonly IWorksDb worksDb;
        private readonly ILogger<StatService> log;

        public StatService(IWorksDb worksDb, ILogger<StatService> log)
        {
            this.worksDb = worksDb;
            this.log = log;
        }

        public LibraryStatsModel GetLibraryStats()
        {
            var stats = new LibraryStatsModel();

            IDbReaderResult<WorkDbo> result = null!;

            do
            {
                result = worksDb.Read(new DbReaderQuery(100, result?.Cursor));
                foreach (var work in result.Values)
                {
                    var total = stats.Works.Total;
                    var lang = stats.Works.PerLanguage.GetOrAdd(work.Language ?? "none", _ => new WorksStatModel.StatModel());

                    total.Count++;
                    lang.Count++;
                    if (work.FilesCodes.Count > 0)
                    {
                        total.CountWithFiles++;
                        lang.CountWithFiles++;
                    }

                    if (!HasFileRefInHierarchy(work, new HashSet<string>()))
                        stats.Works.Unreachable.Add(work.Code);
                }
            } while (!result.IsCompleted);

            return stats;
        }

        private bool HasFileRefInHierarchy(WorkDbo work, HashSet<string> visited)
        {
            if (visited.Contains(work.Code))
                return false;

            visited.Add(work.Code);

            if (visited.Count > 50)
                throw new PhysException($"too deep works tree detected at work {work}");

            if (work.FilesCodes.Count > 0)
                return true;

            if (work.OriginalCode != null)
            {
                var originalWork = worksDb.Find(new WorksDbQuery { Code = work.OriginalCode }).Single();
                if (HasFileRefInHierarchy(originalWork, visited))
                    return true;
            }

            foreach (var subWorkCode in work.SubWorksCodes)
            {
                var subWork = worksDb.Find(new WorksDbQuery { Code = subWorkCode }).Single();
                if (HasFileRefInHierarchy(subWork, visited))
                    return true;
            }

            var translationWorks = worksDb.Find(new WorksDbQuery { OriginalCode = work.Code });
            if (translationWorks.Any(i => HasFileRefInHierarchy(i, visited)))
                return true;

            var collectedWorks = worksDb.Find(new WorksDbQuery { SubWorkCode = work.Code });
            if (collectedWorks.Any(i => HasFileRefInHierarchy(i, visited)))
                return true;

            return false;
        }
    }
}
