using Phys.Lib.Db.Users;
using Shouldly;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Tests.Integration.Db
{
    internal class UsersTests
    {
        private readonly IUsersDb db;
        private readonly IDbReader<UserDbo> usersReader;

        public UsersTests(IUsersDb db, IDbReader<UserDbo> usersReader)
        {
            this.db = db;
            this.usersReader = usersReader;
        }

        public void Run()
        {
            var users = db.Find(new UsersDbQuery());
            users.ShouldBeEmpty();

            db.Create(new UserDbo { Name = "user", NameLowerCase = "user", PasswordHash = "123456" });
            db.Create(new UserDbo { Name = "admin", NameLowerCase = "admin", PasswordHash = "123456" });
            Should.Throw<Exception>(() => db.Create(new UserDbo { Name = "user", NameLowerCase = "user", PasswordHash = "123456" }));
            users = db.Find(new UsersDbQuery());
            users.Count.ShouldBe(2);

            var user = FindByName("user");
            FindByName("admin");

            UpdatePasswordHash(user.NameLowerCase, "1234567");
            AddRole(user.NameLowerCase, "user");
            DeleteRole(user.NameLowerCase, "user");
            DeleteRole(user.NameLowerCase, "user");
            DeleteRole(user.NameLowerCase, "admin");

            Read(1, "user", "admin");
            Read(2, "user", "admin");
            Read(10, "user", "admin");
        }

        private UserDbo FindByName(string name)
        {
            var users = db.Find(new UsersDbQuery { NameLowerCase = name });
            users.Count.ShouldBe(1);
            var user = users.First();
            user.Name.ShouldBe(name);
            return user;
        }

        private void UpdatePasswordHash(string nameLowerCase, string passwordHash)
        {
            db.Update(nameLowerCase, new UserDbUpdate { PasswordHash = passwordHash });
            var user = db.GetByName(nameLowerCase);
            user.PasswordHash.ShouldBe(passwordHash);
        }

        private void AddRole(string nameLowerCase, string role)
        {
            db.Update(nameLowerCase, new UserDbUpdate { AddRole = role });
            var user = db.GetByName(nameLowerCase);
            user.Roles.ShouldContain(role);
        }

        private void DeleteRole(string nameLowerCase, string role)
        {
            db.Update(nameLowerCase, new UserDbUpdate { DeleteRole = role });
            var user = db.GetByName(nameLowerCase);
            user.Roles.ShouldNotContain(role);
        }

        private void Read(int limit, params string[] expectedNames)
        {
            IDbReaderResult<UserDbo> res = null!;
            List<string> actualNames = new List<string>();

            do
            {
                res = usersReader.Read(new DbReaderQuery(limit, res?.Cursor));
                actualNames.AddRange(res.Values.Select(u => u.Name));
            } while (!res.IsCompleted);

            actualNames.ShouldBeEquivalentTo(expectedNames.ToList());
        }
    }
}
