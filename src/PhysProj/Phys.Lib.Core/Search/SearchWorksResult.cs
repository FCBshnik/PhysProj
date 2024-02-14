using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Search
{
    public class SearchWorksResult
    {
        public required List<string> FoundWorksCodes { get; set; }

        public required List<WorkDbo> Works { get; set; }

        public required List<AuthorDbo> Authors { get; set; }

        public required List<FileDbo> Files { get; set; }
    }
}
