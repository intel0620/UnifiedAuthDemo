﻿using System.Text.Json.Serialization;

namespace UnifiedAuthDemo.Models
{
    public class FaceBookLoginModel
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
    }

   
}
