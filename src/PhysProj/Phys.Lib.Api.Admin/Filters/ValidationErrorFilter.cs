﻿using FluentValidation;
using NLog;
using Phys.Lib.Api.Admin.Api;
using Phys.Lib.Api.Admin.Api.Models;

namespace Phys.Lib.Api.Admin.Filters
{
    public class ValidationErrorFilter : IEndpointFilter
    {
        private static readonly NLog.ILogger log = LogManager.GetLogger("api-validation");

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            try
            {
                return await next(context);
            }
            catch (ValidationException e)
            {
                log.Info($"validation error: {e.Message}");
                return Results.BadRequest(new ErrorModel(ErrorCode.InvalidArgument, e.Message));
            }
        }
    }
}