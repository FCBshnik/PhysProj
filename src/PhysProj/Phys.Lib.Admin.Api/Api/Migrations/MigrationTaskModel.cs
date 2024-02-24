using Phys.Lib.Core.Migration;

namespace Phys.Lib.Admin.Api.Api.Migrations
{
    public class MigrationTaskModel
    {
        public required string Migrator { get; set; }

        public required string Source { get; set; }

        public required string Destination { get; set; }

        public MigrationParams Map()
        {
            return new MigrationParams
            {
                Migrator = Migrator,
                Source = Source,
                Destination = Destination,
            };
        }
    }
}
