using SqlKata;

namespace Phys.Lib.Postgres.Works
{
    internal class WorkInsertModel
    {
        [Column("code")]
        public required string Code { get; set; }
    }
}
