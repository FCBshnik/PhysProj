using CommandLine;

namespace Phys.Lib.Cli.Users
{
    [Verb("user-create")]
    public class CreateUserOptions
    {
        [Option("name", Required = true)]
        public string Name { get; set; }

        [Option("password", Required = true)]
        public string Password { get; set; }

        [Option("role", Required = true)]
        public string Role { get; set; }
    }
}
