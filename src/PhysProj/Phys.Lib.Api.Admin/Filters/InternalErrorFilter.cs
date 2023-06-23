using NLog;

namespace Phys.Lib.Api.Admin.Filters
{
    public class InternalErrorFilter : IEndpointFilter
    {
        private static readonly NLog.ILogger log = LogManager.GetLogger("api-log");

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
