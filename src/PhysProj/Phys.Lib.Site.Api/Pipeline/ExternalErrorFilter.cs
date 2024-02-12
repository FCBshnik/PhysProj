using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Phys.Lib.Site.Api.Controllers;

namespace Phys.Lib.Site.Api.Pipeline
{
    public class ExternalErrorFilter : IExceptionFilter
    {
        private readonly ILogger log;

        public ExternalErrorFilter(ILogger log)
        {
            this.log = log;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                log.LogInformation($"validation error: {validationException.Message}");
                context.Result = new ObjectResult(new ErrorModel(ErrorCode.PublicError, validationException.Message)) { StatusCode = 400 };
                context.ExceptionHandled = true;
            }
        }
    }
}
