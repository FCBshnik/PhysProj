using Phys.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationsExecutor : IQueueConsumer<MigrationDto>
    {
        private readonly IMigrationService migrationService;

        public MigrationsExecutor(IMigrationService migrationService)
        {
            this.migrationService = migrationService;
        }

        string IQueueConsumer<MigrationDto>.QueueName => "migrations";

        void IQueueConsumer<MigrationDto>.Consume(MigrationDto message)
        {
            migrationService.Execute(message);
        }
    }
}
