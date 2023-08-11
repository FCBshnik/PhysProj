using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Phys.Lib.Mongo.Files
{
    internal class FileModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("code")]
        public string Code { get; set; }

        [BsonElement("format")]
        public string? Format { get; set; }

        [BsonElement("size")]
        public long? Size { get; set; }

        [BsonElement("links")]
        public List<LinkModel> Links { get; set; } = new List<LinkModel>();

        public class LinkModel
        {
            [BsonElement("type")]
            public string Type { get; set; }

            [BsonElement("path")]
            public string Path { get; set; }
        }
    }
}
