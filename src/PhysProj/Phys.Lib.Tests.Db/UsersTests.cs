using Phys.Lib.Db.Users;
using Shouldly;

namespace Phys.Lib.Tests.Db
{
    internal class UsersTests
    {
        private readonly IUsersDb db;

        public UsersTests(IUsersDb db)
        {
            this.db = db;
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

            UpdatePasswordHash(user.Id, "1234567");
            AddRole(user.Id, "user");
            DeleteRole(user.Id, "user");
            DeleteRole(user.Id, "user");
            DeleteRole(user.Id, "admin");
        }

        private UserDbo FindByName(string name)
        {
            var users = db.Find(new UsersDbQuery { NameLowerCase = name });
            users.Count.ShouldBe(1);
            var user = users.First();
            user.Name.ShouldBe(name);
            return user;
        }

        private void UpdatePasswordHash(string id, string passwordHash)
        {
            db.Update(id, new UserDbUpdate { PasswordHash = passwordHash });
            var user = db.Get(id);
            user.PasswordHash.ShouldBe(passwordHash);
        }

        private void AddRole(string id, string role)
        {
            db.Update(id, new UserDbUpdate { AddRole = role });
            var user = db.Get(id);
            user.Roles.ShouldContain(role);
        }

        private void DeleteRole(string id, string role)
        {
            db.Update(id, new UserDbUpdate { DeleteRole = role });
            var user = db.Get(id);
            user.Roles.ShouldNotContain(role);
        }
    }
}
