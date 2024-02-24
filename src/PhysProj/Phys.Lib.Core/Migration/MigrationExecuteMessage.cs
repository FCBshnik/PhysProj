using Phys.Shared.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationExecuteMessage : IMessage
    {
        public string QueueName => QueueNames.MigrationsExecute;

        public required MigrationDto Migration { get; set; }
    }
}
