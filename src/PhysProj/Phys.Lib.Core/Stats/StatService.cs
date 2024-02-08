﻿using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Works;
using Phys.Shared;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Stats
{
    internal class StatService : IStatService
    {
        private readonly List<IWorksDb> worksDbs;
        private readonly ILogger<StatService> log;

        public StatService(IEnumerable<IWorksDb> worksDbs, ILogger<StatService> log)
        {
            this.worksDbs = worksDbs.Where(db => db.Name != "main").ToList();
            this.log = log;
        }

        public SystemStatsModel GetLibraryStats()
        {
            var stats = new SystemStatsModel();

            foreach (var worksDb in worksDbs)
            {
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
            }

            return stats;
        }

        private bool HasFileRefInHierarchy(IWorksDb db, WorkDbo work, HashSet<string> visited)
        {
            if (visited.Contains(work.Code))
                return false;

            visited.Add(work.Code);

            if (visited.Count > 50)
                throw new PhysException($"too deep works tree detected at work {work}");

            if (work.FilesCodes.Count > 0)
                return true;

            foreach (var subWorkCode in work.SubWorksCodes)
            {
                var subWork = db.Find(new WorksDbQuery { Code = subWorkCode }).Single();
                if (HasFileRefInHierarchy(db, subWork, visited))
                    return true;
            }

            var collectedWorks = db.Find(new WorksDbQuery { SubWorkCode = work.Code });
            if (collectedWorks.Any(i => HasFileRefInHierarchy(db, i, visited)))
                return true;

            return false;
        }
    }
}
