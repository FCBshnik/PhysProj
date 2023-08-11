using SqlKata;

namespace Phys.Lib.Postgres.Works
{
    internal class WorkInsertModel
    {
        [Column("code")]
        public string Code { get; set; }
    }
}
