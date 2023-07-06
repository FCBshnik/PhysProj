namespace Phys.Lib.Core.Authors
{
    public interface IAuthorsSearch
    {
        List<AuthorDbo> Search(string search);

        AuthorDbo? FindByCode(string code);
    }
}
