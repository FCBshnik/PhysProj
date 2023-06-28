using Phys.Lib.Api.Admin.Api.Models;
using Phys.Lib.Api.Admin.Filters;

namespace Phys.Lib.Api.Admin.Api
{
    public static class ApiExtensions
    {
        public static RouteGroupBuilder MapEndpoint(this WebApplication app, string prefix, Action<RouteGroupBuilder> map)
        {
            var builder = app.MapGroup("api/" + prefix)
                .AddEndpointFilter<InternalErrorFilter>()
                .AddEndpointFilter<ValidationErrorFilter>()
                .WithOpenApi()
                .WithTags(prefix);
            map(builder);
            return builder;
        }

        public static RouteHandlerBuilder ProducesResponse<T>(this RouteHandlerBuilder builder, string endpointName)
        {
            return builder.Produces<T>(200)
                .Produces<ErrorModel>(400)
                .WithName(endpointName);
        }

        public static RouteHandlerBuilder ProducesOk<T>(this RouteHandlerBuilder builder)
        {
            return builder.Produces<T>(200);
        }

        public static RouteHandlerBuilder ProducesOk(this RouteHandlerBuilder builder)
        {
            return builder.Produces<OkModel>(200);
        }

        public static RouteHandlerBuilder ProducesError(this RouteHandlerBuilder builder)
        {
            return builder.Produces<ErrorModel>(400);
        }

        public static RouteHandlerBuilder Authorize(this RouteHandlerBuilder builder)
        {
            return builder.RequireAuthorization("admin");
        }
    }
}
