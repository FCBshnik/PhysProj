using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Phys.Lib.Mongo
{
    internal abstract class Collection<TModel> where TModel : MongoModel
    {
        private readonly ILogger<Collection<TModel>> log;
        protected static FilterDefinitionBuilder<TModel> FilterBuilder => Builders<TModel>.Filter;
        protected static UpdateDefinitionBuilder<TModel> UpdateBuilder => Builders<TModel>.Update;
        protected static SortDefinitionBuilder<TModel> SortBuilder => Builders<TModel>.Sort;
        protected static IndexKeysDefinitionBuilder<TModel> IndexBuilder => Builders<TModel>.IndexKeys;

        protected FindOneAndUpdateOptions<TModel, TModel> findOneAndUpdateReturnAfter = new() { ReturnDocument = ReturnDocument.After };

        private readonly Lazy<IMongoCollection<TModel>> lazyCollection;
        private bool initialized;

        protected IMongoCollection<TModel> MongoCollection
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

        protected Collection(Lazy<IMongoCollection<TModel>> collection, ILogger<Collection<TModel>> log)
        {
            lazyCollection = collection;
            this.log = log;
        }

        protected IEnumerable<List<TDbo>> Read<TDbo>(int limit, Func<TModel, TDbo> map)
        {
            string? cursor = null;
            List<TModel>? models = null;

            do
            {
                var filter = FilterBuilder.Empty;
                if (cursor != null)
                    filter = FilterBuilder.And(filter, FilterBuilder.Gt(u => u.Id, cursor));
                var sort = SortBuilder.Ascending(i => i.Id);
                models = MongoCollection.Find(filter).Limit(limit).Sort(sort).ToList();
                cursor = models.LastOrDefault()?.Id;
                yield return models.Select(map).ToList();
            } while (models.Count >= limit);
        }

        protected TModel Insert(TModel item)
        {
            MongoCollection.InsertOne(item);
            return item;
        }

        protected virtual void Init(IMongoCollection<TModel> collection)
        {
        }
    }
}
