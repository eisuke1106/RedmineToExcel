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
using RedmineToExcel.Dialogs;
using MaterialDesignThemes.Wpf;

namespace RedmineToExcel
{
    public class Person
    {
        public string Name { get; set; }
        public List<Person> Children { get; set; }
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Projects projectInfo;
        private ProjectData selectedProject;
        private Issues issueInfo;
        private IssuesStatus issueStatus;
        private Versions versions;

        private ObservableCollection<ProjectData> displayProjectLists { get; set; } = new ObservableCollection<ProjectData>();
        private ObservableCollection<Issue> displayIssueList = new ObservableCollection<Issue>();
        private ObservableCollection<VersionData> displayVersionList = new ObservableCollection<VersionData>();


        public MainWindow()
        {
            InitializeComponent();

            //this.listView.ItemsSource = displayProjectLists;

            //this.TreeView.ItemsSource = displayProjectLists;
            this.issueListView.ItemsSource = displayIssueList;
            this.versionComboBox.ItemsSource = displayVersionList;

            //this.treeView.ItemsSource = new List<ProjectData>
            //{
            //    new ProjectData
            //    {
            //        Name = "田中　太郎",
            //        Children = new List<ProjectData>
            //        {
            //            new ProjectData { Name = "田中　花子" },
            //            new ProjectData { Name = "田中　一郎" },
            //            new ProjectData
            //            {
            //                Name = "木村　貫太郎",
            //                Children = new List<ProjectData>
            //                {
            //                    new ProjectData { Name = "木村　はな" },
            //                    new ProjectData { Name = "木村　梅" },
            //                }
            //            }
            //        }
            //    },
            //    new ProjectData
            //    {
            //        Name = "田中　次郎",
            //        Children = new List<ProjectData>
            //        {
            //            new ProjectData { Name = "田中　三郎" }
            //        }
            //    }
            //};


            ContentRendered += (s, e) =>
            {
                this.getProjectInfo();
                this.treeView.ItemsSource = displayProjectLists;
            };
        }


        /// <summary>
        /// プロジェクト情報を取得します
        /// </summary>
        private void getProjectInfo()
        {
            try
            {
                if (Settings.Instance.IsValid())
                {
                    RedmineApi.apiKey = Settings.Instance.redmineApiKey;
                    RedmineApi.baseUrl = Settings.Instance.redmineUrl;
                    RedmineApi.limit = Settings.Instance.redmineApiLimit;

                    // URL,APIKey指定
                    this.issueStatus = RedmineApi.GetIssueStatus();
                    this.projectInfo = RedmineApi.GetProjects();

                    //bool isChecked = this.showClosedProjectCheckBox.IsChecked == null ? false : (bool)this.showClosedProjectCheckBox.IsChecked;
                    this.displayProjectInfo(true);
                }
                else
                {
                    MessageBox.Show("設定を行って下さい。");
                    ConfigWindow.ShowWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 画面表示用のプロジェクト情報に調整します
        /// </summary>
        /// <param name="showClosed">終了プロジェクトの表示・非表示</param>
        private void displayProjectInfo(bool showClosed)
        {
            if (this.projectInfo == null)
            {
                return;
            }

            displayProjectLists.Clear();

            // ツリービュー
            foreach (var project in this.projectInfo.projects)
            {
                if (project.parent != null)
                {
                    var parent = this.projectInfo.projects.Where(data => data.id == project.parent.id).FirstOrDefault();
                    parent.Children.Add(project);
                }
                else
                {
                    displayProjectLists.Add(project);
                }
            }

            //foreach (var project in this.projectInfo.projects)
            //{
            //    var status = project.status;
            //    var issueStatus = this.issueStatus.issue_statuses.Where(data => data.id == status).FirstOrDefault();
            //    // 終了していないプロジェクトの追加
            //    if (issueStatus != null && issueStatus.is_closed == false)
            //    {
            //        project.isClosed = false;
            //    }
            //    else
            //    {
            //        project.isClosed = true;
            //    }

            //    if (!showClosed)
            //    {
            //        if (!project.isClosed)
            //        {
            //            displayProjectLists.Add(project);
            //        }
            //    }
            //    else
            //    {
            //        displayProjectLists.Add(project);
            //    }
            //}
        }

        /// <summary>
        /// チケット情報を読み込みイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void issueLoadButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ProjectData item = listView.SelectedItem == null ? null : (ProjectData)listView.SelectedItem;
        //    if (item != null)
        //    {
        //        this.selectedProject = item;
        //        this.projectNameLabel.Content = item.name;
        //        this.getProjectIssuInfo(item.id);
        //    }
        //}




        /// <summary>
        /// 画面表示用のチケット情報に調整します
        /// </summary>
        /// <param name="showClosed">終了チケットの表示・非表示</param>
        private void displayIssueInfo(List<Issue> issues)
        {
            // bool showClosed = this.showClosedIssueCheckBox.IsChecked == null ? false : (bool)this.showClosedIssueCheckBox.IsChecked;
            // bool showSubProject = this.showSubProjectIssueCheckBox.IsChecked == null ? false : (bool)this.showSubProjectIssueCheckBox.IsChecked;

            if (issues == null){
                return;
            }

            if (this.selectedProject == null)
            {
                return;
            }
            
            displayIssueList.Clear();

            List<Issue> tempList = new List<Issue>();

            foreach( var issue in issues)
            {
                displayIssueList.Add(issue);
            }
            //foreach (var issue in issues)
            //{
            //    if (!showSubProject)
            //    {
            //        if (this.selectedProject.id == issue.project.id)
            //        {
            //            tempList.Add(issue);
            //        }
            //    }
            //    else
            //    {
            //        tempList.Add(issue);
            //    }
            //}

            //foreach (var issue in tempList)
            //{
            //    if (!showClosed)
            //    {
            //        int status = issue.status.id;
            //        var issueStatus = this.issueStatus.issue_statuses.Where(data => data.id == status && data.is_closed == false).FirstOrDefault();
            //        // 終了していないプロジェクトの追加
            //        if (issueStatus != null)
            //        {
            //            displayIssueList.Add(issue);
            //        }
            //    }
            //    else
            //    {
            //        displayIssueList.Add(issue);
            //    }
            //}
        }

        /// <summary>
        /// 終了プロジェクトを表示・非表示します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closedProjectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // bool isChecked = this.showClosedProjectCheckBox.IsChecked == null ? false : (bool)this.showClosedProjectCheckBox.IsChecked;
            this.displayProjectInfo(true);
        }

        /// <summary>
        /// 終了チケットを表示・非表示します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closedIssueCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.displayIssueInfo(this.issueInfo.issues);
        }

        /// <summary>
        /// 右クリックイベント
        /// プロジェクト情報をブラウザで開きます
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ProjectOpenInBrowser_Click(object sender, RoutedEventArgs e)
        //{
        //    if (listView.SelectedIndex == -1) return;
        //    ProjectData item = listView.SelectedItem == null ? null : (ProjectData)listView.SelectedItem;
        //    if (item != null)
        //    {
        //        Utility.OpenUrl(RedmineApi.GetProjectUrl(item.id));
        //    }
        //}

        /// <summary>
        /// 右クリックイベント
        /// チケット情報をブラウザで開きます
        /// </summary>
        /// <param name = "sender" ></ param >
        /// < param name="e"></param>
        private void IssueOpenInBrowser_Click(object sender, RoutedEventArgs e)
        {
            if (issueListView.SelectedIndex == -1) return;
            Issue item = issueListView.SelectedItem == null ? null : (Issue)issueListView.SelectedItem;
            if (item != null)
            {
                Utility.OpenUrl(RedmineApi.GetIssueUrl(item.id));
            }
        }

        /// <summary>
        /// Excelファイルに出力します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputExcel_Click(object sender, RoutedEventArgs e)
        {
            OutputExcel excel = new OutputExcel(this.selectedProject, this.issueInfo, this.issueStatus);
            excel.Output();
        }

        /// <summary>
        /// 設定画面を開きます
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow.ShowWindow();
        }


