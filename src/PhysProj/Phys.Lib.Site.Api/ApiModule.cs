using Autofac;
using Phys.Lib.Autofac;
using Phys.Lib.Site.Api.Pipeline;

namespace Phys.Lib.Site.Api
{
    public class ApiModule : Module
    {
        private LoggerFactory loggerFactory;
        private IConfiguration configuration;

        public ApiModule(LoggerFactory loggerFactory, IConfiguration config)
        {
            this.loggerFactory = loggerFactory;
            this.configuration = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new DataModule(configuration, loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.RegisterType<StatusCodeLoggingMiddlware>().AsSelf().SingleInstance();
        }
    }
}
