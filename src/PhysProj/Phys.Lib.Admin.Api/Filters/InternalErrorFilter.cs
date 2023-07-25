using NLog;

namespace Phys.Lib.Admin.Api.Filters
{
    public class InternalErrorFilter : IEndpointFilter
    {
        private static readonly Logger log = LogManager.GetLogger("api-internal-err");

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            try
            {
                return await next(context);
            }
            catch (Exception e)
            {
                log.Error(e, $"internal error");
                return Results.StatusCode(500);
            }
        }
    }
}
