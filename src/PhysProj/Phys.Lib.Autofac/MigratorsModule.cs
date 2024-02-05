using Autofac;
using Phys.HistoryDb;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files.Storage;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Migration;
using Phys.Lib.Core.Search;
using Phys.Lib.Core.Users;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Core.Works.Migration;

namespace Phys.Lib.Autofac
{
    internal class MigratorsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
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
    }
}
