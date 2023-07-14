using CliWrap;
using Fluid;
using Testcontainers.MongoDb;

namespace Phys.Lib.Tests.Api
{
    public class ApiTests : IDisposable
    {
        private readonly CancellationTokenSource cts = new();
        private readonly MongoDbContainer mongo = new MongoDbBuilder().WithImage("mongo:4.4.18").Build();
        private readonly ITestOutputHelper output;

        protected HttpClient http = new() { Timeout = TimeSpan.FromSeconds(3) };
        protected DirectoryInfo solutionDir = new(@".\..\..\..\..\");

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
            ArgumentNullException.ThrowIfNull(url);

            if (!projectPath.Exists)
                throw new InvalidOperationException($"Project file '{projectPath}' not found");

            // build
            var appDir = projectPath.Directory;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (appDir.Exists)
                appDir.Delete(true);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            appDir.Create();
            DotNetBuild(projectPath, appDir);

            // add test appsettings.json
            var parser = new FluidParser();
            var ctx = new TemplateContext(new { mongoUrl = GetMongoUrl(), apiUrl = url });
            var appSettings = parser.Parse(File.ReadAllText("appsettings.test.json")).Render(ctx);
            var testSettingsFile = new FileInfo(Path.Combine(appDir.FullName, $"appsettings.api-tests.json"));
            File.WriteAllText(testSettingsFile.FullName, appSettings);

            // run
            var appFile = new FileInfo(Path.Combine(appDir.FullName, appDir.Name) + ".dll");
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
            if (!appFile.Exists)
                throw new InvalidOperationException($"App file '{appFile.FullName}' not found");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _ = Cli.Wrap("dotnet")
                .WithArguments(a =>
                {
                    a.Add(appFile.FullName);
                    a.Add("--appsettings").Add(appSettingsFile.FullName);
                })
                .WithWorkingDirectory(appFile.Directory.FullName)
                .ExecuteAsync(cts.Token);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        protected void Log(string message)
        {
            output.WriteLine($"{DateTime.UtcNow}: {message}");
        }
    }
}
