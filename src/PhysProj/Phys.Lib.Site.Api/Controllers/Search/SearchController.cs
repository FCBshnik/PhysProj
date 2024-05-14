using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Phys.Lib.Core.Library.Models;
using Phys.Lib.Core.Search;

namespace Phys.Lib.Site.Api.Controllers.Search
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(SearchResultModel), 200)]
        [EnableRateLimiting(policyName: "search")]
        [HttpGet]
        public async Task<object> ListWorks([FromQuery] string? search, [FromServices] ISearchService searchService)
        {
            if (search?.Length > 50)
                return BadRequest(new ErrorModel(ErrorCode.InvalidRequest, "Search query is too long"));

            var result = await searchService.SearchWorks(search);
            return SearchResultMapper.Map(result);
        }
    }
}
