using CliWrap;
using Fluid;
using PhysLib.Admin.Client;
using Testcontainers.MongoDb;
using Xunit.Abstractions;

namespace Phys.Lib.Tests.Api.Admin
{
    public class AdminTests : IDisposable
    {
        private readonly ITestOutputHelper output;

        private const string apiUrl = "https://localhost:17188/";

        private CancellationTokenSource cts = new CancellationTokenSource();

        private MongoDbContainer mongo = new MongoDbBuilder()
                .WithImage("mongo:4.4.18")
                .Build();

        private HttpClient http = new HttpClient {  Timeout = TimeSpan.FromSeconds(1)};
        private AdminApiClient client;

        private FileInfo appPath = new FileInfo($"C:\\@yan\\dev\\projects\\physics\\git\\src\\PhysProj\\Phys.Lib.Api.Admin\\bin\\Debug\\net8.0\\Phys.Lib.Api.Admin.dll");

        public AdminTests(ITestOutputHelper output)
        {
            this.output = output;
            Init().Wait();
        }

        public void Dispose()
        {
            Log("disposing");

            cts.Cancel();
            mongo.DisposeAsync().AsTask().Wait();
            http.Dispose();

            Log("disposed");
        }

        private async Task Init()
        {
            Log("initializing");

            await mongo.StartAsync();

            var mongoUrl = mongo.GetConnectionString();

            var parser = new FluidParser();
            var ctx = new TemplateContext(new { mongoUrl = mongoUrl, apiUrl = apiUrl });
            var appSettings = parser.Parse(File.ReadAllText("Admin/appsettings.json")).Render(ctx);
            var testSettingsFile = new FileInfo("appsettings.test.admin.json");
            File.WriteAllText(testSettingsFile.FullName, appSettings);

            _ = Cli.Wrap("dotnet")
                .WithArguments(a =>
                {
                    a.Add(appPath.FullName);
                    a.Add("--appsettings").Add(testSettingsFile.FullName);
                })
                .WithWorkingDirectory(appPath.Directory.FullName)
                .ExecuteAsync(cts.Token);

            await Task.Delay(2000);

            client = new AdminApiClient(apiUrl, http);

            Log("initialized");
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

        private void Log(string message)
        {
            output.WriteLine($"{DateTime.UtcNow}: {message}");
        }
    }
}