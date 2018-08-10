using ClosedXML.Excel;
using Redmine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedmineToExcel
{
    class OutputExcel
    {
        private Issues issueInfo;
        private IssuesStatus issueStatus;
        private ProjectData projectData;
        private List<Issue> issueList;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        OutputExcel()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="projectData">対象ProjectData</param>
        /// <param name="issueInfo">対象Issue</param>
        /// <param name="issueStatus">対象IssueStatus</param>
        public OutputExcel(ProjectData projectData, Issues issueInfo, IssuesStatus issueStatus)
        {
            this.projectData = projectData;
            this.issueInfo = issueInfo;
            this.issueStatus = issueStatus;
        }

        /// <summary>
        /// ファイル出力処理開始
        /// </summary>
        public void Output()
        {
            // 子階層リセット処理（一時対策）
            foreach (Issue issue in issueInfo.issues)
            {
                issue.children = new List<Issue>();
            }

            List<Issue> output = new List<Issue>(issueInfo.issues);

            // 親子階層構造構築
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

            // 階層構造構築後のデータをリスト化
            List<Issue> newIssueList = new List<Issue>();
            this.CreateNewIssueList(filterdOutput, newIssueList);

            // Excelに出力
            this.OutputToExcel(newIssueList);
        }

        /// <summary>
        /// 階層構造を再度リスト化
        /// </summary>
        /// <param name="issues"></param>
        /// <param name="newList"></param>
        private void CreateNewIssueList(List<Issue> issues, List<Issue> newList)
        {
            foreach (Issue issue in issues)
            {
                newList.Add(issue);
                if (issue.children.Count > 0)
                {
                    List<Issue> childList = issue.children.OrderBy(data => data.start_date).ToList();
                    var lastChild = childList.Last();
                    foreach (Issue child in childList)
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
                    CreateNewIssueList(childList, newList);
                }
            }
        }

        /// <summary>
        /// Excelファイルに出力
        /// </summary>
        /// <param name="issues"></param>
        private void OutputToExcel(List<Issue> issues)
        {
            try
            {
                using (var wb = new XLWorkbook("./Files/base.xlsm"))
                {
                    using (var ws = wb.Worksheet("開発線表"))
                    {
                        int offset = 5;

                        // プロジェクト名
                        ws.Cell(2, 2).Value = this.projectData.Name;

                        // 行生成
                        ws.Row(offset).InsertRowsBelow(issues.Count - 1);
                        ws.Rows(offset, issues.Count + offset - 1).Height = 15;

                        for (int i = 0; i < issues.Count; i++)
                        {
                            Issue targetIssue = issues[i];
                            // id
                            ws.Cell(i + offset, 1).Value = i + 1;
                            // タイトル

                            ws.Cell(i + offset, 2).Value = targetIssue.subject;
                            ws.Cell(i + offset, 2).Style.Alignment.Indent = targetIssue.indent;

                            // * Tree表記 *
                            //string space = string.Empty;
                            //for (int indent = 1; indent < targetIssue.indent; indent++)
                            //{
                            //    space += "　";
                            //}
                            //ws.Cell(i + offset, 2).Value = space + targetIssue.indentUnit + targetIssue.subject;
                            // 

                            // 開始日
                            ws.Cell(i + offset, 3).Value = targetIssue.startDateString;
                            // 終了日
                            ws.Cell(i + offset, 4).Value = targetIssue.dueDateString;
                            // 終了日
                            ws.Cell(i + offset, 5).Value = targetIssue.done_ratio / 100.0;
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
                            ws.Cell(i + offset, 10).Value = targetIssue.tracker.name;

                            ws.Cell(i + offset, 11).Value = targetIssue.isClosed == true ? 1 : 0;
                            ws.Column(11).Hide();

                            // 子チケットがあるかどうか
                            ws.Cell(i + offset, 12).Value = targetIssue.children.Count() != 0 ? 1 : 0;
                            ws.Column(12).Hide();

                        }
                        ws.Cell(issues.Count + offset + 1, 1).Value = "■";

                        using (var ws2 = wb.Worksheet("設定シート"))
                        {
                            // 開始日・期間設定
                            if (this.issueInfo.startDateString != "")
                            {
                                ws2.Cell(3, 3).Value = this.issueInfo.startDateString;
                                ws2.Cell(4, 3).Value = this.issueInfo.endDateString;
                            }
                        }

                        using (var ws3 = wb.Worksheet("隠しシート"))
                        {
                            // 表示件数
                            ws3.Cell(2, 2).Value = issues.Count;
                        }

                        // ファイル出力
                        string savePath = string.Empty;
                        string fileName = "[" + this.projectData.Name + "]" + "開発線表_" + DateTime.Now.Date.ToString("yyyyMMdd");
                        if (Utility.OpenFileDialog(fileName, ref savePath))
                        {
                            wb.SaveAs(savePath);
                            var result = MessageBox.Show("Excel出力が完了しました。\nExcelファイルを開きますか？", "出力完了", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                Utility.OpenUrl(savePath);
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
    }
}
