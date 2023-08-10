namespace Phys.Lib.Db.Users
{
    public static class UsersDbExtension
    {
        public static UserDbo Get(this IUsersDb db, string id)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(id);

            var users = db.Find(new UsersDbQuery { Id = id });
            if (users.Count == 1)
                return users[0];

            throw new ApplicationException($"failed get user with id '{id}' from '{db.GetType().FullName}' due to found {users.Count} users");
        }

        public static UserDbo GetByName(this IUsersDb db, string name)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(name);

            var users = db.Find(new UsersDbQuery { NameLowerCase = name.ToLowerInvariant() });
            if (users.Count == 1)
                return users[0];

            throw new ApplicationException($"failed get user with name '{name}' from '{db.GetType().FullName}' due to found {users.Count} users");
        }
    }
}
