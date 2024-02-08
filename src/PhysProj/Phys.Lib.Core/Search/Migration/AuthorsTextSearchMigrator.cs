using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Authors;
using Phys.Lib.Search;

namespace Phys.Lib.Core.Search.Migration
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
            var source = authorsDbs.First(r => r.Name == migration.Source);
            textSearch.Reset(Language.AllAsStrings);

            foreach (var authors in source.Read(100))
            {
                var values = authors.Where(Use).Select(Map).ToList();
                textSearch.Index(values);
                migration.Stats.Updated += values.Count;
                progress.Report(migration);
            }
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
