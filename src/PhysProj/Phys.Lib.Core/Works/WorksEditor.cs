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

            if (worksSearch.FindByCode(code) != null)
                throw ValidationError($"work with the same code already exists");

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

        public WorkDbo LinkWork(WorkDbo work, string subWorkCode)
        {
            if (work is null) throw new ArgumentNullException(nameof(work));
            if (subWorkCode is null) throw new ArgumentNullException(nameof(subWorkCode));

            subWorkCode = Code.NormalizeAndValidate(subWorkCode);

            if (work.SubWorksCodes.Contains(subWorkCode))
                return work;

            var subWork = worksSearch.FindByCode(subWorkCode);
            if (subWork == null)
                throw ValidationError($"work '{subWorkCode}' not found");

            if (subWork.Code == work.Code)
                throw ValidationError($"can not link sub work as self");
            if (subWork.Code == work.OriginalCode)
                throw ValidationError($"can not link sub work which is already linked as original");

            ValidateWorkIsNotLinked(work, subWork.Code, 0);
            ValidateWorkIsNotLinked(subWork, work.Code, 0);

            var update = new WorkDbUpdate { AddSubWork = subWork.Code };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: linked work {subWork}");
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

            var update = new WorkDbUpdate { DeleteSubWork = workCode };
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

            originalCode = Code.NormalizeAndValidate(originalCode);

            if (work.OriginalCode == originalCode)
                return work;

            var original = worksSearch.FindByCode(originalCode);
            if (original == null)
                throw ValidationError($"work '{originalCode}' not found");

            if (original.Code == work.Code)
                throw ValidationError($"can not link original as self");
            if (work.SubWorksCodes.Contains(original.Code))
                throw ValidationError($"can not link original which is already linked as sub work");

            ValidateWorkIsNotLinked(work, original.Code, 0);
            ValidateWorkIsNotLinked(original, work.Code, 0);

            var update = new WorkDbUpdate { Original = original.Code };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: updated original {original}");
            return work;
        }

        private void ValidateWorkIsNotLinked(WorkDbo work, string linkedCode, int depth)
        {
            if (depth > 3)
                throw ValidationError($"too long chain of works links detected");

            if (work.OriginalCode == linkedCode || work.SubWorksCodes.Contains(linkedCode))
                throw ValidationError($"circular chain of works links detected");

            if (work.OriginalCode != null)
                ValidateWorkIsNotLinked(worksSearch.FindByCode(work.OriginalCode) ?? throw new ApplicationException(), linkedCode, depth + 1);

            foreach (var subWorkCode in work.SubWorksCodes)
                ValidateWorkIsNotLinked(worksSearch.FindByCode(subWorkCode) ?? throw new ApplicationException(), linkedCode, depth + 1);
        }

        private Exception ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
