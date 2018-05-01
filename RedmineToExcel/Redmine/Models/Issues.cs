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

        public string startDateString
        {
            get
            {
                DateTime startDate = DateTime.MaxValue;
                foreach (Issue issue in issues)
                {
                    if (issue.start_date == DateTime.MinValue)
                    {
                        continue;
                    }
                    if (issue.start_date < startDate)
                    {
                        startDate = issue.start_date;
                    }
                }

                if (startDate == DateTime.MaxValue || startDate == DateTime.MinValue)
                {
                    return "N/A";
                }

                return startDate.ToShortDateString();
            }
        }

        public string endDateString
        {
            get
            {
                DateTime endDate = DateTime.MinValue;
                foreach (Issue issue in issues)
                {
                    if (issue.due_date == DateTime.MinValue)
                    {
                        continue;
                    }
                    if (endDate < issue.due_date)
                    {
                        endDate = issue.due_date;
                    }
                }

                if (endDate == DateTime.MinValue || endDate == DateTime.MaxValue)
                {
                    return "N/A";
                }

                return endDate.ToShortDateString();
            }
        }

        public int projectTerm
        {
            get
            {
                if (this.startDateString != "N/A" && this.endDateString != "N/A")
                {
                    DateTime startDate = DateTime.Parse(startDateString);
                    DateTime endDate = DateTime.Parse(endDateString);
                    TimeSpan t = endDate - startDate;
                    return t.Days;
                }
                return 30;
            }
        }
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
        public string assignedUser
        {
            get
            {
                if (assigned_to == null)
                {
                    return "-";
                }
                return assigned_to.name;
            }
        }

        public Info parent { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        [JsonIgnore]
        public string startDateString
        {
            get
            {
                if(this.start_date == DateTime.MinValue)
                {
                    return "-";
                }
                else
                {
                    return this.start_date.ToShortDateString();
                }
            }
        }

        public DateTime due_date { get; set; }
        [JsonIgnore]
        public string dueDateString
        {
            get
            {
                if (this.due_date == DateTime.MinValue)
                {
                    return "-";
                }
                else
                {
                    return this.due_date.ToShortDateString();
                }
            }
        }

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
