namespace Phys.Lib.Core.Works
{
    public interface IWorksSearch
    {
        List<WorkDbo> FindByText(string search);

        List<WorkDbo> FindByAuthor(string authorCode);

        List<WorkDbo> FindTranslations(string originalCode);

        WorkDbo? FindByCode(string code);
    }
}
