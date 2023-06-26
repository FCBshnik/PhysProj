using Phys.Lib.Core.Utils;

namespace Phys.Lib.Core.Users
{
    public class UserDbo
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string NameLowerCase { get; set; }

        public string PasswordHash { get; set; }

        public List<string> Roles { get; set; } = CollectionUtils.Empty<string>();

        public bool HasRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

        public override string ToString() => $"{Id} ({Name}, {string.Join(",", Roles)})";
    }
}
