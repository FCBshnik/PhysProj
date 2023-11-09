using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Library.Models;
using Phys.Lib.Core.Works;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phys.Lib.Core.Library
{
    internal class LibraryService : ILibraryService
    {
        private readonly IWorksSearch worksSearch;
        private readonly IAuthorsSearch authorsSearch;
        private readonly IFilesService filesService;

        public LibraryService(IWorksSearch worksSearch, IAuthorsSearch authorsSearch, IFilesService filesService)
        {
            this.worksSearch = worksSearch;
            this.authorsSearch = authorsSearch;
            this.filesService = filesService;
        }

        public List<WorkModel> SearchWorks(string? search)
        {
            var works = worksSearch.Find(search).Where(w => w.FilesCodes.Any()).ToList();
            var authorsCodes = works.SelectMany(w => w.AuthorsCodes).Distinct().ToList();
            var authors = authorsSearch.FindByCodes(authorsCodes).ToDictionary(a => a.Code);
            var filesCodes = works.SelectMany(w => w.FilesCodes).Distinct().ToList();
            var files = filesService.FindByCodes(filesCodes).ToDictionary(a => a.Code);
            return works.Select(w => WorkModel.Map(w, authors, files)).ToList();
        }
    }
}
