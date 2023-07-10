using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Core.Users;
using Phys.Lib.Data.Utils;

namespace Phys.Lib.Data.Users
{
    internal class UsersDb : Collection<UserDbo>, IUsersDb
    {
        public UsersDb(IMongoCollection<UserDbo> collection) : base(collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<UserDbo>(indexBuilder.Ascending(i => i.NameLowerCase), new CreateIndexOptions { Unique = true }));
        }

        public UserDbo Create(UserDbo user)
        {
            user.Id = ObjectId.GenerateNewId().ToString();
            return Insert(user);
        }

        public UserDbo Update(string id, UserDbUpdate user)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (user is null) throw new ArgumentNullException(nameof(user));

            var filter = filterBuilder.Eq(i => i.Id, id);
            var update = updateBuilder.Combine();

            if (user.AddRole.HasValue())
                update = update.Push(i => i.Roles, user.AddRole);
            if (user.DeleteRole.HasValue())
                update = update.Pull(i => i.Roles, user.DeleteRole);
            if (user.PasswordHash.HasValue())
                update = update.Set(i => i.PasswordHash, user.PasswordHash);

            return collection.FindOneAndUpdate(filter, update, findOneAndUpdateReturnAfter)
                ?? throw new ApplicationException($"user '{id}' was not updated due to not found in db");
        }

        public List<UserDbo> Find(UsersDbQuery query)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            var filter = filterBuilder.Empty;
            if (query.NameLowerCase != null)
                filter = filterBuilder.And(filter, filterBuilder.Eq(u => u.NameLowerCase, query.NameLowerCase));

            var sort = sortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }
    }
}
