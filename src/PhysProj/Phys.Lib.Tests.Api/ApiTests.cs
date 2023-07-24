using CliWrap;
using Fluid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            try
            {
                Log("initializing");
                Init().Wait();
                Log("initialized");
            }
            catch
            {
                Dispose();
                throw;
            }
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var appTestDir = new DirectoryInfo(projectPath.Directory.Name);
            if (appTestDir.Exists)
                appTestDir.Delete(true);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            appTestDir.Create();
            DotNetBuild(projectPath, appTestDir);

            // modify appsettings.json for test env
            var appSettingsFile = new FileInfo(Path.Combine(appTestDir.FullName, "appsettings.json"));
            if (!appSettingsFile.Exists)
                throw new InvalidOperationException($"Project '{projectPath}' settings file '{appSettingsFile}' not found");
            var appSettings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(appSettingsFile.FullName));
            appSettings["ConnectionStrings"]["mongo"] = GetMongoUrl();
            appSettings["ConnectionStrings"]["urls"] = url;
            File.WriteAllText(appSettingsFile.FullName, JsonConvert.SerializeObject(appSettings));

            // run
            var appFile = new FileInfo(Path.Combine(appTestDir.FullName, appTestDir.Name) + ".dll");
            DotNetRun(appFile);

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

        private void DotNetRun(FileInfo appFile)
        {
            if (!appFile.Exists)
                throw new InvalidOperationException($"App file '{appFile.FullName}' not found");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _ = Cli.Wrap("dotnet")
                .WithArguments(a =>
                {
                    a.Add(appFile.FullName);
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
