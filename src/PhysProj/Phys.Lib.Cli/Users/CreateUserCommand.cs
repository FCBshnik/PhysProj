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
            var user = users.Create(options.Name, options.Password);
            users.AddRole(user, UserRole.Parse(options.Role));
        }
    }
}
