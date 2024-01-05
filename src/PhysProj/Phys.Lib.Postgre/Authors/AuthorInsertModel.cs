using SqlKata;

namespace Phys.Lib.Postgres.Authors
{
    internal class AuthorInsertModel
    {
        [Column("code")]
        public required string Code { get; set; }
    }
}
