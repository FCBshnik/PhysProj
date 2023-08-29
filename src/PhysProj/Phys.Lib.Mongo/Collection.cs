using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Phys.Lib.Mongo
{
    internal class Collection<T>
    {
        private readonly ILogger<Collection<T>> log;
        protected static FilterDefinitionBuilder<T> FilterBuilder => Builders<T>.Filter;
        protected static UpdateDefinitionBuilder<T> UpdateBuilder => Builders<T>.Update;
        protected static SortDefinitionBuilder<T> SortBuilder => Builders<T>.Sort;
        protected static IndexKeysDefinitionBuilder<T> IndexBuilder => Builders<T>.IndexKeys;

        protected FindOneAndUpdateOptions<T, T> findOneAndUpdateReturnAfter = new() { ReturnDocument = ReturnDocument.After };

        private readonly Lazy<IMongoCollection<T>> lazyCollection;
        private bool initialized;

        protected IMongoCollection<T> collection
        {
            get
            {
                // do trick with lazy initialization in case when mongo is not available
                // bacause mongo driver does not have 'onconnected' event to initialize collection ()create indexes)
                // and wanted to get mongo timeout exception from operation executed but not dependecy resolution exception from DI container
                var value = lazyCollection.Value;
                if (!initialized)
                {
                    Init(value);
                    initialized = true;
                    log.LogInformation($"initialized '{value.CollectionNamespace.CollectionName}' collection");
                }

                return value;
            }
        }

        public Collection(Lazy<IMongoCollection<T>> collection, ILogger<Collection<T>> log)
        {
            lazyCollection = collection;
            this.log = log;
        }

        protected T Insert(T item)
        {
            collection.InsertOne(item);
            return item;
        }

        protected virtual void Init(IMongoCollection<T> collection)
        {
        }
    }
}
