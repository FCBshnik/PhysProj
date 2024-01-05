using CommandLine;

namespace Phys.Lib.Cli.Users
{
    [Verb("user-create")]
    public class CreateUserOptions
    {
        [Option("name", Required = true)]
        public required string Name { get; set; }

        [Option("password", Required = true)]
        public required string Password { get; set; }

        [Option("role", Required = true)]
        public required string Role { get; set; }
    }
}
