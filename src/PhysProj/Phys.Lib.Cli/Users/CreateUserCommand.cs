using Phys.Lib.Core.Users;

namespace Phys.Lib.Cli.Users
{
    internal class CreateUserCommand : ICommand<CreateUserOptions>
    {
        private readonly IUsersService users;

        public CreateUserCommand(IUsersService users)
        {
            this.users = users;
        }

        public void Run(CreateUserOptions options)
        {
            var userData = new UserCreateData
            {
                Name = options.Name,
                Password = options.Password,
                Role = UserRole.Parse(options.Role),
            };

            users.Create(userData);
        }
    }
}
