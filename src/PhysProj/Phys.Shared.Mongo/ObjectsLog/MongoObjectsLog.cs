using MongoDB.Driver;
using Phys.Shared.ItemsLog;
using Phys.Shared.ObjectsLog;

namespace Phys.Shared.Mongo.ObjectsLog
{
    internal class MongoObjectsLog<T> : IObjectsLog<T> where T : IObjectsLogId
    {
        private readonly IMongoCollection<T> collection;

        public MongoObjectsLog(IMongoCollection<T> collection)
        {
            this.collection = collection;
        }

        public void Add(T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            collection.InsertOne(obj);
        }

        public T Get(string id)
        {
            ArgumentNullException.ThrowIfNull(id);

            var filter = Builders<T>.Filter.Eq(i => i.Id, id);
            return collection.Find(filter).FirstOrDefault();
        }

        public List<T> List(ObjectsLogQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var builder = Builders<T>.Filter;

            var filter = builder.And(
                builder.Gte(e => e.Id, query.Interval.Start.ToObjectId().ToString()),
                builder.Lt(e => e.Id, query.Interval.End.ToObjectId().ToString()));

            return collection.Find(filter).Skip(query.Skip).Limit(query.Limit).ToList();
        }
    }
}
