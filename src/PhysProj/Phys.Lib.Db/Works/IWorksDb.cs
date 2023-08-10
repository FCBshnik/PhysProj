namespace Phys.Lib.Db.Works
{
    public interface IWorksDb
    {
        List<WorkDbo> Find(WorksDbQuery query);

        WorkDbo Create(string code);

        WorkDbo Update(string id, WorkDbUpdate update);

        void Delete(string id);
    }
}
