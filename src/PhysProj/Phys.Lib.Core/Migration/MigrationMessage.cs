using Phys.Shared.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationMessage : IQueueMessage
    {
        public string QueueName => QueueNames.Migrations;

        public required MigrationDto Migration { get; set; }
    }
}
