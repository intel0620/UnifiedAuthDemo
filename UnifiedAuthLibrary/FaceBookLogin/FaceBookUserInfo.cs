using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedAuthLibrary
{

    public class Data
    {
        public int height { get; set; }
        public bool is_silhouette { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Picture
    {
        public Data data { get; set; }
    }

    public class FaceBookUserInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public Picture picture { get; set; }
    }
   
}
