using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    internal class WorksLinksWriter : IMigrationWriter<WorkDbo>
    {
        private readonly IWorksDb db;

        public WorksLinksWriter(IWorksDb db)
        {
            this.db = db;
        }

        public string Name => db.Name + "-links";

        public void Write(IEnumerable<WorkDbo> values, MigrationDto.StatsDto stats)
        {
            foreach (var work in values)
            {
                var prev = db.Find(new WorksDbQuery { Code = work.Code }).FirstOrDefault();
                if (work.Equals(prev))
                    continue;

                if (work.OriginalCode != null)
                    db.Update(work.Code, new WorkDbUpdate { Original = work.OriginalCode });

                work.SubWorksCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { AddSubWork = i }));
            }
        }
    }
}
