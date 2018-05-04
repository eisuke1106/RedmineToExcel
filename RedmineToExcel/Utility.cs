using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedmineToExcel
{
    static class Utility
    {
        static public void OpenUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

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
