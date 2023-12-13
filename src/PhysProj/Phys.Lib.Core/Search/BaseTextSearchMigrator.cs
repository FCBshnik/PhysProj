using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Search;

namespace Phys.Lib.Core.Search
{
    internal abstract class BaseTextSearchMigrator<TDbObject, TSearchObject> : IMigrator
    {
        protected readonly List<IDbReader<TDbObject>> readers;
        protected readonly ITextSearch<TSearchObject> textSearch;

        public BaseTextSearchMigrator(string name, IEnumerable<IDbReader<TDbObject>> readers, ITextSearch<TSearchObject> textSearch)
        {
            Name = name;
            this.readers = readers.ToList();
            this.textSearch = textSearch;
        }

        public IEnumerable<string> Sources => readers.Select(r => r.Name);

        public IEnumerable<string> Destinations => new[] { "search" };

        public string Name { get; }

        public virtual void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            IDbReaderResult<TDbObject> result = null!;

            var source = readers.First(r => r.Name == migration.Source);
            textSearch.Reset();

            do
            {
                result = source.Read(new DbReaderQuery(100, result?.Cursor));
                textSearch.Index(result.Values.Where(Use).Select(Map).ToList());
                migration.Stats.Updated += result.Values.Count;
                progress.Report(migration);
            } while (!result.IsCompleted);
        }

        protected abstract bool Use(TDbObject value);

        protected abstract TSearchObject Map(TDbObject value);
    }
}
