using Phys.Queue;
using System.Text;

namespace Phys.Tests.Queue
{
    internal static class QueueTestsExtensions
    {
        public static void Publish(this IMessageQueue queue, string queueName, string message)
        {
            queue.Publish(queueName, Encoding.UTF8.GetBytes(message));
        }
    }
}
