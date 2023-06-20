using Autofac;
using Phys.Lib.Core;
using Phys.Lib.Core.Users;
using Phys.Lib.Data;
using Phys.Lib.Admin.Client;
using System.Net;
using Xunit.Abstractions;

namespace Phys.Lib.Tests.Api.Admin
{
    public class AdminTests : ApiTests
    {
        private const string url = "https://localhost:17188/";

        private AdminApiClient? client;

        private FileInfo projectPath => new FileInfo(Path.Combine(solutionDir.FullName, "Phys.Lib.Api.Admin", "Phys.Lib.Api.Admin.csproj"));

        public AdminTests(ITestOutputHelper output) : base(output)
        {
        }

        public override async Task Init()
        {
            await base.Init();

            StartApp(url, projectPath);

            var container = BuildContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var users = scope.Resolve<IUsers>();
                InitDb(users);
            }

            client = new AdminApiClient(url, http);
        }

        private void InitDb(IUsers users)
        {
            users.Create(new CreateUserData { Name = "user", Password = "123456", Role = UserRole.User });
            users.Create(new CreateUserData { Name = "admin", Password = "123qwe", Role = UserRole.Admin });
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DbModule(GetMongoUrl()));
            builder.RegisterModule(new CoreModule());
            return builder.Build();
        }

        [Fact]
        public async void AllTests()
        {
            Log("testing");

            await HealthCheck();
            await LoginFailed();
            await LoginAsUserFailed();
            var token = await LoginAsAdminSuccess();
            await GetUserInfoUnauthorized();
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await GetUserInfoAuthorized();

            Log("tested");
        }

        private async Task HealthCheck()
        {
            var check = await client.HealthCheckAsync();
            Assert.True(check != null, "Health check");
        }

        private async Task LoginFailed()
        {
            var result = await Assert.ThrowsAsync<ApiException<ErrorModel>>(async () => await client.LoginAsync(new LoginModel
            {
                Username = "badUserName",
                Password = "badUserPassword"
            }));

            Assert.Equal(ErrorCode.LoginFailed, result.Result.Code);
        }

        private async Task LoginAsUserFailed()
        {
            var result = await Assert.ThrowsAsync<ApiException<ErrorModel>>(async () => await client.LoginAsync(new LoginModel
            {
                Username = "user",
                Password = "123456"
            }));

            Assert.Equal(ErrorCode.LoginFailed, result.Result.Code);
        }

        private async Task<string> LoginAsAdminSuccess()
        {
            var result = await client.LoginAsync(new LoginModel
            {
                Username = "admin",
                Password = "123qwe"
            });

            Assert.NotEmpty(result.Token);

            return result.Token;
        }

        private async Task GetUserInfoUnauthorized()
        {
            var result = await Assert.ThrowsAsync<ApiException>(client.GetUserInfoAsync);

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        private async Task GetUserInfoAuthorized()
        {
            var result = await client.GetUserInfoAsync();

            Assert.Equal("admin", result.Name);
        }
    }
}