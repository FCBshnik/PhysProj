using Phys.Lib.Db.Migrations;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    internal class WorksLinksWriter : IDbWriter<WorkDbo>
    {
        private readonly IWorksDb db;

        public WorksLinksWriter(IWorksDb db)
        {
            this.db = db;
        }

        public string Name => db.Name + "-links";

        public void Write(IEnumerable<WorkDbo> values)
        {
            foreach (var work in values)
            {
                if (work.OriginalCode != null)
                    db.Update(work.Code, new WorkDbUpdate { Original = work.OriginalCode });
                work.SubWorksCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { AddSubWork = i }));
            }
        }
    }
}
