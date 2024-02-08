namespace Phys.Lib.Db.Authors
{
    public interface IAuthorsDb : IDbReader<AuthorDbo>
    {
        List<AuthorDbo> Find(AuthorsDbQuery query);

        void Create(string code);

        void Update(string code, AuthorDbUpdate update);

        void Delete(string code);
    }
}
