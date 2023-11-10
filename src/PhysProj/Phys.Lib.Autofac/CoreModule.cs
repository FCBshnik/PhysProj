using Autofac;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Files.Storage;
using Phys.Lib.Core.Migration;
using Phys.Lib.Core.Users;
using Phys.Lib.Core.Validation;
using Phys.Lib.Core.Works;
using Phys.Lib.Db.Users;
using Phys.HistoryDb;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;
using Phys.Lib.Core.Library;

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

            RegisterServices(builder);

            // override last db implementations by decorator which selects main db
            builder.RegisterType<MainUsersDb>().As<IUsersDb>().SingleInstance();
            builder.RegisterType<MainAuthorsDb>().As<IAuthorsDb>().SingleInstance();
            builder.RegisterType<MainWorksDb>().As<IWorksDb>().SingleInstance();
            builder.RegisterType<FilesDbs>().As<IFilesDb>().SingleInstance();

            RegisterMigrators(builder);
        }

        private static void RegisterMigrators(ContainerBuilder builder)
        {
            builder.Register(c => c.Resolve<IHistoryDbFactory>().Create<MigrationDto>("migrations"))
                .As<IHistoryDb<MigrationDto>>()
                .SingleInstance();

            builder.Register(c => c.Resolve<IEnumerable<IUsersDb>>().Where(d => d.Name != DbName.Main).Select(db => new UsersWriter(db)))
                .As<IEnumerable<IMigrationWriter<UserDbo>>>()
                .SingleInstance();
            builder.Register(c => c.Resolve<IEnumerable<IAuthorsDb>>().Where(d => d.Name != DbName.Main).Select(db => new AuthorsWriter(db)))
                .As<IEnumerable<IMigrationWriter<AuthorDbo>>>()
                .SingleInstance();
            builder.Register(c => c.Resolve<IEnumerable<IFilesDb>>().Where(d => d.Name != DbName.Main).Select(db => new FilesWriter(db)))
                .As<IEnumerable<IMigrationWriter<FileDbo>>>()
                .SingleInstance();
            builder.RegisterType<Migrator<UserDbo>>().WithParameter(TypedParameter.From(MigratorName.Users))
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<Migrator<AuthorDbo>>().WithParameter(TypedParameter.From(MigratorName.Authors))
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<Migrator<FileDbo>>().WithParameter(TypedParameter.From(MigratorName.Files))
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<WorksMigrator>()
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<FilesContentMigrator>()
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<LibraryMigrator>()
                .As<IMigrator>()
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
                .RegisterService<LibraryService, ILibraryService>();
        }
    }
}
