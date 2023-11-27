using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Search;

namespace Phys.Lib.Core.Search
{
    internal class AuthorsTextSearchMigrator : BaseTextSearchMigrator<AuthorDbo, AuthorTso>
    {
        public AuthorsTextSearchMigrator(string name, IEnumerable<IDbReader<AuthorDbo>> readers, ITextSearch<AuthorTso> textSearch) : base(name, readers, textSearch)
        {
        }

        protected override AuthorTso Map(AuthorDbo value)
        {
            return new AuthorTso
            {
                Code = value.Code,
                Names = value.Infos.ToDictionary(i => i.Language, i => i.FullName),
            };
        }

        protected override bool Use(AuthorDbo value)
        {
            return value.Infos.Count > 0;
        }
    }
}
