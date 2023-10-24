using Phys.Shared;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Core.Migration
{
    internal class Migrator<T> : IMigrator
    {
        protected readonly List<IDbReader<T>> readers;
        protected readonly List<IMigrationWriter<T>> writers;

        public Migrator(string name, IEnumerable<IDbReader<T>> readers, IEnumerable<IMigrationWriter<T>> writers)
        {
            Name = name;
            this.readers = readers.ToList();
            this.writers = writers.ToList();
        }

        public string Name { get; }

        public IEnumerable<string> Sources => readers.Select(x => x.Name);

        public IEnumerable<string> Destinations => writers.Select(x => x.Name);

        public static void Migrate(IDbReader<T> source, IMigrationWriter<T> destination, MigrationDto migration, IProgress<MigrationDto> progress)
        {
            IDbReaderResult<T> result = null!;

            do
            {
                result = source.Read(new DbReaderQuery(100, result?.Cursor));
                destination.Write(result.Values, migration.Stats);
                progress.Report(migration);
            } while (!result.IsCompleted);
        }

        public virtual void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            var reader = readers.Find(r => string.Equals(r.Name, migration.Source, StringComparison.OrdinalIgnoreCase));
            if (reader == null)
                throw new PhysException($"reader '{migration.Source}' not found for '{typeof(T)}' values");

            var writer = writers.Find(r => string.Equals(r.Name, migration.Destination, StringComparison.OrdinalIgnoreCase));
            if (writer == null)
                throw new PhysException($"writer '{migration.Destination}' not found for '{typeof(T)}' values");

            Migrator<T>.Migrate(reader, writer, migration, progress);
        }
    }
}
