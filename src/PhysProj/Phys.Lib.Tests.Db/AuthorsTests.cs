using Phys.Lib.Db.Authors;
using Shouldly;

namespace Phys.Lib.Tests.Db
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

            UpdateLifetime(author.Id, "1200", "1300");
            UpdateLifetime(author.Id, string.Empty, string.Empty);

            AddInfo(author.Id, "ru");
            DeleteInfo(author.Id, "en");
            DeleteInfo(author.Id, "ru");
            DeleteInfo(author.Id, "ru");
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

        private void UpdateLifetime(string id, string? born, string? died)
        {
            db.Update(id, new AuthorDbUpdate { Born = born, Died = died });
            var author = db.Get(id);
            if (born != null)
                author.Born.ShouldBe(born == string.Empty ? null : born);
            if (died != null)
                author.Died.ShouldBe(died == string.Empty ? null : died);
        }

        private void AddInfo(string id, string language)
        {
            var expected = new AuthorDbo.InfoDbo { Language = language, FullName = "fn", Description = "desc" };
            db.Update(id, new AuthorDbUpdate { AddInfo = expected });
            var author = db.Get(id);
            var info = author.Infos.First(i => i.Language == language);
            info.Description.ShouldBe(expected.Description);
            info.FullName.ShouldBe(expected.FullName);
        }

        private void DeleteInfo(string id, string language)
        {
            db.Update(id, new AuthorDbUpdate { DeleteInfo = language });
            var author = db.Get(id);
            author.Infos.ShouldAllBe(i => i.Language != language);
        }
    }
}
