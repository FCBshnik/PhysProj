using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Works;
using Shouldly;

namespace Phys.Lib.Tests.Db
{
    internal class WorksTests
    {
        private readonly IWorksDb db;

        public WorksTests(IWorksDb db)
        {
            this.db = db;
        }

        public void Run()
        {
            var works = db.Find(new WorksDbQuery());
            works.ShouldBeEmpty();

            db.Create("work-1");
            db.Create("work-2");
            db.Create("work-3");
            Should.Throw<Exception>(() => db.Create("work-3"));

            var work = FindByCode("work-1");
            FindByCode("work-3");

            Search("lalala");
            Search("3", "work-3");
            Search("or", "work-1", "work-2", "work-3");

            AddInfo(work.Code, "ru");
            DeleteInfo(work.Code, "en");
            DeleteInfo(work.Code, "ru");
            DeleteInfo(work.Code, "ru");

            SetOriginal(work.Code, "work-3");
            SetOriginal(work.Code, string.Empty);

            AddAuthor(work.Code, "author-1");
            AddAuthor(work. Code, "author-2");
            DeleteAuthor(work.Code, "author-2");
            DeleteAuthor(work.Code, "author-2");
            DeleteAuthor(work.Code, "author-3");

            AddSubWork(work.Code, "sub-work-1");
            AddSubWork(work.Code, "sub-work-2");
            DeleteSubWork(work.Code, "sub-work-2");
            DeleteSubWork(work.Code, "sub-work-2");
            DeleteSubWork(work.Code, "sub-work-3");

            AddFile(work.Code, "file-1");
            AddFile(work.Code, "file-2");
            DeleteFile(work.Code, "file-2");
            DeleteFile(work.Code, "file-2");
            DeleteFile(work.Code, "file-3");
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
            var authors = db.Find(new WorksDbQuery { Search = search });
            authors.Count.ShouldBe(expectedCodes.Length);
            authors.ForEach(a => a.Code.ShouldBeOneOf(expectedCodes));
        }

        private void AddInfo(string code, string language)
        {
            var expected = new WorkDbo.InfoDbo { Language = language, Name = "fn", Description = "desc" };
            db.Update(code, new WorkDbUpdate { AddInfo = expected });
            var author = db.GetByCode(code);
            var info = author.Infos.First(i => i.Language == language);
            info.Description.ShouldBe(expected.Description);
            info.Name.ShouldBe(expected.Name);
        }

        private void DeleteInfo(string code, string language)
        {
            db.Update(code, new WorkDbUpdate { DeleteInfo = language });
            var author = db.GetByCode(code);
            author.Infos.ShouldAllBe(i => i.Language != language);
        }

        private void SetOriginal(string code, string originalCode)
        {
            db.Update(code, new WorkDbUpdate { Original = originalCode });
            var author = db.GetByCode(code);
            author.OriginalCode.ShouldBe(originalCode == string.Empty ? null : originalCode);
        }

        private void AddAuthor(string code, string authorCode)
        {
            db.Update(code, new WorkDbUpdate { AddAuthor = authorCode });
            var author = db.GetByCode(code);
            author.AuthorsCodes.ShouldContain(authorCode);
        }

        private void DeleteAuthor(string code, string authorCode)
        {
            db.Update(code, new WorkDbUpdate { DeleteAuthor = authorCode });
            var author = db.GetByCode(code);
            author.AuthorsCodes.ShouldNotContain(authorCode);
        }

        private void AddSubWork(string code, string subWorkCode)
        {
            db.Update(code, new WorkDbUpdate { AddSubWork = subWorkCode });
            var author = db.GetByCode(code);
            author.SubWorksCodes.ShouldContain(subWorkCode);
        }

        private void DeleteSubWork(string code, string subWorkCode)
        {
            db.Update(code, new WorkDbUpdate { DeleteSubWork = subWorkCode });
            var author = db.GetByCode(code);
            author.SubWorksCodes.ShouldNotContain(subWorkCode);
        }

        private void AddFile(string code, string fileCode)
        {
            db.Update(code, new WorkDbUpdate { AddFile = fileCode });
            var author = db.GetByCode(code);
            author.FilesCodes.ShouldContain(fileCode);
        }

        private void DeleteFile(string code, string fileCode)
        {
            db.Update(code, new WorkDbUpdate { DeleteFile = fileCode });
            var author = db.GetByCode(code);
            author.FilesCodes.ShouldNotContain(fileCode);
        }
    }
}
