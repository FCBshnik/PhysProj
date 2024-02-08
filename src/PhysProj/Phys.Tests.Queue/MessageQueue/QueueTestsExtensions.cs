using System.Text;
using Phys.Shared.Queue.Broker;

namespace Phys.Tests.Queue.Queue
{
    internal static class QueueTestsExtensions
    {
        public static void Send(this IQueueBroker queue, string queueName, string message)
        {
            queue.Send(queueName, Encoding.UTF8.GetBytes(message));
        }
    }
}
