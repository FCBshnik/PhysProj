using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Phys.Lib.Site.Api.Pipeline
{
    public class InternalErrorFilter : IExceptionFilter
    {
        private readonly ILogger log;

        public InternalErrorFilter(ILogger log)
        {
            this.log = log;
        }

        public void OnException(ExceptionContext context)
        {
            log.LogError(context.Exception, $"internal error");
            context.Result = new StatusCodeResult(500);
        }
    }
}
