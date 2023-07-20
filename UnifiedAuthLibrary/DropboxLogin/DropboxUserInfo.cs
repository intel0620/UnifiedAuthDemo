using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedAuthLibrary
{
    public class Name
    {
        public string abbreviated_name { get; set; }
        public string display_name { get; set; }
        public string familiar_name { get; set; }
        public string given_name { get; set; }
        public string surname { get; set; }
    }

    public class DropboxUserInfo
    {
        public string account_id { get; set; }
        public bool disabled { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
        public bool is_teammate { get; set; }
        public Name name { get; set; }
        public string profile_photo_url { get; set; }
    }


    
}
