using System.Text.Json.Serialization;

namespace Phys.Files.PCloud.Models
{
    public class GetPubLinkDownloadResponse
    {
        [JsonPropertyName("path")]
        public required string Path { get; set; }

        [JsonPropertyName("hosts")]
        public required List<string> Hosts { get; set; }
    }
}
