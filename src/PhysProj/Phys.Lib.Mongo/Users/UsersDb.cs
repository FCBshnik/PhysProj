using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Mongo.Utils;
using Phys.Lib.Db.Users;

namespace Phys.Lib.Mongo.Users
{
    internal class UsersDb : Collection<UserDbo>, IUsersDb
    {
        public UsersDb(Lazy<IMongoCollection<UserDbo>> collection) : base(collection)
        {
        }

        protected override void Init(IMongoCollection<UserDbo> collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<UserDbo>(IndexBuilder.Ascending(i => i.NameLowerCase), new CreateIndexOptions { Unique = true }));
        }

        public void Create(UserDbo user)
        {
            ArgumentNullException.ThrowIfNull(user);

            user.Id = ObjectId.GenerateNewId().ToString();
            Insert(user);
        }

        public void Update(string id, UserDbUpdate user)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(user);

            var filter = FilterBuilder.Eq(i => i.Id, id);
            var update = UpdateBuilder.Combine();

            if (user.AddRole.HasValue())
                update = update.Push(i => i.Roles, user.AddRole);
            if (user.DeleteRole.HasValue())
                update = update.Pull(i => i.Roles, user.DeleteRole);
            if (user.PasswordHash.HasValue())
                update = update.Set(i => i.PasswordHash, user.PasswordHash);

            if (collection.FindOneAndUpdate(filter, update, findOneAndUpdateReturnAfter) == null)
                throw new ApplicationException($"user '{id}' was not updated due to not found in db");
        }

        public List<UserDbo> Find(UsersDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var filter = FilterBuilder.Empty;
            if (query.Id != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.Id, query.Id));
            if (query.NameLowerCase != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.NameLowerCase, query.NameLowerCase));

            var sort = SortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }
    }
}
