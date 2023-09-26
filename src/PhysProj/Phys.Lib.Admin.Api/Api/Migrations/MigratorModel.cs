using Phys.Lib.Core.Migration;

namespace Phys.Lib.Admin.Api.Api.Migrations
{
    public class MigratorModel
    {
        public required string Name { get; set; }

        public required List<string> Sources { get; set; }

        public required List<string> Destinations { get; set; }

        public static MigratorModel Map(IMigrator migrator)
        {
            return new MigratorModel
            {
                Name = migrator.Name,
                Sources = migrator.Sources.ToList(),
                Destinations = migrator.Destinations.ToList(),
            };
        }
    }
}
