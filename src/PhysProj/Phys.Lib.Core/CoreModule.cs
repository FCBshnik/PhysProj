using Autofac;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Files.Storage;
using Phys.Lib.Core.Migration;
using Phys.Lib.Core.Users;
using Phys.Lib.Core.Utils;
using Phys.Lib.Core.Validation;
using Phys.Lib.Core.Works;
using Phys.Lib.Db.Users;
using Phys.Shared.HistoryDb;
using System.Runtime.CompilerServices;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;

[assembly: InternalsVisibleTo("Phys.Lib.Tests.Unit")]
[assembly: InternalsVisibleTo("Phys.Lib.Tests.Db")]

namespace Phys.Lib.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Validator>()
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterService<UsersService, IUsersService>()
                .RegisterService<AuthorsSearch, IAuthorsSearch>()
                .RegisterService<AuthorsEditor, IAuthorsEditor>()
                .RegisterService<WorksSearch, IWorksSearch>()
                .RegisterService<WorksEditor, IWorksEditor>()
                .RegisterService<FileStoragesService, IFileStoragesService>()
                .RegisterService<FilesService, IFilesService>()
                .RegisterService<MigrationService, IMigrationService>();

            builder.RegisterType<UsersDbs>().As<IUsersDbs>().SingleInstance();
            builder.RegisterType<AuthorsDbs>().As<IAuthorsDbs>().SingleInstance();
            builder.RegisterType<WorksDbs>().As<IWorksDbs>().SingleInstance();
            builder.RegisterType<FilesDbs>().As<IFilesDbs>().SingleInstance();

            builder.Register(c => c.Resolve<IHistoryDbFactory>().Create<MigrationDto>("migrations"))
                .As<IHistoryDb<MigrationDto>>()
                .SingleInstance();
            builder.Register(c => c.Resolve<IEnumerable<IUsersDb>>().Select(db => new UsersWriter(db)))
                .As<IEnumerable<IDbWriter<UserDbo>>>()
                .SingleInstance();
            builder.Register(c => c.Resolve<IEnumerable<IAuthorsDb>>().Select(db => new AuthorsWriter(db)))
                .As<IEnumerable<IDbWriter<AuthorDbo>>>()
                .SingleInstance();
            builder.Register(c => c.Resolve<IEnumerable<IFilesDb>>().Select(db => new FilesWriter(db)))
                .As<IEnumerable<IDbWriter<FileDbo>>>()
                .SingleInstance();
            builder.RegisterType<Migrator<UserDbo>>().WithParameter(TypedParameter.From("users"))
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<Migrator<AuthorDbo>>().WithParameter(TypedParameter.From("authors"))
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<Migrator<FileDbo>>().WithParameter(TypedParameter.From("files"))
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<WorksMigrator>()
                .As<IMigrator>()
                .SingleInstance();

            builder.RegisterModule(new ValidationModule(ThisAssembly));
        }
    }
}
