using System.Text.Json.Serialization;

namespace Phys.Files.PCloud.Models
{
    public class GetFilePubLinkResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
