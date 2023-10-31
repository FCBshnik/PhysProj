using Microsoft.AspNetCore.Mvc;

namespace Phys.Lib.Site.Api.Controllers.Health
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [ProducesResponseType(typeof(OkModel), 200)]
        [HttpGet("check")]
        public object Check()
        {
            return Ok(OkModel.Ok);
        }
    }
}
