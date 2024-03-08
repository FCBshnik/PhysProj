using FluentAssertions;
using Microsoft.Extensions.Logging;
using Phys.Files.PCloud;
using Phys.Serilog;
using Phys.Shared.Logging;
using Phys.Shared.Utils;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Phys.Tests.External.PCloud
{
    public class PCloudApiClientTests
    {
        protected static readonly LoggerFactory loggerFactory = new LoggerFactory();

        internal readonly static IPCloudApiClient api = PCloudTestConsts.Api;

        private static string? authToken;

        protected readonly ITestOutputHelper output;

        public PCloudApiClientTests(ITestOutputHelper output)
        {
            this.output = output;

            SerilogConfig.Configure(loggerFactory);
            DiagnosticListener.AllListeners.Subscribe(new HttpRequestsObserver(loggerFactory));
        }

        [Fact]
        public void Login()
        {
            var response = api.GetUserInfo(PCloudTestConsts.Settings.Username, PCloudTestConsts.Settings.Password).Result;
            response.Result.Should().Be(0);
            response.Auth.Should().NotBeNull();
            authToken = response.Auth;
        }

        [Fact]
        public void Upload()
        {
            if (authToken == null)
                Login();

            using var file = GetMockStream();
            var res = api.UploadFile(PCloudTestConsts.Settings.BaseFolderId, authToken!, PCloudTestConsts.TestUploadFileName, file).Result;
            res.Result.Should().Be(0);
            res.Metadata.Count.Should().Be(1);
            res.Metadata.First().Fileid.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ListFolderById()
        {
            if (authToken == null)
                Login();

            var response = api.ListFolder(PCloudTestConsts.Settings.BaseFolderId, authToken!).Result;
            response.Result.Should().Be(0);
            var folder = response.Metadata;
            folder.Should().NotBeNull();
            folder.Folderid.Should().BeGreaterThan(0);
            var file = folder!.Contents.First(c => !c.Isfolder);
            file.Name.Should().NotBeEmpty();
            file.Fileid.Should().BeGreaterThan(0);
            file.Size.Should().BeGreaterThan(0);
            DateTimeUtils.UnixSecondsToDateTime(file.Modified).Should().BeAfter(new DateTime(2000, 01, 01));
            file.Parentfolderid.Should().Be(folder.Folderid);
        }

        [Fact]
        public void GetFileStat()
        {
            if (authToken == null)
                Login();

            var response = api.GetFileStat(long.Parse(PCloudTestConsts.TestFile.Id), authToken!).Result;
            response.Result.Should().Be(0);
            var file = response.Metadata!;
            file.Name.Should().NotBeEmpty();
            file.Fileid.Should().Be(long.Parse(PCloudTestConsts.TestFile.Id));
            file.Size.Should().Be(PCloudTestConsts.TestFile.Size);
            file.Name.Should().Be(Path.GetFileName(PCloudTestConsts.TestFile.Name));
            DateTimeUtils.UnixSecondsToDateTime(file.Modified).Should().BeAfter(new DateTime(2000, 01, 01));
        }

        public static Stream GetMockStream()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("mock file");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}