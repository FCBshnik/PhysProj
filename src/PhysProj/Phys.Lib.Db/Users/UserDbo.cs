namespace Phys.Lib.Db.Users
{
    public class UserDbo
    {
        public required string Name { get; set; }

        public required string NameLowerCase { get; set; }

        public required string PasswordHash { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public bool HasRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

        public override string ToString() => $"{Name} ({string.Join(",", Roles)})";
    }
}
