using FluentValidation;
using NLog;
using Phys.Lib.Core.Utils;
using Phys.Lib.Core.Works;
using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsEditor : IAuthorsEditor
    {
        private static readonly Logger log = LogManager.GetLogger("authors-editor");

        private readonly IAuthorsDb db;
        private readonly IAuthorsSearch authorsSearch;
        private readonly IWorksSearch worksSearch;

        public AuthorsEditor(IAuthorsDb db, IWorksSearch worksSearch, IAuthorsSearch authorsSearch)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.worksSearch = worksSearch ?? throw new ArgumentNullException(nameof(worksSearch));
            this.authorsSearch = authorsSearch ?? throw new ArgumentNullException(nameof(authorsSearch));
        }

        public AuthorDbo Create(string code)
        {
            code = Code.NormalizeAndValidate(code);

            if (authorsSearch.FindByCode(code) != null)
                throw ValidationError($"author with code '{code}' already exists");

            db.Create(code);
            var author = db.GetByCode(code);

            log.Info($"created author {author}");
            return author;
        }

        public void Delete(AuthorDbo author)
        {
            ArgumentNullException.ThrowIfNull(author);

            var works = worksSearch.FindByAuthor(author.Code);
            if (works.Count != 0)
                throw ValidationError($"can not delete author linked to work");

            db.Delete(author.Id);
            log.Info($"deleted author {author}");
        }

        public AuthorDbo UpdateInfo(AuthorDbo author, AuthorDbo.InfoDbo info)
        {
            ArgumentNullException.ThrowIfNull(author);
            ArgumentNullException.ThrowIfNull(info);

            if (author.Infos.Any(i => i.Language == info.Language))
                author = DeleteInfo(author, info.Language);

            db.Update(author.Id, new AuthorDbUpdate { AddInfo = info });
            author = db.Get(author.Id);
            log.Info($"updated author {author}");
            return author;
        }

        public AuthorDbo DeleteInfo(AuthorDbo author, string language)
        {
            ArgumentNullException.ThrowIfNull(author);
            ArgumentNullException.ThrowIfNull(language);

            db.Update(author.Id, new AuthorDbUpdate { DeleteInfo = language });
            author = db.Get(author.Id);
            log.Info($"updated author {author}");
            return author;
        }

        public AuthorDbo UpdateLifetime(AuthorDbo author, string? born, string? died)
        {
            ArgumentNullException.ThrowIfNull(author);
            if (born is null && died is null) throw new ArgumentNullException();

            var update = new AuthorDbUpdate();

            if (born.HasValue())
            {
                update.Born = Date.NormalizeAndValidate(born);

                foreach (var work in worksSearch.FindByAuthor(author.Code))
                    Date.ValidateBornAndPublish(update.Born, work.Publish);
            }

            if (died.HasValue())
                update.Died = Date.NormalizeAndValidate(died);

            if (update.Born.HasValue() || update.Died.HasValue())
                Date.ValidateLifetime(update.Born ?? author.Born, update.Died ?? author.Died);

            db.Update(author.Id, update);
            author = db.Get(author.Id);
            log.Info($"updated author {author}");
            return author;
        }

        private ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
