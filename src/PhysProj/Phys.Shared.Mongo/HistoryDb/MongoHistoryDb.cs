using MongoDB.Driver;
using Phys.Shared.HistoryDb;

namespace Phys.Shared.Mongo.HistoryDb
{
    internal class MongoHistoryDb<T> : IHistoryDb<T> where T : IHistoryDbo
    {
        private readonly IMongoCollection<T> collection;

        public MongoHistoryDb(IMongoCollection<T> collection)
        {
            this.collection = collection;
        }

        public void Save(T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            if (obj.Id == null)
                collection.InsertOne(obj);
            else
                collection.ReplaceOne(Builders<T>.Filter.Eq(i => i.Id, obj.Id), obj);
        }

        public T Get(string id)
        {
            ArgumentNullException.ThrowIfNull(id);

            var filter = Builders<T>.Filter.Eq(i => i.Id, id);
            return collection.Find(filter).FirstOrDefault();
        }

        public List<T> List(HistoryDbQuery query)
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
