using Phys.Lib.Api.Admin.Api.Models;
using Phys.Lib.Api.Admin.Filters;

namespace Phys.Lib.Api.Admin.Api
{
    public static class ApiExtensions
    {
        public static RouteGroupBuilder MapEndpoint(this WebApplication app, string prefix, Action<RouteGroupBuilder> map)
        {
            var builder = app.MapGroup(prefix)
                .AddEndpointFilter<LoggingFilter>()
                .AddEndpointFilter<ValidationErrorFilter>()
                .WithOpenApi();
            map(builder);
            return builder;
        }

        public static RouteHandlerBuilder ProducesOk(this RouteHandlerBuilder builder)
        {
            return builder.Produces<OkModel>(200);
        }

        public static RouteHandlerBuilder ProducesError(this RouteHandlerBuilder builder)
        {
            return builder.Produces<ErrorModel>(400);
        }
    }
}
