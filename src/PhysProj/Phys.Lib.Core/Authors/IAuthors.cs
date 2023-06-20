using System.Linq.Expressions;

namespace Phys.Lib.Core.Authors
{
    public interface IAuthors
    {
        List<AuthorDbo> Search(string search);

        AuthorDbo? GetByCode(string code);

        AuthorDbo Create(string code);

        AuthorDbo Update<T>(AuthorDbo author, Expression<Func<AuthorDbo, T>> field, T value);
    }
}