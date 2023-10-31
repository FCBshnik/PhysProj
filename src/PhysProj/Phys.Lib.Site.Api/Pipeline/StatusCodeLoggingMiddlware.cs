namespace Phys.Lib.Site.Api.Pipeline
{
    public class StatusCodeLoggingMiddlware : IMiddleware
    {
        private readonly ILogger log;

        public StatusCodeLoggingMiddlware(ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger("api");
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method != "GET")
                log.LogInformation("req {method} {path}", context.Request.Method, context.Request.Path.ToString());

            await next(context);

            if (context.Response.StatusCode != (int)System.Net.HttpStatusCode.OK)
                log.LogInformation("res {status} to {method} {path}", context.Response.StatusCode, context.Request.Method, context.Request.Path.ToString());
        }
    }
}
