using System.Text.Json.Serialization;

namespace UnifiedAuthLibrary
{
    public class GoogleLoginVerifyIdTokenResult
    {
        [JsonPropertyName("iss")]
        public string Iss { get; set; }

        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
