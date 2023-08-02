﻿using Phys.Lib.Core.Users;
using Phys.Lib.Db.Users;

namespace Phys.Lib.Admin.Api.Api.User
{
    public class UserResolver
    {
        private readonly IUsersService users;
        private readonly IHttpContextAccessor httpContext;

        public UserResolver(IUsersService users, IHttpContextAccessor httpContext)
        {
            this.users = users;
            this.httpContext = httpContext;
        }

        public UserDbo GetUser()
        {
            return users.GetByName(httpContext.HttpContext.User.Identity.Name);
        }
    }
}
