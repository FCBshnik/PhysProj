namespace Phys.Lib.Db.Authors
{
    public static class AuthorsDbExtensions
    {
        public static AuthorDbo Get(this IAuthorsDb db, string id)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(id);

            return db.Find(new AuthorsDbQuery { Id = id }).FirstOrDefault() ?? throw new ApplicationException($"author '{id}' not found in '{db.GetType().FullName}'");
        }
    }
}
