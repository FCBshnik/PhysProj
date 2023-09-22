using Phys.Lib.Postgres.Utils;
using SqlKata;
using System.Linq.Expressions;

namespace Phys.Lib.Postgres.Works
{
    internal class WorkModel : WorkInsertModel, IPostgresModel
    {
        public static readonly string IdColumn = GetColumn(i => i.Id);
        public static readonly string CodeColumn = GetColumn(i => i.Code);
        public static readonly string LanguageColumn = GetColumn(i => i.Language);
        public static readonly string PublishColumn = GetColumn(i => i.Publish);
        public static readonly string OriginalCodeColumn = GetColumn(i => i.OriginalCode);

        public static string GetColumn<T>(Expression<Func<WorkModel, T>> property) => SchemaUtils.GetColumn(property);

        [Column("id")]
        public long Id { get; set; }

        [Column("publish")]
        public string? Publish { get; set; }

        [Column("language")]
        public string? Language { get; set; }

        [Column("original_code")]
        public string? OriginalCode { get; set; }

        [Ignore]
        public List<InfoModel> Infos { get; set; } = new List<InfoModel>();

        [Ignore]
        public List<SubWorkModel> SubWorks { get; set; } = new List<SubWorkModel>();

        [Ignore]
        public List<AuthorModel> Authors { get; set; } = new List<AuthorModel>();

        [Ignore]
        public List<FileModel> Files { get; set; } = new List<FileModel>();

        public class InfoModel
        {
            public static readonly string WorkCodeColumn = SchemaUtils.GetColumn<InfoModel, string>(i => i.WorkCode);
            public static readonly string InfoLanguageColumn = SchemaUtils.GetColumn<InfoModel, string>(i => i.Language);

            [Column("work_code")]
            public string WorkCode { get; set; }

            [Column("language")]
            public string Language { get; set; }

            [Column("name")]
            public string? Name { get; set; }

            [Column("description")]
            public string? Description { get; set; }
        }

        public class SubWorkModel
        {
            public static readonly string WorkCodeColumn = SchemaUtils.GetColumn<SubWorkModel, string>(i => i.WorkCode);
            public static readonly string SubWorkCodeColumn = SchemaUtils.GetColumn<SubWorkModel, string>(i => i.SubWorkCode);

            [Column("work_code")]
            public string WorkCode { get; set; }

            [Column("sub_work_code")]
            public string SubWorkCode { get; set; }
        }

        public class AuthorModel
        {
            public static readonly string WorkCodeColumn = SchemaUtils.GetColumn<AuthorModel, string>(i => i.WorkCode);
            public static readonly string AuthorCodeColumn = SchemaUtils.GetColumn<AuthorModel, string>(i => i.AuthorCode);

            [Column("work_code")]
            public string WorkCode { get; set; }

            [Column("author_code")]
            public string AuthorCode { get; set; }
        }

        public class FileModel
        {
            public static readonly string WorkCodeColumn = SchemaUtils.GetColumn<FileModel, string>(i => i.WorkCode);
            public static readonly string FileCodeColumn = SchemaUtils.GetColumn<FileModel, string>(i => i.FileCode);

            [Column("work_code")]
            public string WorkCode { get; set; }

            [Column("file_code")]
            public string FileCode { get; set; }
        }
    }
}
