using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Api.Admin.Api.Models;
using Phys.Lib.Core;

namespace Phys.Lib.Api.Admin.Api.Auth
{
    public static class AuthEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapPost("login", ([FromBody]LoginModel model, [FromServices]App app) =>
            {
                app.Validator.Validate(model);

                var user = app.Users.Login(model.UserName, model.Password);
                if (user == null)
                    return Results.BadRequest(new ErrorModel("login-failed", "Invalid username or password"));

                return Results.Ok();
            });
        }
    }
}