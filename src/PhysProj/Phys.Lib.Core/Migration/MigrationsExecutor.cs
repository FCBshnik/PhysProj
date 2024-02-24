using Phys.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationsExecutor : IMessageConsumer<MigrationExecuteMessage>
    {
        private readonly IMigrationService migrationService;

        public MigrationsExecutor(IMigrationService migrationService)
        {
            this.migrationService = migrationService;
        }

        string IMessageConsumer<MigrationExecuteMessage>.QueueName => QueueNames.MigrationsExecute;

        void IMessageConsumer<MigrationExecuteMessage>.Consume(MigrationExecuteMessage message)
        {
            migrationService.Execute(message.Migration);
        }
    }
}
