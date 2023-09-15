namespace Phys.Lib.Core.Migration
{
    public interface IMigrationService
    {
        MigrationDto Create(MigrationTask task);

        void Execute(MigrationDto migration);
    }
}
