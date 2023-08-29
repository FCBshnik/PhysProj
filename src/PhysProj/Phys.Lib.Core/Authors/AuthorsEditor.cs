using FluentValidation;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Utils;
using Phys.Lib.Core.Works;
using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsEditor : IAuthorsEditor
    {
        private readonly ILogger<AuthorsEditor> log;
        private readonly IAuthorsDb db;
        private readonly IAuthorsSearch authorsSearch;
        private readonly IWorksSearch worksSearch;

        public AuthorsEditor(IAuthorsDb db, IWorksSearch worksSearch, IAuthorsSearch authorsSearch, ILogger<AuthorsEditor> log)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.worksSearch = worksSearch ?? throw new ArgumentNullException(nameof(worksSearch));
            this.authorsSearch = authorsSearch ?? throw new ArgumentNullException(nameof(authorsSearch));
            this.log = log;
        }

        public AuthorDbo Create(string code)
        {
            code = Code.NormalizeAndValidate(code);

            if (authorsSearch.FindByCode(code) != null)
                throw ValidationError($"author with code '{code}' already exists");

            db.Create(code);
            var author = db.GetByCode(code);

            log.Log(LogLevel.Information, $"created author {author}");
            return author;
        }

        public void Delete(AuthorDbo author)
        {
            ArgumentNullException.ThrowIfNull(author);

            var works = worksSearch.FindByAuthor(author.Code);
            if (works.Count != 0)
                throw ValidationError($"can not delete author linked to work");

            db.Delete(author.Code);
            log.Log(LogLevel.Information, $"deleted author {author}");
        }

        public AuthorDbo UpdateInfo(AuthorDbo author, AuthorDbo.InfoDbo info)
        {
            ArgumentNullException.ThrowIfNull(author);
            ArgumentNullException.ThrowIfNull(info);

            if (author.Infos.Any(i => i.Language == info.Language))
                author = DeleteInfo(author, info.Language);

            db.Update(author.Code, new AuthorDbUpdate { AddInfo = info });
            author = db.GetByCode(author.Code);
            log.Log(LogLevel.Information, $"updated author {author}");
            return author;
        }

        public AuthorDbo DeleteInfo(AuthorDbo author, string language)
        {
            ArgumentNullException.ThrowIfNull(author);
            ArgumentNullException.ThrowIfNull(language);

            db.Update(author.Code, new AuthorDbUpdate { DeleteInfo = language });
            author = db.GetByCode(author.Code);
            log.Log(LogLevel.Information, $"updated author {author}");
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

            db.Update(author.Code, update);
            author = db.GetByCode(author.Code);
            log.Log(LogLevel.Information, $"updated author {author}");
            return author;
        }

        private ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
