namespace Phys.Lib.Admin.Api.Filters
{
    public class InternalErrorFilter : IEndpointFilter
    {
        private readonly ILogger log;

        public InternalErrorFilter(ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger("api-internal-err");
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            try
            {
                return await next(context);
            }
            catch (Exception e)
            {
                log.LogError(e, $"internal error");
                return Results.StatusCode(500);
            }
        }
    }
}
