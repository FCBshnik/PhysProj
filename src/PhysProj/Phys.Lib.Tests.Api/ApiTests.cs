using CliWrap;
using Fluid;
using Testcontainers.MongoDb;

namespace Phys.Lib.Tests.Api
{
    public class ApiTests : IDisposable
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        private MongoDbContainer mongo = new MongoDbBuilder()
                .WithImage("mongo:4.4.18")
                .Build();

        protected HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };

        protected DirectoryInfo solutionDir = new DirectoryInfo(@".\..\..\..\..\");

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

        protected string GetMongoUrl() => mongo.GetConnectionString();

        protected void StartApp(string url, FileInfo projectPath)
        {
            if (url is null) throw new ArgumentNullException(nameof(url));

            if (!projectPath.Exists)
                throw new InvalidOperationException($"Project file '{projectPath}' not found");

            // build
            var appDir = new DirectoryInfo(projectPath.Directory.Name);
            if (appDir.Exists)
                appDir.Delete(true);
            appDir.Create();
            DotNetBuild(projectPath, appDir);

            // add test appsettings.json
            var parser = new FluidParser();
            var ctx = new TemplateContext(new { mongoUrl = GetMongoUrl(), apiUrl = url });
            var appSettings = parser.Parse(File.ReadAllText("appsettings.test.json")).Render(ctx);
            var testSettingsFile = new FileInfo(Path.Combine(appDir.FullName, $"appsettings.api-tests.json"));
            File.WriteAllText(testSettingsFile.FullName, appSettings);

            // run
            var appFile = new FileInfo(Path.Combine(appDir.FullName, projectPath.Directory.Name) + ".dll");
            if (!appFile.Exists)
                throw new InvalidOperationException($"App file '{appFile.FullName}' not found");
            DotNetRun(appFile, testSettingsFile);

            // todo: wait app started
            Thread.Sleep(2000);
        }

        private void DotNetBuild(FileInfo projectPath, DirectoryInfo outDir)
        {
            Log($"building '{projectPath.FullName}'");

            var cmd = Cli.Wrap("dotnet")
                .WithArguments(a =>
                {
                    a.Add("build");
                    a.Add(projectPath.FullName);
                    a.Add("--output").Add(outDir.FullName);
                })
                .ExecuteAsync(cts.Token);

            cmd.Task.Wait();

            Log($"built '{projectPath.FullName}'");
        }

        private void DotNetRun(FileInfo appFile, FileInfo appSettingsFile)
        {
            _ = Cli.Wrap("dotnet")
                .WithArguments(a =>
                {
                    a.Add(appFile.FullName);
                    a.Add("--appsettings").Add(appSettingsFile.FullName);
                })
                .WithWorkingDirectory(appFile.Directory.FullName)
                .ExecuteAsync(cts.Token);
        }

        protected void Log(string message)
        {
            output.WriteLine($"{DateTime.UtcNow}: {message}");
        }
    }
}
