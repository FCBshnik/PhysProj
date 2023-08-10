namespace Phys.Lib.Db.Authors
{
    public static class AuthorsDbExtensions
    {
        public static AuthorDbo Get(this IAuthorsDb db, string id)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(id);

            var authors = db.Find(new AuthorsDbQuery { Id = id });
            if (authors.Count == 1)
                return authors.First();

            throw new ApplicationException($"failed get author with id '{id}' from '{db.GetType().FullName}' due to found {authors.Count} authors");
        }

        public static AuthorDbo GetByCode(this IAuthorsDb db, string code)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(code);

            var authors = db.Find(new AuthorsDbQuery { Code = code });
            if (authors.Count == 1)
                return authors.First();

            throw new ApplicationException($"failed get author with code '{code}' from '{db.GetType().FullName}' due to found {authors.Count} authors");
        }
    }
}
