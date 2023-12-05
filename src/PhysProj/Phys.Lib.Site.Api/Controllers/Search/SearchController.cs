using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core;
using Phys.Lib.Core.Library;
using Phys.Lib.Core.Library.Models;

namespace Phys.Lib.Site.Api.Controllers.Search
{
    [ApiController]
    [Route("api/search")]
    public class SearchController
    {
        [ProducesResponseType(typeof(SearchResultPao), 200)]
        [HttpGet]
        public async Task<IPublicApiObject> ListWorks([FromQuery] string? search, [FromServices] ILibraryService libraryService)
        {
            return await libraryService.Search(search);
        }
    }
}
