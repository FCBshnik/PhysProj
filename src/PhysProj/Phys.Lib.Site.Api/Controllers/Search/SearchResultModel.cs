namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultModel
    {
        public string? Search { get; set; }

        public List<SearchResultAuthorModel> Authors { get; set; } = new List<SearchResultAuthorModel>();

        public List<SearchResultWorkModel> Works { get; set; } = new List<SearchResultWorkModel>();
    }
}
