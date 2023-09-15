using Phys.Lib.Db.Reader;
using Phys.Lib.Db.Users;

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
                db.Create(user);
                foreach (var role in user.Roles)
                    db.Update(user.NameLowerCase, new UserDbUpdate { AddRole = role });
            }
        }
    }
}
