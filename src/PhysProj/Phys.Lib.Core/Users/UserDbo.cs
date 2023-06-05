using Phys.Lib.Core.Utils;

namespace Phys.Lib.Core.Users
{
    public class UserDbo
    {
        public string Id { get; init; }

        public string Code { get; init; }

        public string Name { get; init; }

        public string NameLowerCase { get; init; }

        public string PasswordHash { get; init; }

        public List<string> Roles { get; init; } = CollectionUtils.Empty<string>();

        public bool HasRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

        public override string ToString() => $"{Id} ({Name}, {string.Join(",", Roles)})";
    }
}
