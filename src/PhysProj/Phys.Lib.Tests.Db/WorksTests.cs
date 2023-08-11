﻿using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;
using Shouldly;

namespace Phys.Lib.Tests.Db
{
    internal class WorksTests
    {
        private readonly IWorksDb db;
        private readonly IAuthorsDb authorsDb;
        private readonly IFilesDb filesDb;

        public WorksTests(IWorksDb db, IAuthorsDb authorsDb, IFilesDb filesDb)
        {
            this.db = db;
            this.authorsDb = authorsDb;
            this.filesDb = filesDb;
        }

        public void Run()
        {
            var works = db.Find(new WorksDbQuery());
            works.ShouldBeEmpty();

            Create("work-1");
            Create("work-2");
            Create("work-3");

            Should.Throw<Exception>(() => db.Create("work-3"));

            var work = FindByCode("work-1");
            FindByCode("work-3");

            Search("lalala");
            Search("3", "work-3");
            Search("or", "work-1", "work-2", "work-3");

            AddInfo(work.Code, "ru");
            AddInfo(work.Code, "es");
            DeleteInfo(work.Code, "en");
            DeleteInfo(work.Code, "ru");
            DeleteInfo(work.Code, "ru");

            SetOriginal(work.Code, "work-3");
            SetOriginal(work.Code, string.Empty);

            authorsDb.Create("work-1-author-1");
            authorsDb.Create("work-1-author-2");

            AddAuthor(work.Code, "work-1-author-1");
            AddAuthor(work.Code, "work-1-author-2");
            DeleteAuthor(work.Code, "work-1-author-2");
            DeleteAuthor(work.Code, "work-1-author-2");
            DeleteAuthor(work.Code, "author-3");

            db.Create("sub-work-1");
            db.Create("sub-work-2");
            AddSubWork(work.Code, "sub-work-1");
            AddSubWork(work.Code, "sub-work-2");
            DeleteSubWork(work.Code, "sub-work-2");
            DeleteSubWork(work.Code, "sub-work-2");
            DeleteSubWork(work.Code, "sub-work-3");

            filesDb.Create(new FileDbo { Code = "file-1" });
            filesDb.Create(new FileDbo { Code = "file-2" });
            AddFile(work.Code, "file-1");
            AddFile(work.Code, "file-2");
            DeleteFile(work.Code, "file-2");
            DeleteFile(work.Code, "file-2");
            DeleteFile(work.Code, "file-3");

            Delete("work-1");
            Delete("work-2");
            Delete("work-3");
            filesDb.Delete("file-1");
            filesDb.Delete("file-2");
        }

        private void Create(string code)
        {
            db.Create(code);
            var works = db.Find(new WorksDbQuery { Code = code });
            works.Count.ShouldBe(1);
        }

        private void Delete(string code)
        {
            db.Delete(code);
            var works = db.Find(new WorksDbQuery { Code = code });
            works.Count.ShouldBe(0);
        }

        private WorkDbo FindByCode(string code)
        {
            var works = db.Find(new WorksDbQuery { Code = code });
            works.Count.ShouldBe(1);
            var work = works.First();
            work.Code.ShouldBe(code);
            return work;
        }

        private void Search(string search, params string[] expectedCodes)
        {
            var works = db.Find(new WorksDbQuery { Search = search });
            works.Count.ShouldBe(expectedCodes.Length);
            works.ForEach(a => a.Code.ShouldBeOneOf(expectedCodes));
        }

        private void AddInfo(string code, string language)
        {
            var expected = new WorkDbo.InfoDbo { Language = language, Name = "fn", Description = "desc" };
            db.Update(code, new WorkDbUpdate { AddInfo = expected });
            var work = db.GetByCode(code);
            var info = work.Infos.First(i => i.Language == language);
            info.Description.ShouldBe(expected.Description);
            info.Name.ShouldBe(expected.Name);
        }

        private void DeleteInfo(string code, string language)
        {
            db.Update(code, new WorkDbUpdate { DeleteInfo = language });
            var work = db.GetByCode(code);
            work.Infos.ShouldAllBe(i => i.Language != language);
        }

        private void SetOriginal(string code, string originalCode)
        {
            db.Update(code, new WorkDbUpdate { Original = originalCode });
            var work = db.GetByCode(code);
            work.OriginalCode.ShouldBe(originalCode == string.Empty ? null : originalCode);
        }

        private void AddAuthor(string code, string authorCode)
        {
            db.Update(code, new WorkDbUpdate { AddAuthor = authorCode });
            var work = db.GetByCode(code);
            work.AuthorsCodes.ShouldContain(authorCode);
        }

        private void DeleteAuthor(string code, string authorCode)
        {
            db.Update(code, new WorkDbUpdate { DeleteAuthor = authorCode });
            var work = db.GetByCode(code);
            work.AuthorsCodes.ShouldNotContain(authorCode);
        }

        private void AddSubWork(string code, string subWorkCode)
        {
            db.Update(code, new WorkDbUpdate { AddSubWork = subWorkCode });
            var work = db.GetByCode(code);
            work.SubWorksCodes.ShouldContain(subWorkCode);
        }

        private void DeleteSubWork(string code, string subWorkCode)
        {
            db.Update(code, new WorkDbUpdate { DeleteSubWork = subWorkCode });
            var work = db.GetByCode(code);
            work.SubWorksCodes.ShouldNotContain(subWorkCode);
        }

        private void AddFile(string code, string fileCode)
        {
            db.Update(code, new WorkDbUpdate { AddFile = fileCode });
            var work = db.GetByCode(code);
            work.FilesCodes.ShouldContain(fileCode);
        }

        private void DeleteFile(string code, string fileCode)
        {
            db.Update(code, new WorkDbUpdate { DeleteFile = fileCode });
            var work = db.GetByCode(code);
            work.FilesCodes.ShouldNotContain(fileCode);
        }
    }
}
