using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Core.Authors;
using Phys.Lib.Data.Utils;
using System.Text.RegularExpressions;

namespace Phys.Lib.Data.Authors
{
    internal class AuthorsDb : Collection<AuthorDbo>, IAuthorsDb
    {
        public AuthorsDb(IMongoCollection<AuthorDbo> collection) : base(collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<AuthorDbo>(IndexBuilder.Ascending(i => i.Code), new CreateIndexOptions { Unique = true }));
        }

        public AuthorDbo Create(AuthorDbo author)
        {
            author.Id = ObjectId.GenerateNewId().ToString();
            return Insert(author);
        }

        public List<AuthorDbo> Find(AuthorsDbQuery query)
        {
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
                    FilterBuilder.ElemMatch(u => u.Infos, infoFilterBuilder.Regex(i => i.Name, regex)),
                    FilterBuilder.ElemMatch(u => u.Infos, infoFilterBuilder.Regex(i => i.Description, regex))));
            }

            var sort = SortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }

        public AuthorDbo Get(string id)
        {
            ArgumentNullException.ThrowIfNull(id);

            return collection.Find(FilterBuilder.Eq(u => u.Id, id)).FirstOrDefault() ?? throw new ApplicationException($"author '{id}' not found in db");
        }

        public AuthorDbo GetByCode(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            return collection.Find(FilterBuilder.Eq(u => u.Code, code)).FirstOrDefault() ?? throw new ApplicationException($"author '{code}' not found in db");
        }

        public AuthorDbo Update(string id, AuthorDbUpdate author)
        {
            ArgumentNullException.ThrowIfNull(id);

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
