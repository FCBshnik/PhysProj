using Autofac;
using Phys.Lib.Autofac;
using Phys.Lib.Site.Api.Pipeline;

namespace Phys.Lib.Site.Api
{
    public class ApiModule : Module
    {
        private LoggerFactory loggerFactory;
        private ConfigurationManager config;

        public ApiModule(LoggerFactory loggerFactory, ConfigurationManager config)
        {
            this.loggerFactory = loggerFactory;
            this.config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new DataModule(config, loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.RegisterType<StatusCodeLoggingMiddlware>().AsSelf().SingleInstance();
        }
    }
}
