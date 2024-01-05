using SqlKata;

namespace Phys.Lib.Postgres.Files
{
    internal class FileInsertModel
    {
        [Column("code")]
        public required string Code { get; set; }

        [Column("format")]
        public required string Format { get; set; }

        [Column("size")]
        public long Size { get; set; }
    }
}
