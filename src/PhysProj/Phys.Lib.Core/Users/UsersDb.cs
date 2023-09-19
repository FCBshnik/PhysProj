using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Users;
using Phys.Shared;

namespace Phys.Lib.Core.Users
{
    internal class UsersDb : IUsersDb
    {
        private readonly Lazy<IUsersDb> db;
        private readonly IConfiguration configuration;
        private readonly ILogger<UsersDb> log;

        public string Name => "main";

        public UsersDb(Lazy<IEnumerable<IUsersDb>> dbs, IConfiguration configuration, ILogger<UsersDb> log)
        {
            this.configuration = configuration;
            this.log = log;
            db = new Lazy<IUsersDb>(() => GetDb(dbs));
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

        private IUsersDb GetDb(Lazy<IEnumerable<IUsersDb>> dbs)
        {
            var dbName = configuration.GetConnectionString("db");
            log.LogInformation($"use db '{dbName}' as {typeof(IUsersDb)}");
            return dbs.Value.FirstOrDefault(db => db.Name == dbName) ?? throw new PhysException();
        }
    }
}
