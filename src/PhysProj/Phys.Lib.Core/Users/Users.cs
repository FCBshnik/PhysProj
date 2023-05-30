using NLog;
using Phys.Lib.Core.Validation;

namespace Phys.Lib.Core.Users
{
    public class Users : IUsers
    {
        private static readonly ILogger log = LogManager.GetLogger("users");

        private readonly IDb db;
        private readonly IValidator validation;

        public Users(IDb db, IValidator validation)
        {
            this.db = db;
            this.validation = validation;
        }

        public UserDbo Login(string userName, string password)
        {
            var user = db.Users.Find(new UsersQuery { NameLowerCase = userName.ToLowerInvariant() }).FirstOrDefault();
            if (user == null)
            {
                log.Info($"login '{userName}' failed: user not found");
                throw new FluentValidation.ValidationException("login failed");
            }

            if (!string.Equals(UserPasswordHasher.HashPassword(password), user.PasswordHash))
            {
                log.Info($"login '{userName}' failed: invalid password");
                throw new FluentValidation.ValidationException("login failed");
            }

            log.Info($"login '{userName}' succeed");
            return user;
        }

        public UserDbo Create(CreateUserData data)
        {
            validation.Validate(data);

            var user = new UserDbo
            {
                Id = db.NewId(),
                Code = data.Code,
                Name = data.Name,
                NameLowerCase = data.NameLowerCase,
                PasswordHash = UserPasswordHasher.HashPassword(data.Password),
                Roles = new List<string> { data.Role.Code },
            };

            user = db.Users.Create(user);
            log.Info($"created user {user}");
            return user;
        }
    }
}
