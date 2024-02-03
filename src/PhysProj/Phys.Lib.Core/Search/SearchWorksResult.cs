using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Search
{
    public class SearchWorksResult
    {
        public List<WorkDbo> Works { get; set; } = new List<WorkDbo>();

        public List<AuthorDbo> Authors { get; set; } = new List<AuthorDbo>();

        public List<FileDbo> Files { get; set; } = new List<FileDbo>();
    }
}
