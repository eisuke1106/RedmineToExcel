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

        public static int limit = 100;

        public static string GetApi(string url)
        {
            try
            {
                Debug.WriteLine("[Redmine]" + url);
                var Req = HttpWebRequest.Create(url + "&limit=" + limit);
                HttpWebResponse Res = (HttpWebResponse)Req.GetResponse();
                StreamReader st_Reader = new StreamReader(Res.GetResponseStream(), new UTF8Encoding(false));
                string response = st_Reader.ReadToEnd();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string projectListEPjson = "/projects.json";

        public static Projects GetProjects()
        {
            try
            {
                Debug.WriteLine("[Redmine]プロジェクト情報を取得します。");
                string url = string.Format(baseUrl + projectListEPjson + "?key=" + apiKey);
                var response = GetApi(url);
                var projects = JsonConvert.DeserializeObject<Projects>(response);
                return projects;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string projectEPjson = @"/projects/{0}.json";

        public static Project GetProject(int projectId)
        {
            try
            {
                Debug.WriteLine("[Redmine]プロジェクト情報(" + projectId + ")を取得します。");
                string url = string.Format(baseUrl + projectEPjson + "?key=" + apiKey, projectId);
                var response = GetApi(url);
                var project = JsonConvert.DeserializeObject<Project>(response);
                return project;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string projectMemberEPjson = @"/projects/{0}/memberships.json";

        public static Members GetProjectMembers(int projectId)
        {
            try
            {
                Debug.WriteLine("[Redmine]プロジェクトMember情報(" + projectId + ")を取得します。");
                string url = string.Format(baseUrl + projectMemberEPjson + "?key=" + apiKey, projectId);
                var response = GetApi(url);
                var result = JsonConvert.DeserializeObject<Members>(response);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string projectVersionEPjson = @"/projects/{0}/versions.json";

        public static Versions GetProjectVersions(int projectId)
        {
            try
            {
                Debug.WriteLine("[Redmine]プロジェクトVersion情報(" + projectId + ")を取得します。");
                string url = string.Format(baseUrl + projectVersionEPjson + "?key=" + apiKey, projectId);
                var response = GetApi(url);
                var result = JsonConvert.DeserializeObject<Versions>(response);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string projectIssuesEPjson = @"";

        public static Issues GetProjectIssues(int projectId, bool includeSubProject = false)
        {
            try
            {
                List<Issues> issues = new List<Issues>();
                int offset = 0;

                while (true)
                {
                    Debug.WriteLine("[Redmine]プロジェクト情報(" + projectId + ")を取得します。");
                    string url = string.Format($"{baseUrl}/issues.json?limit=100&offset={offset}&project_id={projectId}&status_id=*&sort=category:desc&key={apiKey}");
                    var response = GetApi(url);
                    var result = JsonConvert.DeserializeObject<Issues>(response);
                    if (includeSubProject == false)
                    {
                        result.issues = result.issues.Where(data => data.project.id == projectId).ToList();
                    }
                    issues.Add(result);

                    if (result.total_count > result.limit + result.offset)
                    {
                        offset += 100;
                    }
                    else
                    {
                        break;
                    }
                }

                if (issues.Count == 1)
                {
                    return issues[0];
                }
                else
                {
                    var baseIssues = issues[0];
                    foreach ( var issue in issues)
                    {
                        baseIssues.issues.AddRange(issue.issues);
                    }
                    return baseIssues;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Issues GetProjectIssuesWithVersion(int projectId, int version)
        {
            try
            {
                List<Issues> issues = new List<Issues>();
                int offset = 0;

                while (true)
                {
                    Debug.WriteLine($"[Redmine]プロジェクト情報[ID:{projectId}][Ver:{version}]を取得します。");
                    string url = string.Format($"{baseUrl}/issues.json?limit=100&offset={offset}&project_id={projectId}&status_id=*&fixed_version_id={version}&sort=category:desc&key={apiKey}");
                    var response = GetApi(url);
                    var result = JsonConvert.DeserializeObject<Issues>(response);
                    issues.Add(result);

                    if (result.total_count > result.limit + result.offset)
                    {
                        offset += 100;
                    }
                    else
                    {
                        break;
                    }
                }

                if (issues.Count == 1)
                {
                    return issues[0];
                }
                else
                {
                    var baseIssues = issues[0];
                    foreach (var issue in issues)
                    {
                        baseIssues.issues.AddRange(issue.issues);
                    }
                    return baseIssues;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string issuesStatusEPjson = @"";

        public static IssuesStatus GetIssueStatus()
        {
            try
            {
                Debug.WriteLine("[Redmine]ステータス情報を取得します。");
                string url = string.Format($"{baseUrl}/issue_statuses.json?key={apiKey}");
                var response = GetApi(url);
                var result = JsonConvert.DeserializeObject<IssuesStatus>(response);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static string GetIssueUrl(int issueId)
        {
            return baseUrl + "/issues/" + issueId;
        }

        public static string GetProjectUrl(int projectId)
        {
            return baseUrl + "/projects/" + projectId;
        }
    }
}
