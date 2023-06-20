using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Api.Admin.Api.Models;
using Phys.Lib.Core.Users;
using Phys.Lib.Core.Validation;

namespace Phys.Lib.Api.Admin.Api.Auth
{
    public static class AuthEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapPost("login", ([FromBody]LoginModel model, [FromServices]IValidator validator, [FromServices]IUsers users) =>
            {
                validator.Validate(model);

                var user = users.Login(model.Username, model.Password, UserRole.Admin);
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