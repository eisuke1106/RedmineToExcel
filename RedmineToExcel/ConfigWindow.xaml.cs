using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<int> limitsCollection = new ObservableCollection<int> { 50, 100, 300, 500, 1000 };

        public static void ShowWindow()
        {
            ConfigWindow configW = new ConfigWindow();
            configW.ShowDialog();
        }

        private ConfigWindow()
        {
            InitializeComponent();

            urlTextbox.Text = Settings.Instance.redmineUrl;
            apiTextbox.Text = Settings.Instance.redmineApiKey;

            apiLimitComboBox.ItemsSource = limitsCollection;
            int limitIndex = limitsCollection.IndexOf(Settings.Instance.redmineApiLimit);
            apiLimitComboBox.SelectedIndex = limitIndex;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.redmineApiKey = apiTextbox.Text;
            Settings.Instance.redmineUrl = urlTextbox.Text;
            var selectedItem = apiLimitComboBox.SelectedItem;
            if (selectedItem != null)
            {
                Settings.Instance.redmineApiLimit = (int)selectedItem;
            }
            Settings.Instance.Save();
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
