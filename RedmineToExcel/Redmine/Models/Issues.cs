using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redmine.Models
{
    class Issues
    {
        public List<Issue> issues { get; set; }
    }

    class Issue
    {
        public int id { get; set; }
        public Info project { get; set; }
        public Info tracker { get; set; }
        public Info status { get; set; }
        public Info priority { get; set; }
        public Info author { get; set; }
        public Info assigned_to { get; set; }
        public Info parent { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime due_date { get; set; }
        public int done_ratio { get; set; }
        public DateTime created_on { get; set; }
        public DateTime updated_on { get; set; }

        [JsonIgnore]
        public List<Issue> children = new List<Issue>();

        [JsonIgnore]
        public int indent = 0;
        public string indentUnit = "";
    }

}
