using CliWrap;
using Fluid;
using Testcontainers.MongoDb;
using Xunit.Abstractions;

namespace Phys.Lib.Tests.Api
{
    public class ApiTests : IDisposable
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        private MongoDbContainer mongo = new MongoDbBuilder()
                .WithImage("mongo:4.4.18")
                .Build();

        protected HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };

        private readonly ITestOutputHelper output;

        public ApiTests(ITestOutputHelper output)
        {
            this.output = output;

            Log("initializing");
            Init().Wait();
            Log("initialized");
        }

        public void Dispose()
        {
            Log("releasing");
            Release().Wait();
            Log("released");
        }

        public virtual async Task Init()
        {
            await mongo.StartAsync();
        }

        public virtual async Task Release()
        {
            await cts.CancelAsync();
            await mongo.DisposeAsync().AsTask();
            http.Dispose();
        }

        protected void StartApp(string url, string appPath)
        {
            if (url is null) throw new ArgumentNullException(nameof(url));

            var appFile = new FileInfo(appPath);
            if (!appFile.Exists)
                throw new InvalidOperationException($"App file '{appPath}' not found");

            var mongoUrl = mongo.GetConnectionString();
            var parser = new FluidParser();
            var ctx = new TemplateContext(new { mongoUrl = mongoUrl, apiUrl = url });
            var appSettings = parser.Parse(File.ReadAllText("appsettings.test.json")).Render(ctx);
            var testSettingsFile = new FileInfo($"appsettings.test.{appFile.Directory.Name}.json");
            File.WriteAllText(testSettingsFile.FullName, appSettings);

            _ = Cli.Wrap("dotnet")
                .WithArguments(a =>
                {
                    a.Add(appFile.FullName);
                    a.Add("--appsettings").Add(testSettingsFile.FullName);
                })
                .WithWorkingDirectory(appFile.Directory.FullName)
                .ExecuteAsync(cts.Token);

            // todo: wait app started
            Thread.Sleep(2000);
        }

        protected void Log(string message)
        {
            output.WriteLine($"{DateTime.UtcNow}: {message}");
        }
    }
}
