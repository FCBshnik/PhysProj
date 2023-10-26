using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    internal class WorksBaseWriter : IMigrationWriter<WorkDbo>
    {
        private readonly IWorksDb db;
        private readonly ILogger<WorksMigrator> log;

        public WorksBaseWriter(IWorksDb db, ILogger<WorksMigrator> log)
        {
            this.db = db;
            this.log = log;
        }

        public string Name => db.Name + "-base";

        public void Write(IEnumerable<WorkDbo> values, MigrationDto.StatsDto stats)
        {
            foreach (var work in values)
            {
                var prev = db.Find(new WorksDbQuery { Code = work.Code }).FirstOrDefault();
                if (work.Equals(prev))
                {
                    stats.Skipped++;
                    continue;
                }

                if (prev != null)
                {
                    prev.Infos.ForEach(i => db.Update(work.Code, new WorkDbUpdate { DeleteInfo = i.Language }));
                    prev.AuthorsCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { DeleteAuthor = i }));
                    prev.SubWorksCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { DeleteSubWork = i }));
                    prev.FilesCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { DeleteFile = i }));
                }
                else
                    db.Create(work.Code);

                db.Update(work.Code, new WorkDbUpdate { Language = work.Language, Publish = work.Publish, Original = string.Empty });
                work.Infos.ForEach(i => db.Update(work.Code, new WorkDbUpdate { AddInfo = i }));
                work.AuthorsCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { AddAuthor = i }));
                work.FilesCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { AddFile = i }));

                log.LogInformation($"migrated {work}");

                _ = prev == null ? stats.Created++ : stats.Updated++;
            }
        }
    }
}
