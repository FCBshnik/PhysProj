using Phys.Lib.Admin.Api.Api;
using Phys.Lib.Admin.Api.Api.Models;

namespace Phys.Lib.Admin.Api.Api.Health
{
    public static class HealthEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("check", () => Results.Ok(OkModel.Ok))
            .ProducesOk()
            .ProducesError()
            .WithName("HealthCheck");
        }
    }
}
