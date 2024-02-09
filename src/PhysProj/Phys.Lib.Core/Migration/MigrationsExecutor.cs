using Phys.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationsExecutor : IMessageConsumer<MigrationMessage>
    {
        private readonly IMigrationService migrationService;

        public MigrationsExecutor(IMigrationService migrationService)
        {
            this.migrationService = migrationService;
        }

        string IMessageConsumer<MigrationMessage>.QueueName => QueueNames.Migrations;

        void IMessageConsumer<MigrationMessage>.Consume(MigrationMessage message)
        {
            migrationService.Execute(message.Migration);
        }
    }
}
