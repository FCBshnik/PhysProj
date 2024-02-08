using Phys.Lib.Core.Migration;
using Phys.Lib.Db;
using Phys.Lib.Search;

namespace Phys.Lib.Core.Search.Migration
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
            var source = readers.First(r => r.Name == migration.Source);
            textSearch.Reset(Language.AllAsStrings);

            foreach (var values in source.Read(100))
            {
                textSearch.Index(values.Where(Use).Select(Map).ToList());
                migration.Stats.Updated += values.Count;
                progress.Report(migration);
            }
        }

        protected abstract bool Use(TDbObject value);

        protected abstract TSearchObject Map(TDbObject value);
    }
}
