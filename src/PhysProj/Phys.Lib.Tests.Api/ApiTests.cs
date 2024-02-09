using CliWrap;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Phys.NLog;
using Phys.Shared;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace Phys.Lib.Tests.Api
{
    public class ApiTests : IDisposable
    {
        protected readonly LoggerFactory loggerFactory = new LoggerFactory();
        private readonly CancellationTokenSource cts = new();
        private readonly MongoDbContainer mongo = TestContainerFactory.CreateMongo();
        private readonly PostgreSqlContainer postgres = TestContainerFactory.CreatePostgres();

        protected readonly ITestOutputHelper output;

        protected HttpClient http = new() { Timeout = TimeSpan.FromSeconds(3) };
        protected DirectoryInfo solutionDir = new(@".\..\..\..\..\");

        public ApiTests(ITestOutputHelper output)
        {
            this.output = output;

            try
            {
                NLogConfig.Configure(loggerFactory);
                PhysAppContext.Init(loggerFactory);
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

        protected virtual async Task Init()
        {
            await mongo.StartAsync();
            await postgres.StartAsync();
        }

        protected virtual async Task Release()
        {
            await cts.CancelAsync();
            await mongo.DisposeAsync().AsTask();
            await postgres.DisposeAsync().AsTask();
            http.Dispose();
        }

        protected MongoUrl GetMongoUrl() => new MongoUrl(mongo.GetConnectionString());
        protected string GetPostgresUrl() => postgres.GetConnectionString();

        protected DirectoryInfo StartApp(string url, FileInfo projectPath)
        {
            ArgumentNullException.ThrowIfNull(url);

            if (!projectPath.Exists)
                throw new InvalidOperationException($"Project file '{projectPath}' not found");

            // build
            var appTestDir = new DirectoryInfo(projectPath.Directory!.Name);
            if (appTestDir.Exists)
                appTestDir.Delete(true);
            appTestDir.Create();
            DotNetBuild(projectPath, appTestDir);

            // override connection strings
            var srcSettingsFile = new FileInfo(Path.Combine(solutionDir.FullName, "appsettings.lib.dev.json"));
            var appSettings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(srcSettingsFile.FullName));
            appSettings!["ConnectionStrings"]!["mongo-test"] = GetMongoUrl().ToString();
            appSettings!["ConnectionStrings"]!["mongo"] = "mongo-test";
            appSettings!["ConnectionStrings"]!["postgres"] = GetPostgresUrl();
            appSettings!["ConnectionStrings"]!["db"] = "mongo-test";
            appSettings!["ConnectionStrings"]!["urls"] = url;
            appSettings!["ConnectionStrings"]!["works-files"] = "data/files";

            var testSettingsFile = new FileInfo(Path.Combine(appTestDir.FullName, "appsettings.tests.json"));
            File.WriteAllText(testSettingsFile.FullName, JsonConvert.SerializeObject(appSettings));

            // run
            var appFile = new FileInfo(Path.Combine(appTestDir.FullName, appTestDir.Name) + ".dll");
            DotNetRun(appFile, testSettingsFile.FullName);

            // todo: wait app started
            Thread.Sleep(5000);

            return appTestDir;
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

        private void DotNetRun(FileInfo appFile, string appsettingsPath)
        {
            if (!appFile.Exists)
                throw new InvalidOperationException($"App file '{appFile.FullName}' not found");

        _ = Cli.Wrap("dotnet")
            .WithArguments(a =>
            {
                a.Add(appFile.FullName);
                a.Add("--appsettings");
                a.Add(appsettingsPath);
            })
            .WithWorkingDirectory(appFile.Directory!.FullName)
            .ExecuteAsync(cts.Token);
        }

        protected void Log(string message)
        {
            output.WriteLine($"{DateTime.UtcNow}: {message}");
        }
    }
}
