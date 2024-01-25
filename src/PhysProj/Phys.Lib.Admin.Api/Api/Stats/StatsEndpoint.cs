using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core.Stats;

namespace Phys.Lib.Admin.Api.Api.Stats
{
    public static class StatsEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/library", ([FromServices] IStatService statsService) =>
            {
                return statsService.GetLibraryStats();
            })
            .ProducesResponse<SystemStatsModel>("GetLibraryStats");
        }
    }
}
