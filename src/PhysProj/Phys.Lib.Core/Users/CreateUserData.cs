namespace Phys.Lib.Core.Users
{
    public record CreateUserData
    {
        public required string Name { get; init; }

        public required string Password { get; init; }

        public required UserRole Role { get; init; }
    }
}
