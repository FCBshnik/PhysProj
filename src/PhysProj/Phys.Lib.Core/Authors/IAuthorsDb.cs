namespace Phys.Lib.Core.Authors
{
    public interface IAuthorsDb
    {
        List<AuthorDbo> Find(AuthorsDbQuery query);

        AuthorDbo Create(AuthorDbo author);

        AuthorDbo Update(string id, AuthorDbUpdate update);

        void Delete(string id);
    }
}
