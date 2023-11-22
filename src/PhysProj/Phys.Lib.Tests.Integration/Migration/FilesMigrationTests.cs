using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Files.Local;
using Phys.Files;
using Phys.Lib.Autofac;
using Phys.Mongo.HistoryDb;
using Phys.NLog;
using Phys.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MongoDb;
using Phys.Lib.Core.Migration;
using Shouldly;
using Phys.Lib.Core.Files;
using Phys.Lib.Tests.Db;

namespace Phys.Lib.Tests.Integration.Migration
{
    public class FilesMigrationTests : IDisposable
    {
        private readonly IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { { "ConnectionStrings:db", "mongo" } }).Build();

        private readonly MongoDbContainer mongo = TestContainerFactory.CreateMongo();

        protected readonly LoggerFactory loggerFactory = new LoggerFactory();

        protected readonly ITestOutputHelper output;

        public FilesMigrationTests(ITestOutputHelper output)
        {
            this.output = output;

            try
            {
                NLogConfig.Configure(loggerFactory, "tests-db");
                AppUtils.OnRun(loggerFactory);
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

        [Fact]
        public void Tests()
        {
            using var container = BuildContainer();
            using var lifetimeScope = container.BeginLifetimeScope();
            var filesEditor = lifetimeScope.Resolve<IFilesEditor>();
            var filesSearch = lifetimeScope.Resolve<IFilesSearch>();
            var migrations = lifetimeScope.Resolve<IMigrationService>();
            var storages = lifetimeScope.Resolve<IEnumerable<IFileStorage>>();
            var storageSrc = storages.First();
            var storageDst = storages.Skip(1).First();
            var file1 = storageSrc.Upload(GetMockStream("file-1"), "dir-1/file-1.txt");
            var file2 = storageSrc.Upload(GetMockStream("file-2"), "dir-1/file-2.txt");
            var file3 = storageSrc.Upload(GetMockStream("file-3"), "dir-2/file-3.txt");
            filesEditor.CreateFileFromStorage(storageSrc.Code, file1.Id);
            filesEditor.CreateFileFromStorage(storageSrc.Code, file2.Id);
            filesEditor.CreateFileFromStorage(storageSrc.Code, file3.Id);

            var migration = migrations.Create(new MigrationTask { Migrator = "files-content", Source = storageSrc.Code, Destination = storageDst.Code });
            migrations.Execute(migration);
            migration.Error.ShouldBeNull(migration.Error);
            migration.Stats.Created.ShouldBe(3);

            var files = filesSearch.Find();
            files.Count.ShouldBe(3);
            var dstLinks = files.SelectMany(f => f.Links.Where(l => l.StorageCode == storageDst.Code)).ToList();
            dstLinks.Count.ShouldBe(3);
            dstLinks.ForEach(l => storageDst.Get(l.FileId).ShouldNotBeNull());
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => configuration).As<IConfiguration>().SingleInstance();
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new MongoDbModule(mongo.GetConnectionString(), loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.Register(c => new LocalFileStorage("local-1", new DirectoryInfo("data/files-1"), c.Resolve<ILogger<LocalFileStorage>>()))
                    .As<IFileStorage>().SingleInstance();
            builder.Register(c => new LocalFileStorage("local-2", new DirectoryInfo("data/files-2"), c.Resolve<ILogger<LocalFileStorage>>()))
                    .As<IFileStorage>().SingleInstance();

            builder.Register(c => new MongoHistoryDbFactory(mongo.GetConnectionString(), "physlib", "history-", loggerFactory))
                .SingleInstance()
                .AsImplementedInterfaces();

            return builder.Build();
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
        }

        protected virtual async Task Release()
        {
            await mongo.StopAsync();
        }

        protected void Log(string message)
        {
            output.WriteLine($"{DateTime.UtcNow}: {message}");
        }

        public static Stream GetMockStream(string data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
