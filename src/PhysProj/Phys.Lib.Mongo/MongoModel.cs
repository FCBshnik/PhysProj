using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Phys.Lib.Mongo
{
    internal class MongoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("_u")]
        public DateTime UpdatedAt { get; set; }
    }
}
