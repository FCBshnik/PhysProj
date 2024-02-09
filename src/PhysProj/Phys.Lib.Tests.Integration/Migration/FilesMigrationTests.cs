using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Files.Local;
using Phys.Files;
using Phys.Lib.Autofac;
using Phys.Mongo.HistoryDb;
using Testcontainers.MongoDb;
using Phys.Lib.Core.Migration;
using Shouldly;
using Phys.Lib.Core.Files;
using Phys.Lib.Tests.Db;
using MongoDB.Driver;

namespace Phys.Lib.Tests.Integration.Migration
{
    public class FilesMigrationTests : BaseTests
    {
        private readonly IConfiguration configuration = ConfigurationFactory.CreateTestConfiguration();

        private readonly MongoDbContainer mongo = TestContainerFactory.CreateMongo();

        public FilesMigrationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Tests()
        {
            using var lifetimeScope = Container.BeginLifetimeScope();
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

        protected override void BuildContainer(ContainerBuilder builder)
        {
            base.BuildContainer(builder);

            var mongoUrl = new MongoUrl(mongo.GetConnectionString());

            builder.Register(_ => configuration).As<IConfiguration>().SingleInstance();

            builder.RegisterModule(new MongoDbModule(mongoUrl, "mongo", loggerFactory));
            builder.RegisterModule(new CoreModule());

            builder.Register(c => new LocalFileStorage("local-1", new DirectoryInfo("data/files-1"), c.Resolve<ILogger<LocalFileStorage>>()))
                    .As<IFileStorage>().SingleInstance();
            builder.Register(c => new LocalFileStorage("local-2", new DirectoryInfo("data/files-2"), c.Resolve<ILogger<LocalFileStorage>>()))
                    .As<IFileStorage>().SingleInstance();

            builder.Register(c => new MongoHistoryDbFactory(mongoUrl, "physlib", "history-", loggerFactory))
                .SingleInstance()
                .AsImplementedInterfaces();
        }

        protected override async Task Init()
        {
            await mongo.StartAsync();
            await base.Init();
        }

        protected override async Task Release()
        {
            await mongo.StopAsync();
            await base.Release();
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
