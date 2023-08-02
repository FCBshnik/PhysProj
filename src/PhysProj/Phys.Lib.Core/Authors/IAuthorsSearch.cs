using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Authors
{
    public interface IAuthorsSearch
    {
        List<AuthorDbo> Find(string? search = null);

        List<AuthorDbo> FindByCodes(List<string> codes);

        AuthorDbo? FindByCode(string code);
    }
}