        private void showSubProjectIssueCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.displayIssueInfo(this.issueInfo.issues);
        }

   

        /// <summary>
        /// ツリー選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = this.treeView.SelectedItem as ProjectData;
            if (item != null)
            {
                this.selectedProject = item;
                this.Title.Text = item.Name;
                this.versions = RedmineApi.GetProjectVersions(item.id);
                this.displayVersionsInfo();
            }

            // menu close
            MenuToggleButton.IsChecked = false;
        
        }

        /// <summary>
        /// バージョン表示
        /// </summary>
        private void displayVersionsInfo()
        {
            displayVersionList.Clear();

            VersionData noVersion = new VersionData()
            {
                id = 0,
                name = "全体"
            };
            displayVersionList.Add(noVersion);

            foreach (var version in this.versions.versions)
            {
                displayVersionList.Add(version);
            }

            this.versionComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// バージョンコンボボックス変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void versionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.versionComboBox.SelectedItem;
            if (item != null)
            {
                VersionData version = item as VersionData;
                this.getProjectIssuInfo(this.selectedProject.id, version.id);
            }
        }


        /// <summary>
        /// 対象プロジェクトのチケット情報を読み込みます
        /// </summary>
        /// <param name="projectId"></param>
        private void getProjectIssuInfo(int projectId, int versionNo)
        {
            if (versionNo == 0)
            {
                this.issueInfo = RedmineApi.GetProjectIssues(projectId);
            }
            else
            {
                this.issueInfo = RedmineApi.GetProjectIssuesWithVersion(projectId, versionNo);
            }

            if (issueInfo.existMore)
            {
                MessageBox.Show("表示しきれていないチケットがあります。取得件数上限（Redmine Api Limit）を増やして再度実行して下さい。");
            }

            TermLabel.Text = "( " + this.issueInfo.startDateString + " ~ " + this.issueInfo.endDateString + " )";
            this.displayIssueInfo(this.issueInfo.issues);
        }



        /// <summary>
        /// 再読込ボタンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            var item = this.versionComboBox.SelectedItem;
            if (item != null)
            {
                VersionData version = item as VersionData;
                this.getProjectIssuInfo(this.selectedProject.id, version.id);
            }
        }

    }
}
