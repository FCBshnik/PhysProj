using Autofac;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core;
using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Users;
using Phys.Lib.Mongo;
using Phys.Lib.Postgres;
using Phys.Shared.Logging;
using Phys.Shared.Mongo.HistoryDb;
using Phys.Shared.NLog;
using Phys.Shared.Utils;
using Shouldly;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace Phys.Lib.Tests.Db
{
    public class MigrationTests : IDisposable
    {
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
                NLogConfig.Configure(loggerFactory, "testsdb");
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

        [Fact]
        public void Tests()
        {
            using var container = BuildContainer();
            using var lifetimeScope = container.BeginLifetimeScope();
            var srcUsers = lifetimeScope.ResolveNamed<IUsersDb>("mongo");
            var migrations = lifetimeScope.Resolve<IMigrationService>();

            srcUsers.Create(new UserDbo { Name = "user-1", NameLowerCase = "user-1", PasswordHash = "1" });
            srcUsers.Create(new UserDbo { Name = "user-2", NameLowerCase = "user-2", PasswordHash = "1" });

            var migration = migrations.Create(new MigrationTask { Migrator = "users", Source = "mongo", Destination = "postgres" });
            migrations.Execute(migration);
            migration.Error.ShouldBeNull(migration.Error);

            var destUsers = lifetimeScope.ResolveNamed<IUsersDb>("postgres");
            var migratedUsers = destUsers.Find(new UsersDbQuery());
            migratedUsers.Count.ShouldBe(2);
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new PostgresModule(postgres.GetConnectionString(), loggerFactory));
            builder.RegisterModule(new MongoModule(mongo.GetConnectionString(), loggerFactory));

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
