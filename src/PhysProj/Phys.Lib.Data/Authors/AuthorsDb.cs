using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Core.Authors;

namespace Phys.Lib.Data.Authors
{
    internal class AuthorsDb : Collection<AuthorDbo>, IAuthorsDb
    {
        public AuthorsDb(IMongoCollection<AuthorDbo> collection) : base(collection)
        {
        }

        public AuthorDbo Create(AuthorDbo author)
        {
            author.Id = ObjectId.GenerateNewId().ToString();
            return Insert(author);
        }

        public List<AuthorDbo> Find(AuthorsQuery query)
        {
            var filter = filterBuilder.Empty;

            if (query.Code != null)
                filter = filterBuilder.And(filter, filterBuilder.Eq(u => u.Code, query.Code));
            if (query.Search != null)
                filter = filterBuilder.And(filter, filterBuilder.Regex(u => u.Code, query.Search));

            var sort = sortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }

        public AuthorDbo Get(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            return collection.Find(filterBuilder.Eq(u => u.Id, id)).FirstOrDefault() ?? throw new ApplicationException($"author '{id}' not found in db");
        }

        public AuthorDbo Update(string id, AuthorUpdate options)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            var filter = filterBuilder.Eq(i => i.Id, id);
            var update = updateBuilder.Combine();
            if (options.Born != null)
                update = update.Set(i => i.Born, options.Born);
            if (options.Died != null)
                update = update.Set(i => i.Died, options.Died);
            if (options.AddInfo != null)
                update = update.Push(i => i.Infos, options.AddInfo);
            if (options.RemoveInfo != null)
                update = update.PullFilter(i => i.Infos, i => i.Language == options.RemoveInfo);

            return collection.FindOneAndUpdate(filter, update, findOneAndUpdateOptions)
                ?? throw new ApplicationException($"author '{id}' was not updated due to not found in db");
        }

        public void Delete(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            collection.DeleteOne(filterBuilder.Eq(i => i.Id, id));
        }
    }
}
