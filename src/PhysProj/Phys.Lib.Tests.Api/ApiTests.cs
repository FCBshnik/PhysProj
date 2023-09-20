using CliWrap;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Phys.Shared.Logging;
using Phys.Shared.NLog;
using Phys.Shared.Utils;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace Phys.Lib.Tests.Api
{
    public class ApiTests : IDisposable
    {
        protected readonly LoggerFactory loggerFactory = new LoggerFactory();
        private readonly CancellationTokenSource cts = new();
        private readonly MongoDbContainer mongo = new MongoDbBuilder()
            .WithImage("mongo:4.4.18").WithName("physlib-tests-api-mongo").Build();
        private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15.3").WithName("physlib-tests-api-postgres").Build();

        protected readonly ITestOutputHelper output;

        protected HttpClient http = new() { Timeout = TimeSpan.FromSeconds(3) };
        protected DirectoryInfo solutionDir = new(@".\..\..\..\..\");

        public ApiTests(ITestOutputHelper output)
        {
            this.output = output;

            try
            {
                NLogConfig.Configure(loggerFactory, "testsapi");
                ProgramUtils.OnRun(loggerFactory);
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
            await postgres.StartAsync();
        }

        public virtual async Task Release()
        {
            await cts.CancelAsync();
            await mongo.DisposeAsync().AsTask();
            await postgres.DisposeAsync().AsTask();
            http.Dispose();
        }

        protected string GetMongoUrl() => mongo.GetConnectionString();
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

            // modify appsettings.json for test env
            var appSettingsFile = new FileInfo(Path.Combine(appTestDir.FullName, "appsettings.json"));
            if (!appSettingsFile.Exists)
                throw new InvalidOperationException($"Project '{projectPath}' settings file '{appSettingsFile}' not found");
            var appSettings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(appSettingsFile.FullName));
            appSettings!["ConnectionStrings"]!["mongo"] = GetMongoUrl();
            appSettings!["ConnectionStrings"]!["postgres"] = GetPostgresUrl();
            appSettings!["ConnectionStrings"]!["urls"] = url;
            File.WriteAllText(appSettingsFile.FullName, JsonConvert.SerializeObject(appSettings));

            // run
            var appFile = new FileInfo(Path.Combine(appTestDir.FullName, appTestDir.Name) + ".dll");
            DotNetRun(appFile);

            // todo: wait app started
            Thread.Sleep(2000);

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

        private void DotNetRun(FileInfo appFile)
        {
            if (!appFile.Exists)
                throw new InvalidOperationException($"App file '{appFile.FullName}' not found");

        _ = Cli.Wrap("dotnet")
            .WithArguments(a =>
            {
                a.Add(appFile.FullName);
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
