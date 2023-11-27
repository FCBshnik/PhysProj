using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Db.Users;

namespace Phys.Lib.Core.Users
{
    internal class MainUsersDb : MainDb<IUsersDb>, IUsersDb
    {
        public MainUsersDb(Lazy<IEnumerable<IUsersDb>> dbs, IConfiguration configuration, ILogger<MainUsersDb> log)
            : base(dbs, configuration, log)
        {
        }

        public IDbReaderResult<UserDbo> Read(DbReaderQuery query)
        {
            return db.Value.Read(query);
        }

        public void Create(UserDbo user)
        {
            db.Value.Create(user);
        }

        public List<UserDbo> Find(UsersDbQuery query)
        {
            return db.Value.Find(query);
        }

        public void Update(string name, UserDbUpdate update)
        {
            db.Value.Update(name, update);
        }
    }
}
