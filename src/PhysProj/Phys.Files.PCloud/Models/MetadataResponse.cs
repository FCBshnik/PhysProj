using System.Text.Json.Serialization;

namespace Phys.Files.PCloud.Models
{
    public class MetadataResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("ismine")]
        public bool Ismine { get; set; }

        [JsonPropertyName("thumb")]
        public bool Thumb { get; set; }

        [JsonPropertyName("modified")]
        public long Modified { get; set; }

        [JsonPropertyName("comments")]
        public int Comments { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("isshared")]
        public bool Isshared { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("isfolder")]
        public bool Isfolder { get; set; }

        [JsonPropertyName("parentfolderid")]
        public long Parentfolderid { get; set; }

        [JsonPropertyName("folderid")]
        public long Folderid { get; set; }

        [JsonPropertyName("contents")]
        public List<MetadataResponse> Contents { get; set; }

        [JsonPropertyName("fileid")]
        public long Fileid { get; set; }

        [JsonPropertyName("category")]
        public int Category { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("contenttype")]
        public string Contenttype { get; set; }
    }
}
