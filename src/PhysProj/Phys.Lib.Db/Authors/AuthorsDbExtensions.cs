using System.Xml.Linq;

namespace Phys.Lib.Db.Authors
{
    public static class AuthorsDbExtensions
    {
        public static AuthorDbo GetByCode(this IAuthorsDb db, string code)
        {
            return db.FindByCode(code) ?? throw new PhysDbException($"failed get author with code '{code}' from '{db.GetType().FullName}': author not found");
        }

        public static AuthorDbo? FindByCode(this IAuthorsDb db, string code)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(code);

            var authors = db.Find(new AuthorsDbQuery { Code = code });
            if (authors.Count > 1)
                throw new PhysDbException($"failed find author with code '{code}' from '{db.GetType().FullName}' due to found {authors.Count} authors");

            return authors.Count > 0 ? authors[0] : null;
        }
    }
}
