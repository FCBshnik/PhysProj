﻿using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Shouldly;

namespace Phys.Lib.Tests.Integration.Db
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

            FindByCodes("file-1");
            FindByCodes("file-2", "file-3");

            Should.Throw<Exception>(() => Create("file-3"));

            var file = FindByCode("file-1");
            FindByCode("file-2");

            AddLink(file.Code, "local", "file-1.pdf");
            AddLink(file.Code, "local", "file-2.pdf");
            DeleteLink(file.Code, "local", "file-3.pdf");
            DeleteLink(file.Code, "local", "file-1.pdf");

            Delete("file-1");
            Delete("file-2");
            Delete("file-3");
        }

        private void Create(string code)
        {
            db.Create(new FileDbo { Code = code, Format = "txt" });
            var files = db.Find(new FilesDbQuery { Code = code });
            files.Count.ShouldBe(1);
        }

        private void Delete(string code)
        {
            db.Delete(code);
            var files = db.Find(new FilesDbQuery { Code = code });
            files.Count.ShouldBe(0);
        }

        private FileDbo FindByCode(string code)
        {
            var files = db.Find(new FilesDbQuery { Code = code });
            files.Count.ShouldBe(1);
            var file = files.First();
            file.Code.ShouldBe(code);
            return file;
        }

        private void FindByCodes(params string[] codes)
        {
            var files = db.Find(new FilesDbQuery { Codes = codes.ToList() });
            Assert.Equivalent(codes, files.Select(f => f.Code), strict: true);
        }

        private void AddLink(string code, string type, string path)
        {
            var link = new FileDbo.LinkDbo { StorageCode = type, FileId = path };
            db.Update(code, new FileDbUpdate { AddLink = link });
            var file = db.GetByCode(code);
            file.Links.ShouldContain(l => l.StorageCode == link.StorageCode && l.FileId == link.FileId);
        }

        private void DeleteLink(string code, string type, string path)
        {
            var link = new FileDbo.LinkDbo { StorageCode = type, FileId = path };
            db.Update(code, new FileDbUpdate { DeleteLink = link });
            var file = db.GetByCode(code);
            file.Links.ShouldNotContain(l => l.StorageCode == link.StorageCode && l.FileId == link.FileId);
        }
    }
}
