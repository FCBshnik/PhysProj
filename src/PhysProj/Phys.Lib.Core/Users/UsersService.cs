using FluentValidation;
using NLog;

namespace Phys.Lib.Core.Users
{
    public class UsersService : IUsersService
    {
        private static readonly ILogger log = LogManager.GetLogger("users");

        private readonly IUsersDb db;

        public UsersService(IUsersDb db)
        {
            this.db = db;
        }

        public UserDbo GetByName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return db.Find(new UsersDbQuery { NameLowerCase = name.ToLowerInvariant() }).FirstOrDefault() ?? throw new ApplicationException($"user '{name}' not found");
        }

        public Result<UserDbo> Login(string userName, string password, UserRole role)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (role == null) throw new ArgumentNullException(nameof(role));

            var user = db.Find(new UsersDbQuery { NameLowerCase = userName.ToLowerInvariant() }).FirstOrDefault();
            if (user == null)
            {
                log.Info($"login '{userName}' failed: user not found");
                return Result.Fail<UserDbo>("login failed");
            }

            if (!user.HasRole(role))
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

        public UserDbo AddRole(UserDbo user, UserRole role)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (role == null) throw new ArgumentNullException(nameof(role));

            if (user.HasRole(role))
                return user;

            user = db.Update(user.Id, new UserDbUpdate { AddRole = role });
            log.Info($"updated user {user}: added role {role}");
            return user;
        }

        public UserDbo DeleteRole(UserDbo user, UserRole role)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (role == null) throw new ArgumentNullException(nameof(role));

            if (!user.HasRole(role))
                return user;

            user = db.Update(user.Id, new UserDbUpdate { DeleteRole = role });
            log.Info($"updated user {user}: deleted role {role}");
            return user;
        }

        public UserDbo Create(string name, string password)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (password == null) throw new ArgumentNullException(nameof(password));

            UserValidators.Name.ValidateAndThrow(name);
            UserValidators.Password.ValidateAndThrow(password);

            if (db.Find(new UsersDbQuery { NameLowerCase = name.ToLowerInvariant() }).Any())
                throw ValidationError("user name is already taken");

            var user = new UserDbo
            {
                Name = name,
                NameLowerCase = name.ToLowerInvariant(),
                PasswordHash = UserPasswordHasher.HashPassword(password),
            };

            user = db.Create(user);
            log.Info($"created user: {user}");
            return user;
        }

        private Exception ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
