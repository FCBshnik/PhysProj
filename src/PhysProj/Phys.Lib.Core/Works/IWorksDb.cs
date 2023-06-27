namespace Phys.Lib.Core.Works
{
    public interface IWorksDb
    {
        List<WorkDbo> Find(WorksQuery query);

        WorkDbo Get(string id);

        WorkDbo Create(WorkDbo work);

        WorkDbo Update(string id, WorkUpdate update);

        void Delete(string id);
    }
}
