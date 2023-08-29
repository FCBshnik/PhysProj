using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Mongo.Utils;
using Phys.Lib.Db.Users;
using Microsoft.Extensions.Logging;

namespace Phys.Lib.Mongo.Users
{
    internal class UsersDb : Collection<UserModel>, IUsersDb
    {
        public UsersDb(Lazy<IMongoCollection<UserModel>> collection, ILogger<UsersDb> logger) : base(collection, logger)
        {
        }

        protected override void Init(IMongoCollection<UserModel> collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<UserModel>(IndexBuilder.Ascending(i => i.NameLowerCase), new CreateIndexOptions { Unique = true }));
        }

        public void Create(UserDbo user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userModel = new UserModel
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = user.Name,
                NameLowerCase = user.NameLowerCase,
                PasswordHash = user.PasswordHash
            };
            Insert(userModel);
        }

        public void Update(string nameLowerCase, UserDbUpdate user)
        {
            ArgumentNullException.ThrowIfNull(nameLowerCase);
            ArgumentNullException.ThrowIfNull(user);

            var filter = FilterBuilder.Eq(i => i.NameLowerCase, nameLowerCase);
            var update = UpdateBuilder.Combine();

            if (user.AddRole.HasValue())
                update = update.Push(i => i.Roles, user.AddRole);
            if (user.DeleteRole.HasValue())
                update = update.Pull(i => i.Roles, user.DeleteRole);
            if (user.PasswordHash.HasValue())
                update = update.Set(i => i.PasswordHash, user.PasswordHash);

            if (collection.UpdateOne(filter, update).MatchedCount == 0)
                throw new ApplicationException($"user '{nameLowerCase}' update failed");
        }

        public List<UserDbo> Find(UsersDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var filter = FilterBuilder.Empty;
            if (query.NameLowerCase != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.NameLowerCase, query.NameLowerCase));

            var sort = SortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList().Select(UserMapper.Map).ToList();
        }
    }
}
