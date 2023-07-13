namespace Phys.Lib.Core.Works
{
    public interface IWorksSearch
    {
        List<WorkDbo> FindByText(string search);

        List<WorkDbo> FindByAuthor(string authorCode);

        List<WorkDbo> FindTranslations(string originalCode);

        List<WorkDbo> FindCollected(string subWorkCode);

        WorkDbo? FindByCode(string code);
    }
}
