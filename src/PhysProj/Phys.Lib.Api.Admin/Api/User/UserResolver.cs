using Phys.Lib.Core.Users;

namespace Phys.Lib.Api.Admin.Api.User
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
            return users.GetByName(httpContext.HttpContext.User.Identity.Name) ?? throw new ApplicationException();
        }
    }
}
