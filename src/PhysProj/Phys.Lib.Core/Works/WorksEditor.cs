using FluentValidation;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Utils;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    public class WorksEditor : IWorksEditor
    {
        private readonly ILogger<WorksEditor> log;
        private readonly IWorksDb db;
        private readonly IFilesDb filesLinksDb;
        private readonly IWorksSearch worksSearch;
        private readonly IAuthorsSearch authorsSearch;

        public WorksEditor(IWorksDb db, IWorksSearch worksSearch, IAuthorsSearch authorsSearch, IFilesDb filesLinksDb, ILogger<WorksEditor> log)
        {
            this.db = db;
            this.worksSearch = worksSearch;
            this.authorsSearch = authorsSearch;
            this.filesLinksDb = filesLinksDb;
            this.log = log;
        }

        public WorkDbo AddInfo(WorkDbo work, WorkDbo.InfoDbo info)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(info);

            if (work.Infos.Exists(i => i.Language == info.Language))
                work = DeleteInfo(work, info.Language);

            var update = new WorkDbUpdate { AddInfo = info };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} info {lang}", LogEvent.Added, work.Code, info.Language);
            return db.GetByCode(work.Code);
        }

        public WorkDbo Create(string code)
        {
            code = Code.NormalizeAndValidate(code);

            if (worksSearch.FindByCode(code) != null)
                throw ValidationError($"work with the same code already exists");

            db.Create(code);
            log.Log(LogLevel.Information, "{event} work {work}", LogEvent.Created, code);
            return db.GetByCode(code);
        }

        public void Delete(WorkDbo work)
        {
            ArgumentNullException.ThrowIfNull(work);

            if (worksSearch.FindCollected(work.Code).Count != 0)
                throw ValidationError("can not delete work linked as sub-work to collected work");

            db.Delete(work.Code);
            log.Log(LogLevel.Information, "{event} work {work}", LogEvent.Deleted, work.Code);
        }

        public WorkDbo DeleteInfo(WorkDbo work, string language)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(language);

            var update = new WorkDbUpdate { DeleteInfo = language };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} info {lang}", LogEvent.Removed, work.Code, language);
            return db.GetByCode(work.Code);
        }

        public WorkDbo LinkAuthor(WorkDbo work, string authorCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(authorCode);

            var author = authorsSearch.FindByCode(authorCode) ?? throw ValidationError($"author '{authorCode}' not found");
            var update = new WorkDbUpdate { AddAuthor = author.Code };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} author {author}", LogEvent.Linked, work.Code, author.Code);
            return db.GetByCode(work.Code);
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

            ValidateWorkIsNotLinked(work, subWork.Code, 0);
            ValidateWorkIsNotLinked(subWork, work.Code, 0);

            var update = new WorkDbUpdate { AddSubWork = subWork.Code };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} sub-work {sub-work}", LogEvent.Linked, work.Code, subWorkCode);
            return db.GetByCode(work.Code);
        }

        public WorkDbo UnlinkAuthor(WorkDbo work, string authorCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(authorCode);

            var update = new WorkDbUpdate { DeleteAuthor = authorCode };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} author {author}", LogEvent.Unlinked, work.Code, authorCode);
            return db.GetByCode(work.Code);
        }

        public WorkDbo UnlinkWork(WorkDbo work, string subWorkCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(subWorkCode);

            var update = new WorkDbUpdate { DeleteSubWork = subWorkCode };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} sub-work {sub-work}", LogEvent.Unlinked, work.Code, subWorkCode);
            return db.GetByCode(work.Code);
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
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} date", LogEvent.Updated, work.Code);
            return db.GetByCode(work.Code);
        }

        public WorkDbo UpdateLanguage(WorkDbo work, string language)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(language);

            if (language.HasValue())
                language = Language.NormalizeAndValidate(language);

            var update = new WorkDbUpdate { Language = language };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} language", LogEvent.Updated, work.Code);
            return db.GetByCode(work.Code);
        }

        public WorkDbo UpdateIsPublic(WorkDbo work, bool isPublic)
        {
            ArgumentNullException.ThrowIfNull(work);

            if (work.IsPublic == isPublic)
                return work;

            var update = new WorkDbUpdate { IsPublic = isPublic };
            db.Update(work.Code, update);
            return db.GetByCode(work.Code);
        }

        public WorkDbo LinkFile(WorkDbo work, string fileCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(fileCode);

            var file = filesLinksDb.Find(new FilesDbQuery { Code = fileCode }).FirstOrDefault() ?? throw ValidationError($"file links '{fileCode}' not found");
            var update = new WorkDbUpdate { AddFile = file.Code };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} file {file}", LogEvent.Linked, work.Code, fileCode);
            return db.GetByCode(work.Code);
        }

        public WorkDbo UnlinkFile(WorkDbo work, string fileCode)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(fileCode);

            var update = new WorkDbUpdate { DeleteFile = fileCode };
            db.Update(work.Code, update);
            log.Log(LogLevel.Information, "{event} work {work} file {file}", LogEvent.Unlinked, work.Code, fileCode);
            return db.GetByCode(work.Code);
        }

        private void ValidateWorkIsNotLinked(WorkDbo work, string linkedCode, int depth)
        {
            if (depth > 3)
                throw ValidationError($"too long chain of works links detected");

            if (work.SubWorksCodes.Contains(linkedCode))
                throw ValidationError($"circular chain of works links detected");

            foreach (var subWorkCode in work.SubWorksCodes)
                ValidateWorkIsNotLinked(worksSearch.FindByCode(subWorkCode) ?? throw new ApplicationException(), linkedCode, depth + 1);
        }

        private static ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
