using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Migration;
using Phys.Lib.Autofac;

namespace Phys.Lib.App
{
    internal class AppModule : Module
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IConfiguration configuration;

        public AppModule(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this.loggerFactory = loggerFactory;
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new DataModule(configuration, loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.RegisterQueueConsumer<MigrationsExecutor, MigrationDto>();
        }
    }
}
