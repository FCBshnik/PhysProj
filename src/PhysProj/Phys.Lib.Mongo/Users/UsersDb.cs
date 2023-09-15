using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Mongo.Utils;
using Phys.Lib.Db.Users;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;
using Phys.Lib.Db.Reader;

namespace Phys.Lib.Mongo.Users
{
    internal class UsersDb : Collection<UserModel>, IUsersDb, IDbReader<UserDbo>
    {
        public UsersDb(Lazy<IMongoCollection<UserModel>> collection, ILogger<UsersDb> logger) : base(collection, logger)
        {
        }

        public string Name => "mongo";

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

        public void Update(string name, UserDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(update);

            var filter = FilterBuilder.Eq(i => i.NameLowerCase, name.ToLowerInvariant());
            var upd = UpdateBuilder.Combine();

            if (update.AddRole.HasValue())
                upd = upd.Push(i => i.Roles, update.AddRole);
            if (update.DeleteRole.HasValue())
                upd = upd.Pull(i => i.Roles, update.DeleteRole);
            if (update.PasswordHash.HasValue())
                upd = upd.Set(i => i.PasswordHash, update.PasswordHash);

            if (collection.UpdateOne(filter, upd).MatchedCount == 0)
                throw new PhysDbException($"user '{name}' update failed");
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

        IDbReaderResult<UserDbo> IDbReader<UserDbo>.Read(DbReaderQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var filter = FilterBuilder.Empty;
            if (query.Cursor != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Gt(u => u.Id, query.Cursor));
            var sort = SortBuilder.Ascending(i => i.Id);

            var users = collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
            return new DbReaderResult<UserDbo>(users.Select(UserMapper.Map).ToList(), users.LastOrDefault()?.Id);
        }
    }
}
