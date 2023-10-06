using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Phys.HistoryDb;

namespace Phys.Mongo.HistoryDb
{
    public class MongoHistoryDbFactory : IHistoryDbFactory
    {
        private readonly string connectionString;
        private readonly string databaseName;
        private readonly string collectionNamePrefix;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<MongoHistoryDbFactory> log;

        public MongoHistoryDbFactory(string connectionString, string databaseName, string collectionNamePrefix, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(nameof(connectionString));
            ArgumentNullException.ThrowIfNull(nameof(databaseName));
            ArgumentNullException.ThrowIfNull(nameof(collectionNamePrefix));
            ArgumentNullException.ThrowIfNull(nameof(loggerFactory));

            this.connectionString = connectionString;
            this.databaseName = databaseName;
            this.collectionNamePrefix = collectionNamePrefix;
            this.loggerFactory = loggerFactory;

            log = loggerFactory.CreateLogger<MongoHistoryDbFactory>();
        }

        public IHistoryDb<T> Create<T>(string name) where T : IHistoryDbo
        {
            log.LogInformation($"creating objects log '{name}': server '{connectionString}', database '{databaseName}'");

            BsonClassMap.TryRegisterClassMap<T>(m =>
            {
                m.AutoMap();
                m.MapIdProperty(obj => obj.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });

            var client = new MongoClient(new MongoUrl(connectionString));
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<T>($"{collectionNamePrefix}{name}");
            var objectLog = new MongoHistoryDb<T>(collection);
            log.LogInformation($"created objects log '{name}': collection '{collection.CollectionNamespace.CollectionName}'");
            return objectLog;
        }
    }
}
