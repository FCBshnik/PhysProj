using Generator.Equals;

namespace Phys.Lib.Db.Users
{
    [Equatable]
    public sealed partial class UserDbo
    {
        public required string Name { get; set; }

        public required string NameLowerCase { get; set; }

        public string? PasswordHash { get; set; }

        [UnorderedEquality]
        public List<string> Roles { get; set; } = new List<string>();

        public bool HasRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

        public override string ToString() => $"{Name} ({string.Join(",", Roles)})";
    }
}
