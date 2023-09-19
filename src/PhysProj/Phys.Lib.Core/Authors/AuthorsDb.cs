using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsDb : IAuthorsDb
    {
        private readonly List<IAuthorsDb> dbs;

        public AuthorsDb(List<IAuthorsDb> dbs)
        {
            this.dbs = dbs;
        }

        public void Create(string code)
        {
            throw new NotImplementedException();
        }

        public void Delete(string code)
        {
            throw new NotImplementedException();
        }

        public List<AuthorDbo> Find(AuthorsDbQuery query)
        {
            throw new NotImplementedException();
        }

        public void Update(string code, AuthorDbUpdate update)
        {
            throw new NotImplementedException();
        }
    }
}
