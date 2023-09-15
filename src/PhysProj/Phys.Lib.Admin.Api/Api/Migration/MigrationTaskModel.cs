using Phys.Lib.Core.Migration;

namespace Phys.Lib.Admin.Api.Api.Migration
{
    public class MigrationTaskModel
    {
        public required string Migrator { get; set; }

        public required string Source { get; set; }

        public required string Destination { get; set; }

        public MigrationTask Map()
        {
            return new MigrationTask
            {
                Migrator = Migrator,
                Source = Source,
                Destination = Destination,
            };
        }
    }
}
