using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Library.Models
{
    public class AuthorPao : IPublicApiObject
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public static AuthorPao Map(AuthorDbo author)
        {
            return new AuthorPao
            {
                Code = author.Code,
                Name = author.Infos.FirstOrDefault()?.FullName ?? author.Code,
            };
        }
    }
}
