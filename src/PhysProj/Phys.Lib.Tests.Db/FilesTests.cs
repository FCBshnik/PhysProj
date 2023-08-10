using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Shouldly;

namespace Phys.Lib.Tests.Db
{
    internal class FilesTests
    {
        private readonly IFilesDb db;

        public FilesTests(IFilesDb db)
        {
            this.db = db;
        }

        public void Run()
        {
            var files = db.Find(new FilesDbQuery());
            files.ShouldBeEmpty();

            Create("file-1");
            Create("file-2");
            Create("file-3");
            Should.Throw<Exception>(() => Create("file-3"));

            var file = FindByCode("file-1");
            FindByCode("file-2");

            AddLink(file.Id, "local", "file.pdf");
            DeleteLink(file.Id, "local", "file1.pdf");
            DeleteLink(file.Id, "local", "file.pdf");
        }

        private void Create(string code)
        {
            db.Create(new FileDbo { Code = code });
        }

        private FileDbo FindByCode(string code)
        {
            var files = db.Find(new FilesDbQuery { Code = code });
            files.Count.ShouldBe(1);
            var file = files.First();
            file.Code.ShouldBe(code);
            return file;
        }

        private void AddLink(string id, string type, string path)
        {
            var link = new FileDbo.LinkDbo { Type = type, Path = path };
            db.Update(id, new FileDbUpdate { AddLink = link });
            var file = db.Get(id);
            file.Links.ShouldContain(l => l.Type == link.Type && l.Path == link.Path);
        }

        private void DeleteLink(string id, string type, string path)
        {
            var link = new FileDbo.LinkDbo { Type = type, Path = path };
            db.Update(id, new FileDbUpdate { DeleteLink = link });
            var file = db.Get(id);
            file.Links.ShouldNotContain(l => l.Type == link.Type && l.Path == link.Path);
        }
    }
}
