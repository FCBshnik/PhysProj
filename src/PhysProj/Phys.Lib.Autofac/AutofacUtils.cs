using Autofac;
using Microsoft.Extensions.Hosting;
using Phys.Queue;
using Phys.Shared.Queue;

namespace Phys.Lib.Autofac
{
    public static class AutofacUtils
    {
        public static ContainerBuilder RegisterService<S, I>(this ContainerBuilder builder)
            where S : notnull
            where I : notnull
        {
            builder.RegisterType<S>().As<I>().SingleInstance();
            return builder;
        }

        public static ContainerBuilder RegisterHostedService<T>(this ContainerBuilder builder)
            where T : IHostedService
        {
            builder.RegisterType<T>().As<T>().As<IHostedService>().SingleInstance();
            return builder;
        }

        public static ContainerBuilder RegisterQueueConsumer<TConsumer, TMessage>(this ContainerBuilder builder) where TConsumer : IMessageQueueConsumer<TMessage>
        {
            builder.RegisterType<TConsumer>()
                .As<IMessageQueueConsumer<TMessage>>()
                .SingleInstance();

            builder.RegisterBuildCallback(c =>
            {
                var queueService = c.Resolve<QueueHostedService>();
                var consumer = c.Resolve<IMessageQueueConsumer<TMessage>>();
                queueService.AddConsumer(consumer);
            });

            return builder;
        }
    }
}
