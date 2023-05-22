using FluentValidation;
using NLog;

namespace Phys.Lib.Core.Users
{
    public class Users : IUsers
    {
        private static readonly ILogger log = LogManager.GetLogger("users");

        private readonly IDb db;
        private readonly IValidator<CreateUserData> validator;

        public Users(IDb db, IValidator<CreateUserData> validator)
        {
            this.db = db;
            this.validator = validator;
        }

        public UserDbo Create(CreateUserData data)
        {
            validator.ValidateAndThrow(data);

            var user = new UserDbo
            {
                Id = db.NewId(),
                Code = Code.FromString(data.Name),
                Name = data.Name,
                NameLowerCase = data.Name.ToLowerInvariant(),
                PasswordHash = UserPassword.HashPassword(data.Password),
                Roles = new List<string> { data.Role.Code },
            };

            user = db.Users.Create(user);
            log.Info($"created user {user}");
            return user;
        }
    }
}
