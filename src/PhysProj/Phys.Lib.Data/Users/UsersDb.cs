using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Data.Users
{
    internal class UsersDb : Collection<UserDbo>, IUsersDb
    {
        public UsersDb(IMongoCollection<UserDbo> collection) : base(collection)
        {
        }

        public UserDbo Create(UserDbo user)
        {
            user.Id = ObjectId.GenerateNewId().ToString();
            return Insert(user);
        }

        public List<UserDbo> Find(UsersQuery query)
        {
            var filter = filterBuilder.Empty;

            if (query.Code != null)
                filter = filterBuilder.And(filter, filterBuilder.Eq(u => u.Code, query.Code));
            if (query.NameLowerCase != null)
                filter = filterBuilder.And(filter, filterBuilder.Eq(u => u.NameLowerCase, query.NameLowerCase));

            var sort = sortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }

        public UserDbo Get(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            return collection.Find(filterBuilder.Eq(u => u.Id, id)).FirstOrDefault() ?? throw new ApplicationException($"author '{id}' not found in db");
        }
    }
}
