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

            client = new AdminApiClient(url, http);
        }

        [Fact]
        public async void AllTests()
        {
            Log("testing");

            await HealthCheck();
            await LoginFailed();

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
    }
}