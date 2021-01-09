using Microsoft.Win32;
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
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using Project_127.HelperClasses.Keyboard;

namespace Project_127.MySettings
{
	/// <summary>
	/// Class Settings.xaml (Partical class is in SettingsPartial.cs)
	/// </summary>
	public partial class Settings : Page
	{

		/*
		Main GUI Stuff for Settings. Contains the Button Clicks etc.
		SettingsPartial contains the actual Settings Properties.
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
			combox_Set_LanguageSelected.ItemsSource = Enum.GetValues(typeof(Languages)).Cast<Languages>();
			combox_Set_ExitWays.ItemsSource = Enum.GetValues(typeof(ExitWays)).Cast<ExitWays>();
			combox_Set_StartWays.ItemsSource = Enum.GetValues(typeof(StartWays)).Cast<StartWays>();
			combox_Set_SocialClubGameVersion.Items.Add("127");
			combox_Set_SocialClubGameVersion.Items.Add("124");

			SettingsState = LastSettingsState;

			RefreshGUI();

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


			RegistryKey myRKTemp1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Rockstar Games\GTAV");
			RegistryKey myRKTemp2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Rockstar Games\Grand Theft Auto V");
			RegistryKey myRKTemp3 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Rockstar Games\Grand Theft Auto V");

			// Used to not display a popup at the end if it was set during guessing
			bool ChangedRetailerAlready = false;

			// Adding all Guesses
			List<string> GTAVPathGuesses = new List<string>();
			GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicSteam());
			GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicRockstar());
			GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicEpic());
			GTAVPathGuesses.Add(HelperClasses.RegeditHandler.GetValue(myRKTemp1, "InstallFolderSteam"));
			GTAVPathGuesses.Add(HelperClasses.RegeditHandler.GetValue(myRKTemp2, "InstallFolderEpic"));
			GTAVPathGuesses.Add(HelperClasses.RegeditHandler.GetValue(myRKTemp3, "InstallFolder"));
			GTAVPathGuesses.Add(Globals.ProjectInstallationPath.TrimEnd('\\').Substring(0, Globals.ProjectInstallationPath.LastIndexOf('\\')));
			GTAVPathGuesses.Add(Globals.ProjectInstallationPath.TrimEnd('\\'));
			GTAVPathGuesses.Add(Settings.ZIPExtractionPath.TrimEnd('\\').Substring(0, Globals.ProjectInstallationPath.LastIndexOf('\\')));
			GTAVPathGuesses.Add(Settings.ZIPExtractionPath);


			// Loop for the Guesses
			for (int i = 0; i <= GTAVPathGuesses.Count - 1; i++)
			{
				if (String.IsNullOrWhiteSpace(Settings.GTAVInstallationPath))
				{
					HelperClasses.Logger.Log("GTAV Guess Number " + (i + 1).ToString() + " is: '" + GTAVPathGuesses[i] + "'");
					if (LauncherLogic.IsGTAVInstallationPathCorrect(GTAVPathGuesses[i], false))
					{
						HelperClasses.Logger.Log("GTAV Guess Number " + (i + 1).ToString() + " is theoretically VALID. Asking user if he wants it");
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Is: '" + GTAVPathGuesses[i] + "' your GTA V Installation Path?");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
						{
							Settings.GTAVInstallationPath = GTAVPathGuesses[i];
							HelperClasses.Logger.Log("GTAV Guess Number " + (i + 1).ToString() + " was picked by User");

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
							HelperClasses.Logger.Log("GTAV Guess Number " + (i + 1).ToString() + " was NOT picked by User, moving on");
						}
					}
					else
					{
						HelperClasses.Logger.Log("GTAV Guess Number " + (i + 1).ToString() + "is theoretically invalid, moving on");
					}
				}
			}

			// If Setting is STILL not correct
			if ((String.IsNullOrWhiteSpace(Settings.GTAVInstallationPath)))
			{
				HelperClasses.Logger.Log("After " + GTAVPathGuesses.Count + " guesses we still dont have the correct GTAVInstallationPath. User has to do it manually now. Fucking casual");

				while ((String.IsNullOrWhiteSpace(Settings.GTAVInstallationPath)))
				{
					HelperClasses.Logger.Log("If you see this, the user exited out exited out of the SetGTAVPathManually() on FirstLaunch or SettingsReset");
					SetGTAVPathManually(false);
				}
			}


			// Setting ZIP Extract Path now
			HelperClasses.Logger.Log("GTA V Path set, now onto the ZIP Folder");
			Popup yesnoconfirm = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Project 1.27 needs a Folder where it installs all of its Components and saves Files for Upgrading and Downgrading.\nIt is recommend to do this on the same Drive / Partition as your GTAV Installation Path\nBest Case (and default Location) is your GTAV Path.\nDo you want to use your GTAV Path?");
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
				PopupCombobox myPopupRetailer = new PopupCombobox("Retailer of your GTA V Installation?", Retailer);
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
			string StartUpPath = Settings.GTAVInstallationPath;
			if (String.IsNullOrWhiteSpace(StartUpPath))
			{
				StartUpPath = @"C:\";
			}
			else
			{
				StartUpPath = HelperClasses.FileHandling.PathSplitUp(StartUpPath.TrimEnd('\\'))[0];
			}
			string GTAVInstallationPathUserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTA5.exe", StartUpPath);
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
			HelperClasses.Logger.Log("Picked path '" + GTAVInstallationPathUserChoice + "' is valid and will be set as Settings.GTAVInstallationPath.");
			Settings.GTAVInstallationPath = GTAVInstallationPathUserChoice;
			if (CheckIfDefaultForCopyHardlinkNeedsChanging)
			{
				SetDefaultEnableCopyingHardlinking();
			}
		}



		/// <summary>
		/// Event that gets raised when the ComboBox of Retailers changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void combox_Set_Retail_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Retailer = (Retailers)System.Enum.Parse(typeof(Retailers), combox_Set_Retail.SelectedItem.ToString());
			RefreshGUI();
		}


		/// <summary>
		/// Event that gets raised when the ComboBox of Languages changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void combox_Set_LanguageSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LanguageSelected = (Languages)System.Enum.Parse(typeof(Languages), combox_Set_LanguageSelected.SelectedItem.ToString());
		}


		/// <summary>
		/// Event that gets raised when the ComboBox of StartWays changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void combox_Set_StartWays_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			StartWay = (StartWays)System.Enum.Parse(typeof(StartWays), combox_Set_StartWays.SelectedItem.ToString());
		}


		/// <summary>
		/// Event that gets raised when the ComboBox of ExitWays changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void combox_Set_ExitWays_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ExitWay = (ExitWays)System.Enum.Parse(typeof(ExitWays), combox_Set_ExitWays.SelectedItem.ToString());
		}


		/// <summary>
		/// Event for LeftClick on any Button which handles a Path
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Path_Click(object sender, RoutedEventArgs e)
		{
			btn_Path_Magic((Button)sender);
		}


		/// <summary>
		/// Event for RightClick on any Button which handles a Path
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Path_RightClick(object sender, MouseButtonEventArgs e)
		{
			btn_Path_Magic((Button)sender, true);
		}


		/// <summary>
		/// Method which handles all LeftClick and RightClick for all Path Related Buttons
		/// </summary>
		/// <param name="myBtn"></param>
		/// <param name="IsRightClick"></param>
		private void btn_Path_Magic(Button myBtn, bool IsRightClick = false)
		{
			string explorerStartPath = "";

			switch (myBtn.Name)
			{
				case "btn_Set_GTAVInstallationPath":
					explorerStartPath = Settings.GTAVInstallationPath;
					if (!IsRightClick)
					{
						//Originally this was here:
						// SetGTAVPathManually();

						// I changed it to this...hope its correct LOL
						SetGTAVPathManually(true);
					}
					break;
				case "btn_Set_ZIPExtractionPath":
					explorerStartPath = Settings.ZIPExtractionPath;

					if (!IsRightClick)
					{
						// Grabbing the new Path from FolderDialogThingy
						string StartUpPathZIP = Settings.ZIPExtractionPath;
						if (String.IsNullOrWhiteSpace(StartUpPathZIP))
						{
							StartUpPathZIP = @"C:\";
						}
						else
						{
							StartUpPathZIP = HelperClasses.FileHandling.PathSplitUp(StartUpPathZIP.TrimEnd('\\'))[0];
						}
						string _ZIPExtractionPath = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder where this Program will store its Data.", StartUpPathZIP);
						HelperClasses.Logger.Log("Changing ZIPExtractionPath.");
						HelperClasses.Logger.Log("Old ZIPExtractionPath: '" + Settings.ZIPExtractionPath + "'");
						HelperClasses.Logger.Log("Potential New ZIPExtractionPath: '" + _ZIPExtractionPath + "'");

						// If its a valid Path (no "") and if its a new Path
						if (ChangeZIPExtractionPath(_ZIPExtractionPath))
						{
							HelperClasses.Logger.Log("Changing ZIP Path worked");
						}
						else
						{
							HelperClasses.Logger.Log("Changing ZIP Path did not work. Probably non existing Path or same Path as before");
							new Popup(Popup.PopupWindowTypes.PopupOk, "Changing ZIP Path did not work. Probably non existing Path or same Path as before");
						}
					}
					break;
				case "btn_Set_PathLiveSplit":
					explorerStartPath = HelperClasses.FileHandling.PathSplitUp(Settings.PathLiveSplit)[0].TrimEnd('\\');

					if (!IsRightClick)
					{
						string StartUpPath = Settings.PathLiveSplit;
						if (!HelperClasses.FileHandling.doesFileExist(StartUpPath))
						{
							StartUpPath = @"C:\";
						}
						else
						{
							StartUpPath = HelperClasses.FileHandling.PathSplitUp(StartUpPath.TrimEnd('\\'))[0];
						}

						string UserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Pick your LiveSplit Executable", StartUpPath);

						if (!String.IsNullOrWhiteSpace(UserChoice))
						{
							Settings.PathLiveSplit = UserChoice;
						}
					}
					break;
				case "btn_Set_PathStreamProgram":
					explorerStartPath = HelperClasses.FileHandling.PathSplitUp(Settings.PathStreamProgram)[0].TrimEnd('\\');

					if (!IsRightClick)
					{
						string StartUpPath2 = Settings.PathStreamProgram;
						if (!HelperClasses.FileHandling.doesFileExist(StartUpPath2))
						{
							StartUpPath2 = @"C:\";
						}
						else
						{
							StartUpPath2 = HelperClasses.FileHandling.PathSplitUp(StartUpPath2.TrimEnd('\\'))[0];
						}

						string UserChoice2 = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Pick your Stream Program Executable", StartUpPath2);

						if (!String.IsNullOrWhiteSpace(UserChoice2))
						{
							Settings.PathStreamProgram = UserChoice2;
						}
					}
					break;
				case "btn_Set_PathNohboard":
					explorerStartPath = HelperClasses.FileHandling.PathSplitUp(Settings.PathNohboard)[0].TrimEnd('\\');

					if (!IsRightClick)
					{
						string StartUpPath3 = Settings.PathNohboard;
						if (!HelperClasses.FileHandling.doesFileExist(StartUpPath3))
						{
							StartUpPath3 = @"C:\";
						}
						else
						{
							StartUpPath3 = HelperClasses.FileHandling.PathSplitUp(StartUpPath3.TrimEnd('\\'))[0];
						}

						string UserChoice3 = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Pick your Nohboard Executable", StartUpPath3);

						if (!String.IsNullOrWhiteSpace(UserChoice3))
						{
							Settings.PathNohboard = UserChoice3;
						}
					}
					break;
				case "btn_Set_PathFPSLimiter":
					explorerStartPath = HelperClasses.FileHandling.PathSplitUp(Settings.PathFPSLimiter)[0].TrimEnd('\\');

					if (!IsRightClick)
					{
						string StartUpPath4 = Settings.PathFPSLimiter;
						if (!HelperClasses.FileHandling.doesFileExist(StartUpPath4))
						{
							StartUpPath4 = @"C:\";
						}
						else
						{
							StartUpPath4 = HelperClasses.FileHandling.PathSplitUp(StartUpPath4.TrimEnd('\\'))[0];
						}

						string UserChoice4 = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Pick your FPS Limiter Executable", StartUpPath4);

						if (!String.IsNullOrWhiteSpace(UserChoice4))
						{
							Settings.PathFPSLimiter = UserChoice4;
						}
					}

					break;
			}

			if (IsRightClick)
			{
				if (!HelperClasses.FileHandling.doesPathExist(explorerStartPath))
				{
					explorerStartPath = @"C:\";
				}

				HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: explorerStartPath);
			}

			RefreshGUI();
		}

		/// <summary>
		/// Button Click to change the Key1 of the JumpScript
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void btn_Set_JumpScriptKey1_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key]";
			System.Windows.Forms.Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != System.Windows.Forms.Keys.None && MyNewKey != System.Windows.Forms.Keys.Escape)
			{
				Settings.JumpScriptKey1 = MyNewKey;
			}
			((Button)sender).Content = Settings.JumpScriptKey1;
		}

		/// <summary>
		/// Button Click to change the Key2 of the JumpScript
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void btn_Set_JumpScriptKey2_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key]";
			System.Windows.Forms.Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != System.Windows.Forms.Keys.None && MyNewKey != System.Windows.Forms.Keys.Escape)
			{
				Settings.JumpScriptKey2 = MyNewKey;
			}
			((Button)sender).Content = Settings.JumpScriptKey2;
		}


		/// <summary>
		/// Method which gets called when the "Import ZIP" Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Import_Zip_Click(object sender, RoutedEventArgs e)
		{
			string ZipFileLocation = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Import ZIP File", Globals.ProjectInstallationPath, pFilter: "ZIP Files|*.zip*");
			if (HelperClasses.FileHandling.doesFileExist(ZipFileLocation))
			{
				LauncherLogic.ImportZip(ZipFileLocation);
			}
			else
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "No ZIP File selected").ShowDialog();
			}
			RefreshGUI();
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
				Settings.FirstLaunch = false;
				RefreshGUI();
			}
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
			string ExistingControlUserXmlPath = MyFolderReturn.TrimEnd('\\') + @"  ";
			string correctControlUserXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
											@"\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4\control\user.xml";


			int copiedFiles = 0;

			if (HelperClasses.FileHandling.doesFileExist(ExistingPCSettingsBinPath))
			{
				HelperClasses.FileHandling.deleteFile(correctPCSettingsBinPath);
				HelperClasses.FileHandling.copyFile(ExistingPCSettingsBinPath, correctPCSettingsBinPath);
				copiedFiles += 1;
			}

			if (HelperClasses.FileHandling.doesFileExist(ExistingControlUserXmlPath))
			{
				HelperClasses.FileHandling.deleteFile(correctControlUserXmlPath);
				HelperClasses.FileHandling.copyFile(ExistingControlUserXmlPath, correctControlUserXmlPath);
				copiedFiles += 1;
			}

			new Popup(Popup.PopupWindowTypes.PopupOk, copiedFiles + " Files were imported from the selected Folder").ShowDialog();
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
		private void RefreshGUI(bool FromRegedit = false)
		{
			if (FromRegedit)
			{
				Settings.LoadSettings();
			}
			btn_Set_GTAVInstallationPath.Content = "GTA V Installation Path: " + Settings.GTAVInstallationPath;
			btn_Set_ZIPExtractionPath.Content = "Components Installation Path: " + Settings.ZIPExtractionPath;
			btn_Set_PathFPSLimiter.Content = "FPS Limiter Path: " + Settings.PathFPSLimiter;
			btn_Set_PathLiveSplit.Content = "LiveSplit Path: " + Settings.PathLiveSplit;
			btn_Set_PathStreamProgram.Content = "Stream Program Path: " + Settings.PathStreamProgram;
			btn_Set_PathNohboard.Content = "Nohboard Path: " + Settings.PathNohboard;

			btn_Import_Zip.Content = "Import ZIP Manually (Current Version: " + Globals.ZipVersion + ")";

			combox_Set_Retail.SelectedItem = Settings.Retailer;
			combox_Set_LanguageSelected.SelectedItem = Settings.LanguageSelected;
			combox_Set_ExitWays.SelectedItem = Settings.ExitWay;
			combox_Set_StartWays.SelectedItem = Settings.StartWay;
			combox_Set_SocialClubGameVersion.SelectedItem = Settings.SocialClubLaunchGameVersion;

			tb_Set_InGameName.Text = Settings.InGameName;
			btn_Set_JumpScriptKey1.Content = Settings.JumpScriptKey1;
			btn_Set_JumpScriptKey2.Content = Settings.JumpScriptKey2;

			ButtonMouseOverMagic(btn_Refresh);
			ButtonMouseOverMagic(btn_cb_Set_EnableLogging);
			ButtonMouseOverMagic(btn_cb_Set_CopyFilesInsteadOfHardlinking);
			ButtonMouseOverMagic(btn_cb_Set_EnableAlternativeLaunch);
			ButtonMouseOverMagic(btn_cb_Set_CopyFilesInsteadOfSyslinking_SocialClub);
			ButtonMouseOverMagic(btn_cb_Set_EnablePreOrderBonus);
			ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartFPSLimiter);
			ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartLiveSplit);
			ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartNohboard);
			ButtonMouseOverMagic(btn_cb_Set_EnableOverlay);
			ButtonMouseOverMagic(btn_cb_Set_EnableOverlayMultiMonitor);
			ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartStreamProgram);
			ButtonMouseOverMagic(btn_cb_Set_AutoSetHighPriority);
			ButtonMouseOverMagic(btn_cb_Set_OnlyAutoStartProgramsWhenDowngraded);
			ButtonMouseOverMagic(btn_cb_Set_EnableDontLaunchThroughSteam);
			ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartJumpScript);
			ButtonMouseOverMagic(btn_cb_Set_SlowCompare);
			ButtonMouseOverMagic(btn_cb_Set_LegacyAuth);

			RefreshIfOptionsHide();
		}


		/// <summary>
		/// Enum for all ReadMeStates
		/// </summary>
		public enum SettingsStates
		{
			General,
			GTA,
			Extra
		}

		/// <summary>
		/// Internal Value
		/// </summary>
		private SettingsStates _SettingsState = SettingsStates.General;

		private static SettingsStates LastSettingsState = SettingsStates.General;

		/// <summary>
		/// Value we get and set. Setters are gucci. 
		/// </summary>
		public SettingsStates SettingsState
		{
			get
			{
				return _SettingsState;
			}
			set
			{
				_SettingsState = value;

				// Saving it in LastReadMeState
				Settings.LastSettingsState = value;

				if (value == SettingsStates.General)
				{
					sv_Settings_General.Visibility = Visibility.Visible;
					sv_Settings_GTA.Visibility = Visibility.Hidden;
					sv_Settings_Extra.Visibility = Visibility.Hidden;

					btn_SettingsGeneral.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;
					btn_SettingsGTA.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
					btn_SettingsExtra.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					lbl_SettingsHeader.Content = "General P127 Settings";
					sv_Settings_General.ScrollToVerticalOffset(0);
				}
				else if (value == SettingsStates.GTA)
				{
					sv_Settings_General.Visibility = Visibility.Hidden;
					sv_Settings_GTA.Visibility = Visibility.Visible;
					sv_Settings_Extra.Visibility = Visibility.Hidden;

					btn_SettingsGeneral.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
					btn_SettingsGTA.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;
					btn_SettingsExtra.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					lbl_SettingsHeader.Content = "GTA Settings";
					sv_Settings_GTA.ScrollToVerticalOffset(0);
				}
				else if (value == SettingsStates.Extra)
				{
					sv_Settings_General.Visibility = Visibility.Hidden;
					sv_Settings_GTA.Visibility = Visibility.Hidden;
					sv_Settings_Extra.Visibility = Visibility.Visible;

					btn_SettingsGeneral.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
					btn_SettingsGTA.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
					btn_SettingsExtra.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

					lbl_SettingsHeader.Content = "Settings of Extra Features";
					sv_Settings_Extra.ScrollToVerticalOffset(0);
				}
			}
		}

		/// <summary>
		/// MouseEnter event for all Buttons which use Images (Refresh and Checkboxes)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			Button myBtn = (Button)sender;
			ButtonMouseOverMagic(myBtn);
		}

		/// <summary>
		/// MouseLeave event for all Buttons which use Images (Refresh and Checkboxes)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			Button myBtn = (Button)sender;
			ButtonMouseOverMagic(myBtn);
		}

		/// <summary>
		/// Sets the Background for one specific CheckboxButton. Needs the second property to know if it should be checked or not
		/// </summary>
		/// <param name="myBtn"></param>
		/// <param name="pChecked"></param>
		private void SetCheckBoxBackground(Button myBtn, bool pChecked)
		{
			string BaseURL = @"Artwork/checkbox";
			if (pChecked)
			{
				BaseURL += "_true";
			}
			else
			{
				BaseURL += "_false";
			}
			if (myBtn.IsMouseOver)
			{
				BaseURL += "_mo.png";
			}
			else
			{
				BaseURL += ".png";
			}
			MainWindow.MW.SetControlBackground(myBtn, BaseURL);
		}

		/// <summary>
		/// Logic behind all MouseOver Stuff. Checkboxes and Refresh Button
		/// </summary>
		/// <param name="myBtn"></param>
		private void ButtonMouseOverMagic(Button myBtn)
		{
			switch (myBtn.Name)
			{
				case "btn_Refresh":
					if (myBtn.IsMouseOver)
					{
						MainWindow.MW.SetControlBackground(btn_Refresh, "Artwork/refresh_mo.png");
					}
					else
					{
						MainWindow.MW.SetControlBackground(btn_Refresh, "Artwork/refresh.png");
					}
					break;
				case "btn_cb_Set_EnableLogging":
					SetCheckBoxBackground(myBtn, Settings.EnableLogging);
					break;
				case "btn_cb_Set_LegacyAuth":
					SetCheckBoxBackground(myBtn, Settings.EnableLegacyAuth);
					break;
				case "btn_cb_Set_SlowCompare":
					SetCheckBoxBackground(myBtn, Settings.EnableSlowCompare);
					break;
				case "btn_cb_Set_CopyFilesInsteadOfHardlinking":
					SetCheckBoxBackground(myBtn, Settings.EnableCopyFilesInsteadOfHardlinking);
					break;
				case "btn_cb_Set_EnableAlternativeLaunch":
					SetCheckBoxBackground(myBtn, Settings.EnableAlternativeLaunch);
					break;
				case "btn_cb_Set_CopyFilesInsteadOfSyslinking_SocialClub":
					SetCheckBoxBackground(myBtn, Settings.EnableCopyFilesInsteadOfSyslinking_SocialClub);
					break;
				case "btn_cb_Set_EnablePreOrderBonus":
					SetCheckBoxBackground(myBtn, Settings.EnablePreOrderBonus);
					break;
				case "btn_cb_Set_AutoSetHighPriority":
					SetCheckBoxBackground(myBtn, Settings.EnableAutoSetHighPriority);
					break;
				case "btn_cb_Set_OnlyAutoStartProgramsWhenDowngraded":
					SetCheckBoxBackground(myBtn, Settings.EnableOnlyAutoStartProgramsWhenDowngraded);
					break;
				case "btn_cb_Set_EnableAutoStartLiveSplit":
					SetCheckBoxBackground(myBtn, Settings.EnableAutoStartLiveSplit);
					break;
				case "btn_cb_Set_EnableAutoStartFPSLimiter":
					SetCheckBoxBackground(myBtn, Settings.EnableAutoStartFPSLimiter);
					break;
				case "btn_cb_Set_EnableAutoStartStreamProgram":
					SetCheckBoxBackground(myBtn, Settings.EnableAutoStartStreamProgram);
					break;
				case "btn_cb_Set_EnableAutoStartNohboard":
					SetCheckBoxBackground(myBtn, Settings.EnableAutoStartNohboard);
					break;
				case "btn_cb_Set_EnableDontLaunchThroughSteam":
					SetCheckBoxBackground(myBtn, Settings.EnableDontLaunchThroughSteam);
					break;
				case "btn_cb_Set_EnableOverlay":
					SetCheckBoxBackground(myBtn, Settings.EnableOverlay);
					break;
				case "btn_cb_Set_EnableOverlayMultiMonitor":
					SetCheckBoxBackground(myBtn, Settings.OverlayMultiMonitorMode);
					break;
				case "btn_cb_Set_EnableAutoStartJumpScript":
					SetCheckBoxBackground(myBtn, Settings.EnableAutoStartJumpScript);
					break;
			}
		}

		/// <summary>
		/// Click behind the Refresh Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Refresh_Click(object sender, RoutedEventArgs e)
		{
			RefreshGUI(true);
		}


		/// <summary>
		/// Logic to decide which Settings we need to not show (overlay with Grey Rectangle to say that they are disabled
		/// </summary>
		private void RefreshIfOptionsHide()
		{
			Rect_HideOptions_Tease.Visibility = Visibility.Hidden;

			if (Settings.EnableOverlay)
			{
				Rect_HideOption_OverlayMM.Visibility = Visibility.Hidden;
			}
			else
			{
				Rect_HideOption_OverlayMM.Visibility = Visibility.Visible;
			}

			if (Settings.Retailer != Retailers.Steam)
			{
				Rect_HideOption_HideFromSteam.Visibility = Visibility.Visible;
			}
			else
			{
				Rect_HideOption_HideFromSteam.Visibility = Visibility.Hidden;
			}

			if (Settings.Retailer == Retailers.Epic)
			{
				Rect_HideOptions_AllLaunchThroughSocialClubThings.Visibility = Visibility.Visible;
				Rect_HideOptions_LaunchThroughSocialClubOptions.Visibility = Visibility.Hidden;
				Rect_HideOptions_OrigEmu.Visibility = Visibility.Hidden;
			}
			else
			{
				Rect_HideOptions_AllLaunchThroughSocialClubThings.Visibility = Visibility.Hidden;

				if (Settings.EnableAlternativeLaunch)
				{
					Rect_HideOptions_LaunchThroughSocialClubOptions.Visibility = Visibility.Hidden;
					Rect_HideOptions_OrigEmu.Visibility = Visibility.Visible;
				}
				else
				{
					Rect_HideOptions_LaunchThroughSocialClubOptions.Visibility = Visibility.Visible;
					Rect_HideOptions_OrigEmu.Visibility = Visibility.Hidden;
				}
			}

			// Remove this...

			// Rect_HideOptions_Tease.Visibility = Visibility.Visible;
			// Rect_HideOptions.Visibility = Visibility.Hidden;
			// Rect_HideOptions2.Visibility = Visibility.Hidden;
			// Rect_HideOptions3.Visibility = Visibility.Hidden;
		}


		/// <summary>
		/// Click on ANY checkbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_cb_Click(object sender, RoutedEventArgs e)
		{
			Button myBtn = (Button)sender;

			switch (myBtn.Name)
			{
				case "btn_cb_Set_EnableLogging":
					Settings.EnableLogging = !Settings.EnableLogging;
					break;
				case "btn_cb_Set_LegacyAuth":
					Settings.EnableLegacyAuth = !Settings.EnableLegacyAuth;
					break;
				case "btn_cb_Set_SlowCompare":
					Settings.EnableSlowCompare = !Settings.EnableSlowCompare;
					break;
				case "btn_cb_Set_EnableAlternativeLaunch":
					Settings.EnableAlternativeLaunch = !Settings.EnableAlternativeLaunch;
					RefreshIfOptionsHide();
					break;
				case "btn_cb_Set_CopyFilesInsteadOfHardlinking":
					Settings.EnableCopyFilesInsteadOfHardlinking = !Settings.EnableCopyFilesInsteadOfHardlinking;
					SetDefaultEnableCopyingHardlinking();
					break;
				case "btn_cb_Set_CopyFilesInsteadOfSyslinking_SocialClub":
					Settings.EnableCopyFilesInsteadOfSyslinking_SocialClub = !Settings.EnableCopyFilesInsteadOfSyslinking_SocialClub;
					break;
				case "btn_cb_Set_EnablePreOrderBonus":
					Settings.EnablePreOrderBonus = !Settings.EnablePreOrderBonus;
					break;
				case "btn_cb_Set_AutoSetHighPriority":
					Settings.EnableAutoSetHighPriority = !Settings.EnableAutoSetHighPriority;
					break;
				case "btn_cb_Set_OnlyAutoStartProgramsWhenDowngraded":
					Settings.EnableOnlyAutoStartProgramsWhenDowngraded = !Settings.EnableOnlyAutoStartProgramsWhenDowngraded;
					break;
				case "btn_cb_Set_EnableAutoStartLiveSplit":
					Settings.EnableAutoStartLiveSplit = !Settings.EnableAutoStartLiveSplit;
					break;
				case "btn_cb_Set_EnableAutoStartFPSLimiter":
					Settings.EnableAutoStartFPSLimiter = !Settings.EnableAutoStartFPSLimiter;
					break;
				case "btn_cb_Set_EnableAutoStartStreamProgram":
					Settings.EnableAutoStartStreamProgram = !Settings.EnableAutoStartStreamProgram;
					break;
				case "btn_cb_Set_EnableAutoStartNohboard":
					Settings.EnableAutoStartNohboard = !Settings.EnableAutoStartNohboard;
					break;
				case "btn_cb_Set_EnableDontLaunchThroughSteam":
					Settings.EnableDontLaunchThroughSteam = !Settings.EnableDontLaunchThroughSteam;
					break;
				case "btn_cb_Set_EnableOverlay":
					Settings.EnableOverlay = !Settings.EnableOverlay;
					RefreshIfOptionsHide();
					break;
				case "btn_cb_Set_EnableOverlayMultiMonitor":
					Settings.OverlayMultiMonitorMode = !Settings.OverlayMultiMonitorMode;
					break;
				case "btn_cb_Set_EnableAutoStartJumpScript":
					Settings.EnableAutoStartJumpScript = !Settings.EnableAutoStartJumpScript;
					break;
			}
			RefreshGUI();
		}

		/// <summary>
		/// Checking for Update
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_CheckForUpdate_Click(object sender, RoutedEventArgs e)
		{
			//Globals.CheckForBigThree();
			//Globals.CheckForZipUpdate();
			Globals.CheckForUpdate();
			Globals.SetUpDownloadManager();
			new Popup(Popup.PopupWindowTypes.PopupOk, "Update check complete. All updates handled.\nIf you want to re-download certain files,\nyou can do so in the 'Component Manager'").ShowDialog();
		}


		/// <summary>
		/// Switching to NoteOverlay specific Page (Notes)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_Overlay_Notes_Click(object sender, RoutedEventArgs e)
		{
			NoteOverlay.LoadNoteOverlayWithCustomPage = NoteOverlay.NoteOverlayPages.NoteFiles;
			Globals.PageState = Globals.PageStates.NoteOverlay;
		}

		/// <summary>
		/// Switching to NoteOverlay specific Page (Hotkeys) 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_Overlay_Hotkeys_Click(object sender, RoutedEventArgs e)
		{
			NoteOverlay.LoadNoteOverlayWithCustomPage = NoteOverlay.NoteOverlayPages.Keybinds;
			Globals.PageState = Globals.PageStates.NoteOverlay;
		}

		/// <summary>
		/// Switching to NoteOverlay specific Page (Looks) 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_Overlay_Looks_Click(object sender, RoutedEventArgs e)
		{
			NoteOverlay.LoadNoteOverlayWithCustomPage = NoteOverlay.NoteOverlayPages.Look;
			Globals.PageState = Globals.PageStates.NoteOverlay;
		}


		/// <summary>
		/// Resetting Everything
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Reset_Everything_Click(object sender, RoutedEventArgs e)
		{
			ResetEverything();
		}

		/// <summary>
		/// Resetting Everything
		/// </summary>
		public static void ResetEverything(bool uninstallAfterAutomatically = false)
		{
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "This resets the whole Project 1.27 Client including:\nSettings\nZIP Files Downloaded\nBacked up GTA Files to upgrade when downgraded\n\YOU NEED TO repair / verify GTA V through Steam / Epic / Rockstar AFTER THIS\n\nYou will be asked if you want to un-install P127 after this.");
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "This might take a while, just hit \"OK\" and wait a bit.\nProject 1.27 will Exit once its done.").ShowDialog();

				HelperClasses.Logger.Log("Initiating a full Reset:");

				HelperClasses.Logger.Log("Making an Upgrade.");
				LauncherLogic.Upgrade();
				HelperClasses.Logger.Log("Done making an Upgrade.", 1);

				HelperClasses.Logger.Log("Lets make a full Repair.");
				LauncherLogic.Repair();
				HelperClasses.Logger.Log("Done making an a full Repair.", 1);

				HelperClasses.Logger.Log("Deleting all Regedit Values.");
				RegeditHandler.DeleteKey();
				HelperClasses.Logger.Log("Done deleting all Regedit Values.", 1);

				HelperClasses.Logger.Log("Deleting all extracted ZIP Files.");
				HelperClasses.FileHandling.DeleteFolder(LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files");
				HelperClasses.Logger.Log("Done deleting all extracted ZIP Files.", 1);

				Popup yesno2 = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you want to uninstall Project 1.27?");
				yesno2.ShowDialog();
				if (yesno2.DialogResult == true)
				{
					HelperClasses.Logger.Log("User wants no uninstall...Gonna close this now I guess...cya");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=== FULL REPAIR DONE ===");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=== UNINSTALL COMMING ===");
					HelperClasses.Logger.Log("=");

					string FilePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\unins000.exe";
					ProcessHandler.StartProcess(FilePath, Globals.ProjectInstallationPath);

					Globals.ProperExit();
				}
				else
				{
					HelperClasses.Logger.Log("User wants no uninstall...Gonna close this now I guess...cya");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=== FULL REPAIR DONE ===");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");
					HelperClasses.Logger.Log("=");

					Globals.ProperExit();
				}

			}
		}


		/// <summary>
		/// Method which gets called when the Update Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_RepairGTA_Click(object sender, RoutedEventArgs e)
		{
			if (!LauncherLogic.IsGTAVInstallationPathCorrect())
			{

				HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");

				string msg = "Error: GTA V Installation Path incorrect.\nGTAV Installation Path: '" + LauncherLogic.GTAVFilePath + "'";

				if (Globals.Branch != "master")
				{
					Popup yesno2 = new Popup(Popup.PopupWindowTypes.PopupYesNo, msg + "\n. Force this Repair?");
					yesno2.ShowDialog();
					if (yesno2.DialogResult == false)
					{
						return;
					}
				}
				else
				{
					new Popup(Popup.PopupWindowTypes.PopupOkError, msg).ShowDialog();
				}
			}

			if (HelperClasses.BuildVersionTable.IsUpgradedGTA(LauncherLogic.UpgradeFilePathBackup))
			{
				string msg3 = "Found Backup for UpgradeFiles.\nDo you want to try that instead of Repairing?";
				Popup yesno2 = new Popup(Popup.PopupWindowTypes.PopupYesNo, msg3);
				yesno2.ShowDialog();
				if (yesno2.DialogResult == true)
				{
					LauncherLogic.UseBackup();
					return;
				}
			}

			string msg2 = "";
			msg2 += "Do you want to do a quick Repair or a deep Repair?\n";
			msg2 += "\nquick Repair:\n";
			msg2 += "You need to repair / verify your GTA via your Retailer";
			msg2 += "\n(Steam, Rockstar, Epic) before or after this.\n";
			msg2 += "deep Repair:\n";
			msg2 += "You need to repair / verify your GTA via your Retailer";
			msg2 += "\n(Steam, Rockstar, Epic) AFTER this.";
			msg2 += "\nThis really removes all P127 Files and Traces\nfrom your GTA V Installation";
			//quick repair is true
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNoRepair, msg2);
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				// Quick Repair
				LauncherLogic.Repair(true);
			}
			else
			{
				// Deep Repair
				LauncherLogic.Repair();
			}

			MainWindow.MW.UpdateGUIDispatcherTimer();
		}


		/// <summary>
		/// Create Backup Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_CreateBackup_Click(object sender, RoutedEventArgs e)
		{
			LauncherLogic.CreateBackup();
		}



		/// <summary>
		/// Create Backup Rightclick
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_CreateBackup_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			PopupCreateBackup.IsOpen = true;
		}

		/// <summary>
		/// Use Backup Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_UseBackup_Click(object sender, RoutedEventArgs e)
		{
			LauncherLogic.UseBackup();
		}


		/// <summary>
		/// Use Backup Rightclick
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_UseBackup_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			PopupUseBackup.IsOpen = true;
		}


		/// <summary>
		/// Click on Subpage of Settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SettingsGeneral_Click(object sender, RoutedEventArgs e)
		{
			SettingsState = SettingsStates.General;
		}

		/// <summary>
		/// Click on Subpage of Settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SettingsGTA_Click(object sender, RoutedEventArgs e)
		{
			SettingsState = SettingsStates.GTA;
		}


		/// <summary>
		/// Click on Subpage of Settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SettingsExtra_Click(object sender, RoutedEventArgs e)
		{
			SettingsState = SettingsStates.Extra;
		}

		/// <summary>
		/// Rightclick Settings Header to get Mode Popup
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbl_SettingsHeader_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (SettingsState == SettingsStates.General)
			{
				new PopupMode().ShowDialog();
			}
			else if (SettingsState == SettingsStates.GTA)
			{
				btn_SettingsGTA_MouseRightButtonDown(null, null);
			}
		}


		/// <summary>
		/// Selection Changed of Version ComboBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void combox_Set_SocialClubGameVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Settings.SocialClubLaunchGameVersion = combox_Set_SocialClubGameVersion.SelectedItem.ToString();
		}


		/// <summary>
		/// Empty
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PopupCreateBackup_Closed(object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Method gets called when Popup of CreateBackup is opened
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PopupCreateBackup_Opened(object sender, EventArgs e)
		{
			tb_Set_BackupName.Text = "BackupName";
		}


		/// <summary>
		/// Keydown event to click "yes" on Enter on textbox of SetBackup 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tb_Set_BackupName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				btn_CreateBackupName_Click(null, null);
			}
		}

		/// <summary>
		/// Creating a Backup
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void btn_CreateBackupName_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(tb_Set_BackupName.Text))
			{
				string prevName = btn_CreateBackupName.Content.ToString();
				btn_CreateBackupName.Content = "Invalid Name";
				await Task.Delay(750);
				btn_CreateBackupName.Content = prevName;
			}
			else
			{
				string newPath = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"_Backup_" + tb_Set_BackupName.Text;
				if (HelperClasses.FileHandling.doesPathExist(newPath))
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Backup with that name ('" + tb_Set_BackupName.Text + "') already exists.\nDo you want to delete it?");
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						HelperClasses.FileHandling.DeleteFolder(newPath);
					}
					else
					{
						return;
					}
				}
				HelperClasses.FileHandling.createPath(newPath);
				if (HelperClasses.FileHandling.doesPathExist(newPath))
				{
					LauncherLogic.CreateBackup(tb_Set_BackupName.Text);
				}
				else
				{
					string prevName = btn_CreateBackupName.Content.ToString();
					btn_CreateBackupName.Content = "Cant create that Folder.";
					await Task.Delay(750);
					btn_CreateBackupName.Content = prevName;
				}
			}
		}

		/// <summary>
		/// MouseLeave on Popup Borders
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Border_MouseLeave(object sender, MouseEventArgs e)
		{
			PopupCreateBackup.IsOpen = false;
			PopupUseBackup.IsOpen = false;
		}

		/// <summary>
		/// Empty
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PopupUseBackup_Closed(object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Use Backup Popup opened.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PopupUseBackup_Opened(object sender, EventArgs e)
		{
			combox_UseBackup.Items.Clear();


			foreach (string MyFolder in HelperClasses.FileHandling.GetSubFolders(LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\"))
			{
				if (MyFolder.Contains(LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles_Backup_"))
				{
					string NiceName = MyFolder.Substring(MyFolder.LastIndexOf('_') + 1);
					combox_UseBackup.Items.Add(NiceName);
				}
			}

		}

		/// <summary>
		/// use Backup Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void btn_UseBackupName_Click(object sender, RoutedEventArgs e)
		{
			if (combox_UseBackup.SelectedItem == null)
			{
				string prevName = btn_UseBackupName.Content.ToString();
				btn_UseBackupName.Content = "Invalid Selection";
				await Task.Delay(750);
				btn_UseBackupName.Content = prevName;
				return;
			}


			if (String.IsNullOrEmpty(combox_UseBackup.SelectedItem.ToString()))
			{
				string prevName = btn_UseBackupName.Content.ToString();
				btn_UseBackupName.Content = "Invalid Selection";
				await Task.Delay(750);
				btn_UseBackupName.Content = prevName;
				return;
			}

			string newName = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles_Backup_" + combox_UseBackup.SelectedItem.ToString();
			if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(newName).Length >= 2))
			{
				string prevName = btn_UseBackupName.Content.ToString();
				btn_UseBackupName.Content = "No Files in that Folder";
				await Task.Delay(750);
				btn_UseBackupName.Content = prevName;
				return;
			}


			Globals.DebugPopup(newName);

			LauncherLogic.UseBackup(combox_UseBackup.SelectedItem.ToString());
		}

		/// <summary>
		/// Empty
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void combox_UseBackup_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		/// <summary>
		/// Resetting location of multi monitor mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_cb_Set_EnableOverlayMultiMonitor_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			NoteOverlay.OverlaySettingsChanged();

			Settings.OL_MM_Left = 0;
			Settings.OL_MM_Top = 0;

			NoteOverlay.OverlaySettingsChanged();
		}

		/// <summary>
		/// Rightclick on General Subpage Button. MOde popup.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SettingsGeneral_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			new PopupMode().ShowDialog();
		}

		/// <summary>
		/// Check for Update rightclick ( getting specific build)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_CheckForUpdate_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			PopupTextbox tb = new PopupTextbox("Build Name:", "MyBuildName");
			tb.ShowDialog();
			if (tb.DialogResult == true)
			{
				// Check if we can Download the build from the branch we are currently on.

				if (HelperClasses.FileHandling.URLExists(tb.MyReturnString))
				{
					HelperClasses.Logger.Log("Importing Build. Links: ");
					HelperClasses.Logger.Log("DDL: " + tb.MyReturnString);
					Globals.ImportBuildFromUrl(tb.MyReturnString);
				}
				else if (tb.MyReturnString != "")
				{
					string DLLinkBranch = "https://github.com/TwosHusbandS/Project-127/raw/" + Globals.Branch + "/Installer/Builds/" + tb.MyReturnString.TrimEnd(".exe") + ".exe";
					string DLLinkMaster = "https://github.com/TwosHusbandS/Project-127/raw/Master" + "/Installer/Builds/" + tb.MyReturnString.TrimEnd(".exe") + ".exe";

					HelperClasses.Logger.Log("Importing Build. Links: ");
					HelperClasses.Logger.Log("DLLinkBranch: " + DLLinkBranch);
					HelperClasses.Logger.Log("DLLinkMaster: " + DLLinkMaster);

					if (HelperClasses.FileHandling.URLExists(DLLinkBranch))
					{
						Globals.ImportBuildFromUrl(DLLinkBranch);
					}
					else
					{
						if (HelperClasses.FileHandling.URLExists(DLLinkMaster))
						{
							Globals.ImportBuildFromUrl(DLLinkMaster);
						}
					}

					HelperClasses.Logger.Log("Both not reachable...");

				}


				new Popup(Popup.PopupWindowTypes.PopupOk, "Cant find that build online.").ShowDialog();
			}
		}



		private void btn_SettingsGTA_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (LauncherLogic.GameState == LauncherLogic.GameStates.NonRunning)
			{
				ROSCommunicationBackend.setFlag(ROSCommunicationBackend.Flags.indicateTheLessThanLegalProcurementOfMotorVehicles, true);
				Overlay.GTAOverlay.indicateTheLessThanLegalProcurementOfMotorVehicles = true;
				new Popup(Popup.PopupWindowTypes.PopupOk, "'Stealy Wheely Automobiley' activated.\nRestart P127 to disable.").ShowDialog();
			}
			else
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "Cant do that while the game is running").ShowDialog();
			}
		}
	} // End of Class
} // End of Namespace 
