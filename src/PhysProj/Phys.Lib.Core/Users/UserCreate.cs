namespace Phys.Lib.Core.Users
{
    public record UserCreate
    {
        public required string Name { get; init; }

        public required string Password { get; init; }

        public required UserRole Role { get; init; }

        public string Code => Core.Code.FromString(Name);

        public string NameLowerCase => Name.ToLowerInvariant();
    }
}
