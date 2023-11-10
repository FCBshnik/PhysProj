using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Files.Storage;
using Phys.Lib.Core.Library;
using Phys.Lib.Core.Library.Models;

namespace Phys.Lib.Site.Api.Controllers.Works
{
    [ApiController]
    [Route("api/works")]
    public class WorksController : ControllerBase
    {
        [ProducesResponseType(typeof(List<WorkModel>), 200)]
        [HttpGet(Name = "ListWorks")]
        public object ListWorks([FromQuery]string? search, [FromServices]ILibraryService libraryService)
        {
            var works = libraryService.SearchWorks(search);
            return Ok(works);
        }
    }
}
