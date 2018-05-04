using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedmineToExcel
{
    /// <summary>
    /// 設定情報統括クラス
    /// </summary>
    class Settings
    {
        private static Settings instance = new Settings();
        public string redmineUrl = null;
        public string redmineApiKey = null;
        public int redmineApiLimit = 100;

        /// <summary>
        /// 共有インスタンス
        /// </summary>
        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Settings()
        {
            this.redmineUrl = Properties.Settings.Default.Redmine_Url;
            this.redmineApiKey = Properties.Settings.Default.ApiKey;
            this.redmineApiLimit = Properties.Settings.Default.ApiLitmi;
        }

        /// <summary>
        /// 設定が有効かどうかを返します
        /// </summary>
        /// <returns>true：有効 false：無効</returns>
        public bool IsValid()
        {
            if (this.redmineApiKey == "" || this.redmineUrl == "")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 設定を保存します
        /// </summary>
        public void Save()
        {
            Properties.Settings.Default.Redmine_Url = this.redmineUrl;
            Properties.Settings.Default.ApiKey = this.redmineApiKey;
            Properties.Settings.Default.ApiLitmi = this.redmineApiLimit;
            Properties.Settings.Default.Save();
        }
    }
}
