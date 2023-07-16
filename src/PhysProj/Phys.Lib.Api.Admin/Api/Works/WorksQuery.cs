using Microsoft.AspNetCore.Mvc;

namespace Phys.Lib.Api.Admin.Api.Works
{
    public class WorksQuery
    {
        [FromQuery]
        public string? Search { get; set; }
    }
}
