using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Search;
using Phys.Lib.Core.Works.Cache;
using Phys.Lib.Core.Authors.Cache;
using Phys.Lib.Core.Files.Cache;

namespace Phys.Lib.Core.Search
{
    internal class SearchService : ISearchService
    {
        private readonly ITextSearch<WorkTso> worksTextSearch;
        private readonly IFilesSearch filesSearch;
        private readonly IAuthorsCache authorsCache;
        private readonly IWorksCache worksCache;

        public SearchService(ITextSearch<WorkTso> worksTextSearch, IFilesSearch filesSearch, IAuthorsCache authorsCache, IWorksCache worksCache)
        {
            this.worksTextSearch = worksTextSearch;
            this.filesSearch = filesSearch;
            this.authorsCache = authorsCache;
            this.worksCache = worksCache;
        }

        public async Task<SearchWorksResult> SearchWorks(string? search, int limit = 16)
        {
            var foundWorks = await worksTextSearch.Search(search ?? string.Empty);

            // first add works found by search with files
            var worksCodesResult = new List<string>();
            foreach (var found in foundWorks.Where(w => w.Info.HasFiles))
            {
                worksCodesResult.Add(found.Code);
            }

            // than add all linked works with files
            foreach (var found in foundWorks)
            {
                AddCodesWithFiles(worksCodesResult, found.Info, root: true);
            }

            worksCodesResult = worksCodesResult.Take(limit).ToList();
            var works = worksCache.GetWorks(worksCodesResult);

            var subWorksCodes = works.SelectMany(w => w.SubWorksCodes).Distinct().ToList();
            var subWorks = worksCache.GetWorks(subWorksCodes);

            var authorsCodes = works.SelectMany(w => w.AuthorsCodes).Concat(subWorks.SelectMany(w => w.AuthorsCodes)).Distinct().ToList();
            var authors = authorsCache.GetAuthors(authorsCodes);
            var filesCodes = works.SelectMany(w => w.FilesCodes).Concat(subWorks.SelectMany(w => w.FilesCodes)).Distinct().ToList();
            var files = filesSearch.FindByCodes(filesCodes);

            return new SearchWorksResult
            {
                FoundWorksCodes = worksCodesResult,
                Works = works.Concat(subWorks).ToList(),
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
