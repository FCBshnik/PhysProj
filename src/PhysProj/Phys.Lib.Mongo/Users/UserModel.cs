using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Phys.Lib.Mongo.Users
{
    internal class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("nameLc")]
        public string NameLowerCase { get; set; }

        [BsonElement("pwdHash")]
        public string PasswordHash { get; set; }

        [BsonElement("roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
