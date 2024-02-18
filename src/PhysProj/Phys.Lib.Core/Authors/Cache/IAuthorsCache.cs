using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Authors.Cache
{
    public interface IAuthorsCache
    {
        List<AuthorDbo> GetAuthors(IEnumerable<string> codes);

        void Set(AuthorDbo author);
    }
}
