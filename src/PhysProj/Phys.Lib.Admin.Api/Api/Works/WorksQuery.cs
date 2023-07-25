using Microsoft.AspNetCore.Mvc;

namespace Phys.Lib.Admin.Api.Api.Works
{
    public class WorksQuery
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }
    }
}
