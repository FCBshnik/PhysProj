using Phys.Lib.Core;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Cli.Users
{
    internal class CreateUserCommand : ICommand<CreateUserOptions>
    {
        private readonly App app;

        public CreateUserCommand(App app)
        {
            this.app = app;
        }

        public void Run(CreateUserOptions options)
        {
            var userData = new CreateUserData
            {
                Name = options.Name,
                Password = options.Password,
                Role = UserRole.Parse(options.Role),
            };

            app.Users.Create(userData);
        }
    }
}
