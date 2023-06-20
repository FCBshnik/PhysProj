using System.Linq.Expressions;

namespace Phys.Lib.Core.Authors
{
    public interface IAuthorsDb
    {
        AuthorDbo Create(AuthorDbo author);

        AuthorDbo Get(string id);

        List<AuthorDbo> Find(AuthorsQuery query);

        AuthorDbo Update<T>(string id, Expression<Func<AuthorDbo, T>> field, T value);
    }
}
