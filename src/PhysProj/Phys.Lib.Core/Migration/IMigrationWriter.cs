using Phys.Shared;

namespace Phys.Lib.Core.Migration
{
    /// <summary>
    /// Writes values under migration to destination
    /// </summary>
    public interface IMigrationWriter<in T> : INamed
    {
        void Write(IEnumerable<T> values, MigrationDto.StatsDto stats);
    }
}
