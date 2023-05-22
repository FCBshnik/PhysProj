namespace Phys.Lib.Core.Users
{
    public class UserRole
    {
        public static readonly UserRole User = new UserRole("user");
        public static readonly UserRole Admin = new UserRole("admin");

        private static readonly Dictionary<string, UserRole> roles = new[] { User, Admin }.ToDictionary(r => r.Code, StringComparer.OrdinalIgnoreCase);

        public string Code { get; }

        private UserRole(string role)
        {
            Code = role;
        }

        public override string ToString() => Code;

        public static UserRole Parse(string role)
        {
            if (!roles.ContainsKey(role))
                throw new ArgumentException($"invalid role '{role}'");

            return roles[role];
        }
    }
}
