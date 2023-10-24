using Phys.Lib.Db.Users;
using Phys.Lib.Core.Migration;

namespace Phys.Lib.Core.Users
{
    internal class UsersWriter : IMigrationWriter<UserDbo>
    {
        private readonly IUsersDb db;

        public UsersWriter(IUsersDb db)
        {
            this.db = db;
        }

        public string Name => db.Name;

        public void Write(IEnumerable<UserDbo> values, MigrationDto.StatsDto stats)
        {
            foreach (var user in values)
            {
                var prev = db.FindByName(user.Name);
                if (user.Equals(prev))
                {
                    stats.Skipped++;
                    continue;
                }

                if (prev != null)
                    foreach (var role in prev.Roles)
                        db.Update(user.NameLowerCase, new UserDbUpdate { DeleteRole = role });
                else
                    db.Create(user);

                db.Update(user.NameLowerCase, new UserDbUpdate { PasswordHash = user.PasswordHash });
                foreach (var role in user.Roles)
                    db.Update(user.NameLowerCase, new UserDbUpdate { AddRole = role });

                _ = prev == null ? stats.Created++ : stats.Updated++;
            }
        }
    }
}
