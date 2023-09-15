namespace Phys.Lib.Core.Migration
{
    public class MigrationTask
    {
        public required string Migrator { get; set; }

        public required string Source { get; set; }

        public required string Destination { get; set; }
    }
}
