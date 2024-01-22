using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db;
using Phys.Lib.Db.Users;
using SqlKata;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Postgres.Users
{
    internal class UsersDb : PostgresTable, IUsersDb
    {
        private readonly Lazy<NpgsqlDataSource> dataSource;

        public string Name => "postgres";

        public UsersDb(Lazy<NpgsqlDataSource> dataSource, string tableName, ILogger<UsersDb> logger) : base(tableName, logger)
        {
            this.dataSource = dataSource;
        }

        public void Create(UserDbo user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userModel = new UserInsertModel { Name = user.Name, NameLowerCase = user.NameLowerCase, PasswordHash = user.PasswordHash };

            using (var cnx = dataSource.Value.OpenConnection())
            Insert(cnx, userModel);
        }

        public List<UserDbo> Find(UsersDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var cmd = new Query(tableName)
                .Limit(query.Limit);

            if (query.NameLowerCase != null)
                cmd = cmd.Where(UserModel.NameLowerCaseColumn, query.NameLowerCase);

            using var cnx = dataSource.Value.OpenConnection();
            return Find<UserModel>(cnx, cmd).ConvertAll(UserMapper.Map);
        }

        public void Update(string name, UserDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(update);

            using var cnx = dataSource.Value.OpenConnection();
            using (var trx = cnx.BeginTransaction())
            {
                var updateDic = new Dictionary<string, object>();
                var user = Find(new UsersDbQuery { NameLowerCase = name.ToLowerInvariant() }).FirstOrDefault() ?? throw new PhysDbException($"user '{name}' not not found");

                if (update.AddRole != null)
                {
                    user.Roles.Add(update.AddRole);
                    // set some token being to replace
                    updateDic[UserModel.RolesColumn] = $"#{UserModel.RolesColumn}#";
                }

                if (update.DeleteRole != null)
                {
                    user.Roles.Remove(update.DeleteRole);
                    // set some token being to replace
                    updateDic[UserModel.RolesColumn] = $"#{UserModel.RolesColumn}#";
                }

                if (update.PasswordHash != null)
                    updateDic[UserModel.PasswordHashColumn] = update.PasswordHash;

                var updateCmd = new Query(tableName)
                    .Where(UserModel.NameLowerCaseColumn, name)
                    .AsUpdate(updateDic);

                // SqlKata can't deal with arrays, replace array binding in compiled sql to array field (dapper can do it)
                // TODO: try to use postgres function 'array_append'
                var res = Execute(cnx, updateCmd, sql =>
                {
                    var pair = sql.NamedBindings.FirstOrDefault(p => (p.Value as string) == $"#{UserModel.RolesColumn}#");
                    if (pair.Key != null)
                        sql.NamedBindings[pair.Key] = user.Roles;
                });
                if (res == 0)
                    throw new PhysDbException($"user '{name}' update failed");

                trx.Commit();
            }
        }

        IDbReaderResult<UserDbo> IDbReader<UserDbo>.Read(DbReaderQuery query)
        {
            using var cnx = dataSource.Value.OpenConnection();
            return Read<UserDbo, UserModel>(cnx, query, UserModel.IdColumn, UserMapper.Map);
        }
    }
}
