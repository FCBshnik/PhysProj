using MongoDB.Bson;
using NodaTime;

namespace Phys.Mongo
{
    internal static class MongoExtensions
    {
        public static ObjectId ToObjectId(this Instant instant)
        {
            return ObjectId.GenerateNewId(instant.ToDateTimeUtc());
        }
    }
}
