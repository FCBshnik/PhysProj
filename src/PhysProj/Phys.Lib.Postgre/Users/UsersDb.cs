using Npgsql;
using Phys.Lib.Db.Users;
using SqlKata;

namespace Phys.Lib.Postgres.Users
{
    internal class UsersDb : PostgresTable, IUsersDb
    {
        private readonly NpgsqlDataSource dataSource;

        public UsersDb(NpgsqlDataSource dataSource, string tableName) : base(tableName)
        {
            this.dataSource = dataSource;
        }

        public void Create(UserDbo user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userModel = new UserInsertModel { Name = user.Name, NameLowerCase = user.NameLowerCase, PasswordHash = user.PasswordHash };

            using (var cnx = dataSource.OpenConnection())
            Insert(cnx, userModel);
        }

        public List<UserDbo> Find(UsersDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var cmd = new Query(tableName).Limit(query.Limit);

            if (query.NameLowerCase != null)
                cmd = cmd.Where(UserModel.NameLowerCaseColumn, query.NameLowerCase);

            using var cnx = dataSource.OpenConnection();
            return Find<UserDbo>(cnx, cmd);
        }

        public void Update(string nameLowerCase, UserDbUpdate update)
        {
            ArgumentNullException.ThrowIfNull(nameLowerCase);
            ArgumentNullException.ThrowIfNull(update);

            using var cnx = dataSource.OpenConnection();
            using (var trx = cnx.BeginTransaction())
            {
                var updateDic = new Dictionary<string, object>();
                var user = Find(new UsersDbQuery { NameLowerCase = nameLowerCase }).FirstOrDefault() ?? throw new ApplicationException($"user '{nameLowerCase}' not not found");

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
                    .Where(UserModel.NameLowerCaseColumn, nameLowerCase)
                    .AsUpdate(updateDic);

                // SqlKata can't deal with arrays, replace array binding in compiled sql to array field (dapper can do it)
                // todo: try to use postgres function 'array_append'
                var res = Execute(cnx, updateCmd, sql =>
                {
                    var pair = sql.NamedBindings.FirstOrDefault(p => (p.Value as string) == $"#{UserModel.RolesColumn}#");
                    if (pair.Key != null)
                        sql.NamedBindings[pair.Key] = user.Roles;
                });
                if (res == 0)
                    throw new ApplicationException($"user '{nameLowerCase}' update failed");

                trx.Commit();
            }
        }
    }
}
