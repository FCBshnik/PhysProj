using Phys.Lib.Admin.Api.Api.Models;

namespace Phys.Lib.Admin.Api.Api.Health
{
    public static class HealthEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("check", () => TypedResults.Ok(OkModel.Ok))
            .ProducesError()
            .WithName("HealthCheck");
        }
    }
}
