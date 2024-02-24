using Autofac;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Files.Storage;
using Phys.Lib.Core.Migration;
using Phys.Lib.Core.Users;
using Phys.Lib.Core.Validation;
using Phys.Lib.Core.Works;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;
using Phys.Lib.Core.Stats;
using Phys.Lib.Core.Search;
using Phys.Shared.Broker;
using Phys.Lib.Core.Works.Cache;
using Phys.Lib.Core.Authors.Cache;
using Phys.Lib.Core.Files.Cache;
using Phys.Queue;
using Phys.Shared.EventBus;

namespace Phys.Lib.Autofac
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var childCtx = c.Resolve<Func<IComponentContext>>();
                return new Validator(t => childCtx().Resolve(t));
            }).As<IValidator>().SingleInstance();

            builder.RegisterModule(new ValidationModule(System.Reflection.Assembly.GetEntryAssembly()!));
            builder.RegisterModule(new ValidationModule(ThisAssembly));
            builder.RegisterModule(new MigratorsModule());

            RegisterServices(builder);

            // override existing db implementations by decorators which selects main db
            builder.RegisterType<MainUsersDb>().As<IUsersDb>().SingleInstance();
            builder.RegisterType<MainAuthorsDb>().As<IAuthorsDb>().SingleInstance();
            builder.RegisterType<MainWorksDb>().As<IWorksDb>().SingleInstance();
            builder.RegisterType<MainFilesDb>().As<IFilesDb>().SingleInstance();

            // brokers
            builder.RegisterType<JsonMessageBroker>()
                .As<IMessageQueue>()
                .As<IEventBus>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(Core.EventNames).Assembly)
                .AssignableTo<IEvent>()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetProperties().Length == 1)
                .As<IEvent>()
                .SingleInstance();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterService<UsersService, IUsersService>()
                .RegisterService<AuthorsSearch, IAuthorsSearch>()
                .RegisterService<AuthorsEditor, IAuthorsEditor>()
                .RegisterService<WorksSearch, IWorksSearch>()
                .RegisterService<WorksEditor, IWorksEditor>()
                .RegisterService<FilesSearch, IFilesSearch>()
                .RegisterService<FilesEditor, IFilesEditor>()
                .RegisterService<FileStoragesService, IFileStorages>()
                .RegisterService<FileDownloadService, IFileDownloadService>()
                .RegisterService<MigrationService, IMigrationService>()
                .RegisterService<StatService, IStatService>()
                .RegisterService<SearchService, ISearchService>()
                .RegisterService<WorksCache, IWorksCache>()
                .RegisterService<AuthorsCache, IAuthorsCache>()
                .RegisterService<FilesCache, IFilesCache>()
                .RegisterHostedService<BrokerRegistrarService>();
        }
    }
}
