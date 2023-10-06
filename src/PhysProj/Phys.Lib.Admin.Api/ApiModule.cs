using Autofac;
using Phys.Lib.Admin.Api.Api.User;
using Phys.Lib.Admin.Api.Filters;
using Phys.Lib.Core;
using Phys.Lib.Core.Validation;
using Phys.Lib.Mongo;
using Phys.Lib.Postgres;
using Phys.Logging;
using Phys.Mongo.HistoryDb;
using Phys.Queue;
using Phys.RabbitMQ;
using RabbitMQ.Client;
using System.Reflection;
using Phys.Files;
using Phys.Files.Local;

namespace Phys.Lib.Admin.Api
{
    internal class ApiModule : Autofac.Module
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

            builder.RegisterModule(new MongoModule(configuration.GetConnectionString("mongo"), loggerFactory));
            builder.RegisterModule(new PostgresModule(configuration.GetConnectionString("postgres"), loggerFactory));
            builder.Register(c => new SystemFileStorage("local", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/files"), c.Resolve<ILogger<SystemFileStorage>>()))
                .As<IFileStorage>()
            .SingleInstance();
            builder.Register(c => new MongoHistoryDbFactory(configuration.GetConnectionString("mongo"), "phys-lib", "history-", loggerFactory))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new ValidationModule(Assembly.GetExecutingAssembly()));

            builder.Register(c => new ConnectionFactory { HostName = configuration.GetConnectionString("rabbitmq") })
                .As<IConnectionFactory>().SingleInstance();
            builder.RegisterType<RabbitQueue>()
                .As<IMessageQueue>().SingleInstance();
            builder.RegisterType<JsonQueue>()
                .As<IObjectQueue>().SingleInstance();

            builder.RegisterType<StatusCodeLoggingMiddlware>().AsSelf().SingleInstance();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<UserResolver>().InstancePerDependency();
            builder.Register(c => c.Resolve<UserResolver>().GetUser()).InstancePerDependency();
        }
    }
}