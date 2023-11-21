using Phys.Lib.Db.Migrations;
using Phys.Lib.Db.Search;
using Phys.Lib.Db.Works;
using Phys.Shared.Search;

namespace Phys.Lib.Core.Search
{
    internal class WorksTextSearchMigrator : BaseTextSearchMigrator<WorkDbo, WorkTso>
    {
        public WorksTextSearchMigrator(string name, IEnumerable<IDbReader<WorkDbo>> readers, ITextSearch<WorkTso> textSearch) : base(name, readers, textSearch)
        {
        }

        protected override WorkTso Map(WorkDbo value)
        {
            return new WorkTso
            {
                Code = value.Code,
                Names = value.Infos.ToDictionary(i => i.Language, i => i.Name),
            };
        }
    }
}
