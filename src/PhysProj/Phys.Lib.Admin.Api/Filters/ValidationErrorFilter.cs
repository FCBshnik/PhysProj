using FluentValidation;
using NLog;
using Phys.Lib.Admin.Api.Api;
using Phys.Lib.Admin.Api.Api.Models;

namespace Phys.Lib.Admin.Api.Filters
{
    public class ValidationErrorFilter : IEndpointFilter
    {
        private static readonly Logger log = LogManager.GetLogger("api-validation-err");

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            try
            {
                return await next(context);
            }
            catch (ValidationException e)
            {
                log.Info($"validation error: {e.Message}");
                var message = e.Errors.Any() ? e.Errors.First().ErrorMessage : e.Message;
                return Results.BadRequest(new ErrorModel(ErrorCode.InvalidArgument, message));
            }
        }
    }
}
