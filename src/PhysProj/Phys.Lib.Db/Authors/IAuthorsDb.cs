namespace Phys.Lib.Db.Authors
{
    public interface IAuthorsDb
    {
        List<AuthorDbo> Find(AuthorsDbQuery query);

        AuthorDbo Create(string code);

        AuthorDbo Update(string id, AuthorDbUpdate update);

        void Delete(string id);
    }
}
