using Phys.Lib.Core.Users;

namespace Phys.Lib.Api.Admin.Api.Auth
{
    public class UserResolver
    {
        private readonly IUsers users;
        private readonly IHttpContextAccessor httpContext;

        public UserResolver(IUsers users, IHttpContextAccessor httpContext)
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
