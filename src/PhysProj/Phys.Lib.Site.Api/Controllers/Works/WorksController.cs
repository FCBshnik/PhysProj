using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Works;

namespace Phys.Lib.Site.Api.Controllers.Works
{
    [ApiController]
    [Route("api/works")]
    public class WorksController : ControllerBase
    {
        [ProducesResponseType(typeof(List<WorkModel>), 200)]
        [HttpGet("", Name = "ListWorks")]
        public object ListWorks([FromServices] IWorksSearch worksSearch, [FromServices]IAuthorsSearch authorsSearch)
        {
            var works = worksSearch.Find().Where(w => w.FilesCodes.Any()).ToList();
            var authorsCodes = works.SelectMany(w => w.AuthorsCodes).Distinct().ToList();
            var authors = authorsSearch.FindByCodes(authorsCodes).ToDictionary(a => a.Code);
            return Ok(works.Select(w => WorkModel.Map(w, authors)).ToList());
        }
    }
}
