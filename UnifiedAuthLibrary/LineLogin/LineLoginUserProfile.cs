﻿using System.Text.Json.Serialization;

namespace UnifiedAuthLibrary
{
    public class LineLoginUserProfile
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("pictureUrl")]
        public string PictureUrl { get; set; }

        [JsonPropertyName("statusMessage")]
        public string StatusMessage { get; set; }
    }
}
