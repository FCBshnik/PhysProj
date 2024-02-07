using Phys.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationsExecutor : IQueueConsumer<MigrationMessage>
    {
        private readonly IMigrationService migrationService;

        public MigrationsExecutor(IMigrationService migrationService)
        {
            this.migrationService = migrationService;
        }

        string IQueueConsumer<MigrationMessage>.QueueName => QueueNames.Migrations;

        void IQueueConsumer<MigrationMessage>.Consume(MigrationMessage message)
        {
            migrationService.Execute(message.Migration);
        }
    }
}
