namespace Phys.Lib.Core.Works
{
    public class WorksSearch : IWorksSearch
    {
        private readonly IWorksDb db;

        public WorksSearch(IWorksDb db)
        {
            this.db = db;
        }

        public WorkDbo? FindByCode(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            return db.Find(new WorksDbQuery { Code = code }).FirstOrDefault();
        }

        public List<WorkDbo> Find(string? search = null)
        {
            return db.Find(new WorksDbQuery { Search = search });
        }

        public List<WorkDbo> FindByAuthor(string authorCode)
        {
            ArgumentNullException.ThrowIfNull(authorCode);

            return db.Find(new WorksDbQuery { AuthorCode = authorCode });
        }

        public List<WorkDbo> FindTranslations(string originalCode)
        {
            ArgumentNullException.ThrowIfNull(originalCode);

            return db.Find(new WorksDbQuery { OriginalCode = originalCode });
        }

        public List<WorkDbo> FindCollected(string subWorkCode)
        {
            ArgumentNullException.ThrowIfNull(subWorkCode);

            return db.Find(new WorksDbQuery { SubWorkCode = subWorkCode });
        }
    }
}
