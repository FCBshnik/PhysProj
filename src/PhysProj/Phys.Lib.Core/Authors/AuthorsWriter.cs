using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsWriter : IDbWriter<AuthorDbo>
    {
        private readonly IAuthorsDb db;

        public AuthorsWriter(IAuthorsDb db)
        {
            this.db = db;
        }

        public string Name => db.Name;

        public void Write(IEnumerable<AuthorDbo> values)
        {
            foreach (var author in values)
            {
                db.Create(author.Code);
                db.Update(author.Code, new AuthorDbUpdate { Born = author.Born, Died = author.Died, });
                foreach (var info in author.Infos)
                    db.Update(author.Code, new AuthorDbUpdate { AddInfo = info });
            }
        }
    }
}
