using System.Text.Json.Serialization;

namespace Phys.Files.PCloud.Models
{
    public class UploadFileResponse : IPCloudResponse
    {
        [JsonPropertyName("result")]
        public int Result { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("metadata")]
        public List<MetadataResponse>? Metadata { get; set; }
    }
}
