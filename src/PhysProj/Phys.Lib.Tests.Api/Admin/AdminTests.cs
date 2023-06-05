using Autofac;
using Fluid;
using MongoDB.Driver;
using Phys.Lib.Core;
using Phys.Lib.Core.Users;
using Phys.Lib.Data;
using PhysLib.Admin.Client;
using Xunit.Abstractions;

namespace Phys.Lib.Tests.Api.Admin
{
    public class AdminTests : ApiTests
    {
        private const string url = "https://localhost:17188/";

        private AdminApiClient client;

        private FileInfo appPath = new FileInfo($"C:\\@yan\\dev\\projects\\physics\\git\\src\\PhysProj\\Phys.Lib.Api.Admin\\bin\\Debug\\net8.0\\Phys.Lib.Api.Admin.dll");

        public AdminTests(ITestOutputHelper output) : base(output)
        {
        }

        public override async Task Init()
        {
            await base.Init();

            StartApp(url, appPath.FullName);

            var container = BuildContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<App>();
                InitDb(app);
            }

            client = new AdminApiClient(url, http);
        }

        private void InitDb(App app)
        {
            app.Users.Create(new CreateUserData { Name = "user", Password = "123456", Role = UserRole.User });
            app.Users.Create(new CreateUserData { Name = "admin", Password = "123qwe", Role = UserRole.Admin });
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
            await LoginSuccess();

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
                UserName = "badUserName",
                Password = "badUserPassword"
            }));

            Assert.Equal(ErrorCode.LoginFailed, result.Result.Code);
        }

        private async Task LoginSuccess()
        {
            var result = await client.LoginAsync(new LoginModel
            {
                UserName = "user",
                Password = "123456"
            });

            Assert.Equal("user", result.UserName);
            Assert.NotEmpty(result.Token);
        }
    }
}