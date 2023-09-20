using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using Phys.Lib.Db.Files;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;

namespace Phys.Lib.Mongo.Files
{
    internal class FilesDb : Collection<FileModel>, IFilesDb
    {
        public string Name => "mongo";

        public FilesDb(Lazy<IMongoCollection<FileModel>> collection, ILogger<FilesDb> logger) : base(collection, logger)
        {
        }

        protected override void Init(IMongoCollection<FileModel> collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<FileModel>(IndexBuilder.Ascending(i => i.Code), new CreateIndexOptions { Unique = true }));
        }

        public void Create(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            var fileModel = new FileModel
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Code = file.Code,
                Format = file.Format,
                Size = file.Size
            };
            Insert(fileModel);
        }

        public void Delete(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            collection.DeleteOne(FilterBuilder.Eq(i => i.Code, code));
        }

        public List<FileDbo> Find(FilesDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var filter = FilterBuilder.Empty;

            if (query.Code != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.Code, query.Code));
            if (query.Search != null)
            {
                var regex = Regex.Escape(query.Search);
                var linkFilterBuilder = Builders<FileModel.LinkModel>.Filter;
                filter = FilterBuilder.And(filter, FilterBuilder.Or(
                    FilterBuilder.Regex(u => u.Code, regex),
                    FilterBuilder.ElemMatch(u => u.Links, linkFilterBuilder.Regex(i => i.Path, regex))));
            }

            var sort = SortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList().Select(FileMapper.Map).ToList();
        }

        public void Update(string code, FileDbUpdate file)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(file);

            var filter = FilterBuilder.Eq(i => i.Code, code);
            var update = UpdateBuilder.Combine();

            if (file.AddLink != null)
                update = update.Push(i => i.Links, FileMapper.Map(file.AddLink));
            if (file.DeleteLink != null)
                update = update.PullFilter(i => i.Links, l => l.Type == file.DeleteLink.Type && l.Path == file.DeleteLink.Path);

            if (collection.UpdateOne(filter, update).MatchedCount == 0)
                throw new PhysDbException($"file '{code}' update failed");
        }
    }
}
