using Phys.Lib.Db.Authors;
using Shouldly;

namespace Phys.Lib.Tests.Integration.Db
{
    internal class AuthorsTests
    {
        private readonly IAuthorsDb db;

        public AuthorsTests(IAuthorsDb db)
        {
            this.db = db;
        }

        public void Run()
        {
            var authors = db.Find(new AuthorsDbQuery());
            authors.ShouldBeEmpty();

            db.Create("author-1");
            db.Create("author-2");
            db.Create("author-3");

            Should.Throw<Exception>(() => db.Create("author-3"));
            authors = db.Find(new AuthorsDbQuery());
            authors.Count.ShouldBe(3);

            var author = FindByCode("author-1");
            FindByCode("author-2");
            FindByCodes("author-1", "author-2");
            Search("lalala");
            Search("3", "author-3");
            Search("th", "author-1", "author-2", "author-3");

            UpdateLifetime(author.Code, "1200", "1300");
            UpdateLifetime(author.Code, string.Empty, string.Empty);

            AddInfo(author.Code, "ru");
            AddInfo(author.Code, "es");
            DeleteInfo(author.Code, "en");
            DeleteInfo(author.Code, "ru");
            DeleteInfo(author.Code, "ru");

            Delete("author-1");
            Delete("author-2");
            Delete("author-3");
        }

        private void Delete(string code)
        {
            db.Delete(code);
            var authors = db.Find(new AuthorsDbQuery { Code = code });
            authors.Count.ShouldBe(0);
        }

        private AuthorDbo FindByCode(string code)
        {
            var authors = db.Find(new AuthorsDbQuery { Code = code });
            authors.Count.ShouldBe(1);
            var author = authors.First();
            author.Code.ShouldBe(code);
            return author;
        }

        private void FindByCodes(params string[] codes)
        {
            var authors = db.Find(new AuthorsDbQuery { Codes = codes.ToList() });
            authors.Count.ShouldBe(codes.Length);
            authors.ForEach(a => a.Code.ShouldBeOneOf(codes));
        }

        private void Search(string search, params string[] expectedCodes)
        {
            var authors = db.Find(new AuthorsDbQuery { Search = search });
            authors.Count.ShouldBe(expectedCodes.Length);
            authors.ForEach(a => a.Code.ShouldBeOneOf(expectedCodes));
        }

        private void UpdateLifetime(string code, string? born, string? died)
        {
            db.Update(code, new AuthorDbUpdate { Born = born, Died = died });
            var author = db.GetByCode(code);
            if (born != null)
                author.Born.ShouldBe(born == string.Empty ? null : born);
            if (died != null)
                author.Died.ShouldBe(died == string.Empty ? null : died);
        }

        private void AddInfo(string code, string language)
        {
            var expected = new AuthorDbo.InfoDbo { Language = language, FullName = "fn", Description = "desc" };
            db.Update(code, new AuthorDbUpdate { AddInfo = expected });
            var author = db.GetByCode(code);
            var info = author.Infos.First(i => i.Language == language);
            info.Description.ShouldBe(expected.Description);
            info.FullName.ShouldBe(expected.FullName);
        }

        private void DeleteInfo(string code, string language)
        {
            db.Update(code, new AuthorDbUpdate { DeleteInfo = language });
            var author = db.GetByCode(code);
            author.Infos.ShouldAllBe(i => i.Language != language);
        }
    }
}
