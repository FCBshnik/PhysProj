using Phys.Lib.Db.Users;
using Phys.Lib.Db.Migrations;
using System.Data;

namespace Phys.Lib.Core.Users
{
    internal class UsersWriter : IDbWriter<UserDbo>
    {
        private readonly IUsersDb db;

        public UsersWriter(IUsersDb db)
        {
            this.db = db;
        }

        public string Name => db.Name;

        public void Write(IEnumerable<UserDbo> values)
        {
            foreach (var user in values)
            {
                var existing = db.FindByName(user.Name);
                if (existing != null)
                {
                    db.Update(user.NameLowerCase, new UserDbUpdate { PasswordHash = user.PasswordHash });
                    foreach (var role in existing.Roles)
                        db.Update(user.NameLowerCase, new UserDbUpdate { DeleteRole = role });
                }
                else
                {
                    db.Create(user);
                }

                foreach (var role in user.Roles)
                    db.Update(user.NameLowerCase, new UserDbUpdate { AddRole = role });
            }
        }
    }
}
