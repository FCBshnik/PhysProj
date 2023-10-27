namespace Phys.Lib.Core.Migration
{
    internal class LibraryMigrator : IMigrator
    {
        private static readonly List<string> orderNames = new List<string> { MigratorName.Authors, MigratorName.Files, MigratorName.Works };

        private readonly Lazy<List<IMigrator>> migrators;

        public LibraryMigrator(Lazy<IEnumerable<IMigrator>> migrators)
        {
            this.migrators = new Lazy<List<IMigrator>>(() => GetMigrators(migrators));
        }

        public IEnumerable<string> Sources => migrators.Value.SelectMany(m => m.Sources).Distinct();

        public IEnumerable<string> Destinations => migrators.Value.SelectMany(m => m.Destinations).Distinct();

        public string Name => MigratorName.Library;

        public void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            foreach (var migrator in migrators.Value)
                migrator.Migrate(migration, progress);
        }

        private List<IMigrator> GetMigrators(Lazy<IEnumerable<IMigrator>> migrators)
        {
            return migrators.Value
                .Where(m => orderNames.Contains(m.Name))
                .OrderBy(m => orderNames.IndexOf(m.Name))
                .ToList();
        }
    }
}
