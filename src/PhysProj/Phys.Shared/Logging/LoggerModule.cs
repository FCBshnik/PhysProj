using Autofac;
using Microsoft.Extensions.Logging;

namespace Phys.Shared.Logging
{
    public class LoggerModule : Module
    {
        private readonly ILoggerFactory loggerFactory;

        public LoggerModule(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => loggerFactory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
        }
    }
}
