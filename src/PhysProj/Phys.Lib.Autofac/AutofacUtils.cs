using Autofac;
using Microsoft.Extensions.Hosting;
using Phys.Queue;
using Phys.Shared.Broker;
using Phys.Shared.EventBus;

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

        public static ContainerBuilder RegisterQueueConsumer<TConsumer, TMessage>(this ContainerBuilder builder) where TConsumer : IMessageConsumer<TMessage>
        {
            builder.RegisterType<TConsumer>()
                .As<IMessageConsumer<TMessage>>()
                .SingleInstance();

            builder.RegisterBuildCallback(c =>
            {
                var queueService = c.Resolve<BrokerRegistrarService>();
                var consumer = c.Resolve<IMessageConsumer<TMessage>>();
                queueService.AddConsumer(consumer);
            });

            return builder;
        }

        public static ContainerBuilder RegisterEventHandler<THandler, TEvent>(this ContainerBuilder builder) where THandler : IEventHandler<TEvent>
        {
            // in app can be many handlers for same event
            // register handlers as named
            var handlerName = typeof(THandler).FullName!;

            builder.RegisterType<THandler>()
                .Named<IEventHandler<TEvent>>(handlerName)
                .SingleInstance();

            builder.RegisterBuildCallback(c =>
            {
                var queueService = c.Resolve<BrokerRegistrarService>();
                var consumer = c.ResolveNamed<IEventHandler<TEvent>>(handlerName);
                queueService.AddHandler(consumer);
            });

            return builder;
        }
    }
}
