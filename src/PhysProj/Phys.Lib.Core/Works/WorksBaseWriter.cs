using Phys.Lib.Db.Migrations;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    internal class WorksBaseWriter : IDbWriter<WorkDbo>
    {
        private readonly IWorksDb db;

        public WorksBaseWriter(IWorksDb db)
        {
            this.db = db;
        }

        public string Name => db.Name + "-base";

        public void Write(IEnumerable<WorkDbo> values)
        {
            foreach (var work in values)
            {
                db.Create(work.Code);
                db.Update(work.Code, new WorkDbUpdate { Language = work.Language, Publish = work.Publish });
                work.Infos.ForEach(i => db.Update(work.Code, new WorkDbUpdate { AddInfo = i }));
                work.AuthorsCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { AddAuthor = i }));
                work.FilesCodes.ForEach(i => db.Update(work.Code, new WorkDbUpdate { AddFile = i }));
            }
        }
    }
}
