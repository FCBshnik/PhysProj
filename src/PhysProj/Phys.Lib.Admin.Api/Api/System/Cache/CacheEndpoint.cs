using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Admin.Api.Api.Models;
using Phys.Lib.Core.Works.Cache;
using Phys.Shared.EventBus;

namespace Phys.Lib.Admin.Api.Api.System.Cache
{
    public static class CacheEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapPost("works/invalidate", ([FromServices]IEventBus eventBus) =>
            {
                eventBus.Publish(new WorksCacheInvalidatedEvent());
                return TypedResults.Ok(OkModel.Ok);
            })
            .ProducesError()
            .WithName("InvalidateWorksCache");
        }
    }
}
