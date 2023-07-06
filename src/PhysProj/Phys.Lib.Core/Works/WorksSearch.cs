using NLog;

namespace Phys.Lib.Core.Works
{
    public class WorksSearch : IWorksSearch
    {
        private static readonly ILogger log = LogManager.GetLogger("works-search");

        private readonly IWorksDb db;

        public WorksSearch(IWorksDb db)
        {
            this.db = db;
        }

        public WorkDbo? FindByCode(string code)
        {
            if (code is null) throw new ArgumentNullException(nameof(code));

            return db.Find(new WorksDbQuery { Code = code }).FirstOrDefault();
        }

        public List<WorkDbo> FindByText(string search)
        {
            if (search is null) throw new ArgumentNullException(nameof(search));

            return db.Find(new WorksDbQuery { Search = search });
        }

        public List<WorkDbo> FindByAuthor(string authorCode)
        {
            if (authorCode is null) throw new ArgumentNullException(nameof(authorCode));

            return db.Find(new WorksDbQuery { AuthorCode = authorCode });
        }
    }
}
