using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using Phys.Lib.Mongo.Utils;
using Phys.Lib.Db.Authors;

namespace Phys.Lib.Mongo.Authors
{
    internal class AuthorsDb : Collection<AuthorDbo>, IAuthorsDb
    {
        public AuthorsDb(Lazy<IMongoCollection<AuthorDbo>> collection) : base(collection)
        {
        }

        protected override void Init(IMongoCollection<AuthorDbo> collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<AuthorDbo>(IndexBuilder.Ascending(i => i.Code), new CreateIndexOptions { Unique = true }));
        }

        public AuthorDbo Create(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            var author = new AuthorDbo
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Code = code
            };

            return Insert(author);
        }

        public List<AuthorDbo> Find(AuthorsDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var filter = FilterBuilder.Empty;
            if (query.Code != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.Code, query.Code));
            if (query.Codes != null)
                filter = FilterBuilder.And(filter, FilterBuilder.In(u => u.Code, query.Codes));
            if (query.Search != null)
            {
                var regex = Regex.Escape(query.Search);
                var infoFilterBuilder = Builders<AuthorDbo.InfoDbo>.Filter;
                filter = FilterBuilder.And(filter, FilterBuilder.Or(
                    FilterBuilder.Regex(u => u.Code, regex),
                    FilterBuilder.ElemMatch(u => u.Infos, infoFilterBuilder.Regex(i => i.FullName, regex)),
                    FilterBuilder.ElemMatch(u => u.Infos, infoFilterBuilder.Regex(i => i.Description, regex))));
            }

            var sort = SortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }

        public AuthorDbo Update(string id, AuthorDbUpdate author)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(author);

            var filter = FilterBuilder.Eq(i => i.Id, id);
            var update = UpdateBuilder.Combine();

            if (author.Born.IsEmpty())
                update = update.Unset(i => i.Born);
            else if (author.Born.HasValue())
                update = update.Set(i => i.Born, author.Born);

            if (author.Died.IsEmpty())
                update = update.Unset(i => i.Died);
            else if (author.Died.HasValue())
                update = update.Set(i => i.Died, author.Died);

            if (author.AddInfo != null)
                update = update.Push(i => i.Infos, author.AddInfo);
            if (author.DeleteInfo != null)
                update = update.PullFilter(i => i.Infos, i => i.Language == author.DeleteInfo);

            return collection.FindOneAndUpdate(filter, update, findOneAndUpdateReturnAfter)
                ?? throw new ApplicationException($"author '{id}' was not updated due to not found in db");
        }

        public void Delete(string id)
        {
            ArgumentNullException.ThrowIfNull(id);

            collection.DeleteOne(FilterBuilder.Eq(i => i.Id, id));
        }
    }
}
