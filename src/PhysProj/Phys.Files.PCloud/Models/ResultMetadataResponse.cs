using System.Text.Json.Serialization;

namespace Phys.Files.PCloud.Models
{
    public class ResultMetadataResponse
    {
        [JsonPropertyName("result")]
        public int Result { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("metadata")]
        public MetadataResponse Metadata { get; set; }
    }
}
