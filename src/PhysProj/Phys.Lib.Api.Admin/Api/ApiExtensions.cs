using Phys.Lib.Api.Admin.Filters;

namespace Phys.Lib.Api.Admin.Api
{
    public static class ApiExtensions
    {
        public static RouteGroupBuilder MapEndpoint(this WebApplication app, string prefix, Action<RouteGroupBuilder> map)
        {
            var builder = app.MapGroup(prefix)
                .AddEndpointFilter<ValidationErrorFilter>()
                .WithOpenApi();
            map(builder);
            return builder;
        }
    }
}
