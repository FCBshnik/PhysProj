using Phys.HistoryDb;

namespace Phys.Lib.Core.Migration
{
    public interface IMigrationService
    {
        MigrationDto Create(MigrationParams task);

        MigrationDto Get(string id);

        void Execute(MigrationDto migration);

        List<MigrationDto> ListHistory(HistoryDbQuery query);

        List<IMigrator> ListMigrators();
    }
}
