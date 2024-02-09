using Phys.Shared.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationMessage : IMessage
    {
        public string QueueName => QueueNames.Migrations;

        public required MigrationDto Migration { get; set; }
    }
}
