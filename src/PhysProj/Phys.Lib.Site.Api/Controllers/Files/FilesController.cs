using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core.Files;
using Phys.Lib.Site.Api.Controllers.Works;

namespace Phys.Lib.Site.Api.Controllers.Files
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(FileLinkModel), 200)]
        [HttpGet("{code}/download/link", Name = "GetFileDownloadLink")]
        public object GetFileDownloadLink(string code, [FromServices] IFilesSearch filesService, [FromServices] IFileDownloadService downloadService)
        {
            var file = filesService.FindByCode(code);
            if (file == null)
                return BadRequest(new ErrorModel(ErrorCode.NotFound, "file not found"));

            var link = downloadService.GetDownloadLink(file);
            return new FileLinkModel { Url = link.Url.AbsoluteUri };
        }
    }
}
