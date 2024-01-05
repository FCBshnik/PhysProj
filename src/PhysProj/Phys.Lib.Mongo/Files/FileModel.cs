using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Phys.Lib.Mongo.Files
{
    internal class FileModel : MongoModel
    {
        [BsonElement("code")]
        public required string Code { get; set; }

        [BsonElement("format")]
        public required string Format { get; set; }

        [BsonElement("size")]
        public long Size { get; set; }

        [BsonElement("links")]
        public List<LinkModel> Links { get; set; } = new List<LinkModel>();

        public class LinkModel
        {
            [BsonElement("type")]
            public required string Type { get; set; }

            [BsonElement("path")]
            public required string Path { get; set; }
        }
    }
}
