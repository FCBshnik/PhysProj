using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Phys.Lib.Site.Api.Controllers;

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
            context.Result = new ObjectResult(new ErrorModel(ErrorCode.InternalError, "something go wrong...")) { StatusCode = 500 };
            context.ExceptionHandled = true;
        }
    }
}
