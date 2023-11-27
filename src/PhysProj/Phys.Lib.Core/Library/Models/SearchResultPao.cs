namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultPao : IPublicApiObject
    {
        public required string Search { get; set; }

        public List<AuthorPao> Authors { get; set; } = new List<AuthorPao>();

        public List<WorkPao> Works { get; set; } = new List<WorkPao>();
    }
}
