using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core;
using Phys.Lib.Mongo;
using Phys.Lib.Postgres;
using Phys.Shared.Logging;

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
            var mongoUrl = config.GetConnectionString("mongo");
            if (mongoUrl != null)
                builder.RegisterModule(new MongoModule(mongoUrl, loggerFactory));
            var postgresUrl = config.GetConnectionString("postgres");
            if (postgresUrl != null)
                builder.RegisterModule(new PostgresModule(postgresUrl, loggerFactory));

            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(ICommand<>))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
