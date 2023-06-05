using CommandLine;
using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Api.Admin.Api.Models;
using Phys.Lib.Core;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Api.Admin.Api.Auth
{
    public static class AuthEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapPost("login", ([FromBody]LoginModel model, [FromServices]App app) =>
            {
                app.Validator.Validate(model);

                var user = app.Users.Login(model.Username, model.Password, UserRole.Admin);
                if (user.Fail)
                    return Results.BadRequest(new ErrorModel(ErrorCode.LoginFailed, user.Error));

                return Results.Ok(new LoginSuccessModel { Token = TokenGenerator.CreateToken(user.Value) });
            })
            .ProducesOk<LoginSuccessModel>()
            .ProducesError()
            .WithName("Login");

            builder.MapGet("user", ([FromServices]UserDbo user) =>
            {
                return Results.Ok(new UserModel
                {
                    Name = user.Name,
                    Roles = user.Roles,
                });
            })
            .Authorize()
            .ProducesOk<UserModel>()
            .WithName("GetUserInfo");
        }
    }
}