using Autofac;
using Phys.Lib.Admin.Api.Api.User;
using Phys.Lib.Admin.Api.Filters;
using Phys.Lib.Autofac;

namespace Phys.Lib.Admin.Api
{
    internal class ApiModule : Module
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IConfiguration configuration;

        public ApiModule(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this.loggerFactory = loggerFactory;
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new DataModule(configuration, loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.RegisterType<StatusCodeLoggingMiddlware>().AsSelf().SingleInstance();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<UserResolver>().InstancePerDependency();
            builder.Register(c => c.Resolve<UserResolver>().GetUser()).InstancePerDependency();
        }
    }
}