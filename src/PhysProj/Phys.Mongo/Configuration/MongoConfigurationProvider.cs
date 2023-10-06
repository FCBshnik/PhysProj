using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Phys.Mongo.Configuration
{
    public class MongoConfigurationProvider : ConfigurationProvider
    {
        private readonly string connectionString;
        private readonly ILogger<MongoConfigurationProvider> log;

        public MongoConfigurationProvider(string connectionString, ILogger<MongoConfigurationProvider> log)
        {
            ArgumentNullException.ThrowIfNull(connectionString);
            ArgumentNullException.ThrowIfNull(log);

            this.connectionString = connectionString;
            this.log = log;
        }

        public void SaveIfNotExists(string json)
        {
            log.LogInformation($"mongo connection: {connectionString}");
            var client = new MongoClient(new MongoUrl(connectionString));
            var db = client.GetDatabase("physproj-config");
            var collection = db.GetCollection<BsonDocument>("configs");
            var filter = new FilterDefinitionBuilder<BsonDocument>().Empty;
            var documents = collection.Find(filter).ToList();
            if (documents.Count == 0)
            {
                collection.InsertOne(BsonDocument.Parse(json));
                log.LogInformation($"saved config");
            }
            else 
                log.LogInformation($"config already exists");
        }

        public override void Load()
        {
            log.LogInformation($"mongo connection: {connectionString}");
            var client = new MongoClient(new MongoUrl(connectionString));
            var db = client.GetDatabase("physproj-config");
            var collection = db.GetCollection<BsonDocument>("configs");
            var filter = new FilterDefinitionBuilder<BsonDocument>().Empty;
            var documents = collection.Find(filter).ToList();
            foreach (var document in documents)
                Traverse(document, string.Empty);
        }

        private void Traverse(BsonValue value, string prefix)
        {
            if (value.IsObjectId)
                return;

            if (value.IsString)
                Data[prefix.TrimStart(':')] = value.AsString;

            if (value.IsBsonDocument)
                foreach (var elem in value.AsBsonDocument.Elements)
                    Traverse(elem.Value, prefix + ":" + elem.Name);
        }
    }
}
