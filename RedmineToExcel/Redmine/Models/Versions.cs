using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redmine.Models
{
    class Versions
    {
        public List<VersionData> versions { get; set; }
    }

    class VersionData
    {
        public int id { get; set; }
        public Info project { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public DateTime due_date { get; set; }
        public string sharing { get; set; }
        public DateTime created_on { get; set; }
        public DateTime updated_on { get; set; }
    }
}
