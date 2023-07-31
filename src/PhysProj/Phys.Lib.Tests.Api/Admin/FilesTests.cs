using Phys.Lib.Admin.Client;

namespace Phys.Lib.Tests.Api.Admin
{
    public partial class AdminTests
    {
        private class FilesTests
        {
            private readonly AdminApiClient api;

            public FilesTests(AdminApiClient api)
            {
                this.api = api;
            }

            public void ListStorages(params string[] expectedCodes)
            {
                var result = api.ListFileStoragesAsync().Result;
                result.Select(a => a.Code).Should().BeEquivalentTo(expectedCodes);
            }

            public void ListStorageFiles(string storageCode, params string[] expectedPaths)
            {
                var result = api.ListFileStorageFilesAsync(storageCode, search: null).Result;
                result.Select(a => a.Path).Should().BeEquivalentTo(expectedPaths);
            }

            public void ListFilesLinks(params string[] expectedCodes)
            {
                var result = api.ListFilesLinksAsync(search: null).Result;
                result.Select(a => a.Code).Should().BeEquivalentTo(expectedCodes);
            }

            public void LinkFileStorageFile(string storageCode, string path)
            {
                var result = api.LinkFileStorageFileAsync(storageCode, new FileStorageLinkModel { FilePath = path }).Result;
                result.Links.Should().HaveCount(1);
                result.Links.First().Type.Should().Be(storageCode);
                result.Links.First().Path.Should().Be(path);
            }

            public Stream GetMockStream()
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write("mock data");
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }
    }
}
