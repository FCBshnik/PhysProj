using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core.Library.Models;
using Phys.Lib.Core.Search;

namespace Phys.Lib.Site.Api.Controllers.Search
{
    [ApiController]
    [Route("api/search")]
    public class SearchController
    {
        [ProducesResponseType(typeof(SearchResultModel), 200)]
        [HttpGet]
        public async Task<SearchResultModel> ListWorks([FromQuery] string? search, [FromServices] ISearchService searchService)
        {
            var result = await searchService.SearchWorks(search);
            var files = result.Files.ToDictionary(f => f.Code);
            var authors = result.Authors.ToDictionary(f => f.Code);

            return new SearchResultModel
            {
                Search = search,
                Works = result.Works.Select(w => SearchResultWorkModel.Map(w, files)).ToList(),
                Authors = result.Authors.Select(a => SearchResultAuthorModel.Map(a)).ToList(),
            };
        }
    }
}
