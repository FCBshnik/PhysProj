using NLog;
using Phys.Lib.Core.Validation;

namespace Phys.Lib.Core.Users
{
    public class UsersService : IUsersService
    {
        private static readonly ILogger log = LogManager.GetLogger("users");

        private readonly IUsersDb db;
        private readonly IValidator validation;

        public UsersService(IUsersDb db, IValidator validation)
        {
            this.db = db;
            this.validation = validation;
        }

        public UserDbo? GetByName(string userName)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            return db.Find(new UsersQuery { NameLowerCase = userName.ToLowerInvariant() }).FirstOrDefault();
        }

        public Result<UserDbo> Login(string userName, string password, UserRole withRole)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (withRole == null) throw new ArgumentNullException(nameof(withRole));

            var user = db.Find(new UsersQuery { NameLowerCase = userName.ToLowerInvariant() }).FirstOrDefault();
            if (user == null)
            {
                log.Info($"login '{userName}' failed: user not found");
                return Result.Fail<UserDbo>("login failed");
            }

            if (!user.HasRole(withRole))
            {
                log.Info($"login '{userName}' failed: not in role");
                return Result.Fail<UserDbo>("login failed");
            }

            if (!string.Equals(UserPasswordHasher.HashPassword(password), user.PasswordHash))
            {
                log.Info($"login '{userName}' failed: invalid password");
                return Result.Fail<UserDbo>("login failed");
            }

            log.Info($"login: {user}");
            return Result.Ok(user);
        }

        public UserDbo Create(UserCreateData data)
        {
            validation.Validate(data);

            var user = new UserDbo
            {
                Code = data.Code,
                Name = data.Name,
                NameLowerCase = data.NameLowerCase,
                PasswordHash = UserPasswordHasher.HashPassword(data.Password),
                Roles = new List<string> { data.Role.Code },
            };

            user = db.Create(user);
            log.Info($"created user: {user}");
            return user;
        }
    }
}
