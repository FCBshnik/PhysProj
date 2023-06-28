using Phys.Lib.Admin.Client;
using System.Net;

namespace Phys.Lib.Tests.Api.Admin
{
    public partial class AdminTests
    {
        private class UsersTests
        {
            private readonly AdminApiClient api;

            public UsersTests(AdminApiClient api)
            {
                this.api = api;
            }

            public void LoginFailed(LoginModel model)
            {
                api.Invoking(i => i.LoginAsync(model).Result)
                    .Should().Throw<ApiException<ErrorModel>>().Which.Result.Code.Should().Be(ErrorCode.LoginFailed);
            }

            public string LoginSuccess(LoginModel model)
            {
                var result = api.LoginAsync(model).Result;
                result.Token.Should().NotBeEmpty();
                return result.Token;
            }

            public void GetUserInfoUnauthorized()
            {
                var result = Assert.ThrowsAsync<ApiException>(api.GetUserInfoAsync).Result;
                result.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
            }

            public void GetUserInfoAuthorized(string username)
            {
                var result = api.GetUserInfoAsync().Result;
                result.Name.Should().Be(username);
            }
        }
    }
}