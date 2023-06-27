using MongoDB.Driver;

namespace Phys.Lib.Data
{
    internal class Collection<T>
    {
        protected FilterDefinitionBuilder<T> filterBuilder => Builders<T>.Filter;
        protected UpdateDefinitionBuilder<T> updateBuilder => Builders<T>.Update;
        protected SortDefinitionBuilder<T> sortBuilder => Builders<T>.Sort;

        protected FindOneAndUpdateOptions<T, T> findOneAndUpdateReturnAfter = new FindOneAndUpdateOptions<T, T> { ReturnDocument = ReturnDocument.After };

        protected readonly IMongoCollection<T> collection;

        public Collection(IMongoCollection<T> collection)
        {
            this.collection = collection;
        }

        protected T Insert(T item)
        {
            collection.InsertOne(item);
            return item;
        }
    }
}
