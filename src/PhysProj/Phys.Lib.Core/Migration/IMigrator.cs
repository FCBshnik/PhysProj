using Phys.Shared;

namespace Phys.Lib.Core.Migration
{
    /// <summary>
    /// Migrates data from source to destination
    /// </summary>
    public interface IMigrator : INamed
    {
        /// <summary>
        /// List of sources available for migration
        /// </summary>
        IEnumerable<string> Sources { get; }

        /// <summary>
        /// List of destinations available for migration
        /// </summary>
        IEnumerable<string> Destinations { get; }

        void Migrate(MigrationDto migration, IProgress<MigrationDto> progress);
    }
}
