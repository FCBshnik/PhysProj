using Microsoft.AspNetCore.Mvc;

namespace Phys.Lib.Api.Admin.Api.Works
{
    public class WorksQuery
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }
    }
}
