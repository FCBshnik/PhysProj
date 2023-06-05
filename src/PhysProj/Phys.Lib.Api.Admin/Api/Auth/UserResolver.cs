using Phys.Lib.Core;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Api.Admin.Api.Auth
{
    public class UserResolver
    {
        private readonly App app;
        private readonly IHttpContextAccessor httpContext;

        public UserResolver(App app, IHttpContextAccessor httpContext)
        {
            this.app = app;
            this.httpContext = httpContext;
        }

        public UserDbo GetUser()
        {
            return app.Users.FindByName(httpContext.HttpContext.User.Identity.Name) ?? throw new ApplicationException();
        }
    }
}
