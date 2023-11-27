using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Core.Authors
{
    internal class MainAuthorsDb : MainDb<IAuthorsDb>, IAuthorsDb
    {
        public MainAuthorsDb(Lazy<IEnumerable<IAuthorsDb>> dbs, IConfiguration configuration, ILogger<MainAuthorsDb> log)
            :base(dbs, configuration, log)
        {
        }

        public IDbReaderResult<AuthorDbo> Read(DbReaderQuery query)
        {
            return db.Value.Read(query);
        }

        public void Create(string code)
        {
            db.Value.Create(code);
        }

        public void Delete(string code)
        {
            db.Value.Delete(code);
        }

        public List<AuthorDbo> Find(AuthorsDbQuery query)
        {
            return db.Value.Find(query);
        }

        public void Update(string code, AuthorDbUpdate update)
        {
            db.Value.Update(code, update);
        }
    }
}
