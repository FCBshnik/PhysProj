namespace Phys.Lib.Db.Users
{
    public static class UsersDbExtension
    {
        public static UserDbo Get(this IUsersDb db, string id)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(id);

            return db.Find(new UsersDbQuery { Id = id }).FirstOrDefault() ?? throw new ApplicationException($"user '{id}' not found in '{db.GetType().FullName}'");
        }
    }
}
