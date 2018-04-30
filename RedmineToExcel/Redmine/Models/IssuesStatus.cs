using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Redmine.Models
{
    class IssuesStatus
    {
        public List<Status> issue_statuses { get; set; }
    }

    class Status : Info
    {
        [DefaultValue(false)]
        public bool is_closed { get; set; }
    }
}
