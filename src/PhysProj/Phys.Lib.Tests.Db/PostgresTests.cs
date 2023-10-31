using Autofac;
using Phys.Lib.Autofac;
using Testcontainers.PostgreSql;

namespace Phys.Lib.Tests.Db
{
    public class PostgresTests : DbTests
    {
        private readonly PostgreSqlContainer postgres = TestContainerFactory.CreatePostgres();

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
            builder.RegisterModule(new PostgresDbModule(postgres.GetConnectionString(), loggerFactory));
        }
    }
}
