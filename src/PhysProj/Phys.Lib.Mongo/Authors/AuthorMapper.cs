using Phys.Lib.Db.Authors;

namespace Phys.Lib.Mongo.Authors
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
                Infos = author.Infos.Select(Map).ToList(),
            };
        }

        public static AuthorDbo.InfoDbo Map(AuthorModel.InfoModel info)
        {
            return new AuthorDbo.InfoDbo
            {
                Description = info.Description,
                FullName = info.FullName,
                Language = info.Language,
            };
        }

        public static AuthorModel.InfoModel Map(AuthorDbo.InfoDbo info)
        {
            return new AuthorModel.InfoModel
            {
                Description = info.Description,
                FullName = info.FullName,
                Language = info.Language,
            };
        }
    }
}
