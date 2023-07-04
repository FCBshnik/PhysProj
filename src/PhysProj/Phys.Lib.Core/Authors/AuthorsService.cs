using NLog;
using Phys.Lib.Core.Utils;

namespace Phys.Lib.Core.Authors
{
    public class AuthorsService : IAuthorsService
    {
        private static readonly ILogger log = LogManager.GetLogger("authors");

        private readonly IAuthorsDb db;

        public AuthorsService(IAuthorsDb db)
        {
            this.db = db;
        }

        public AuthorDbo Create(string code)
        {
            code = Code.NormalizeAndValidate(code);

            var author = db.Create(new AuthorDbo
            {
                Code = code,
            });

            log.Info($"created author {author}");
            return author;
        }

        public void Delete(AuthorDbo author)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));

            db.Delete(author.Id);
            log.Info($"deleted author {author}");
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

        public AuthorDbo Update(AuthorDbo author, AuthorUpdate update)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));
            if (update is null) throw new ArgumentNullException(nameof(update));

            if (update.Born.HasValue())
                update.Born = Date.NormalizeAndValidate(update.Born);

            if (update.Died.HasValue())
                update.Died = Date.NormalizeAndValidate(update.Died);

            if (update.Born.HasValue() || update.Died.HasValue())
                Date.ValidateBornAndDied(author.Born ?? update.Born, author.Died ?? update.Died);

            author = db.Update(author.Id, update);
            log.Info($"updated author {author}");
            return author;
        }
    }
}
