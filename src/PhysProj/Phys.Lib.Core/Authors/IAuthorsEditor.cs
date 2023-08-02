using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Authors
{
    public interface IAuthorsEditor
    {
        AuthorDbo Create(string code);

        AuthorDbo UpdateLifetime(AuthorDbo author, string? born, string? died);

        AuthorDbo UpdateInfo(AuthorDbo author, AuthorDbo.InfoDbo info);

        AuthorDbo DeleteInfo(AuthorDbo author, string language);

        void Delete(AuthorDbo author);
    }
}