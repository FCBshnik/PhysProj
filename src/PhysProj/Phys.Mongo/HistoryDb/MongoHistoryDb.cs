using MongoDB.Bson;
using MongoDB.Driver;
using Phys.HistoryDb;

namespace Phys.Mongo.HistoryDb
{
    internal class MongoHistoryDb<T> : IHistoryDb<T> where T : IHistoryDbo
    {
        private readonly IMongoCollection<T> collection;

        public MongoHistoryDb(IMongoCollection<T> collection)
        {
            this.collection = collection;
        }

        public string GetNewId()
        {
            return ObjectId.GenerateNewId(DateTime.UtcNow).ToString();
        }

        public void Save(T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            collection.ReplaceOne(Builders<T>.Filter.Eq(i => i.Id, obj.Id), obj, new ReplaceOptions { IsUpsert = true });
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

            var sort = Builders<T>.Sort.Descending(e => e.Id);

            return collection.Find(filter).Sort(sort).Skip(query.Skip).Limit(query.Limit).ToList();
        }
    }
}
