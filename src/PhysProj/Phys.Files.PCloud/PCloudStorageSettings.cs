using System.Text.Json.Serialization;

namespace Phys.Files.PCloud
{
    public class PCloudStorageSettings
    {
        public static readonly PCloudStorageSettings Default = new PCloudStorageSettings { BaseFolderId = 0, Username = "test", Password = "123456" };

        [JsonPropertyName("username")]
        public required string Username { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }

        [JsonPropertyName("baseFolderId")]
        public required long BaseFolderId { get; set; }
    }
}
