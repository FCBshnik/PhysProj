using System.Text.Json.Serialization;

namespace Phys.Files.PCloud.Models
{
    public class UserInfoResponse : IPCloudResponse
    {
        [JsonPropertyName("result")]
        public int Result { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("cryptosetup")]
        public bool Cryptosetup { get; set; }

        [JsonPropertyName("plan")]
        public int Plan { get; set; }

        [JsonPropertyName("cryptosubscription")]
        public bool Cryptosubscription { get; set; }

        [JsonPropertyName("publiclinkquota")]
        public long Publiclinkquota { get; set; }

        [JsonPropertyName("userid")]
        public int Userid { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("trashrevretentiondays")]
        public int Trashrevretentiondays { get; set; }

        [JsonPropertyName("auth")]
        public string? Auth { get; set; }

        [JsonPropertyName("emailverified")]
        public bool Emailverified { get; set; }

        [JsonPropertyName("usedpublinkbranding")]
        public bool Usedpublinkbranding { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("agreedwithpp")]
        public bool Agreedwithpp { get; set; }

        [JsonPropertyName("haspassword")]
        public bool Haspassword { get; set; }

        [JsonPropertyName("quota")]
        public long Quota { get; set; }

        [JsonPropertyName("cryptolifetime")]
        public bool Cryptolifetime { get; set; }

        [JsonPropertyName("premium")]
        public bool Premium { get; set; }

        [JsonPropertyName("premiumlifetime")]
        public bool Premiumlifetime { get; set; }

        [JsonPropertyName("business")]
        public bool Business { get; set; }

        [JsonPropertyName("usedquota")]
        public int Usedquota { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("haspaidrelocation")]
        public bool Haspaidrelocation { get; set; }

        [JsonPropertyName("freequota")]
        public long Freequota { get; set; }

        [JsonPropertyName("registered")]
        public long Registered { get; set; }

        [JsonPropertyName("journey")]
        public JourneyResponse? Journey { get; set; }

        public class JourneyResponse
        {
            [JsonPropertyName("steps")]
            public StepsResponse? Steps { get; set; }
        }

        public class StepsResponse
        {
            [JsonPropertyName("verifymail")]
            public bool Verifymail { get; set; }

            [JsonPropertyName("uploadfile")]
            public bool Uploadfile { get; set; }

            [JsonPropertyName("autoupload")]
            public bool Autoupload { get; set; }

            [JsonPropertyName("downloadapp")]
            public bool Downloadapp { get; set; }

            [JsonPropertyName("downloaddrive")]
            public bool Downloaddrive { get; set; }

            [JsonPropertyName("sentinvitation")]
            public bool Sentinvitation { get; set; }
        }
    }
}
