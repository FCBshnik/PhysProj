using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Search;

namespace Phys.Lib.Core.Search
{
    internal class AuthorsTextSearchMigrator : IMigrator
    {
        private readonly IEnumerable<IAuthorsDb> authorsDbs;
        private readonly ITextSearch<AuthorTso> textSearch;

        public AuthorsTextSearchMigrator(string name, IEnumerable<IAuthorsDb> authorsDbs, ITextSearch<AuthorTso> textSearch)
        {
            Name = name;
            this.authorsDbs = authorsDbs;
            this.textSearch = textSearch;
        }

        public string Name { get; }

        public IEnumerable<string> Sources => authorsDbs.Select(r => r.Name);

        public IEnumerable<string> Destinations => new[] { "search" };

        public virtual void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            IDbReaderResult<AuthorDbo> result = null!;

            var source = authorsDbs.First(r => r.Name == migration.Source);
            textSearch.Reset(Language.AllAsStrings);

            do
            {
                result = source.Read(new DbReaderQuery(100, result?.Cursor));
                var values = result.Values.Where(Use).Select(Map).ToList();
                textSearch.Index(values);
                migration.Stats.Updated += result.Values.Count;
                progress.Report(migration);
            } while (!result.IsCompleted);
        }

        private AuthorTso Map(AuthorDbo value)
        {
            return new AuthorTso
            {
                Code = value.Code,
                Names = value.Infos.ToDictionary(i => i.Language, i => i.FullName),
            };
        }

        private bool Use(AuthorDbo value)
        {
            return value.Infos.Count > 0;
        }
    }
}
