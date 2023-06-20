using NLog;
using System.Linq.Expressions;

namespace Phys.Lib.Core.Authors
{
    public class Authors : IAuthors
    {
        private static readonly ILogger log = LogManager.GetLogger("authors");

        private readonly IAuthorsDb db;

        public Authors(IAuthorsDb db)
        {
            this.db = db;
        }

        public AuthorDbo Create(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException(nameof(code));

            return db.Create(new AuthorDbo
            {
                Code = code,
            });
        }

        public AuthorDbo? GetByCode(string code)
        {
            if (code is null) throw new ArgumentNullException(nameof(code));

            return db.Find(new AuthorsQuery { Code = code }).FirstOrDefault();
        }

        public List<AuthorDbo> Search(string search)
        {
            if (search is null) throw new ArgumentNullException(nameof(search));

            return db.Find(new AuthorsQuery { Search = search });
        }

        public AuthorDbo Update<T>(AuthorDbo author, Expression<Func<AuthorDbo, T>> field, T value)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));
            if (field is null) throw new ArgumentNullException(nameof(field));

            return db.Update(author.Id, field, value);
        }
    }
}
