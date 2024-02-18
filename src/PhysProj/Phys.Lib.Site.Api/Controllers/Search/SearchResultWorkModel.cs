namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultWorkModel
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public string? Language { get; set; }

        public required List<string> Authors { get; set; }

        public required List<SearchResultWorkModel> SubWorks { get; set; }

        public required List<SearchResultFileModel> Files { get; set; }

        public required bool IsTranslation { get; set; }
    }
}
