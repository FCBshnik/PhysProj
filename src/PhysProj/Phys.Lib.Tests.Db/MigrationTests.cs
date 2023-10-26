using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Autofac;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Migration;
using Phys.Lib.Core.Users;
using Phys.Lib.Core.Works;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;
using Phys.Mongo.HistoryDb;
using Phys.NLog;
using Phys.Utils;
using Shouldly;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace Phys.Lib.Tests.Db
{
    public class MigrationTests : IDisposable
    {
        private readonly IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { { "ConnectionStrings:db", "mongo" } }).Build();

        private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15.3")
            .WithName("physproj-tests-db-postgres")
            .Build();

        private readonly MongoDbContainer mongo = new MongoDbBuilder()
            .WithImage("mongo:4.4.18")
            .WithName("physproj-tests-db-mongo")
            .Build();

        protected readonly LoggerFactory loggerFactory = new LoggerFactory();

        protected readonly ITestOutputHelper output;

        public MigrationTests(ITestOutputHelper output)
        {
            this.output = output;

            try
            {
                NLogConfig.Configure(loggerFactory, "tests-db");
                ProgramUtils.OnRun(loggerFactory);
                Log("initializing");
                Init().Wait();
                Log("initialized");
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        [Theory]
        [InlineData("mongo", "postgres")]
        [InlineData("postgres", "mongo")]
        public void Tests(string source, string destination)
        {
            using var container = BuildContainer();
            using var lifetimeScope = container.BeginLifetimeScope();

            UsersTests(source, destination, lifetimeScope);
            AuthorsTests(source, destination, lifetimeScope);
            FilesTests(source, destination, lifetimeScope);
            WorksTests(source, destination, lifetimeScope);
        }

        private void WorksTests(string source, string destination, ILifetimeScope lifetimeScope)
        {
            var srcDb = lifetimeScope.ResolveNamed<IWorksDb>(source);
            var migrations = lifetimeScope.Resolve<IMigrationService>();

            var works = new[]
            {
                new WorkDbo { Code = "work-1", Language = "ru", Publish = "1234" },
                new WorkDbo { Code = "work-2", Infos = new List<WorkDbo.InfoDbo> { new WorkDbo.InfoDbo { Language = "en", Description = "desc" } } },
                new WorkDbo { Code = "work-3", AuthorsCodes = new List<string> { "author-1" } },
                new WorkDbo { Code = "work-4", FilesCodes = new List<string> { "file-1" } },
                new WorkDbo { Code = "work-5", SubWorksCodes = new List<string> { "work-6" } },
                new WorkDbo { Code = "work-6", OriginalCode = "work-7" },
                new WorkDbo { Code = "work-7" },
            }.OrderBy(u => u.Code).ToList();
            new WorksBaseWriter(srcDb, loggerFactory.CreateLogger<WorksMigrator>()).Write(works, new MigrationDto.StatsDto());
            new WorksLinksWriter(srcDb).Write(works, new MigrationDto.StatsDto());

            var migration = migrations.Create(new MigrationTask { Migrator = "works", Source = source, Destination = destination });
            migrations.Execute(migration);
            migration.Error.ShouldBeNull(migration.Error);

            var dstUsers = lifetimeScope.ResolveNamed<IWorksDb>(destination);
            var migrated = dstUsers.Find(new WorksDbQuery()).OrderBy(u => u.Code).ToList();
            migrated.ShouldBeEquivalentTo(works);
        }

        private static void FilesTests(string source, string destination, ILifetimeScope lifetimeScope)
        {
            var srcDb = lifetimeScope.ResolveNamed<IFilesDb>(source);
            var migrations = lifetimeScope.Resolve<IMigrationService>();

            var files = new[]
            {
                new FileDbo { Code = "file-1", Format = "pdf", Links = new List<FileDbo.LinkDbo> { new FileDbo.LinkDbo { StorageCode = "type-1", FileId = "path-1" } } },
                new FileDbo { Code = "file-2", Size = 1024, Links = new List<FileDbo.LinkDbo> { new FileDbo.LinkDbo { StorageCode = "type-2", FileId = "path-2" } } },
            }.OrderBy(u => u.Code).ToList();
            new FilesWriter(srcDb).Write(files, new MigrationDto.StatsDto());

            var migration = migrations.Create(new MigrationTask { Migrator = "files", Source = source, Destination = destination });
            migrations.Execute(migration);
            migration.Error.ShouldBeNull(migration.Error);

            var dstUsers = lifetimeScope.ResolveNamed<IFilesDb>(destination);
            var migrated = dstUsers.Find(new FilesDbQuery()).OrderBy(u => u.Code).ToList();
            migrated.ShouldBeEquivalentTo(files);
        }

        private static void AuthorsTests(string source, string destination, ILifetimeScope lifetimeScope)
        {
            var srcDb = lifetimeScope.ResolveNamed<IAuthorsDb>(source);
            var migrations = lifetimeScope.Resolve<IMigrationService>();

            var authors = new[]
            {
                new AuthorDbo { Code = "author-1", Born = "1234", Infos = new List<AuthorDbo.InfoDbo> { new AuthorDbo.InfoDbo { Language = "en", FullName = "FN" } } },
                new AuthorDbo { Code = "author-2", Died = "2345" },
            }.OrderBy(u => u.Code).ToList();
            new AuthorsWriter(srcDb).Write(authors, new MigrationDto.StatsDto());

            var migration = migrations.Create(new MigrationTask { Migrator = "authors", Source = source, Destination = destination });
            migrations.Execute(migration);
            migration.Error.ShouldBeNull(migration.Error);

            var dstAuthors = lifetimeScope.ResolveNamed<IAuthorsDb>(destination);
            var migrated = dstAuthors.Find(new AuthorsDbQuery()).OrderBy(u => u.Code).ToList();
            migrated.ShouldBeEquivalentTo(authors);
        }

        private static void UsersTests(string source, string destination, ILifetimeScope lifetimeScope)
        {
            var srcDb = lifetimeScope.ResolveNamed<IUsersDb>(source);
            var migrations = lifetimeScope.Resolve<IMigrationService>();

            var users = new[]
            {
                new UserDbo { Name = "user-1", NameLowerCase = "user-1", PasswordHash = "1", Roles = new List<string> { "role1", "role2" } },
                new UserDbo { Name = "user-2", NameLowerCase = "user-2", PasswordHash = "1", Roles = new List<string> { "role3" } }
            }.OrderBy(u => u.Name).ToList();
            new UsersWriter(srcDb).Write(users, new MigrationDto.StatsDto());

            var migration = migrations.Create(new MigrationTask { Migrator = "users", Source = source, Destination = destination });
            migrations.Execute(migration);
            migration.Error.ShouldBeNull(migration.Error);

            var dstUsers = lifetimeScope.ResolveNamed<IUsersDb>(destination);
            var migrated = dstUsers.Find(new UsersDbQuery()).OrderBy(u => u.Name).ToList();
            migrated.ShouldBeEquivalentTo(users);
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => configuration).As<IConfiguration>().SingleInstance();
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new PostgresDbModule(postgres.GetConnectionString(), loggerFactory));
            builder.RegisterModule(new MongoDbModule(mongo.GetConnectionString(), loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.Register(c => new MongoHistoryDbFactory(mongo.GetConnectionString(), "physlib", "history-", loggerFactory))
                .SingleInstance()
                .AsImplementedInterfaces();

            return builder.Build();
        }

        public void Dispose()
        {
            Log("releasing");
            Release().Wait();
            Log("released");
        }

        protected virtual async Task Init()
        {
            await mongo.StartAsync();
            await postgres.StartAsync();
        }

        protected virtual async Task Release()
        {
            await mongo.StopAsync();
            await postgres.StopAsync();
        }

        protected void Log(string message)
        {
            output.WriteLine($"{DateTime.UtcNow}: {message}");
        }
    }
}
