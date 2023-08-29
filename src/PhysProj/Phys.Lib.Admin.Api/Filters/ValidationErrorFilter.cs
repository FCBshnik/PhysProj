using FluentValidation;
using Phys.Lib.Admin.Api.Api;
using Phys.Lib.Admin.Api.Api.Models;

namespace Phys.Lib.Admin.Api.Filters
{
    public class ValidationErrorFilter : IEndpointFilter
    {
        private readonly ILogger log;

        public ValidationErrorFilter(ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger("api-validation-err");
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            try
            {
                return await next(context);
            }
            catch (ValidationException e)
            {
                log.LogInformation($"validation error: {e.Message}");
                var message = e.Errors.Any() ? e.Errors.First().ErrorMessage : e.Message;
                return Results.BadRequest(new ErrorModel(ErrorCode.InvalidArgument, message));
            }
        }
    }
}
