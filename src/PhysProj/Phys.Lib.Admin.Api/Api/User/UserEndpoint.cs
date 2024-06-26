﻿using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core.Users;
using Phys.Lib.Core.Validation;
using Phys.Lib.Admin.Api.Api.Models;
using Phys.Lib.Db.Users;

namespace Phys.Lib.Admin.Api.Api.User
{
    public static class UserEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapPost("login", ([FromBody] LoginModel model, [FromServices] IValidator validator, [FromServices] IUsersService service) =>
            {
                validator.Validate(model);

                var user = service.Login(model.Username, model.Password, UserRole.Admin);
                if (user.Fail)
                    return Results.BadRequest(new ErrorModel(ErrorCode.LoginFailed, user.Error));

                return TypedResults.Ok(new LoginSuccessModel { Token = TokenGenerator.CreateToken(user.Value) });
            })
            .ProducesResponse<LoginSuccessModel>("Login");

            builder.MapGet("/", ([FromServices] UserDbo user) =>
            {
                return TypedResults.Ok(new UserModel
                {
                    Name = user.Name,
                    Roles = user.Roles,
                });
            })
            .Authorize()
            .WithName("GetUserInfo");
        }
    }
}
