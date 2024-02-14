using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

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

        public static SearchResultWorkModel Map(WorkDbo work, IDictionary<string, FileDbo> files, IDictionary<string, WorkDbo> works, WorkDbo? parentWork = null)
        {
            return new SearchResultWorkModel
            {
                Code = work.Code,
                Name = work.Infos.FirstOrDefault(i => i.Language == Core.Language.Default)?.Name ?? work.Infos.FirstOrDefault()?.Name ?? work.Code,
                Language = work.Language,
                Authors = work.AuthorsCodes,
                Files = work.FilesCodes.Select(a => SearchResultFileModel.Map(files[a])).ToList(),
                SubWorks = work.SubWorksCodes.Select(c => works[c]).Select(w => Map(w, files, works, parentWork: work)).ToList(),
                IsTranslation = parentWork?.Language != null && work.Language != null && parentWork.Language != work.Language
            };
        }
    }
}
