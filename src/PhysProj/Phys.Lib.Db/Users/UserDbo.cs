namespace Phys.Lib.Db.Users
{
    public class UserDbo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string NameLowerCase { get; set; }

        public string PasswordHash { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public bool HasRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

        public override string ToString() => $"{Id} ({Name}, {string.Join(",", Roles)})";
    }
}
