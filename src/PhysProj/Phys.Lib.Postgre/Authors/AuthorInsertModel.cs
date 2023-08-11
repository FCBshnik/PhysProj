using SqlKata;

namespace Phys.Lib.Postgres.Authors
{
    internal class AuthorInsertModel
    {
        [Column("code")]
        public string Code { get; set; }
    }
}
