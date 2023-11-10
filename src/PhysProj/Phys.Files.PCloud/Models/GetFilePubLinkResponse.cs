using System.Text.Json.Serialization;

namespace Phys.Files.PCloud.Models
{
    public class GetFilePubLinkResponse
    {
        [JsonPropertyName("result")]
        public int Result { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
