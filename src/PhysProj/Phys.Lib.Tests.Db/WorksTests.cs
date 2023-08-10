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

            var work = db.Create("work-1");
            db.Create("work-2");
            db.Create("work-3");
            Should.Throw<Exception>(() => db.Create("work-3"));

            FindByCode("work-1");
            FindByCode("work-3");

            Search("lalala");
            Search("3", "work-3");
            Search("or", "work-1", "work-2", "work-3");

            AddInfo(work.Id, "ru");
            DeleteInfo(work.Id, "en");
            DeleteInfo(work.Id, "ru");
            DeleteInfo(work.Id, "ru");

            SetOriginal(work.Id, "work-3");
            SetOriginal(work.Id, string.Empty);

            AddAuthor(work.Id, "author-1");
            AddAuthor(work.Id, "author-2");
            DeleteAuthor(work.Id, "author-2");
            DeleteAuthor(work.Id, "author-2");
            DeleteAuthor(work.Id, "author-3");

            AddSubWork(work.Id, "sub-work-1");
            AddSubWork(work.Id, "sub-work-2");
            DeleteSubWork(work.Id, "sub-work-2");
            DeleteSubWork(work.Id, "sub-work-2");
            DeleteSubWork(work.Id, "sub-work-3");

            AddFile(work.Id, "file-1");
            AddFile(work.Id, "file-2");
            DeleteFile(work.Id, "file-2");
            DeleteFile(work.Id, "file-2");
            DeleteFile(work.Id, "file-3");
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

        private void AddInfo(string id, string language)
        {
            var expected = new WorkDbo.InfoDbo { Language = language, Name = "fn", Description = "desc" };
            db.Update(id, new WorkDbUpdate { AddInfo = expected });
            var author = db.Get(id);
            var info = author.Infos.First(i => i.Language == language);
            info.Description.ShouldBe(expected.Description);
            info.Name.ShouldBe(expected.Name);
        }

        private void DeleteInfo(string id, string language)
        {
            db.Update(id, new WorkDbUpdate { DeleteInfo = language });
            var author = db.Get(id);
            author.Infos.ShouldAllBe(i => i.Language != language);
        }

        private void SetOriginal(string id, string originalCode)
        {
            db.Update(id, new WorkDbUpdate { Original = originalCode });
            var author = db.Get(id);
            author.OriginalCode.ShouldBe(originalCode == string.Empty ? null : originalCode);
        }

        private void AddAuthor(string id, string authorCode)
        {
            db.Update(id, new WorkDbUpdate { AddAuthor = authorCode });
            var author = db.Get(id);
            author.AuthorsCodes.ShouldContain(authorCode);
        }

        private void DeleteAuthor(string id, string authorCode)
        {
            db.Update(id, new WorkDbUpdate { DeleteAuthor = authorCode });
            var author = db.Get(id);
            author.AuthorsCodes.ShouldNotContain(authorCode);
        }

        private void AddSubWork(string id, string subWorkCode)
        {
            db.Update(id, new WorkDbUpdate { AddSubWork = subWorkCode });
            var author = db.Get(id);
            author.SubWorksCodes.ShouldContain(subWorkCode);
        }

        private void DeleteSubWork(string id, string subWorkCode)
        {
            db.Update(id, new WorkDbUpdate { DeleteSubWork = subWorkCode });
            var author = db.Get(id);
            author.SubWorksCodes.ShouldNotContain(subWorkCode);
        }

        private void AddFile(string id, string fileCode)
        {
            db.Update(id, new WorkDbUpdate { AddFile = fileCode });
            var author = db.Get(id);
            author.FilesCodes.ShouldContain(fileCode);
        }

        private void DeleteFile(string id, string fileCode)
        {
            db.Update(id, new WorkDbUpdate { DeleteFile = fileCode });
            var author = db.Get(id);
            author.FilesCodes.ShouldNotContain(fileCode);
        }
    }
}
