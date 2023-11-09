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

        [HttpGet("files/{code}/download", Name = "GetFileLink")]
        public object GetFileLink(string code, [FromServices]IFilesService filesService, [FromServices]IFileStorages fileStorages)
        {
            var file = filesService.FindByCode(code);
            if (file == null)
                return BadRequest(new ErrorModel(ErrorCode.NotFound, "file not found"));

            var link = file.Links.FirstOrDefault();
            if (link == null)
                return BadRequest(new ErrorModel(ErrorCode.NotFound, "file is not available"));

            var storage = fileStorages.Get(link.StorageCode);
            var data = storage.Download(link.FileId);
            return File(data, "application/octet-stream", $"{file.Code}.{file.Format}");
        }
    }
}
