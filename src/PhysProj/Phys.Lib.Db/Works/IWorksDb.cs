namespace Phys.Lib.Db.Works
{
    public interface IWorksDb
    {
        List<WorkDbo> Find(WorksDbQuery query);

        void Create(string code);

        void Update(string id, WorkDbUpdate update);

        void Delete(string id);
    }
}
