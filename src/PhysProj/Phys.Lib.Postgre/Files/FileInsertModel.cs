using SqlKata;

namespace Phys.Lib.Postgres.Files
{
    internal class FileInsertModel
    {
        [Column("code")]
        public string Code { get; set; }

        [Column("format")]
        public string? Format { get; set; }

        [Column("size")]
        public long Size { get; set; }
    }
}
