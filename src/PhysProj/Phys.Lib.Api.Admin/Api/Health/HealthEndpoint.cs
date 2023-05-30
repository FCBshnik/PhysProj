namespace Phys.Lib.Api.Admin.Api.Health
{
    public static class HealthEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("check", () => Results.Ok());
        }
    }
}
