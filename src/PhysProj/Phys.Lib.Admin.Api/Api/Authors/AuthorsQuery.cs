using Microsoft.AspNetCore.Mvc;

namespace Phys.Lib.Admin.Api.Api.Authors
{
    public class AuthorsQuery
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }
    }
}
