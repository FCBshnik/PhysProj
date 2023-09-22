using Phys.Lib.Postgres.Utils;
using SqlKata;
using System.Linq.Expressions;

namespace Phys.Lib.Postgres.Authors
{
    internal class AuthorModel : AuthorInsertModel, IPostgresModel
    {
        public static readonly string IdColumn = GetColumn(i => i.Id);
        public static readonly string CodeColumn = GetColumn(i => i.Code);
        public static readonly string BornColumn = GetColumn(i => i.Born);
        public static readonly string DiedColumn = GetColumn(i => i.Died);

        public static string GetColumn<T>(Expression<Func<AuthorModel, T>> property) => SchemaUtils.GetColumn(property);

        [Column("id")]
        public long Id { get; set; }

        [Column("born")]
        public string Born { get; set; }

        [Column("died")]
        public string Died { get; set; }

        [Ignore]
        public List<InfoModel> Infos { get; set; } = new List<InfoModel>();

        public class InfoModel
        {
            public static readonly string AuthorCodeColumn = SchemaUtils.GetColumn<InfoModel, string>(i => i.AuthorCode);
            public static readonly string LanguageColumn = SchemaUtils.GetColumn<InfoModel, string>(i => i.Language);

            [Column("author_code")]
            public string AuthorCode { get; set; }

            [Column("language")]
            public string Language { get; set; }

            [Column("full_name")]
            public string FullName { get; set; }

            [Column("description")]
            public string Description { get; set; }
        }
    }
}
