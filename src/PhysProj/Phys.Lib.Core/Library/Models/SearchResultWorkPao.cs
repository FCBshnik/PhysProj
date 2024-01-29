using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultWorkPao : IPublicApiObject
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public required List<string> Authors { get; set; }

        public required List<SearchResultFilePao> Files { get; set; }

        public static SearchResultWorkPao Map(WorkDbo work, IDictionary<string, FileDbo> files)
        {
            return new SearchResultWorkPao
            {
                Code = work.Code,
                Name = work.Infos.FirstOrDefault()?.Name ?? work.Code,
                Authors = work.AuthorsCodes,
                Files = work.FilesCodes.Select(a => SearchResultFilePao.Map(files[a])).ToList(),
            };
        }
    }
}
