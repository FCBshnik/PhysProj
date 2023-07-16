using FluentValidation;
using NLog;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Utils;

namespace Phys.Lib.Core.Works
{
    public class WorksEditor : IWorksEditor
    {
        private static readonly Logger log = LogManager.GetLogger("works-editor");

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
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(info);

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
            ArgumentNullException.ThrowIfNull(work);

            if (worksSearch.FindTranslations(work.Code).Count != 0)
                throw ValidationError("can not delete work linked as original to translated work");
            if (worksSearch.FindCollected(work.Code).Count != 0)
                throw ValidationError("can not delete work linked as sub-work to collected work");

            db.Delete(work.Id);
            log.Info($"deleted work {work}");
        }

        public WorkDbo DeleteInfo(WorkDbo work, string language)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(language);

            var update = new WorkDbUpdate { DeleteInfo = language };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: deleted info {language}");
            return work;
        }

        public WorkDbo LinkAuthor(WorkDbo work, string authorCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(authorCode);

            var author = authorsSearch.FindByCode(authorCode) ?? throw ValidationError($"author '{authorCode}' not found");
            var update = new WorkDbUpdate { AddAuthor = author.Code };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: linked author {author}");
            return work;
        }

        public WorkDbo LinkWork(WorkDbo work, string subWorkCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(subWorkCode);

            subWorkCode = Code.NormalizeAndValidate(subWorkCode);

            if (work.SubWorksCodes.Contains(subWorkCode))
                return work;

            var subWork = worksSearch.FindByCode(subWorkCode) ?? throw ValidationError($"work '{subWorkCode}' not found");
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
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(authorCode);

            var update = new WorkDbUpdate { DeleteAuthor = authorCode };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: unlinked author {authorCode}");
            return work;
        }

        public WorkDbo UnlinkWork(WorkDbo work, string workCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(workCode);

            var update = new WorkDbUpdate { DeleteSubWork = workCode };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: unlinked work {workCode}");
            return work;
        }

        public WorkDbo UpdateDate(WorkDbo work, string date)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(date);

            if (date.HasValue())
            {
                date = Date.NormalizeAndValidate(date);

                if (work.AuthorsCodes.Count != 0)
                {
                    foreach (var author in authorsSearch.FindByCodes(work.AuthorsCodes))
                        Date.ValidateBornAndPublish(author.Born, date);
                }
            }

            var update = new WorkDbUpdate { Publish = date };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: updated date {date}");
            return work;
        }

        public WorkDbo UpdateLanguage(WorkDbo work, string language)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(language);

            if (language.HasValue())
                language = Language.NormalizeAndValidate(language);

            var update = new WorkDbUpdate { Language = language };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: updated language {language}");
            return work;
        }

        public WorkDbo LinkOriginal(WorkDbo work, string originalCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(originalCode);

            originalCode = Code.NormalizeAndValidate(originalCode);

            if (work.OriginalCode == originalCode)
                return work;

            var original = worksSearch.FindByCode(originalCode) ?? throw ValidationError($"work '{originalCode}' not found");
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

        public WorkDbo UnlinkOriginal(WorkDbo work)
        {
            ArgumentNullException.ThrowIfNull(work);

            var update = new WorkDbUpdate { Original = string.Empty };
            work = db.Update(work.Id, update);
            log.Info($"updated work {work}: unlink original");
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

        private static ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
