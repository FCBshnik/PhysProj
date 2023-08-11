using Phys.Lib.Postgres.Utils;
using SqlKata;
using System.Linq.Expressions;

namespace Phys.Lib.Postgres.Users
{
    internal class UserModel : UserInsertModel
    {
        public static readonly string IdColumn = GetColumn(i => i.Id);
        public static readonly string NameLowerCaseColumn = GetColumn(i => i.NameLowerCase);
        public static readonly string RolesColumn = GetColumn(i => i.Roles);
        public static readonly string PasswordHashColumn = GetColumn(i => i.PasswordHash);

        public static string GetColumn<T>(Expression<Func<UserModel, T>> property) => SchemaUtils.GetColumn(property);

        [Column("id")]
        public long Id { get; set; }

        [Column("roles")]
        public string[] Roles { get; set; }
    }
}
