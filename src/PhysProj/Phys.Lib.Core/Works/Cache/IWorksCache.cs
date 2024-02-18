using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works.Cache
{
    public interface IWorksCache
    {
        List<WorkDbo> GetWorks(IEnumerable<string> codes);

        void Set(WorkDbo work);
    }
}
