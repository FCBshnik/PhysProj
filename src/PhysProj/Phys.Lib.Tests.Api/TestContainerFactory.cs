using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace Phys.Lib.Tests.Api
{
    internal static class TestContainerFactory
    {
        private static readonly Uri dockerHost = new Uri("tcp://192.168.2.67:2375");

        public static MongoDbContainer CreateMongo()
        {
            return new MongoDbBuilder()
                .WithDockerEndpoint(dockerHost)
                .WithImage("mongo:4.4.18")
                .WithPortBinding("57017", "27017")
                .Build();
        }

        public static PostgreSqlContainer CreatePostgres()
        {
            return new PostgreSqlBuilder()
                .WithDockerEndpoint(dockerHost)
                .WithImage("postgres:15.3")
                .Build();
        }
    }
}
