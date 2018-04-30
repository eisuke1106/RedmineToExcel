using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedmineToExcel
{
    class Settings
    {
        private static Settings instance = new Settings();
        public string redmineUrl = null;
        public string redmineApiKey = null;

        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        private Settings()
        {
            this.redmineUrl = Properties.Settings.Default.Redmine_Url;
            this.redmineApiKey = Properties.Settings.Default.ApiKey;
        }

        public bool IsValid()
        {
            if (this.redmineApiKey == "" || this.redmineUrl == "")
            {
                return false;
            }

            return true;
        }

        public void Save()
        {
            Properties.Settings.Default.Redmine_Url = this.redmineUrl;
            Properties.Settings.Default.ApiKey = this.redmineApiKey;
            Properties.Settings.Default.Save();
        }
    }
}
