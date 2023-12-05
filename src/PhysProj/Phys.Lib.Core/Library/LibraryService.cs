using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Library.Models;
using Phys.Lib.Core.Works;
using Phys.Lib.Search;

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
            var worksCodes = (await worksTextSearch.Search(search ?? string.Empty)).Select(w => w.Code).ToList();
            var works = worksSearch.FindByCodes(worksCodes).Where(w => w.FilesCodes.Any()).ToList();
            var authorsCodes = works.SelectMany(w => w.AuthorsCodes).Distinct().ToList();
            var authors = authorsSearch.FindByCodes(authorsCodes).ToDictionary(a => a.Code);
            var filesCodes = works.SelectMany(w => w.FilesCodes).Distinct().ToList();
            var files = filesSearch.FindByCodes(filesCodes).ToDictionary(a => a.Code);
            return works.Select(w => WorkPao.Map(w, authors, files)).ToList();
        }

        public async Task<List<AuthorPao>> SearchAuthors(string? search)
        {
            var authorsCodes = (await authorsTextSearch.Search(search ?? string.Empty)).Select(w => w.Code).ToList();
            var authors = authorsSearch.FindByCodes(authorsCodes).ToList();
            return authors.Select(AuthorPao.Map).ToList();
        }
    }
}
