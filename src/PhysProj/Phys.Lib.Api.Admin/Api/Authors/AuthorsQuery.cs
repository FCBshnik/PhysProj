using Microsoft.AspNetCore.Mvc;

namespace Phys.Lib.Api.Admin.Api.Authors
{
    public class AuthorsQuery
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }
    }
}
