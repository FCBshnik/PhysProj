using FluentAssertions;
using Microsoft.Extensions.Logging;
using Phys.Files.PCloud;
using Phys.NLog;
using Phys.Shared.Logging;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Phys.Tests.External.PCloud
{
    public class PCloudFileStorageTests
    {
        protected static readonly LoggerFactory loggerFactory = new LoggerFactory();

        private readonly PCloudFileStorage storage = new PCloudFileStorage(PCloudTestConsts.Api, () => PCloudTestConsts.Settings, loggerFactory.CreateLogger<PCloudFileStorage>());

        protected readonly ITestOutputHelper output;

        public PCloudFileStorageTests(ITestOutputHelper output)
        {
            this.output = output;
            NLogConfig.Configure(loggerFactory);
            DiagnosticListener.AllListeners.Subscribe(new HttpRequestsObserver(loggerFactory));
        }

        [Fact]
        public void ListNoSearch_ReturnsAll()
        {
            var files = storage.List(null);
            files.Count.Should().Be(3);
        }

        [Fact]
        public void ListWithSearch_ReturnsMatched()
        {
            var files = storage.List("Nature");
            files.Count.Should().Be(1);
            files.First().Name.Should().Be("Lucretius - On the Nature of Things [tr William Ellery Leonard].txt");
        }

        [Fact]
        public void GetExisting_Found()
        {
            var info = storage.Get(PCloudTestConsts.TestFile.Id);
            info.Should().NotBeNull();
            info!.Size.Should().Be(PCloudTestConsts.TestFile.Size);
            info!.Name.Should().Be(PCloudTestConsts.TestFile.Name);
            info.Updated.Should().BeAfter(new DateTime(2000, 01, 01));
        }

        [Fact]
        public void GetNotExisting_NotFound()
        {
            var info = storage.Get("111");
            info.Should().BeNull();
        }

        [Fact]
        public void DownloadExisting_Downloaded()
        {
            using var stream = storage.Download(PCloudTestConsts.TestFile.Id);
            using var content = new MemoryStream();
            stream.CopyTo(content);
            content.Length.Should().Be(PCloudTestConsts.TestFile.Size);
        }

        [Fact]
        public void UploadWithoutPath_Uploaded()
        {
            using var stream = PCloudApiClientTests.GetMockStream();
            var info = storage.Upload(stream, "test.txt");
            info.Id.Should().NotBeNull();
            info.Name.Should().Be("test.txt");
        }

        [Fact]
        public void UploadWithPath_Uploaded()
        {
            using var stream = PCloudApiClientTests.GetMockStream();
            var info = storage.Upload(stream, "level0/level1/test.txt");
            info.Id.Should().NotBeNull();
            info.Name.Should().Be("level0/level1/test.txt");
        }
    }
}
