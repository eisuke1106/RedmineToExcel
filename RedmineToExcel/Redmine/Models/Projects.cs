using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Redmine.Models
{
    public class Projects
    {
        public List<ProjectData> projects { get; set; }

        [JsonIgnore]
        public List<ProjectData> excludeEndProjects
        {
            get
            {
                if (this.projects == null)
                {
                    return null;
                }

                return this.projects.Where(data => data.isClosed == false).ToList();
            }
        }
    }

    public class Project
    {
        public ProjectData project { get; set; }
    }

    public class ProjectData
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string identifier { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public DateTime created_on { get; set; }
        public DateTime updated_on { get; set; }
        public Boolean is_public { get; set; }
        public Info parent { get; set; }
        public List<ProjectData> Children { get; set; } = new List<ProjectData>();

        [JsonIgnore]
        public bool isClosed {
            get
            {
                if (this.status == 5) return true;
                else return false;
            }
        }

        [JsonIgnore]
        public bool isExpanded { get; set; }

        public Visibility isVisible {
        get
            {
                if (this.isClosed)
                {
                    return Visibility.Collapsed;
                }
                else return Visibility.Visible;
            }
        }


        [JsonIgnore]
        public string statusName
        {
            get
            {
                if (isClosed)
                    return "終了";
                else
                    return "進行中";
            }
        }

    }
}
