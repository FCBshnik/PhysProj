namespace Phys.Lib.Db.Users
{
    public static class UsersDbExtension
    {
        public static UserDbo GetByName(this IUsersDb db, string name)
        {
            return FindByName(db, name) ?? throw new PhysDbException($"failed get user with name '{name}' from '{db.GetType().FullName}' due user not found");
        }

        public static UserDbo? FindByName(this IUsersDb db, string name)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(name);

            var users = db.Find(new UsersDbQuery { NameLowerCase = name.ToLowerInvariant() });
            if (users.Count > 1)
                throw new PhysDbException($"failed find user with name '{name}' from '{db.GetType().FullName}' due to found {users.Count} users");

            return users.Count > 0 ? users[0] : null;
        }
    }
}
