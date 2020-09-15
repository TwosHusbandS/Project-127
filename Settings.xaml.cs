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

namespace Project_127
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
		//       zCris backend

		//      private void ResetButton_Click(object sender, RoutedEventArgs e)
		//      {
		//          LauncherLogic.ResetSettings();
		//      }

		//      private void _500KCheckBox_Click(object sender, RoutedEventArgs e)
		//      {
		//          LauncherLogic.Settings.Set("500KBonus", _500KCheckBox.IsChecked.ToString());
		//      }

		//      private void InstallationPathButton_Click(object sender, RoutedEventArgs e)
		//      {
		//          FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		//          folderBrowserDialog.SelectedPath = LauncherLogic.Settings.Get("GTAVInstallationPath");
		//          if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		//          {
		//              LauncherLogic.Settings.Set("GTAVInstallationPath", Environment.ExpandEnvironmentVariables(folderBrowserDialog.SelectedPath));
		//          }
		//      }

		//      private void LauncherPathButton_Click(object sender, RoutedEventArgs e)
		//      {
		//          FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		//          folderBrowserDialog.SelectedPath = LauncherLogic.Settings.Get("FilesFolder");
		//          if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		//          {
		//              LauncherLogic.Settings.Set("FilesFolder", Environment.ExpandEnvironmentVariables(folderBrowserDialog.SelectedPath));
		//          }
		//      }




		// OWN STUFF

		/// <summary>
		/// Property we use to make rows visible and non visible depending on the checkbox above
		/// </summary>
		private static int InnerSettingsRowHeight = 40;


		/// <summary>
		/// Consturor of Settings Window 
		/// </summary>
		public Settings()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			LoadSettings();
		}


		/// <summary>
		/// Function with loads settings.
		/// </summary>
		private void LoadSettings()
		{
			// Load settings from regedit / memory.

			MakeRowVisibleFromCheckbox(cb_Set_FPSProgram);
			MakeRowVisibleFromCheckbox(cb_Set_Livesplit);
			MakeRowVisibleFromCheckbox(cb_Set_FPSProgram);
		}


		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }


		private void btn_Set_GTAVPath_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_Set_BackupPath_Click(object sender, RoutedEventArgs e)
		{

		}


		private void cb_Set_Preorder_CheckedChanged(object sender, RoutedEventArgs e)
		{

		}

		private void cb_Set_Logging_CheckedChanged(object sender, RoutedEventArgs e)
		{

		}

		private void cb_Set_Livesplit_CheckedChanged(object sender, RoutedEventArgs e)
		{
			MakeRowVisibleFromCheckbox(cb_Set_Livesplit);
		}

		private void btn_Set_Livesplit_Click(object sender, RoutedEventArgs e)
		{

		}


		private void cb_Set_StreamProgram_CheckedChanged(object sender, RoutedEventArgs e)
		{
			MakeRowVisibleFromCheckbox(cb_Set_StreamProgram);
		}

		private void btn_Set_StreamProgram_Click(object sender, RoutedEventArgs e)
		{

		}

		private void cb_Set_FPSProgram_CheckedChanged(object sender, RoutedEventArgs e)
		{
			MakeRowVisibleFromCheckbox(cb_Set_FPSProgram);
		}

		private void btn_Set_FPSProgram_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_Set_ThemePath_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MakeRowVisibleFromCheckbox(CheckBox pCB)
		{
			// Check if Checkbox is null
			if (pCB != null)
			{
				// Get Grid-Name from Name of Checkbox
				string MyGridName = "Grid" + pCB.Name.Substring(2);

				// Get Grid from Grid-Name
				Grid MyGrid = (Grid)this.FindName(MyGridName);

				// Check if We have at least 2 rows
				if (MyGrid != null && MyGrid.RowDefinitions.Count > 1)
				{
					// Check the status of the Checkbox and Set the Grid Rowheight accordingly
					if (pCB.IsChecked == true)
					{
						MyGrid.RowDefinitions[1].Height = new GridLength(Settings.InnerSettingsRowHeight);
					}
					else
					{
						MyGrid.RowDefinitions[1].Height = new GridLength(0);
					}
				}
			}
		}
	} // End of Class
} // End of Namespace
