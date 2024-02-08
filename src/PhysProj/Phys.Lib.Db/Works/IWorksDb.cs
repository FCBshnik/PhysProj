namespace Phys.Lib.Db.Works
{
    public interface IWorksDb : IDbReader<WorkDbo>
    {
        List<WorkDbo> Find(WorksDbQuery query);

        void Create(string code);

        void Update(string code, WorkDbUpdate update);

        void Delete(string code);
    }
}
