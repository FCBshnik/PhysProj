using Phys.Lib.Admin.Api.Api.System.Cache;

namespace Phys.Lib.Admin.Api.Api.System
{
    public static class SystemEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            CacheEndpoint.Map(builder.MapGroup("cache"));
        }
    }
}
