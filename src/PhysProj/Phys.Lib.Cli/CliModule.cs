using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Autofac;

namespace Phys.Lib.Cli
{
    internal class CliModule : Module
    {
        private readonly IConfiguration config;
        private readonly LoggerFactory loggerFactory;

        public CliModule(IConfiguration config, LoggerFactory loggerFactory)
        {
            this.config = config;
            this.loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new DataModule(config, loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(ICommand<>))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
