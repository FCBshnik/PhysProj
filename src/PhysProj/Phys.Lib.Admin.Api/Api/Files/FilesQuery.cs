using Microsoft.AspNetCore.Mvc;

namespace Phys.Lib.Admin.Api.Api.Files
{
    public class FilesQuery
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }
    }
}
