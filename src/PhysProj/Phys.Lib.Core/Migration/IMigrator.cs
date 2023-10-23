using Phys.Shared;

namespace Phys.Lib.Core.Migration
{
    /// <summary>
    /// Migrates data from source to destination
    /// </summary>
    public interface IMigrator : INamed
    {
        IEnumerable<string> Sources { get; }

        IEnumerable<string> Destinations { get; }

        void Migrate(MigrationDto migration, IProgress<MigrationDto> progress);
    }
}
