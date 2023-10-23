using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Db.Works;
using Phys.Shared;

namespace Phys.Lib.Core.Works
{
    internal class WorksMigrator : IMigrator
    {
        protected readonly List<IDbReader<WorkDbo>> readers;
        protected readonly List<IWorksDb> dbs;

        public WorksMigrator(IEnumerable<IDbReader<WorkDbo>> readers, IEnumerable<IWorksDb> dbs)
        {
            this.readers = readers.ToList();
            this.dbs = dbs.ToList();
        }

        public string Name => "works";

        public IEnumerable<string> Sources => readers.Select(x => x.Name);

        public IEnumerable<string> Destinations => dbs.Select(x => x.Name);

        public void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            var reader = readers.Find(r => string.Equals(r.Name, migration.Source, StringComparison.OrdinalIgnoreCase));
            if (reader == null)
                throw new PhysException($"reader '{migration.Source}' not found for '{typeof(WorkDbo)}' values");

            var db = dbs.Find(r => string.Equals(r.Name, migration.Destination, StringComparison.OrdinalIgnoreCase));
            if (db == null)
                throw new PhysException($"db '{migration.Destination}' not found for '{typeof(WorkDbo)}' values");

            var baseWriter = new WorksBaseWriter(db);
            var linksWriter = new WorksLinksWriter(db);

            // migrate work in two passess for support relational DBMS with db constrains because of works is self referenced
            // in first pass links to works excluded
            Migrator<WorkDbo>.Migrate(reader, baseWriter, migration, progress);
            // in second pass copy all links to works
            Migrator<WorkDbo>.Migrate(reader, linksWriter, migration, progress);
        }
    }
}
