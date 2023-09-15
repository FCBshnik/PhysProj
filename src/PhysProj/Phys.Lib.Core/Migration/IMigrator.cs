using Phys.Shared;

namespace Phys.Lib.Core.Migration
{
    public interface IMigrator : INamed
    {
        void Migrate(MigrationDto migration);
    }
}
