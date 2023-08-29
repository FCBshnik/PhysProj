using Autofac;
using Phys.Lib.Postgres;
using Testcontainers.PostgreSql;

namespace Phys.Lib.Tests.Db
{
    public class PostgresTests : DbTests
    {
        private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15.3")
            .WithName("physproj-tests-db-postgres")
            .Build();

        public PostgresTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override async Task Init()
        {
            await base.Init();
            await postgres.StartAsync();
        }

        protected override async Task Release()
        {
            await base.Release();
            await postgres.DisposeAsync();
        }

        protected override void Register(ContainerBuilder builder)
        {
            base.Register(builder);
            builder.RegisterModule(new PostgresModule(postgres.GetConnectionString(), loggerFactory));
        }
    }
}
