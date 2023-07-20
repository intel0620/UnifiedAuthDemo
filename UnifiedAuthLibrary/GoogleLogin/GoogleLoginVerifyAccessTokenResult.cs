using System.Text.Json.Serialization;

namespace UnifiedAuthLibrary
{
    public class GoogleLoginVerifyAccessTokenResult
    {
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("client_id")]
        public string CliendId { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
