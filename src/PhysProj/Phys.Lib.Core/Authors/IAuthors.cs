namespace Phys.Lib.Core.Authors
{
    public interface IAuthors
    {
        List<AuthorDbo> Search(string search);

        AuthorDbo? GetByCode(string code);

        AuthorDbo Create(string code);

        AuthorDbo Update(AuthorDbo author, AuthorUpdate update);

        void Delete(AuthorDbo author);
    }
}