using Phys.Files;
using Phys.Files.PCloud;
using Refit;

namespace Phys.Tests.External.PCloud
{
    internal static class PCloudTestConsts
    {
        internal readonly static IPCloudApiClient Api = RestService.For<IPCloudApiClient>("https://eapi.pcloud.com/");

        internal readonly static PCloudStorageSettings Settings = new PCloudStorageSettings
        {
            Username = "fcbshnik@gmail.com",
            Password = "e331380e840b72810ec0fe230553da22",
            BaseFolderId = 8084200774,
        };

        internal readonly static StorageFileInfo TestFile = new StorageFileInfo
        {
            Id = "33055797181",
            Name = "sub-test/За материализм.pdf",
            Size = 281375
        };

        internal readonly static string TestUploadFileName = "test-upload.txt";
    }
}
