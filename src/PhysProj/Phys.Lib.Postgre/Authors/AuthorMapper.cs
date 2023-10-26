using Phys.Lib.Db.Authors;

namespace Phys.Lib.Postgres.Authors
{
    internal static class AuthorMapper
    {
        public static AuthorDbo Map(AuthorModel author)
        {
            return new AuthorDbo
            {
                Code = author.Code,
                Born = author.Born,
                Died = author.Died,
                Infos = author.Infos.Values.Select(Map).ToList(),
            };
        }

        public static AuthorDbo.InfoDbo Map(AuthorModel.InfoModel info)
        {
            return new AuthorDbo.InfoDbo
            {
                Language = info.Language,
                FullName = info.FullName,
                Description = info.Description,
            };
        }
    }
}
