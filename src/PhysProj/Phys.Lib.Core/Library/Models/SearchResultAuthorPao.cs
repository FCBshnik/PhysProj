using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultAuthorPao : IPublicApiObject
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public static SearchResultAuthorPao Map(AuthorDbo author)
        {
            return new SearchResultAuthorPao
            {
                Code = author.Code,
                Name = author.Infos.FirstOrDefault()?.FullName ?? author.Code,
            };
        }
    }
}
