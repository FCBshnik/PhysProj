using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Search;
using Phys.Shared.Utils;
using Phys.Shared;
using Phys.Lib.Core.Works.Cache;

namespace Phys.Lib.Core.Search
{
    internal class SearchService : ISearchService
    {
        private readonly ITextSearch<WorkTso> worksTextSearch;
        private readonly IFilesSearch filesSearch;
        private readonly IAuthorsSearch authorsSearch;
        private readonly IWorksCache worksCache;

        public SearchService(ITextSearch<WorkTso> worksTextSearch, IFilesSearch filesSearch, IAuthorsSearch authorsSearch, IWorksCache worksCache)
        {
            this.worksTextSearch = worksTextSearch;
            this.filesSearch = filesSearch;
            this.authorsSearch = authorsSearch;
            this.worksCache = worksCache;
        }

        public async Task<SearchWorksResult> SearchWorks(string? search, int limit = 16)
        {
            var foundWorks = await worksTextSearch.Search(search ?? string.Empty);

            // first add works found by search with files
            var codes = new List<string>();
            foreach (var found in foundWorks.Where(w => w.Info.HasFiles))
            {
                codes.Add(found.Code);
            }

            // than add all linked works with files
            foreach (var found in foundWorks)
            {
                AddCodesWithFiles(codes, found.Info, root: true);
            }

            codes = codes.Take(limit).ToList();

            var worksMap = worksCache.GetWorks(codes).ToDictionary(w => w.Code);
            if (worksMap.Count != codes.Count)
                throw new PhysException($"works {codes.Except(worksMap.Keys).Join()} found by search but missed in db");

            var works = codes.Select(w => worksMap[w]).ToList();
            var authors = authorsSearch.FindByCodes(works.SelectMany(w => w.AuthorsCodes).Distinct().ToList());
            var files = filesSearch.FindByCodes(works.SelectMany(w => w.FilesCodes).Distinct().ToList());

            return new SearchWorksResult
            {
                Works = works,
                Authors = authors,
                Files = files,
            };
        }

        private void AddCodesWithFiles(ICollection<string> codes, WorkInfoTso info, bool root)
        {
            if (!root && codes.Contains(info.Code))
                return;

            if (!root && info.HasFiles)
                codes.Add(info.Code);

            foreach (var subWork in info.SubWorks)
                AddCodesWithFiles(codes, subWork, false);
            foreach (var translation in info.Translations)
                AddCodesWithFiles(codes, translation, false);
            foreach (var collected in info.Collected)
                AddCodesWithFiles(codes, collected, false);
        }
    }
}
