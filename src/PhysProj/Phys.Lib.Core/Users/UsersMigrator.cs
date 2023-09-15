using Phys.Lib.Db.Reader;
using Phys.Lib.Db.Users;

namespace Phys.Lib.Core.Users
{
    internal class UsersMigrator
    {
        private readonly IDbReader<UserDbo> src;
        private readonly IUsersDb dst;

        public UsersMigrator(IDbReader<UserDbo> src, IUsersDb dst)
        {
            this.src = src;
            this.dst = dst;
        }

        public void Migrate()
        {
            IDbReaderResult<UserDbo> result = null!;
            do
            {
                result = src.Read(new DbReaderQuery(100, result?.Cursor));

                foreach (var user in result.Values)
                {
                    dst.Create(user);
                    foreach (var role in user.Roles)
                        dst.Update(user.NameLowerCase, new UserDbUpdate { AddRole = role });
                }

            } while (!result.IsCompleted);
        }
    }
}
