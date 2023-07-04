namespace Phys.Lib.Core.Authors
{
    public interface IAuthorsDb
    {
        List<AuthorDbo> Find(AuthorsQuery query);

        AuthorDbo Get(string id);

        AuthorDbo GetByCode(string code);

        AuthorDbo Create(AuthorDbo author);

        AuthorDbo Update(string id, AuthorUpdate update);

        void Delete(string id);
    }
}
