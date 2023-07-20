using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedAuthLibrary
{
    public class Data2
    {
        public string id { get; set; }
        public string profile_image_url { get; set; }
        public string username { get; set; }
        public string name { get; set; }
    }

    public class TwitterUserInfo
    {
        public Data2 data { get; set; }
    }

}
