namespace Phys.Lib.Admin.Api.Filters
{
    public class StatusCodeLoggingMiddlware : IMiddleware
    {
        private readonly ILogger log;

        public StatusCodeLoggingMiddlware(ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger("api");
        }

        public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
        {
            if (ctx.Request.Method != "GET")
                log.LogInformation("req {method} {path}", ctx.Request.Method, ctx.Request.Path.ToString());

            await next(ctx);

            if (ctx.Response.StatusCode != (int)System.Net.HttpStatusCode.OK)
                log.LogInformation("res {status} to {method} {path}", ctx.Response.StatusCode, ctx.Request.Method, ctx.Request.Path.ToString());
        }
    }
}
