using Phys.Lib.Db.Files;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Core.Files
{
    internal class FilesWriter : IDbWriter<FileDbo>
    {
        private readonly IFilesDb db;

        public FilesWriter(IFilesDb db)
        {
            this.db = db;
        }

        public string Name => db.Name;

        public void Write(IEnumerable<FileDbo> values)
        {
            foreach (var file in values)
            {
                db.Create(file);
                file.Links.ForEach(l => db.Update(file.Code, new FileDbUpdate { AddLink = l }));
            }
        }
    }
}
