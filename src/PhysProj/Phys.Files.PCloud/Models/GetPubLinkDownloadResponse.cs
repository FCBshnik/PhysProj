using System.Text.Json.Serialization;

namespace Phys.Files.PCloud.Models
{
    public class GetPubLinkDownloadResponse
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("hosts")]
        public List<string> Hosts { get; set; }
    }
}
