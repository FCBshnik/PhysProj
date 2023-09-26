using FluentValidation;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;
using Phys.Lib.Db.Users;

namespace Phys.Lib.Core.Users
{
    public class UsersService : IUsersService
    {
        private readonly ILogger<UsersService> log;
        private readonly IUsersDbs db;

        public UsersService(IUsersDbs db, ILogger<UsersService> log)
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
            return FindByName(name) ?? throw new PhysDbException($"user '{name}' not found");
        }

        public Result<UserDbo> Login(string name, string password, UserRole role)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(password);
            ArgumentNullException.ThrowIfNull(role);

            var user = db.Find(new UsersDbQuery { NameLowerCase = name.ToLowerInvariant() }).FirstOrDefault();
            if (user == null)
            {
                log.Log(LogLevel.Information, "{event} as '{user}': {reason}", "login-fail", name, "user not found");
                return Result.Fail<UserDbo>("login failed");
            }

            if (!user.HasRole(role))
            {
                log.Log(LogLevel.Information, "{event} as '{user}': {reason}", "login-fail", name, "not in role");
                return Result.Fail<UserDbo>("login failed");
            }

            if (!string.Equals(UserPasswordHasher.HashPassword(password), user.PasswordHash))
            {
                log.Log(LogLevel.Information, "{event} as '{user}': {reason}", "login-fail", name, "invalid password");
                return Result.Fail<UserDbo>("login failed");
            }

            log.Log(LogLevel.Information, "{event} as '{user}'", "login-success", name);
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
            log.Log(LogLevel.Information, "role {role} added to user {user}", role.Code, user.Name);
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
            log.Log(LogLevel.Information, "role '{role}' deleted from user {user}", role.Code, user.Name);
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
            log.Log(LogLevel.Information, "{event} user {user}", LogEvent.Created, user.Name);
            return user;
        }

        private ValidationException ValidationError(string message)
        {
            return new ValidationException(message);
        }
    }
}
