using SqlKata;

namespace Phys.Lib.Postgres.Users
{
    internal class UserInsertModel
    {
        [Column("name")]
        public required string Name { get; set; }

        [Column("name_lower_case")]
        public required string NameLowerCase { get; set; }

        [Column("password_hash")]
        public string? PasswordHash { get; set; }
    }
}
