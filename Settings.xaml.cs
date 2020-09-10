using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_127
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        /// <summary>
        /// Consturor of Settings Window
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            //_500KCheckBox.IsChecked = bool.Parse(LauncherLogic.Settings.Get("500KBonus"));
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            LauncherLogic.ResetSettings();
        }

        private void _500KCheckBox_Click(object sender, RoutedEventArgs e)
        {
            //LauncherLogic.Settings.Set("500KBonus", _500KCheckBox.IsChecked.ToString());
        }

        private void InstallationPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = LauncherLogic.Settings.Get("GTAVInstallationPath");
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LauncherLogic.Settings.Set("GTAVInstallationPath", Environment.ExpandEnvironmentVariables(folderBrowserDialog.SelectedPath));
            }
        }

        private void LauncherPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = LauncherLogic.Settings.Get("FilesFolder");
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LauncherLogic.Settings.Set("FilesFolder", Environment.ExpandEnvironmentVariables(folderBrowserDialog.SelectedPath));
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
