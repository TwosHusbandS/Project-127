using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		/*
		Main GUI Stuff for Settings. Most Logic should be in SettingsPartial.cs
		*/


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

			combox_Set_LanguageSelected.ItemsSource = Enum.GetValues(typeof(Languages)).Cast<Languages>();
			combox_Set_LanguageSelected.SelectedItem = Settings.LanguageSelected;

			tb_Set_InGameName.Text = Settings.InGameName;

			this.DataContext = this;
		}



		/// <summary>
		/// Initiating GTAV InstallationPath, ZIP Extraction Path and enabling Copy over Hardlinking
		/// </summary>
		public static void InitImportantSettings()
		{
			// Cleaning the GTAV Installation Path since we are guessing (and manually getting it)
			Settings.GTAVInstallationPath = "";

			HelperClasses.Logger.Log("InitImportantSettings when Settings Reset or FirstLaunch or Paths wrong on Launch");
			HelperClasses.Logger.Log("Playing the GTAV Guessing Game");

			// Used to not display a popup at the end if it was set during guessing
			bool ChangedRetailerAlready = false;

			// Adding all Guesses
			List<string> GTAVPathGuesses = new List<string>();
			GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicSteam());
			GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicRockstar());
			GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicEpic());
			GTAVPathGuesses.Add(Globals.ProjectInstallationPath.TrimEnd('\\').Substring(0, Globals.ProjectInstallationPath.LastIndexOf('\\')));
			GTAVPathGuesses.Add(Globals.ProjectInstallationPath.TrimEnd('\\'));
			GTAVPathGuesses.Add(Settings.ZIPExtractionPath.TrimEnd('\\').Substring(0, Globals.ProjectInstallationPath.LastIndexOf('\\')));
			GTAVPathGuesses.Add(Settings.ZIPExtractionPath);


			// Loop for the Guesses
			for (int i = 0; i <= GTAVPathGuesses.Count - 1; i++)
			{
				if (!String.IsNullOrWhiteSpace(Settings.GTAVInstallationPath))
				{
					HelperClasses.Logger.Log("GTAV Guess Number " + i + 1 + "is: '" + GTAVPathGuesses[i] + "'");
					if (LauncherLogic.IsGTAVInstallationPathCorrect(GTAVPathGuesses[i], false))
					{
						HelperClasses.Logger.Log("GTAV Guess Number " + i + 1 + "is theoretically valid. Asking user if he wants it");
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Is: '" + GTAVPathGuesses[i] + "' your GTA V Installation Path?");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
						{
							Settings.GTAVInstallationPath = GTAVPathGuesses[i];
							HelperClasses.Logger.Log("GTAV Guess Number " + i + 1 + " was picked by User");

							// Guessing the Retail Version
							string myRetailGuess = "";
							switch (i)
							{
								case 0:
									myRetailGuess = "Steam";
									break;
								case 1:
									myRetailGuess = "Rockstar";
									break;
								case 2:
									myRetailGuess = "Epic";
									break;
								default:
									break;
							}
							// If a Guess was made
							if (!string.IsNullOrWhiteSpace(myRetailGuess))
							{
								HelperClasses.Logger.Log("Our Retail Guess is: '" + myRetailGuess + "'");
								// Ask user if he wants it
								Popup RetailerGuessPopup;
								RetailerGuessPopup = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Retailer: '" + myRetailGuess + "' detected. Is that correct?");
								RetailerGuessPopup.ShowDialog();
								if (RetailerGuessPopup.DialogResult == true)
								{
									// User does want new Retailer
									HelperClasses.Logger.Log("User wants our Retail Guess");
									Settings.Retailer = (Retailers)System.Enum.Parse(typeof(Retailers), myRetailGuess);
									ChangedRetailerAlready = true;
								}
								else
								{
									HelperClasses.Logger.Log("User does not want our Retail Guess");
									// User does not want retail Guess
								}
							}
						}
						else
						{
							HelperClasses.Logger.Log("GTAV Guess Number " + i + 1 + " was NOT picked by User, moving on");
						}
					}
					else
					{
						HelperClasses.Logger.Log("GTAV Guess Number " + i + 1 + "is theoretically invalid, moving on");
					}
				}
			}

			// If Setting is STILL not correct
			// Needs to be while since you can exit out of SetGTAVPathManually
			while (!(LauncherLogic.IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath, false)))
			{
				// Log
				HelperClasses.Logger.Log("After " + GTAVPathGuesses.Count + " guesses we still dont have the correct GTAVInstallationPath. User has to do it manually now. Fucking casual");
				HelperClasses.Logger.Log("If you see this more than once, user exited out of the SetGTAVPathManually()");

				// Ask User for Path
				SetGTAVPathManually(false);
			}


			// Setting ZIP Extract Path now
			HelperClasses.Logger.Log("GTA V Path set, now onto the ZIP Folder");
			Popup yesnoconfirm = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Project 1.27 needs a Folder where it extracts the Content of the ZIP File to store all Sorts of Files and Data\nIt is recommend to do this on the same Drive / Partition as your GTAV Installation Path\nBest Case (and default Location) is your GTAV Path.\nDo you want to use your GTAV Path?");
			yesnoconfirm.ShowDialog();

			// If User wants default Settings
			if (yesnoconfirm.DialogResult == true)
			{
				HelperClasses.Logger.Log("User wants default ZIP Folder. Setting it to GTAV InstallationPath and calling the default CopyHardlink Method");

				if (!ChangeZIPExtractionPath(Settings.GTAVInstallationPath))
				{
					HelperClasses.Logger.Log("Changing ZIP Path did not work. Probably non existing Path or same Path as before (from Settings.InitImportantSettings())");
					new Popup(Popup.PopupWindowTypes.PopupOk, "Changing ZIP Path did not work. Probably non existing Path or same Path as before\nIf you read this message to anyone, tell them youre in Settings.InitImportantSettings()");
				}
			}
			// If User does not
			else
			{
				HelperClasses.Logger.Log("User does not want the default ZIP Folder");
				new Popup(Popup.PopupWindowTypes.PopupOk, "Okay, please pick a Folder where this Program will store most of its Files\n(For Upgrading / Downgrading / SaveFileHandling)").ShowDialog();
				string newZIPFileLocation = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder where this Program will store its Data.", Settings.ZIPExtractionPath);
				HelperClasses.Logger.Log("User provided own Location: ('" + newZIPFileLocation + "'). Calling the Method to move zip File");
				if (!ChangeZIPExtractionPath(newZIPFileLocation))
				{
					HelperClasses.Logger.Log("Changing ZIP Path did not work. Probably non existing Path or same Path as before (from Settings.InitImportantSettings())");
					new Popup(Popup.PopupWindowTypes.PopupOk, "Changing ZIP Path did not work. Probably non existing Path or same Path as before\nIf you read this message to anyone, tell them youre in Settings.InitImportantSettings()");
				}
			}

			// Making sure the CopyInsteadOfHardlink is correct
			Settings.SetDefaultEnableCopyingHardlinking();

			if (!ChangedRetailerAlready)
			{
				HelperClasses.Logger.Log("Have not changed Retailer already, will throw Popup");
				Popup myPopupRetailer = new Popup(Popup.PopupWindowTypes.PopupOkComboBox, "Retailer of your GTA V Installation?", pEnum: Retailer);
				myPopupRetailer.ShowDialog();
				if (myPopupRetailer.DialogResult == true)
				{
					HelperClasses.Logger.Log("User picked '" + myPopupRetailer.MyReturnString + "' as their Retailer");
					Retailer = (Retailers)System.Enum.Parse(typeof(Retailers), myPopupRetailer.MyReturnString);
				}
				else
				{
					HelperClasses.Logger.Log("Dialog Result is false.");
				}
			}

			HelperClasses.Logger.Log("Throwing Popup for Language Selection");
			Popup myPopup = new Popup(Popup.PopupWindowTypes.PopupOkComboBox, "Language you want as your downgraded GTA V Language?", pEnum: LanguageSelected);
			myPopup.ShowDialog();
			if (myPopup.DialogResult == true)
			{
				HelperClasses.Logger.Log("User picked '" + myPopup.MyReturnString + "' as their Language");
				LanguageSelected = (Languages)System.Enum.Parse(typeof(Languages), myPopup.MyReturnString);
			}
			else
			{
				HelperClasses.Logger.Log("Dialog Result is false.");
			}

			HelperClasses.Logger.Log("LogInfo - GTAVInstallationPath: '" + Settings.GTAVInstallationPath + "'");
			HelperClasses.Logger.Log("LogInfo - ZIPExtractionPath: '" + Settings.ZIPExtractionPath + "'");
			HelperClasses.Logger.Log("LogInfo - EnableCopyOverHardlink: '" + Settings.EnableCopyFilesInsteadOfHardlinking + "'");
			HelperClasses.Logger.Log("LogInfo - Retailer: '" + Settings.Retailer + "'");
			HelperClasses.Logger.Log("End of InitImportantSettings");
		}



		/// <summary>
		/// Method to set the GTA V Path manually
		/// </summary>
		public static void SetGTAVPathManually(bool CheckIfDefaultForCopyHardlinkNeedsChanging = true)
		{
			HelperClasses.Logger.Log("Asking User for GTA V Installation path");
			string GTAVInstallationPathUserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTA5.exe", @"C:\");
			HelperClasses.Logger.Log("Users picked path is: '" + GTAVInstallationPathUserChoice + "'");

			while (!(LauncherLogic.IsGTAVInstallationPathCorrect(GTAVInstallationPathUserChoice, false)))
			{
				if (String.IsNullOrWhiteSpace(GTAVInstallationPathUserChoice))
				{
					HelperClasses.Logger.Log("No Folder selected. Canceling User Action of Changing GTAV Installation Path");
					return;
				}
				HelperClasses.Logger.Log("Users picked path detected to be faulty. Asking user to try again");
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "GTA V Path detected to be not correct. Are you sure?\nForce '" + GTAVInstallationPathUserChoice + "' as your GTAV Installation Location?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("Will force the Path that user picked even tho Algorithm think its faulty.");
					Settings.GTAVInstallationPath = GTAVInstallationPathUserChoice;
					break;
				}
				GTAVInstallationPathUserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTA5.exe", @"C:\");
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
			HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: Settings.GTAVInstallationPath);
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
		/// Event that gets triggered when the ComboBox of Languages changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void combox_Set_LanguageSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LanguageSelected = (Languages)System.Enum.Parse(typeof(Languages), combox_Set_LanguageSelected.SelectedItem.ToString());
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


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_ZIPExtractionPath_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: Settings.ZIPExtractionPath);
		}



		/// <summary>
		/// Called then the TextBox of Ingame looses Focus
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tb_Set_InGameName_LostFocus(object sender, RoutedEventArgs e)
		{
			string txt = Regex.Replace(tb_Set_InGameName.Text, @"[^0-9A-Za-z_]", @"");
			if (String.IsNullOrEmpty(txt)) { txt = "HiMomImOnYoutube"; }
			if (txt.Length < 3) { txt = "HiMomImOnYoutube"; }
			if (txt.Length > 16) { txt = txt.Substring(0, 16); }
			Settings.InGameName = txt;
			tb_Set_InGameName.Text = txt;
		}

		/// <summary>
		/// Called when user wants to import settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_ImportGTAVSettings_Click(object sender, RoutedEventArgs e)
		{
			string MyFolderReturn = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick your profile folder you want to copy all settings from", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
								@"\Rockstar Games\GTA V\Profiles");

			// Close this if return is empty or ""
			if (String.IsNullOrEmpty(MyFolderReturn))
			{
				return;
			}

			string ExistingPCSettingsBinPath = MyFolderReturn.TrimEnd('\\') + @"\pc_settings.bin";
			string correctPCSettingsBinPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
											@"\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4\pc_settings.bin";
			string ExistingControlUserXmlPath = MyFolderReturn.TrimEnd('\\') + @"\control\user.xml";
			string correctControlUserXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
											@"\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4\control\user.xml";

			if (HelperClasses.FileHandling.doesFileExist(ExistingPCSettingsBinPath))
			{
				HelperClasses.FileHandling.deleteFile(correctPCSettingsBinPath);
				HelperClasses.FileHandling.copyFile(ExistingPCSettingsBinPath, correctPCSettingsBinPath);
			}

			if (HelperClasses.FileHandling.doesFileExist(ExistingControlUserXmlPath))
			{
				HelperClasses.FileHandling.deleteFile(correctControlUserXmlPath);
				HelperClasses.FileHandling.copyFile(ExistingControlUserXmlPath, correctControlUserXmlPath);
			}
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
			combox_Set_LanguageSelected.SelectedItem = Settings.LanguageSelected;
			tb_Set_InGameName.Text = Settings.InGameName;
			cb_Set_AutoSetHighPriority.IsChecked = Settings.EnableAutoSetHighPriority;
			//cb_Set_OnlyAutoStartProgramsWhenDowngraded.IsChecked = Settings.EnableOnlyAutoStartProgramsWhenDowngraded;
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
