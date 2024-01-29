namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultPao : IPublicApiObject
    {
        public required string Search { get; set; }

        public List<SearchResultAuthorPao> Authors { get; set; } = new List<SearchResultAuthorPao>();

        public List<SearchResultWorkPao> Works { get; set; } = new List<SearchResultWorkPao>();
    }
}
