using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Cli.Users
{
    internal class CreateUserCommand : ICommand<CreateUserOptions>
    {
        private readonly IUsersService users;
        private readonly ILogger<CreateUserCommand> log;

        public CreateUserCommand(IUsersService users, ILogger<CreateUserCommand> log)
        {
            this.users = users;
            this.log = log;
        }

        public void Run(CreateUserOptions options)
        {
            log.Log(LogLevel.Information, $"creating user '{options.Name}'");

            var user = users.FindByName(options.Name) ?? users.Create(options.Name, options.Password);
            if (!user.HasRole(options.Role))
                users.AddRole(user, UserRole.Parse(options.Role));
        }
    }
}
