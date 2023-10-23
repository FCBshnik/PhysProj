using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Phys.Shared;
using Phys.Shared.Settings;
using System.Collections.Concurrent;

namespace Phys.Mongo.Settings
{
    public class MongoSettings : ISettings
    {
        private readonly string connectionString;
        private readonly string databaseName;
        private readonly string collectionName;
        private readonly ILogger<MongoSettings> log;

        private readonly Lazy<IMongoCollection<SettingsModel>> collection;
        private readonly ConcurrentDictionary<string, Type> settingsTypes = new ConcurrentDictionary<string, Type>();

        public MongoSettings(string connectionString, string databaseName, string collectionName, ILogger<MongoSettings> log)
        {
            this.connectionString = connectionString;
            this.databaseName = databaseName;
            this.collectionName = collectionName;
            this.log = log;

            collection = new Lazy<IMongoCollection<SettingsModel>>(Create);
        }

        private IMongoCollection<SettingsModel> Create()
        {
            log.LogInformation($"creating settings: server '{connectionString}', database '{databaseName}'");

            BsonClassMap.TryRegisterClassMap<SettingsModel>(m => m.AutoMap());

            var client = new MongoClient(new MongoUrl(connectionString));
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<SettingsModel>(collectionName);
            log.LogInformation($"created settings: collection '{collection.CollectionNamespace.CollectionName}'");
            collection.Indexes.CreateOne(new CreateIndexModel<SettingsModel>(Builders<SettingsModel>.IndexKeys.Ascending(i => i.Code),
                new CreateIndexOptions { Unique = true }));
            return collection;
        }

        public Type GetType(string code)
        {
            return settingsTypes[code];
        }

        public object Get(string code)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(code);

            var filter = Builders<SettingsModel>.Filter.Eq(s => s.Code, code);

            var settings = collection.Value.Find(filter).FirstOrDefault();
            if (settings == null)
                throw new PhysException($"settings '{code}' not found");

            return BsonSerializer.Deserialize(settings.Value, settingsTypes[code]);
        }

        public List<string> List()
        {
            return settingsTypes.Keys.ToList();
        }

        public void Register<T>(string code, T defaultValue)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(code);
            ArgumentNullException.ThrowIfNull(defaultValue);

            BsonClassMap.TryRegisterClassMap<T>(m => m.AutoMap());

            var filter = Builders<SettingsModel>.Filter.Eq(s => s.Code, code);
            var settings = collection.Value.Find(filter).FirstOrDefault();
            if (settings == null)
                collection.Value.InsertOne(new SettingsModel { Code = code, Value = defaultValue.ToBsonDocument() });

            settingsTypes.TryAdd(code, typeof(T));

            log.LogInformation($"registered settings '{code}' of type {typeof(T)}");
        }

        public void Set(string code, object value)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(code);
            ArgumentNullException.ThrowIfNull(value);

            var type = settingsTypes[code];
            var filter = Builders<SettingsModel>.Filter.Eq(s => s.Code, code);
            var update = Builders<SettingsModel>.Update.Set(s => s.Value, value.ToBsonDocument(type));

            collection.Value.UpdateOne(filter, update);

            log.LogInformation($"updated settings '{code}'");
        }

        private class SettingsModel
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("code")]
            public string Code { get; set; }

            [BsonElement("value")]
            public BsonDocument Value { get; set; }
        }
    }
}
