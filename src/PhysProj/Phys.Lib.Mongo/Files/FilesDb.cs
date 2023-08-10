using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using Phys.Lib.Db.Files;

namespace Phys.Lib.Mongo.Files
{
    internal class FilesDb : Collection<FileDbo>, IFilesDb
    {
        public FilesDb(Lazy<IMongoCollection<FileDbo>> collection) : base(collection)
        {
        }

        protected override void Init(IMongoCollection<FileDbo> collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<FileDbo>(IndexBuilder.Ascending(i => i.Code), new CreateIndexOptions { Unique = true }));
        }

        public FileDbo Create(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            file.Id = ObjectId.GenerateNewId().ToString();
            return Insert(file);
        }

        public void Delete(string id)
        {
            ArgumentNullException.ThrowIfNull(id);

            collection.DeleteOne(FilterBuilder.Eq(i => i.Id, id));
        }

        public List<FileDbo> Find(FilesDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var filter = FilterBuilder.Empty;

            if (query.Id != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.Id, query.Id));
            if (query.Code != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.Code, query.Code));
            if (query.Search != null)
            {
                var regex = Regex.Escape(query.Search);
                var linkFilterBuilder = Builders<FileDbo.LinkDbo>.Filter;
                filter = FilterBuilder.And(filter, FilterBuilder.Or(
                    FilterBuilder.Regex(u => u.Code, regex),
                    FilterBuilder.ElemMatch(u => u.Links, linkFilterBuilder.Regex(i => i.Path, regex))));
            }

            var sort = SortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }

        public FileDbo Update(string id, FileDbUpdate file)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(file);

            var filter = FilterBuilder.Eq(i => i.Id, id);
            var update = UpdateBuilder.Combine();

            if (file.AddLink != null)
                update = update.Push(i => i.Links, file.AddLink);
            if (file.DeleteLink != null)
                update = update.PullFilter(i => i.Links, l => l.Type == file.DeleteLink.Type && l.Path == file.DeleteLink.Path);

            return collection.FindOneAndUpdate(filter, update, findOneAndUpdateReturnAfter)
                ?? throw new ApplicationException($"file '{id}' was not updated due to not found in db");
        }
    }
}
