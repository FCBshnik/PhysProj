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
                if (user.Fail)
                    return Results.BadRequest(new ErrorModel(ErrorCode.LoginFailed, user.Error));

                return Results.Ok(new LoginSuccessModel { UserName = user.Value.Name, Token = "token" });
            })
            .Produces<LoginSuccessModel>(200)
            .ProducesError()
            .WithName("Login");
        }
    }
}