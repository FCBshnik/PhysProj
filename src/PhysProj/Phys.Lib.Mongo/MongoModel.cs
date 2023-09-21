using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Phys.Lib.Mongo
{
    internal class MongoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
