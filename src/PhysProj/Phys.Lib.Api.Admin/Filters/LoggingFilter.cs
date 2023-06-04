using NLog;

namespace Phys.Lib.Api.Admin.Filters
{
    public class LoggingFilter : IEndpointFilter
    {
        private static readonly NLog.ILogger log = LogManager.GetLogger("api-log");

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var request = context.HttpContext.Request;
            var isLog = request.Method != "GET";
            if (isLog)
                log.Info($"req [{request.Method}] {request.Path}");

            try
            {
                var result = await next(context);

                if (isLog)
                    log.Info($"res [{request.Method}] [{(result as IStatusCodeHttpResult)?.StatusCode}] {request.Path}");

                return result;
            }
            catch (Exception e)
            {
                log.Error(e, $"internal error");
                return Results.StatusCode(500);
            }
        }
    }
}
