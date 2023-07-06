namespace Phys.Lib.Core.Authors
{
    public interface IAuthorsSearch
    {
        List<AuthorDbo> FindByText(string search);

        List<AuthorDbo> FindByCodes(List<string> codes);

        AuthorDbo? FindByCode(string code);
    }
}
