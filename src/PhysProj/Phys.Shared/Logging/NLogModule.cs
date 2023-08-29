using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Phys.Shared.Logging
{
    public class NLogModule : Module
    {
        private readonly ILoggerFactory loggerFactory;

        public NLogModule(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            loggerFactory.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true,
                ReplaceLoggerFactory = true,
            });
            builder.Register(c => loggerFactory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
        }
    }
}
