namespace Phys.Lib.Core.Works
{
    public interface IWorksService
    {
        List<WorkDbo> Search(string search);

        WorkDbo? GetByCode(string code);

        WorkDbo Create(string code);

        WorkDbo Update(WorkDbo work, WorkUpdate update);

        void Delete(WorkDbo work);
    }
}
