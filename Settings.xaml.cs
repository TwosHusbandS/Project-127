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
	/// Class Settings.xaml (Partical class is in SettingsPartial.cs)
	/// </summary>
	public partial class Settings : Window
	{
		/// <summary>
		/// Constructor of Settings Window 
		/// </summary>
		public Settings()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			// Sets this for DataBinding
			this.DataContext = this;
		}


		// There are only Button Clicks and GUI Functions here. Some of the Functionality is in SettingsPartial.cs


		/// <summary>
		/// Button Click to change the Path of GTA V Installation Path
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_GTAVInstallationPath_Click(object sender, RoutedEventArgs e)
		{
			string MyGTAVInstallationPath = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTAV.exe", @"C:\");
			if (LauncherLogic.IsGTAVInstallationPathCorrect(GTAVInstallationPath))
			{
				HelperClasses.Logger.Log("Choosen Path of Set_GTAVInstallationPath Button is theoretical valid. Changing Settings");
				Settings.GTAVInstallationPath = MyGTAVInstallationPath;
			}
			else
			{
				HelperClasses.Logger.Log("Choosen Path of Set_GTAVInstallationPath Button is theoretical INVALID. NOT Changing Settings");
				new Popup(Popup.PopupWindowTypes.PopupOk, "GTA V Path detected to be wrong. Settings will not change").ShowDialog();
			}
			btn_Set_GTAVInstallationPath.Content = Settings.GTAVInstallationPath;
			// This just updates the button content
		}

		/// <summary>
		/// Button Click to change the Path of ZIPExtractionPath which we use to use for all Contents of ZIP File etc.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_ZIPExtractionPath_Click(object sender, RoutedEventArgs e)
		{
			// Grabbing the new Path from FolderDialogThingy
			string _ZIPExtractionPath = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder where the ZIP Files (Upgrade / Downgrade / Savefiles will be extracted. ", @"C:\");
			
			// If its a valid Path (no "") and if its a new Path
			if (HelperClasses.FileHandling.doesPathExist(_ZIPExtractionPath) && _ZIPExtractionPath.TrimEnd('\\') != Settings.ZIPExtractionPath.TrimEnd('\\'))
			{
				// List of File Operations for the ZIP Move progress
				List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

				// List of FileNames
				string[] FilesInOldZIPExtractionPath = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(Settings.ZIPExtractionPath);
				string[] FilesInNewZIPExtractionPath = new string[FilesInOldZIPExtractionPath.Length];

				// Loop through all Files there
				for (int i = 0; i <= FilesInOldZIPExtractionPath.Length - 1; i++)
				{
					// Build new Path of each File
					FilesInNewZIPExtractionPath[i] = _ZIPExtractionPath.TrimEnd('\\') + @"\" + FilesInOldZIPExtractionPath[i].Substring((Settings.ZIPExtractionPath.TrimEnd('\\') + @"\").Length);

					// Add File Operation for that new File
					MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, FilesInOldZIPExtractionPath[i], FilesInNewZIPExtractionPath[i], "Moving File '" + FilesInOldZIPExtractionPath[i] + "' to Location '" + FilesInNewZIPExtractionPath[i] + "' while moving ZIP Files",0));
				}

				// Execute all File Operations
				new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Moving ZIP File Location" , MyFileOperations).ShowDialog();

				// Grabbign the current Installation State
				LauncherLogic.InstallationStates myOldInstallationState = LauncherLogic.InstallationState;

				// Actually changing the Settings here
				Settings.ZIPExtractionPath = _ZIPExtractionPath;

				// Repeating a Downgrade or Upgrade so we are back to original State and Hardlinks are now properly set again
				if (myOldInstallationState == LauncherLogic.InstallationStates.Upgraded)
				{
					LauncherLogic.Upgrade();
				}
				else
				{
					LauncherLogic.Downgrade();
				}

				// Now Updating Button
				btn_Set_ZIPExtractionPath.Content = ZIPExtractionPath;
			}
		}

		/// <summary>
		/// Button Click to change the Path of LiveSplit Executable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_PathLiveSplit_Click(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// Button Click to change the Path of StreamProgram Executable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_PathStreamProgram_Click(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// Button Click to change the Path of FPS Limiter Executable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_PathFPSLimiter_Click(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// Button Click to change the Key1 of the JumpScript
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_JumpScriptKey1_Click(object sender, RoutedEventArgs e)
		{
			// Needs to be implemented I guess
			(new Popup(Popup.PopupWindowTypes.PopupOk, "This does nothing so far")).ShowDialog();
		}

		/// <summary>
		/// Button Click to change the Key2 of the JumpScript
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_JumpScriptKey2_Click(object sender, RoutedEventArgs e)
		{
			// Needs to be implemented I guess
			(new Popup(Popup.PopupWindowTypes.PopupOk, "This does nothing so far")).ShowDialog();
		}

		/// <summary>
		/// Button Click to change the Path of Nohboard executable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_PathNohboard_Click(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// Button Click to select the Folder for the Theme
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_Theme_Click(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// Button Click on Close.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Close_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Button Click on Reset.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Reset_Click(object sender, RoutedEventArgs e)
		{
			// TODO CTRLF FIX HAVING TO RELOAD WINDOW
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you want to reset settings?");
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				HelperClasses.Logger.Log("Resetting Settings STARTED, this will explain the following messages");
				Settings.ResetSettings();
				RefreshGUI();
				LauncherLogic.GTAVPathGuessingGame();
				btn_Set_GTAVInstallationPath.Content = Settings.GTAVInstallationPath;
				HelperClasses.Logger.Log("Resetting Settings DONE");
			}
		}

		/// <summary>
		/// Refresh GUI Method...
		/// </summary>
		private void RefreshGUI()
		{
			btn_Set_GTAVInstallationPath.Content = Settings.GTAVInstallationPath;
			btn_Set_ZIPExtractionPath.Content = Settings.ZIPExtractionPath;
			cb_Set_EnableLogging.IsChecked = Settings.EnableLogging;
			//cb_Set_TempFixSteamLaunch.IsChecked = Settings.EnableTempFixSteamLaunch;
			//cb_Set_EnablePreOrderBonus.IsChecked = Settings.EnablePreOrderBonus;
			//cb_Set_AutoSetHighPriority.IsChecked = Settings.EnableAutoSetHighPriority;
			//btn_Set_PathFPSLimiter.Content = Settings.PathFPSLimiter;
			//btn_Set_PathLiveSplit.Content = Settings.PathLiveSplit;
			//btn_Set_PathStreamProgram.Content = Settings.PathStreamProgram;
			//btn_Set_PathNohboard.Content = Settings.PathNohboard;
			//btn_Set_JumpScriptKey1.Content = Settings.JumpScriptKey1;
			//btn_Set_JumpScriptKey2.Content = Settings.JumpScriptKey2;
			//btn_Set_Theme.Content = Settings.Theme;
			//cb_Set_EnableAutoStartFPSLimiter.IsChecked = Settings.EnableAutoStartFPSLimiter;
			//cb_Set_EnableAutoStartJumpScript.IsChecked = Settings.EnableAutoStartJumpScript;
			//cb_Set_EnableAutoStartLiveSplit.IsChecked = Settings.EnableAutoStartLiveSplit;
			//cb_Set_EnableAutoStartNohboard.IsChecked = Settings.EnableAutoStartNohboard;
			//cb_Set_EnableAutoStartStreamProgram.IsChecked = Settings.EnableAutoStartStreamProgram;
			//cb_Set_EnableNohboardBurhac.IsChecked = Settings.EnableNohboardBurhac;
		}

		// Below are Methods we need to make the behaviour of this nice.

		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}


	} // End of Class
} // End of Namespace
