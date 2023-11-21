using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Migrations;
using Phys.Shared.Search;

namespace Phys.Lib.Core.Search
{
    internal abstract class BaseTextSearchMigrator<TDbo, TTso> : IMigrator
    {
        protected readonly List<IDbReader<TDbo>> readers;
        private readonly ITextSearch<TTso> textSearch;

        public BaseTextSearchMigrator(string name, IEnumerable<IDbReader<TDbo>> readers, ITextSearch<TTso> textSearch)
        {
            Name = name;
            this.readers = readers.ToList();
            this.textSearch = textSearch;
        }

        public IEnumerable<string> Sources => readers.Select(r => r.Name);

        public IEnumerable<string> Destinations => new[] { Name };

        public string Name { get; }

        public void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            IDbReaderResult<TDbo> result = null!;

            var source = readers.First(r => r.Name == migration.Source);
            textSearch.Reset();

            do
            {
                result = source.Read(new DbReaderQuery(100, result?.Cursor));
                textSearch.Add(result.Values.Select(Map).ToList());
                migration.Stats.Updated += result.Values.Count;
                progress.Report(migration);
            } while (!result.IsCompleted);
        }

        protected abstract TTso Map(TDbo value);
    }
}
