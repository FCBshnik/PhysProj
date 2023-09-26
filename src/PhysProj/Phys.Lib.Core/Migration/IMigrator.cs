using Phys.Shared;

namespace Phys.Lib.Core.Migration
{
    public interface IMigrator : INamed
    {
        IEnumerable<string> Sources { get; }

        IEnumerable<string> Destinations { get; }

        void Migrate(MigrationDto migration);
    }
}
