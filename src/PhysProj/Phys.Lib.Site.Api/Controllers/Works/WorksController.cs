using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core.Library;
using Phys.Lib.Core.Library.Models;

namespace Phys.Lib.Site.Api.Controllers.Works
{
    [ApiController]
    [Route("api/works")]
    public class WorksController : ControllerBase
    {
        [Obsolete]
        [ProducesResponseType(typeof(List<WorkPao>), 200)]
        [HttpGet(Name = "ListWorks")]
        public object ListWorks([FromQuery]string? search, [FromServices]ILibraryService libraryService)
        {
            var works = libraryService.SearchWorks(search);
            return Ok(works);
        }
    }
}
