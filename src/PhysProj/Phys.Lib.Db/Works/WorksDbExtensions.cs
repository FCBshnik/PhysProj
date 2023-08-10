namespace Phys.Lib.Db.Works
{
    public static class WorksDbExtensions
    {
        public static WorkDbo Get(this IWorksDb db, string id)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(id);

            return db.Find(new WorksDbQuery { Id = id }).FirstOrDefault() ?? throw new ApplicationException($"work '{id}' not found in '{db.GetType().FullName}'");
        }
    }
}
