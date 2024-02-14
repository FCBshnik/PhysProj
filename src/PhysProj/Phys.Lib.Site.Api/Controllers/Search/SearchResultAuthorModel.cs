using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultAuthorModel
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public static SearchResultAuthorModel Map(AuthorDbo author)
        {
            return new SearchResultAuthorModel
            {
                Code = author.Code,
                Name = author.Infos.FirstOrDefault(i => i.Language == Language.Default)?.FullName ?? author.Infos.FirstOrDefault()?.FullName ?? author.Code,
            };
        }
    }
}
