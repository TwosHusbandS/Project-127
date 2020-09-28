using System;
using System.Collections.Generic;
using System.Diagnostics;
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

			// Needed for GUI Shit
			combox_Set_Retail.ItemsSource = Enum.GetValues(typeof(Retailers)).Cast<Retailers>();
			combox_Set_Retail.SelectedItem = Settings.Retailer;
			this.DataContext = this;
		}




		/// <summary>
		/// Method to set the GTA V Path manually
		/// </summary>
		public static void SetGTAVPathManually(bool CheckIfDefaultForCopyHardlinkNeedsChanging = true)
		{
			HelperClasses.Logger.Log("Asking User for GTA V Installation path");
			string GTAVInstallationPathUserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTAV.exe", @"C:\");
			HelperClasses.Logger.Log("Users picked path is: '" + GTAVInstallationPathUserChoice + "'");
			if (String.IsNullOrEmpty(GTAVInstallationPathUserChoice))
			{
				HelperClasses.Logger.Log("No Folder selected. Canceling User Action of Changing GTAV Installation Path");
				return;
			}
			while (!(LauncherLogic.IsGTAVInstallationPathCorrect(GTAVInstallationPathUserChoice, false)))
			{
				HelperClasses.Logger.Log("Users picked path detected to be faulty. Asking user to try again");
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "GTA V Path detected to be not correct. Are you sure?\nForce '" + GTAVInstallationPathUserChoice + "' as your GTAV Installation Location?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("Will force the Path that user picked even tho Algorithm think its faulty.");
					Settings.GTAVInstallationPath = GTAVInstallationPathUserChoice;
					break;
				}
				GTAVInstallationPathUserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTAV.exe", @"C:\");
				HelperClasses.Logger.Log("New Users picked path is: '" + GTAVInstallationPathUserChoice + "'");
			}
			HelperClasses.Logger.Log("Picked path '" + GTAVInstallationPathUserChoice + "'´is valid and will be set as Settings.GTAVInstallationPath.");
			Settings.GTAVInstallationPath = GTAVInstallationPathUserChoice;
			if (CheckIfDefaultForCopyHardlinkNeedsChanging)
			{
			SetDefaultEnableCopyingHardlinking();
			}
		}


		// There are only Button Clicks and GUI Functions here. Some of the Functionality is in SettingsPartial.cs



		/// <summary>
		/// Button Click to change the Path of GTA V Installation Path
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_GTAVInstallationPath_Click(object sender, RoutedEventArgs e)
		{
			SetGTAVPathManually();
			RefreshGUI();
		}


		/// <summary>
		/// RightClick on the GTA V Path Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_GTAVInstallationPath_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("explorer.exe", Settings.GTAVInstallationPath);
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
			HelperClasses.Logger.Log("Changing ZIPExtractionPath.");
			HelperClasses.Logger.Log("Old ZIPExtractionPath: '" + Settings.ZIPExtractionPath + "'");
			HelperClasses.Logger.Log("Potential New ZIPExtractionPath: '" + _ZIPExtractionPath + "'");

			// If its a valid Path (no "") and if its a new Path
			if(ChangeZIPExtractionPath(_ZIPExtractionPath))
			{
				HelperClasses.Logger.Log("Changing ZIP Path worked");
			}
			else
			{
				HelperClasses.Logger.Log("Changing ZIP Path did not work. Probably non existing Path or same Path as before");
				new Popup(Popup.PopupWindowTypes.PopupOk, "Changing ZIP Path did not work. Probably non existing Path or same Path as before");
			}

			RefreshGUI();
		}


		/// <summary>
		/// Method which gets called when changing the Path of the ZIP Extraction
		/// </summary>
		/// <param name="pNewZIPPath"></param>
		/// <returns></returns>
		public static bool ChangeZIPExtractionPath(string pNewZIPPath)
		{
			HelperClasses.Logger.Log("Called Method to Change ZIP File Path");
			if (HelperClasses.FileHandling.doesPathExist(pNewZIPPath) && pNewZIPPath.TrimEnd('\\') != Settings.ZIPExtractionPath.TrimEnd('\\'))
			{
				HelperClasses.Logger.Log("Potential New ZIPExtractionPath exists and is new, lets continue");

				// List of File Operations for the ZIP Move progress
				List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

				// List of FileNames
				string OldZipExtractionPath = Settings.ZIPExtractionPath;
				string[] FilesInOldZIPExtractionPath = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(Settings.ZIPExtractionPath);
				string[] FilesInNewZIPExtractionPath = new string[FilesInOldZIPExtractionPath.Length];

				// Loop through all Files there
				for (int i = 0; i <= FilesInOldZIPExtractionPath.Length - 1; i++)
				{
					// Only copy the folder in the zip, not all the other stuff in the same path where ZIP was extracted
					if (FilesInOldZIPExtractionPath[i].Contains(@"\Project_127_Files\"))
					{
						// Build new Path of each File
						FilesInNewZIPExtractionPath[i] = pNewZIPPath.TrimEnd('\\') + @"\" + FilesInOldZIPExtractionPath[i].Substring((Settings.ZIPExtractionPath.TrimEnd('\\') + @"\").Length);

						// Add File Operation for that new File
						MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, FilesInOldZIPExtractionPath[i], FilesInNewZIPExtractionPath[i], "Moving File '" + FilesInOldZIPExtractionPath[i] + "' to Location '" + FilesInNewZIPExtractionPath[i] + "' while moving ZIP Files", 0));
					}
				}

				// Execute all File Operations
				HelperClasses.Logger.Log("About to move all relevant Files (" + MyFileOperations.Count + ")");
				new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Moving ZIP File Location", MyFileOperations).ShowDialog();
				HelperClasses.Logger.Log("Done with moving all relevant Files");

				HelperClasses.Logger.Log("Creating new Folders if needed");
				HelperClasses.FileHandling.CreateAllZIPPaths(pNewZIPPath);
				HelperClasses.Logger.Log("Done creating new Folders we needed");

				// Grabbign the current Installation State
				LauncherLogic.InstallationStates myOldInstallationState = LauncherLogic.InstallationState;
				HelperClasses.Logger.Log("Old Installation State = '" + myOldInstallationState + "'");

				// Actually changing the Settings here
				Settings.ZIPExtractionPath = pNewZIPPath;

				// Repeating a Downgrade if it was downgraded before. We do not need an Upgrade if it was upgraded before.
				if (myOldInstallationState == LauncherLogic.InstallationStates.Downgraded)
				{
					HelperClasses.Logger.Log("Since Old Installation State was Downgraded, we will apply a Downgrade again");
					LauncherLogic.Downgrade();
				}
				else
				{
					HelperClasses.Logger.Log("Since Old Installation State was Upgraded, we do not need to apply another Upgrade for everything to work");
				}

				SetDefaultEnableCopyingHardlinking();

				HelperClasses.FileHandling.DeleteFolder(OldZipExtractionPath.TrimEnd('\\') + @"\Project_127_Files");

				return true;
			}
			else
			{
				HelperClasses.Logger.Log("Potential New ZIPExtractionPath does not exist or is the same as the old");
				return false;
			}
		}


		/// <summary>
		/// Method which checks what Setting it recommends (Hardlinking or Copying
		/// </summary>
		public static void SetDefaultEnableCopyingHardlinking()
		{
			bool currentSetting = Settings.EnableCopyFilesInsteadOfHardlinking;
			bool recommendSetting = !(Settings.ZIPExtractionPath[0] == Settings.GTAVInstallationPath[0]);

			HelperClasses.Logger.Log("Checking to see if Settings.EnableCopyFilesInsteadOfHardlinking is on recommended value");
			HelperClasses.Logger.Log("Settings.ZIPExtractionPath: '" + Settings.ZIPExtractionPath + "'");
			HelperClasses.Logger.Log("Settings.GTAVInstallationPath: '" + Settings.GTAVInstallationPath + "'");
			HelperClasses.Logger.Log("currentSettingsValue: '" + currentSetting + "'");
			HelperClasses.Logger.Log("recommendSettingsValue: '" + recommendSetting + "'");

			if (currentSetting == recommendSetting)
			{
				HelperClasses.Logger.Log("Recommend Settings Value is the Current Settings Value");
			}
			else
			{
				HelperClasses.Logger.Log("Recommend Settings Value is NOT the Current Settings Value. Asking User what he wants to do");
				Popup yesno;
				if (recommendSetting == true)
				{
					yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "It is recommended to use Copying instead of Hardlinking for File Operations.\nDo you want to do that?");
				}
				else
				{
					yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "It is recommended to use Hardlinking instead of Copying for File Operations.\nDo you want to do that?");
				}
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("User wants recommended Setting");
					Settings.EnableCopyFilesInsteadOfHardlinking = recommendSetting;
				}
				else
				{
					HelperClasses.Logger.Log("User does NOT want recommended Setting");
				}
			}
		}


		/// <summary>
		/// Event that gets triggered when the ComboBox of Retailers changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void combox_Set_Retail_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Retailer = (Retailers)System.Enum.Parse(typeof(Retailers), combox_Set_Retail.SelectedItem.ToString());
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

		}

		/// <summary>
		/// Button Click to change the Key2 of the JumpScript
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_JumpScriptKey2_Click(object sender, RoutedEventArgs e)
		{

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
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you want to reset settings?");
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				HelperClasses.Logger.Log("Resetting Settings STARTED, this will explain the following messages");
				Settings.ResetSettings();
				Settings.InitImportantSettings();
				RefreshGUI();
			}
		}


		private void btn_Set_ZIPExtractionPath_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("explorer.exe", Settings.ZIPExtractionPath);
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_RefreshGUI_Click(object sender, RoutedEventArgs e)
		{
			RefreshGUI();
		}


		/// <summary>
		/// Refresh GUI Method...
		/// </summary>
		private void RefreshGUI()
		{
			btn_Set_GTAVInstallationPath.Content = Settings.GTAVInstallationPath;
			btn_Set_ZIPExtractionPath.Content = Settings.ZIPExtractionPath;
			cb_Set_EnableLogging.IsChecked = Settings.EnableLogging;
			cb_Set_CopyFilesInsteadOfHardlinking.IsChecked = Settings.EnableCopyFilesInsteadOfHardlinking;
			cb_Set_EnablePreOrderBonus.IsChecked = Settings.EnablePreOrderBonus;
			combox_Set_Retail.SelectedItem = Settings.Retailer;
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
