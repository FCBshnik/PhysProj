using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Phys.Shared.ItemsLog;
using Phys.Shared.ObjectsLog;

namespace Phys.Shared.Mongo.ObjectsLog
{
    public class MongoObjectsLogFactory : IObjectsLogFactory
    {
        private readonly string connectionString;
        private readonly string databaseName;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<MongoObjectsLogFactory> log;

        public MongoObjectsLogFactory(string connectionString, string databaseName, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(nameof(connectionString));
            ArgumentNullException.ThrowIfNull(nameof(databaseName));
            ArgumentNullException.ThrowIfNull(nameof(loggerFactory));

            this.connectionString = connectionString;
            this.databaseName = databaseName;
            this.loggerFactory = loggerFactory;

            log = loggerFactory.CreateLogger<MongoObjectsLogFactory>();
        }

        public IObjectsLog<T> Create<T>(string name) where T : IObjectsLogId
        {
            log.LogInformation($"creating objects log '{name}': server '{connectionString}', database '{databaseName}'");

            BsonClassMap.TryRegisterClassMap<T>(m =>
            {
                m.AutoMap();
                m.MapIdProperty(obj => obj.Id);
            });

            var client = new MongoClient(new MongoUrl(connectionString));
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<T>($"log-{name}");
            var objectLog = new MongoObjectsLog<T>(collection);
            log.LogInformation($"created objects log '{name}': collection '{collection.CollectionNamespace.CollectionName}'");
            return objectLog;
        }
    }
}
