using NLog;
using Phys.Lib.Core.Utils;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsEditor : IAuthorsEditor
    {
        private static readonly ILogger log = LogManager.GetLogger("authors-editor");

        private readonly IAuthorsDb db;

        public AuthorsEditor(IAuthorsDb db)
        {
            this.db = db ?? throw new ArgumentNullException();
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

        public AuthorDbo UpdateInfo(AuthorDbo author, AuthorDbo.InfoDbo info)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));
            if (info is null) throw new ArgumentNullException(nameof(info));

            author = db.Update(author.Id, new AuthorDbUpdate { AddInfo = info });
            log.Info($"updated author {author}");
            return author;
        }

        public AuthorDbo DeleteInfo(AuthorDbo author, string language)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));
            if (language is null) throw new ArgumentNullException(nameof(language));

            author = db.Update(author.Id, new AuthorDbUpdate { DeleteInfo = language });
            log.Info($"updated author {author}");
            return author;
        }

        public AuthorDbo UpdateLifetime(AuthorDbo author, string? born, string? died)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));
            if (born is null && died is null) throw new ArgumentNullException();

            var update = new AuthorDbUpdate();
            if (born.HasValue())
                update.Born = Date.NormalizeAndValidate(born);
            if (died.HasValue())
                update.Died = Date.NormalizeAndValidate(died);

            if (update.Born.HasValue() || update.Died.HasValue())
                Date.ValidateBornAndDied(author.Born ?? update.Born, author.Died ?? update.Died);

            author = db.Update(author.Id, update);
            log.Info($"updated author {author}");
            return author;
        }
    }
}
