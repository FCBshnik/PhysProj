using Phys.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationsExecutor : IMessageQueueConsumer<MigrationMessage>
    {
        private readonly IMigrationService migrationService;

        public MigrationsExecutor(IMigrationService migrationService)
        {
            this.migrationService = migrationService;
        }

        string IMessageQueueConsumer<MigrationMessage>.QueueName => QueueNames.Migrations;

        void IMessageQueueConsumer<MigrationMessage>.Consume(MigrationMessage message)
        {
            migrationService.Execute(message.Migration);
        }
    }
}
