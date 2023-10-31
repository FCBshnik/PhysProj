using Autofac;
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
            builder.RegisterType<StatusCodeLoggingMiddlware>().AsSelf().SingleInstance();
        }
    }
}
