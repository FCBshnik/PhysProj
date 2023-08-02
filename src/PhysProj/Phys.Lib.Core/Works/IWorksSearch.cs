using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    public interface IWorksSearch
    {
        List<WorkDbo> Find(string? search = null);

        List<WorkDbo> FindByAuthor(string authorCode);

        List<WorkDbo> FindTranslations(string originalCode);

        List<WorkDbo> FindCollected(string subWorkCode);

        WorkDbo? FindByCode(string code);
    }
}
