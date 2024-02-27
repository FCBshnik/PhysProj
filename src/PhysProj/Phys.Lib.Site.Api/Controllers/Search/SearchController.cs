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
            return SearchResultMapper.Map(result);
        }
    }
}
