using MongoDB.Driver;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Data.Users
{
    internal class UsersDb : Collection<UserDbo>, IUsersDb
    {
        private readonly IMongoCollection<UserDbo> collection;

        public UsersDb(IMongoCollection<UserDbo> collection)
        {
            this.collection = collection;
        }

        public UserDbo Create(UserDbo user)
        {
            collection.InsertOne(user);
            return user;
        }

        public List<UserDbo> Find(UsersQuery query)
        {
            var filter = Filter.Empty;

            if (query.Code != null)
                filter = Filter.And(filter, Filter.Eq(u => u.Code, query.Code));
            if (query.NameLowerCase != null)
                filter = Filter.And(filter, Filter.Eq(u => u.NameLowerCase, query.NameLowerCase));

            return collection.Find(filter).ToList();
        }

        public UserDbo Get(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            return collection.Find(Filter.Eq(u => u.Id, id)).FirstOrDefault();
        }
    }
}
