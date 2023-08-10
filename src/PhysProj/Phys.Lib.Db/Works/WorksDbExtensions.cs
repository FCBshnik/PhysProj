namespace Phys.Lib.Db.Works
{
    public static class WorksDbExtensions
    {
        public static WorkDbo Get(this IWorksDb db, string id)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(id);

            var works = db.Find(new WorksDbQuery { Id = id });
            if (works.Count == 1)
                return works[0];

            throw new ApplicationException($"failed get work with id '{id}' from '{db.GetType().FullName}' due to found {works.Count} works");
        }

        public static WorkDbo GetByCode(this IWorksDb db, string code)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(code);

            var works = db.Find(new WorksDbQuery { Code = code });
            if (works.Count == 1)
                return works[0];

            throw new ApplicationException($"failed get work with code '{code}' from '{db.GetType().FullName}' due to found {works.Count} works");
        }
    }
}
