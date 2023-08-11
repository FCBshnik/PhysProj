using Phys.Lib.Postgres.Utils;
using SqlKata;
using System.Linq.Expressions;

namespace Phys.Lib.Postgres.Files
{
    internal class FileModel
    {
        public static readonly string CodeColumn = GetColumn(i => i.Code);

        public static string GetColumn<T>(Expression<Func<FileModel, T>> property) => SchemaUtils.GetColumn(property);

        [Column("code")]
        public string Code { get; set; }

        [Column("format")]
        public string? Format { get; set; }

        [Column("size")]
        public long? Size { get; set; }

        [Ignore]
        public List<LinkModel> Links { get; set; } = new List<LinkModel>();

        public class LinkModel
        {
            public static readonly string FileCodeColumn = GetColumn(i => i.FileCode);
            public static readonly string TypeColumn = GetColumn(i => i.Type);
            public static readonly string PathColumn = GetColumn(i => i.Path);

            public static string GetColumn<T>(Expression<Func<LinkModel, T>> property) => SchemaUtils.GetColumn(property);

            [Column("file_code")]
            public string FileCode { get; set; }

            [Column("type")]
            public string Type { get; set; }

            [Column("path")]
            public string Path { get; set; }
        }
    }
}
