using MongoDB.Driver;

namespace Phys.Lib.Data
{
    internal class Collection<T>
    {
        protected static FilterDefinitionBuilder<T> FilterBuilder => Builders<T>.Filter;
        protected static UpdateDefinitionBuilder<T> UpdateBuilder => Builders<T>.Update;
        protected static SortDefinitionBuilder<T> SortBuilder => Builders<T>.Sort;
        protected static IndexKeysDefinitionBuilder<T> IndexBuilder => Builders<T>.IndexKeys;

        protected FindOneAndUpdateOptions<T, T> findOneAndUpdateReturnAfter = new() { ReturnDocument = ReturnDocument.After };

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
