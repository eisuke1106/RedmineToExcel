using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Redmine;
using Redmine.Models;
using System.Collections.ObjectModel;
using log4net;
using ClosedXML.Excel;
using Microsoft.Win32;

namespace RedmineToExcel
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Projects projectInfo;
        private Issues issueInfo;
        private IssuesStatus issueStatus;

        private ObservableCollection<ProjectData> displayProjectLists = new ObservableCollection<ProjectData>();
        private ObservableCollection<Issue> displayIssueList = new ObservableCollection<Issue>();

        public MainWindow()
        {
            InitializeComponent();

            this.listView.ItemsSource = displayProjectLists;
            this.issueListView.ItemsSource = displayIssueList;

            ContentRendered += (s, e) =>
            {
                if (Settings.Instance.IsValid())
                {
                    RedmineApi.apiKey = Settings.Instance.redmineApiKey;
                    RedmineApi.baseUrl = Settings.Instance.redmineUrl;

                    this.issueStatus = RedmineApi.GetIssueStatus();
                    this.projectInfo = RedmineApi.GetProjects();

                    bool isChecked = this.showClosedProjectCheckBox.IsChecked == null ? false : (bool)this.showClosedProjectCheckBox.IsChecked;
                    this.loadProjectInfo(isChecked);
                }
                else
                {
                    this.showSettingWindow();
                }
            };
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Instance.IsValid())
            {
                RedmineApi.apiKey = Settings.Instance.redmineApiKey;
                RedmineApi.baseUrl = Settings.Instance.redmineUrl;

                this.issueStatus = RedmineApi.GetIssueStatus();
                this.projectInfo = RedmineApi.GetProjects();

                bool isChecked = this.showClosedProjectCheckBox.IsChecked == null ? false : (bool)this.showClosedProjectCheckBox.IsChecked;
                this.loadProjectInfo(isChecked);
            }
            else
            {
                MessageBox.Show("設定を行って下さい。");
            }
        }


        private void loadProjectInfo(bool showClosed)
        {
            displayProjectLists.Clear();
            if (this.projectInfo == null)
            {
                return;
            }

            foreach (var project in this.projectInfo.projects)
            {
                var status = project.status;
                var issueStatus = this.issueStatus.issue_statuses.Where(data => data.id == status).FirstOrDefault();
                // 終了していないプロジェクトの追加
                if (issueStatus != null && issueStatus.is_closed == false)
                {
                    project.isClosed = false;
                }
                else
                {
                    project.isClosed = true;
                }

                if (!showClosed)
                {
                    if (!project.isClosed)
                    {
                        displayProjectLists.Add(project);
                    }
                }
                else
                {
                    displayProjectLists.Add(project);
                }
            }
        }

        private void getProjectIssuInfo(int projectId)
        {
            bool isChecked = this.showClosedIssueCheckBox.IsChecked == null ? false : (bool)this.showClosedIssueCheckBox.IsChecked;

            this.issueInfo = RedmineApi.GetProjectIssues(projectId);
            this.loadIssueInfo(isChecked);
        }

        private void loadIssueInfo(bool showClosed)
        {
            if (this.issueInfo == null){
                return;
            }

            projectTermLabel.Content = "( " + this.issueInfo.startDateString + " - " + this.issueInfo.endDateString + " )";

            displayIssueList.Clear();
            foreach (var issue in this.issueInfo.issues)
            {
                if (!showClosed)
                {
                    int status = issue.status.id;
                    var issueStatus = this.issueStatus.issue_statuses.Where(data => data.id == status).FirstOrDefault();
                    // 終了していないプロジェクトの追加
                    if (issueStatus != null && issueStatus.is_closed == false)
                    {
                        displayIssueList.Add(issue);
                    }
                }
                else
                {
                    displayIssueList.Add(issue);
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ProjectData item = listView.SelectedItem == null ? null : (ProjectData)listView.SelectedItem;
            if (item != null)
            {
                this.projectNameLabel.Content = item.name;
                this.getProjectIssuInfo(item.id);
            }
        }

        private void closedProjectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = this.showClosedProjectCheckBox.IsChecked == null ? false : (bool)this.showClosedProjectCheckBox.IsChecked;
            this.loadProjectInfo(isChecked);
        }

        private void closedIssueCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = this.showClosedIssueCheckBox.IsChecked == null ? false : (bool)this.showClosedIssueCheckBox.IsChecked;
            this.loadIssueInfo(isChecked);
        }

        private void ProjectOpenInBrowser_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex == -1) return;
            ProjectData item = listView.SelectedItem == null ? null : (ProjectData)listView.SelectedItem;
            if (item != null)
            {
                string url = Settings.Instance.redmineUrl + "/projects/" + item.id;
                this.openBrower(url);
            }
        }
        private void IssueOpenInBrowser_Click(object sender, RoutedEventArgs e)
        {
            if (issueListView.SelectedIndex == -1) return;
            Issue item = issueListView.SelectedItem == null ? null : (Issue)issueListView.SelectedItem;
            if (item != null)
            {
                string url = Settings.Instance.redmineUrl + "/issues/" + item.id;
                this.openBrower(url);
            }
        }

        

        private void openBrower(string url)
        {
            System.Diagnostics.Process.Start(url);
        } 


        private void DataCompletion_Click(object sender, RoutedEventArgs e)
        {
            List<Issue> output = new List<Issue>(issueInfo.issues);

            foreach (Issue issue in output)
            {
                if (issue.parent != null)
                {
                    var parentId = issue.parent.id;
                    var parentIssue = issueInfo.issues.Where(data => data.id == parentId).FirstOrDefault();
                    if (parentIssue != null)
                    {
                        parentIssue.children.Add(issue);
                    }
                }
            }

            List<Issue> filterdOutput = output.Where(data => data.parent == null).OrderBy(data => data.start_date).ToList();

            this.OutputToExcel(filterdOutput);
        }


        private void OutputToExcel(List<Issue> issues)
        {
            try
            {
                using (var wb = new XLWorkbook("./Files/base.xlsm"))
                {
                    using (var ws = wb.Worksheet("開発線表"))
                    {
                        int offset = 5;

                        List<Issue> newList = new List<Issue>();
                        this.WriteIssues(issues, newList);

                        // プロジェクト名
                        ws.Cell(2, 2).Value = this.projectNameLabel.Content;

                        // 行生成
                        ws.Row(offset).InsertRowsBelow(newList.Count - 1);
                        ws.Rows(offset, newList.Count + offset - 1).Height = 15;

                        for (int i = 0; i < newList.Count; i++)
                        {
                            Issue targetIssue = newList[i];
                            // id
                            ws.Cell(i + offset, 1).Value = i + 1;
                            // タイトル

                            // ws.Cell(i + offset, 2).Value = targetIssue.subject;
                            // ws.Cell(i + offset, 2).Style.Alignment.Indent = targetIssue.indent;

                            string space = string.Empty;
                            for (int indent = 1; indent < targetIssue.indent; indent++)
                            {
                                space += "　";
                            }
                            ws.Cell(i + offset, 2).Value = space + targetIssue.indentUnit + targetIssue.subject;

                            // 開始日
                            ws.Cell(i + offset, 3).Value = targetIssue.startDateString;
                            // 終了日
                            ws.Cell(i + offset, 4).Value = targetIssue.dueDateString;
                            // 終了日
                            ws.Cell(i + offset, 5).Value = targetIssue.done_ratio / 100;
                            // ステータス
                            ws.Cell(i + offset, 6).Value = targetIssue.status.name;
                            if (this.issueStatus.issue_statuses.Exists(data => data.id == targetIssue.status.id && data.is_closed))
                            {
                                ws.Cell(i + offset, 6).Style.Fill.BackgroundColor = XLColor.LightGray;
                            }

                            // 担当者
                            ws.Cell(i + offset, 7).Value = targetIssue.assigned_to == null ? "-" : targetIssue.assigned_to.name;
                            // チケット番号
                            ws.Cell(i + offset, 8).Value = "#" + targetIssue.id;
                            ws.Cell(i + offset, 8).Hyperlink = new XLHyperlink(Settings.Instance.redmineUrl + "/issues/" + targetIssue.id);
                            // 親チケット
                            ws.Cell(i + offset, 9).Value = targetIssue.parent == null ? "" : "#" + targetIssue.parent.id.ToString();
                            if (targetIssue.parent != null)
                            {
                                ws.Cell(i + offset, 9).Hyperlink = new XLHyperlink(Settings.Instance.redmineUrl + "/issues/" + targetIssue.parent.id);
                            }

                        }
                        
                        using (var ws2 = wb.Worksheet("設定シート"))
                        {
                            // 開始日
                            if (this.issueInfo.startDateString != "")
                            {
                                ws2.Cell(3, 3).Value = this.issueInfo.startDateString;
                                ws2.Cell(4, 3).Value = this.issueInfo.projectTerm;
                            }
                        }

                        string savePath = string.Empty;
                        string fileName = "[" + this.projectNameLabel.Content.ToString() + "]" + "開発線表_" + DateTime.Now.Date.ToString("yyyyMMdd");
                        if (openFileDialog(fileName, ref savePath))
                        {
                            wb.SaveAs(savePath);
                            var result = MessageBox.Show("Excel出力が完了しました。\nExcelファイルを開きますか？", "出力完了", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                System.Diagnostics.Process.Start(savePath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel出力に失敗しました。\nエラー：" + ex.ToString());
            }
        }

        private void WriteIssues( List<Issue> issues, List<Issue> newList)
        {
            foreach (Issue issue in issues)
            {
                newList.Add(issue);
                if (issue.children.Count > 0)
                {
                    List<Issue> childList = issue.children.OrderBy(data => data.start_date).ToList();

                    var lastChild = childList.Last();
                    foreach(Issue child in childList)
                    {
                        child.indent = issue.indent + 1;
                        if (child == lastChild)
                        {
                            child.indentUnit = "└";
                        }
                        else
                        {
                            child.indentUnit = "├";
                        }
                    }
                    WriteIssues(childList, newList);
                }
            }
        }

        private void settingButton_Click(object sender, RoutedEventArgs e)
        {
            this.showSettingWindow();
        }

        private void showSettingWindow()
        {
            ConfigWindow configW = new ConfigWindow();
            configW.ShowDialog();
        }

        private bool openFileDialog(string defaultName, ref string savePath)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = defaultName + ".xlsm";
            sfd.InitialDirectory = @"C:\";
            // sfd.Filter = "xlsmファイル(*.xlsm)";
            sfd.Title = "保存先のファイルを選択してください";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            if (sfd.ShowDialog() == true)
            {
                savePath = sfd.FileName;
                return true;
            }
            return false;
        }

    }
}
