using Autofac;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Nest;
using Phys.Lib.Admin.Api.Api.User;
using Phys.Lib.Admin.Api.Filters;
using Phys.Lib.Core;
using Phys.Lib.Core.Validation;
using Phys.Lib.Files;
using Phys.Lib.Files.Local;
using Phys.Lib.Mongo;
using Phys.Shared.Logging;
using Phys.Shared.Mongo.HistoryDb;
using System.Reflection;

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
            var mongoUrl = configuration.GetConnectionString("mongo") ?? throw new ApplicationException();

            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new MongoModule(mongoUrl, loggerFactory));
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new ValidationModule(Assembly.GetExecutingAssembly()));

            builder.Register(c => new SystemFileStorage("local", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/files"), c.Resolve<ILogger<SystemFileStorage>>()))
                .As<IFileStorage>()
            .SingleInstance();

            builder.Register(c => new MongoHistoryDbFactory(mongoUrl, "physlib", "history-", loggerFactory))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterType<StatusCodeLoggingMiddlware>().AsSelf().SingleInstance();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<UserResolver>().InstancePerDependency();
            builder.Register(c => c.Resolve<UserResolver>().GetUser()).InstancePerDependency();
        }
    }
}