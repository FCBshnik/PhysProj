using Autofac;
using Phys.Lib.Autofac;
using Phys.Lib.Core.Authors.Cache;
using Phys.Lib.Core.Files.Cache;
using Phys.Lib.Core.Files.Events;
using Phys.Lib.Core.Works.Cache;
using Phys.Lib.Core.Works.Events;
using Phys.Lib.Site.Api.Pipeline;

namespace Phys.Lib.Site.Api
{
    public class SiteApiModule : Module
    {
        private LoggerFactory loggerFactory;
        private IConfiguration configuration;

        public SiteApiModule(LoggerFactory loggerFactory, IConfiguration configuration)
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

            builder.RegisterHostedService<WorksCacheEventsHandler>();
            builder.RegisterEventHandler<WorksCacheEventsHandler, WorksCacheInvalidatedEvent>();
            builder.RegisterEventHandler<WorksCacheEventsHandler, WorkUpdatedEvent>();

            builder.RegisterHostedService<AuthorsCacheEventsHandler>();
            builder.RegisterEventHandler<AuthorsCacheEventsHandler, AuthorsCacheInvalidatedEvent>();
            builder.RegisterEventHandler<AuthorsCacheEventsHandler, AuthorUpdatedEvent>();

            builder.RegisterHostedService<FilesCacheEventsHandler>();
            builder.RegisterEventHandler<FilesCacheEventsHandler, FilesCacheInvalidatedEvent>();
            builder.RegisterEventHandler<FilesCacheEventsHandler, FileUpdatedEvent>();
        }
    }
}
