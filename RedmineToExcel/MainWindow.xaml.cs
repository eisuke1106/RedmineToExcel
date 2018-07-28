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
            this.issueListView.ItemsSource = displayIssueList;
            this.versionComboBox.ItemsSource = displayVersionList;

            ContentRendered += (s, e) =>
            {
                this.getProjectInfo();
                this.treeView.ItemsSource = displayProjectLists;
                this.MenuToggleButton.IsChecked = true;
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

                    this.displayProjectInfo(this.projectInfo.projects);
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
        private void displayProjectInfo(List<ProjectData> projects)
        {
            if (projects == null)
            {
                return;
            }

            this.treeView.ItemsSource = null;

            foreach (var project in projects)
            {
                project.Children.Clear();
            }

            displayProjectLists.Clear();

            // ツリービュー
            foreach (var project in projects)
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

            this.treeView.ItemsSource = displayProjectLists;
        }

        private void hideClosedProjectButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.projectInfo.projects == null)
            {
                return;
            }

            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked == true)
            {
                this.displayProjectInfo(this.projectInfo.excludeEndProjects);
            }
            else
            {
                this.displayProjectInfo(this.projectInfo.projects);
            }
        }


        /// <summary>
        /// 画面表示用のチケット情報に調整します
        /// </summary>
        /// <param name="showClosed">終了チケットの表示・非表示</param>
        private void displayIssueInfo(List<Issue> issues)
        {
            if (issues == null) {
                return;
            }

            if (this.selectedProject == null)
            {
                return;
            }

            displayIssueList.Clear();

            List<Issue> tempList = new List<Issue>();

            foreach (var issue in issues)
            {
                displayIssueList.Add(issue);
            }
        }

        /// <summary>
        /// 終了プロジェクトを表示・非表示します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void closedProjectCheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    // bool isChecked = this.showClosedProjectCheckBox.IsChecked == null ? false : (bool)this.showClosedProjectCheckBox.IsChecked;
        //    this.displayProjectInfo();
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

            TermLabel.Text = "( " + this.issueInfo.startDateString + " ~ " + this.issueInfo.endDateString + " )";
            this.displayIssueInfo(this.issueInfo.issues);
        }

        
        /// <summary>
        /// 終了チケットを表示・非表示します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closedIssueCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.issueInfo == null)
            {
                return;
            }

            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked == true)
            {
                this.displayIssueInfo(GetClosedIssueLists(this.issueInfo.issues));
            }
            else
            {
                this.displayIssueInfo(this.issueInfo.issues);
            }
        }

        private List<Issue> GetClosedIssueLists(List<Issue> issues)
        {
            return this.issueInfo.issues.Where(data => data.isClosed == false).ToList();
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
                if(this.showClosedIssueCheckBox.IsChecked == true)
                {
                    this.displayIssueInfo(GetClosedIssueLists(this.issueInfo.issues));
                }
            }
        }

    }
}
