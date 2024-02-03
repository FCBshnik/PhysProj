using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultWorkModel
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public required List<string> Authors { get; set; }

        public required List<SearchResultFileModel> Files { get; set; }

        public static SearchResultWorkModel Map(WorkDbo work, IDictionary<string, FileDbo> files)
        {
            return new SearchResultWorkModel
            {
                Code = work.Code,
                Name = work.Infos.FirstOrDefault()?.Name ?? work.Code,
                Authors = work.AuthorsCodes,
                Files = work.FilesCodes.Select(a => SearchResultFileModel.Map(files[a])).ToList(),
            };
        }
    }
}
