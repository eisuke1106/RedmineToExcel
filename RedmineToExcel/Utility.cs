using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedmineToExcel
{
    /// <summary>
    /// ユーティリティクラス
    /// </summary>
    static class Utility
    {
        /// <summary>
        /// システムで開きます。
        /// httpの場合はブラウザを開く。
        /// ファイルの場合は規定のプログラムで開きます。
        /// </summary>
        /// <param name="url"></param>
        static public void OpenUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// 保存用ファイルダイアログを開きます。
        /// </summary>
        /// <param name="defaultName">規定名</param>
        /// <param name="savePath">保存先パスを格納して返す</param>
        /// <returns>true：成功 false：失敗（キャンセル）</returns>
        static public bool OpenFileDialog(string defaultName, ref string savePath)
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
