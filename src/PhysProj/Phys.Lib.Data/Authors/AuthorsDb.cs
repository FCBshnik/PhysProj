using MongoDB.Driver;
using Phys.Lib.Core.Authors;
using System.Linq.Expressions;

namespace Phys.Lib.Data.Authors
{
    internal class AuthorsDb : Collection<AuthorDbo>, IAuthorsDb
    {
        public AuthorsDb(IMongoCollection<AuthorDbo> collection) : base(collection)
        {
        }

        public AuthorDbo Create(AuthorDbo author)
        {
            return Insert(author);
        }

        public List<AuthorDbo> Find(AuthorsQuery query)
        {
            var filter = filterBuilder.Empty;

            if (query.Code != null)
                filter = filterBuilder.And(filter, filterBuilder.Eq(u => u.Code, query.Code));
            //if (query.Search != null)
            //    filter = Filter.And(filter, Filter.Regex(u => u.Code, query.Search));

            var sort = sortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }

        public AuthorDbo Get(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            return collection.Find(filterBuilder.Eq(u => u.Id, id)).FirstOrDefault() ?? throw new ApplicationException($"author '{id}' not found in db");
        }

        public AuthorDbo Update<T>(string id, Expression<Func<AuthorDbo, T>> field, T value)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            var filter = filterBuilder.Eq(i => i.Id, id);
            var update = updateBuilder.Set(field, value);

            return collection.FindOneAndUpdate(filter, update) ?? throw new ApplicationException($"author '{id}' was not updated due to not found in db");
        }
    }
}
