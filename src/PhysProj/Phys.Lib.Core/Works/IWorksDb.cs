namespace Phys.Lib.Core.Works
{
    public interface IWorksDb
    {
        List<WorkDbo> Find(WorksDbQuery query);

        WorkDbo Get(string id);

        WorkDbo Create(WorkDbo work);

        WorkDbo Update(string id, WorkDbUpdate update);

        void Delete(string id);
    }
}
