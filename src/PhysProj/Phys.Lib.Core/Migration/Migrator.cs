using Phys.Lib.Db.Reader;
using Phys.Shared;

namespace Phys.Lib.Core.Migration
{
    internal class Migrator<T> : IMigrator
    {
        private readonly List<IDbReader<T>> readers;
        private readonly List<IDbWriter<T>> writers;

        public Migrator(string name, IEnumerable<IDbReader<T>> readers, IEnumerable<IDbWriter<T>> writers)
        {
            Name = name;
            this.readers = readers.ToList();
            this.writers = writers.ToList();
        }

        public string Name { get; }

        public void Migrate(IDbReader<T> source, IDbWriter<T> destination, MigrationDto migration)
        {
            IDbReaderResult<T> result = null!;

            do
            {
                result = source.Read(new DbReaderQuery(100, result?.Cursor));
                destination.Write(result.Values);
                migration.MigratedCount += result.Values.Count;
            } while (!result.IsCompleted);
        }

        public void Migrate(MigrationDto migration)
        {
            var reader = readers.Find(r => string.Equals(r.Name, migration.Source, StringComparison.OrdinalIgnoreCase));
            if (reader == null)
                throw new PhysException($"reader '{migration.Source}' not found for '{typeof(T)}' values");

            var writer = writers.Find(r => string.Equals(r.Name, migration.Destination, StringComparison.OrdinalIgnoreCase));
            if (writer == null)
                throw new PhysException($"writer '{migration.Destination}' not found for '{typeof(T)}' values");

            Migrate(reader, writer, migration);
        }
    }
}
