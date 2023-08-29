using FluentValidation;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Users;

namespace Phys.Lib.Core.Users
{
    public class UsersService : IUsersService
    {
        private readonly ILogger<UsersService> log;
        private readonly IUsersDb db;

        public UsersService(IUsersDb db, ILogger<UsersService> log)
        {
            this.db = db;
            this.log = log;
        }

        public UserDbo? FindByName(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            return db.Find(new UsersDbQuery { NameLowerCase = name.ToLowerInvariant() }).FirstOrDefault();
        }

        public UserDbo GetByName(string name)
        {
            return FindByName(name) ?? throw new ApplicationException($"user '{name}' not found");
        }

        public Result<UserDbo> Login(string userName, string password, UserRole role)
        {
            ArgumentNullException.ThrowIfNull(userName);
            ArgumentNullException.ThrowIfNull(password);
            ArgumentNullException.ThrowIfNull(role);

            var user = db.Find(new UsersDbQuery { NameLowerCase = userName.ToLowerInvariant() }).FirstOrDefault();
            if (user == null)
            {
                log.Log(LogLevel.Information, $"login '{userName}' failed: user not found");
                return Result.Fail<UserDbo>("login failed");
            }

            if (!user.HasRole(role))
            {
                log.Log(LogLevel.Information, $"login '{userName}' failed: not in role");
                return Result.Fail<UserDbo>("login failed");
            }

            if (!string.Equals(UserPasswordHasher.HashPassword(password), user.PasswordHash))
            {
                log.Log(LogLevel.Information, $"login '{userName}' failed: invalid password");
                return Result.Fail<UserDbo>("login failed");
            }

            log.Log(LogLevel.Information, $"login: {user}");
            return Result.Ok(user);
        }

        public UserDbo AddRole(UserDbo user, UserRole role)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(role);

            if (user.HasRole(role))
                return user;

            db.Update(user.NameLowerCase, new UserDbUpdate { AddRole = role });
            user = db.GetByName(user.NameLowerCase);
            log.Log(LogLevel.Information, $"updated user {user}: added role {role}");
            return user;
        }

        public UserDbo DeleteRole(UserDbo user, UserRole role)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(role);

            if (!user.HasRole(role))
                return user;

            db.Update(user.NameLowerCase, new UserDbUpdate { DeleteRole = role });
            user = db.GetByName(user.NameLowerCase);
            log.Log(LogLevel.Information, $"updated user {user}: deleted role {role}");
            return user;
        }

        public UserDbo Create(string name, string password)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(password);

            UserValidators.Name.ValidateAndThrow(name);
            UserValidators.Password.ValidateAndThrow(password);

            if (db.Find(new UsersDbQuery { NameLowerCase = name.ToLowerInvariant() }).Count != 0)
                throw ValidationError("user name is already taken");

            var user = new UserDbo
            {
                Name = name,
                NameLowerCase = name.ToLowerInvariant(),
                PasswordHash = UserPasswordHasher.HashPassword(password),
            };

            db.Create(user);
            user = db.GetByName(user.Name);
            log.Log(LogLevel.Information, $"created user: {user}");
            return user;
        }

        private ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
