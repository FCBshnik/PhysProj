using FluentValidation;
using NLog;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Utils;

namespace Phys.Lib.Core.Works
{
    public class WorksEditor : IWorksEditor
    {
        private static readonly ILogger log = LogManager.GetLogger("works-editor");

        private readonly IWorksDb db;
        private readonly IWorksSearch worksSearch;
        private readonly IAuthorsSearch authorsSearch;

        public WorksEditor(IWorksDb db, IWorksSearch worksSearch, IAuthorsSearch authorsSearch)
        {
            this.db = db;
            this.worksSearch = worksSearch;
            this.authorsSearch = authorsSearch;
        }

        public WorkDbo AddInfo(WorkDbo work, WorkDbo.InfoDbo info)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (info is null) throw new ArgumentNullException(nameof(info));

            var update = new WorkDbUpdate { AddInfo = info };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: added info {info}");
            return work;
        }

        public WorkDbo Create(string code)
        {
            code = Code.NormalizeAndValidate(code);

            var work = db.Create(new WorkDbo
            {
                Code = code,
            });

            log.Info($"created work {work}");
            return work;
        }

        public void Delete(WorkDbo work)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));

            db.Delete(work.Id);
            log.Info($"deleted work {work}");
        }

        public WorkDbo DeleteInfo(WorkDbo work, string language)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (language is null) throw new ArgumentNullException(nameof(language));

            var update = new WorkDbUpdate { DeleteInfo = language };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: deleted info {language}");
            return work;
        }

        public WorkDbo LinkAuthor(WorkDbo work, string authorCode)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (authorCode is null) throw new ArgumentNullException(nameof(authorCode));

            var author = authorsSearch.FindByCode(authorCode);
            if (author == null)
                throw ValidationError($"author '{authorCode}' not found");

            var update = new WorkDbUpdate { AddAuthor = author.Code };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: linked author {author}");
            return work;
        }

        public WorkDbo LinkWork(WorkDbo work, string workCode)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (workCode is null) throw new ArgumentNullException(nameof(workCode));

            var linkWork = worksSearch.FindByCode(workCode);
            if (linkWork == null)
                throw ValidationError($"work '{workCode}' not found");

            var update = new WorkDbUpdate { AddWork = linkWork.Code };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: linked work {linkWork}");
            return work;
        }

        public WorkDbo UnlinkAuthor(WorkDbo work, string authorCode)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (authorCode is null) throw new ArgumentNullException(nameof(authorCode));

            var update = new WorkDbUpdate { DeleteAuthor = authorCode };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: unlinked author {authorCode}");
            return work;
        }

        public WorkDbo UnlinkWork(WorkDbo work, string workCode)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (workCode is null) throw new ArgumentNullException(nameof(workCode));

            var update = new WorkDbUpdate { DeleteWork = workCode };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: unlinked work {workCode}");
            return work;
        }

        public WorkDbo UpdateDate(WorkDbo work, string date)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (date is null) throw new ArgumentNullException(nameof(date));

            if (date.HasValue())
            {
                date = Date.NormalizeAndValidate(date);

                if (work.AuthorsCodes.Any())
                    foreach (var author in authorsSearch.FindByCodes(work.AuthorsCodes))
                        Date.ValidateBornAndPublish(author.Born, date);
            }

            var update = new WorkDbUpdate { Publish = date };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: updated date {date}");
            return work;
        }

        public WorkDbo UpdateLanguage(WorkDbo work, string language)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (language is null) throw new ArgumentNullException(nameof(language));

            if (language.HasValue())
                language = Language.NormalizeAndValidate(language);

            var update = new WorkDbUpdate { Language = language };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: updated language {language}");
            return work;
        }

        public WorkDbo UpdateOriginal(WorkDbo work, string originalCode)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (originalCode is null) throw new ArgumentNullException(nameof(originalCode));

            var original = worksSearch.FindByCode(originalCode);
            if (original == null)
                throw ValidationError($"work '{originalCode}' not found");

            var update = new WorkDbUpdate { Original = original.Code };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: updated original {original}");
            return work;
        }

        private Exception ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
