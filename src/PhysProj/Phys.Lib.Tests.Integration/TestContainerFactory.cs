using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Phys.Lib.Tests.Db
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

        public static RabbitMqContainer CreateRabbit()
        {
            return new RabbitMqBuilder()
                .WithDockerEndpoint(dockerHost)
                .WithImage("rabbitmq:3.12")
                .Build();
        }
    }
}
