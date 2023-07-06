using NLog;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsSearch : IAuthorsSearch
    {
        private static readonly ILogger log = LogManager.GetLogger("authors-search");

        private readonly IAuthorsDb db;

        public AuthorsSearch(IAuthorsDb db)
        {
            this.db = db ?? throw new ArgumentNullException();
        }

        public AuthorDbo? FindByCode(string code)
        {
            if (code is null) throw new ArgumentNullException(nameof(code));

            return db.Find(new AuthorsDbQuery { Code = code }).FirstOrDefault();
        }

        public List<AuthorDbo> Search(string search)
        {
            if (search is null) throw new ArgumentNullException(nameof(search));

            return db.Find(new AuthorsDbQuery { Search = search });
        }
    }
}
