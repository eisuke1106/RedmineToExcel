using Newtonsoft.Json;
using Redmine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Redmine
{
    static class RedmineApi
    {
        public static string baseUrl = "";

        public static string apiKey = "";

        public static string GetApi(string url)
        {
            try
            {
                var Req = HttpWebRequest.Create(url);
                HttpWebResponse Res = (HttpWebResponse)Req.GetResponse();
                StreamReader st_Reader = new StreamReader(Res.GetResponseStream(), new UTF8Encoding(false));
                string response = st_Reader.ReadToEnd();
                return response;
            }
            catch (Exception)
            {

                //throw;
            }

            return "";
        }

        private static string projectListEPjson = "/projects.json";

        public static Projects GetProjects()
        {
            Debug.WriteLine("[Redmine]プロジェクト情報を取得します。");
            string url = string.Format(baseUrl + projectListEPjson + "?key=" + apiKey);
            var response = GetApi(url);
            var projects = JsonConvert.DeserializeObject<Projects>(response);
            return projects;
        }

        private static string projectEPjson = @"/projects/{0}.json";

        public static Project GetProject(int projectId)
        {
            Debug.WriteLine("[Redmine]プロジェクト情報(" + projectId + ")を取得します。");
            string url = string.Format(baseUrl + projectEPjson + "?key=" + apiKey, projectId);
            var response = GetApi(url);
            var project = JsonConvert.DeserializeObject<Project>(response);
            return project;
        }

        private static string projectMemberEPjson = @"/projects/{0}/memberships.json";

        public static Members GetProjectMembers(int projectId)
        {
            Debug.WriteLine("[Redmine]プロジェクト情報(" + projectId + ")を取得します。");
            string url = string.Format(baseUrl + projectMemberEPjson + "?key=" + apiKey, projectId);
            var response = GetApi(url);
            var result = JsonConvert.DeserializeObject<Members>(response);
            return result;
        }

        private static string projectIssuesEPjson = @"/issues.json?project_id={0}&status_id=*";

        public static Issues GetProjectIssues(int projectId)
        {
            Debug.WriteLine("[Redmine]プロジェクト情報(" + projectId + ")を取得します。");
            string url = string.Format(baseUrl + projectIssuesEPjson + "&key=" + apiKey, projectId);
            var response = GetApi(url);
            var result = JsonConvert.DeserializeObject<Issues>(response);
            return result;
        }

        private static string issuesStatusEPjson = @"/issue_statuses.json";

        public static IssuesStatus GetIssueStatus()
        {
            Debug.WriteLine("[Redmine]ステータス情報を取得します。");
            string url = string.Format(baseUrl + issuesStatusEPjson + "?key=" + apiKey);
            var response = GetApi(url);
            var result = JsonConvert.DeserializeObject<IssuesStatus>(response);
            return result;
        }
    }
}
