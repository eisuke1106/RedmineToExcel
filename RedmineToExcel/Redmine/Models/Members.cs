using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Redmine.Models
{
    class Members
    {
        public List<MemberData> memberships { get; set; }
    }

    class MemberData
    {
        public int id { get; set; }
        public Info user { get; set; }
        public Info group { get; set; }

        [JsonIgnore]
        public bool isUser
        {
            get
            {
                if (this.user != null)
                    return true;
                else
                    return false;
            }
        }

        [JsonIgnore]
        public bool isGroup
        {
            get
            {
                if (this.group != null)
                    return true;
                else
                    return false;
            }
        }
    }
}
