using Autofac;
using Phys.Lib.Mongo;
using Testcontainers.MongoDb;

namespace Phys.Lib.Tests.Db
{
    public class MongoTests : DbTests
    {
        private readonly MongoDbContainer mongo = new MongoDbBuilder()
            .WithImage("mongo:4.4.18")
            .WithName("physproj-tests-db-mongo")
            .Build();

        public MongoTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override async Task Init()
        {
            await base.Init();
            await mongo.StartAsync();
        }

        protected override async Task Release()
        {
            await base.Release();
            await mongo.DisposeAsync();
        }

        protected override void Register(ContainerBuilder builder)
        {
            base.Register(builder);
            builder.RegisterModule(new MongoModule(mongo.GetConnectionString()));
        }
    }
}
