using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Phys.Lib.Mongo.Works
{
    internal class WorkModel : MongoModel
    {
        [BsonElement("code")]
        public string Code { get; set; }

        [BsonElement("pubd")]
        public string? Publish { get; set; }

        [BsonElement("lang")]
        public string? Language { get; set; }

        [BsonElement("infos")]
        public List<InfoModel> Infos { get; set; } = new List<InfoModel>();

        [BsonElement("subw")]
        public List<string> SubWorksCodes { get; set; } = new List<string>();

        [BsonElement("auth")]
        public List<string> AuthorsCodes { get; set; } = new List<string>();

        [BsonElement("orig")]
        public string? OriginalCode { get; set; }

        [BsonElement("files")]
        public List<string> FilesCodes { get; set; } = new List<string>();

        public class InfoModel
        {
            [BsonElement("lang")]
            public string Language { get; set; }

            [BsonElement("name")]
            public string? Name { get; set; }

            [BsonElement("desc")]
            public string? Description { get; set; }
        }
    }
}
