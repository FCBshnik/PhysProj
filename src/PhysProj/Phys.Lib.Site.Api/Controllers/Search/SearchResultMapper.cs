using Phys.Lib.Core;
using Phys.Lib.Core.Library.Models;
using Phys.Lib.Core.Search;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Site.Api.Controllers.Search
{
    internal static class SearchResultMapper
    {
        public static SearchResultAuthorModel Map(AuthorDbo author)
        {
            return new SearchResultAuthorModel
            {
                Code = author.Code,
                Name = author.Infos.FirstOrDefault(i => i.Language == Language.Default)?.FullName ?? author.Infos.FirstOrDefault()?.FullName ?? author.Code,
            };
        }

        public static SearchResultFileModel Map(FileDbo file)
        {
            return new SearchResultFileModel
            {
                Code = file.Code,
                Format = file.Format,
                Size = file.Size,
            };
        }

        public static SearchResultWorkModel Map(WorkDbo work, IDictionary<string, FileDbo> files, IDictionary<string, WorkDbo> works, WorkDbo? parentWork = null)
        {
            return new SearchResultWorkModel
            {
                Code = work.Code,
                Name = work.Infos.FirstOrDefault(i => i.Language == Language.Default)?.Name ?? work.Infos.FirstOrDefault()?.Name ?? work.Code,
                Language = work.Language,
                Authors = work.AuthorsCodes,
                Files = work.FilesCodes.Select(a => Map(files[a])).ToList(),
                SubWorks = work.SubWorksCodes.Select(c => works[c]).Select(w => Map(w, files, works, parentWork: work)).ToList(),
                IsTranslation = parentWork?.Language != null && work.Language != null && parentWork.Language != work.Language
            };
        }

        public static SearchResultModel Map(SearchWorksResult result)
        {
            var works = result.Works.ToDictionary(f => f.Code);
            var files = result.Files.ToDictionary(f => f.Code);

            return new SearchResultModel
            {
                Search = string.Empty,
                Works = result.FoundWorksCodes.Select(w => Map(works[w], files, works)).ToList(),
                Authors = result.Authors.Select(Map).ToList(),
            };
        }
    }
}
