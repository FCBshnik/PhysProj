using Phys.Lib.Core.Users;
using Phys.Lib.Db.Users;
using Phys.Shared;

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
            var name = httpContext.HttpContext?.User?.Identity?.Name;
            if (name == null)
                throw new PhysException($"identity name is missed in http context");

            return users.GetByName(name);
        }
    }
}
