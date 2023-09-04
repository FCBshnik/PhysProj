using Phys.Lib.Db.Users;

namespace Phys.Lib.Postgres.Users
{
    internal static class UserMapper
    {
        public static UserDbo Map(UserModel user)
        {
            return new UserDbo
            {
                Name = user.Name,
                NameLowerCase = user.NameLowerCase,
                PasswordHash = user.PasswordHash,
                Roles = user.Roles?.ToList() ?? new List<string>(),
            };
        }
    }
}
