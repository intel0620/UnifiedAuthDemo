using System.Text.Json.Serialization;

namespace UnifiedAuthDemo.Models
{
    public class LineLoginModel
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
    }

    public class UserInfo
    {
        public string userId { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }
        public string pictureUrl { get; set; }

    }
}
