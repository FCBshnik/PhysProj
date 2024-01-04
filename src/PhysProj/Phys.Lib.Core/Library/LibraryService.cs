using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Library.Models;
using Phys.Lib.Core.Works;
using Phys.Lib.Search;
using Phys.Shared;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Library
{
    internal class LibraryService : ILibraryService
    {
        private readonly IWorksSearch worksSearch;
        private readonly IAuthorsSearch authorsSearch;
        private readonly IFilesSearch filesSearch;
        private readonly ITextSearch<WorkTso> worksTextSearch;
        private readonly ITextSearch<AuthorTso> authorsTextSearch;

        public LibraryService(IWorksSearch worksSearch, IAuthorsSearch authorsSearch, IFilesSearch filesSearch,
            ITextSearch<WorkTso> worksTextSearch, ITextSearch<AuthorTso> authorsTextSearch)
        {
            this.worksSearch = worksSearch;
            this.authorsSearch = authorsSearch;
            this.filesSearch = filesSearch;
            this.worksTextSearch = worksTextSearch;
            this.authorsTextSearch = authorsTextSearch;
        }

        public async Task<SearchResultPao> Search(string? search)
        {
            return new SearchResultPao
            {
                Search = search ?? string.Empty,
                Works = await SearchWorks(search),
                Authors = await SearchAuthors(search),
            };
        }

        public async Task<List<WorkPao>> SearchWorks(string? search)
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

            var worksMap = worksSearch.FindByCodes(codes).ToDictionary(w => w.Code);
            if (worksMap.Count != codes.Count)
                throw new PhysException($"works {codes.Except(worksMap.Keys).Join()} found by search but missed in db");

            var works = codes.Select(w => worksMap[w]).ToList();
            var authors = authorsSearch.GetByWorksAsMap(works);
            var files = filesSearch.GetByWorksAsMap(works);

            return works.Select(w => WorkPao.Map(w, authors, files)).ToList();
        }

        private void AddCodesWithFiles(ICollection<string> codes, WorkInfoTso info, bool root)
        {
            if (!root && codes.Contains(info.Code))
                return;

            if (!root && info.HasFiles)
                codes.Add(info.Code);

            if (info.Original != null)
                AddCodesWithFiles(codes, info.Original, false);
            foreach (var subWork in info.SubWorks)
                AddCodesWithFiles(codes, subWork, false);
            foreach (var translation in info.Translations)
                AddCodesWithFiles(codes, translation, false);
            foreach (var collected in info.Collected)
                AddCodesWithFiles(codes, collected, false);
        }

        public async Task<List<AuthorPao>> SearchAuthors(string? search)
        {
            var authorsCodes = (await authorsTextSearch.Search(search ?? string.Empty)).Select(w => w.Code).ToList();
            var authors = authorsSearch.FindByCodes(authorsCodes).ToList();
            return authors.Select(AuthorPao.Map).ToList();
        }
    }
}
