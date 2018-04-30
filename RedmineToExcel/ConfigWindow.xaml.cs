using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RedmineToExcel
{
    /// <summary>
    /// Config.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();

            urlTextbox.Text = Settings.Instance.redmineUrl;
            apiTextbox.Text = Settings.Instance.redmineApiKey;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.redmineApiKey = apiTextbox.Text;
            Settings.Instance.redmineUrl = urlTextbox.Text;
            Settings.Instance.Save();
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
