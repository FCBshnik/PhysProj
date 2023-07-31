using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Core.Files;
using System.Text.RegularExpressions;

namespace Phys.Lib.Data.Files
{
    internal class FilesLinksDb : Collection<FileLinksDbo>, IFilesLinksDb
    {
        public FilesLinksDb(Lazy<IMongoCollection<FileLinksDbo>> collection) : base(collection)
        {
        }

        protected override void Init(IMongoCollection<FileLinksDbo> collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<FileLinksDbo>(IndexBuilder.Ascending(i => i.Code), new CreateIndexOptions { Unique = true }));
        }

        public FileLinksDbo Create(FileLinksDbo file)
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

        public List<FileLinksDbo> Find(FileLinksDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var filter = FilterBuilder.Empty;

            if (query.Code != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.Code, query.Code));
            if (query.Search != null)
            {
                var regex = Regex.Escape(query.Search);
                var linkFilterBuilder = Builders<FileLinksDbo.LinkDbo>.Filter;
                filter = FilterBuilder.And(filter, FilterBuilder.Or(
                    FilterBuilder.Regex(u => u.Code, regex),
                    FilterBuilder.ElemMatch(u => u.Links, linkFilterBuilder.Regex(i => i.Path, regex))));
            }

            var sort = SortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }

        public FileLinksDbo Update(string id, FileLinksDbUpdate fileLinks)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(fileLinks);

            var filter = FilterBuilder.Eq(i => i.Id, id);
            var update = UpdateBuilder.Combine();

            if (fileLinks.AddLink != null)
                update = update.Push(i => i.Links, fileLinks.AddLink);
            if (fileLinks.DeleteLink != null)
                update = update.PullFilter(i => i.Links, l => l.Type == fileLinks.DeleteLink.Type && l.Path == fileLinks.DeleteLink.Path);

            return collection.FindOneAndUpdate(filter, update, findOneAndUpdateReturnAfter)
                ?? throw new ApplicationException($"file links '{id}' was not updated due to not found in db");
        }
    }
}
