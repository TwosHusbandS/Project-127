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
using Microsoft.Windows.Themes;
using System.Windows.Shapes;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using Project_127.HelperClasses.Keyboard;
using CefSharp.DevTools.CSS;
using static Project_127.ComponentManager;
using GSF;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Windows.Media.TextFormatting;
using GSF.Parsing;
using System.Windows.Interop;
using CefSharp.DevTools.Page;

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
            combox_Set_PostMTLAction.ItemsSource = Enum.GetValues(typeof(PostMTLActions)).Cast<PostMTLActions>();
            combox_Set_LanguageSelected.ItemsSource = Enum.GetValues(typeof(Languages)).Cast<Languages>();
            combox_Set_ExitWays.ItemsSource = Enum.GetValues(typeof(ExitWays)).Cast<ExitWays>();
            combox_Set_StartWays.ItemsSource = Enum.GetValues(typeof(StartWays)).Cast<StartWays>();
            combox_Set_SocialClubGameVersion.Items.Add("127");
            combox_Set_SocialClubGameVersion.Items.Add("124");
            combox_Set_DragonEmuGameVersion.Items.Add("127");
            combox_Set_DragonEmuGameVersion.Items.Add("124");

            SettingsState = LastSettingsState;

            RefreshGUI();

            this.DataContext = this;
        }



        /// <summary>
        /// Initiating GTAV InstallationPath, ZIP Extraction Path and enabling Copy over Hardlinking
        /// </summary>
        public static void InitImportantSettings()
        {
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                InitImportantSettings IIS = new InitImportantSettings();
                IIS.ShowDialog();
            });
            return;

            /*
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
			GTAVPathGuesses.Add(Settings.ZIPExtractionPath.TrimEnd('\\').Substring(0, Settings.ZIPExtractionPath.LastIndexOf('\\')));
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
						PopupWrapper.PopupYesNo("Is: '" + GTAVPathGuesses[i] + "' your GTA V Installation Path?");
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
								RetailerGuessPopup = PopupWrapper.PopupYesNo("Retailer: '" + myRetailGuess + "' detected. Is that correct?");
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
					HelperClasses.Logger.Log("If you see this more than once, the user exited out exited out of the SetGTAVPathManually() on FirstLaunch or SettingsReset");
					SetGTAVPathManually(false);
				}
			}


			// Setting ZIP Extract Path now
			HelperClasses.Logger.Log("GTA V Path set, now onto the ZIP Folder");
			Popup yesnoconfirm = PopupWrapper.PopupYesNo("Project 1.27 needs a Folder where it installs all of its Components and saves Files for Upgrading and Downgrading.\nIt is recommend to do this on the same Drive / Partition as your GTAV Installation Path\nBest Case (and default Location) is your GTAV Path.\nDo you want to use your GTAV Path?");
			yesnoconfirm.ShowDialog();

			// If User wants default Settings
			if (yesnoconfirm.DialogResult == true)
			{
				HelperClasses.Logger.Log("User wants default ZIP Folder. Setting it to GTAV InstallationPath and calling the default CopyHardlink Method");

				if (!ChangeZIPExtractionPath(Settings.GTAVInstallationPath))
				{
					HelperClasses.Logger.Log("Changing ZIP Path did not work. Probably non existing Path or same Path as before (from Settings.InitImportantSettings())");
					PopupWrapper.PopupOk("Changing ZIP Path did not work. Probably non existing Path or same Path as before\nIf you read this message to anyone, tell them youre in Settings.InitImportantSettings()");
				}
			}
			// If User does not
			else
			{
				HelperClasses.Logger.Log("User does not want the default ZIP Folder");
				PopupWrapper.PopupOk("Okay, please pick a Folder where this Program will store most of its Files\n(For Upgrading / Downgrading / SaveFileHandling)").ShowDialog();
				string newZIPFileLocation = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder where this Program will store its Data.", Settings.ZIPExtractionPath);
				HelperClasses.Logger.Log("User provided own Location: ('" + newZIPFileLocation + "'). Calling the Method to move zip File");
				if (!ChangeZIPExtractionPath(newZIPFileLocation))
				{
					HelperClasses.Logger.Log("Changing ZIP Path did not work. Probably non existing Path or same Path as before (from Settings.InitImportantSettings())");
					PopupWrapper.PopupOk("Changing ZIP Path did not work. Probably non existing Path or same Path as before\nIf you read this message to anyone, tell them youre in Settings.InitImportantSettings()");
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
			*/
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
                bool yesno = PopupWrapper.PopupYesNo("GTA V Path detected to be not correct. Are you sure?\nForce '" + GTAVInstallationPathUserChoice + "' as your GTAV Installation Location?");
                if (yesno == true)
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


            if (Settings.ZIPExtractionPath.TrimEnd('\\').ToLower() != GTAVInstallationPathUserChoice.TrimEnd('\\').ToLower())
            {
                // Setting ZIP Extract Path now
                HelperClasses.Logger.Log("GTA V Path manually set, asking user if he wants to move ZIP Folder. Old ZIP Folder is: '" + Settings.ZIPExtractionPath + "'.");

                bool yesnoconfirm = PopupWrapper.PopupYesNo("Project 1.27 needs a Folder where it installs all of its Components and saves Files for Upgrading and Downgrading.\nIt is recommend to do this on the same Drive / Partition as your GTAV Installation Path\nBest Case (and default Location) is your GTAV Path.\nDo you want to move from your Path\n(" + Settings.ZIPExtractionPath + ")\nto the new GTA V Installation Location?\n\nI recommend Yes.?");

                // If User wants default Settings
                if (yesnoconfirm == true)
                {
                    HelperClasses.Logger.Log("User wants default ZIP Folder. Changing it to '" + GTAVInstallationPathUserChoice + "'.");

                    if (!ChangeZIPExtractionPath(GTAVInstallationPathUserChoice))
                    {
                        HelperClasses.Logger.Log("Changing ZIP Path did not work. Probably non existing Path or same Path as before (from Settings.SetGTAVPathManually())");
                        PopupWrapper.PopupError("Changing ZIP Path did not work. Probably non existing Path or same Path as before\nIf you read this message to anyone, tell them youre in Settings.SetGTAVPathManually()");
                    }
                }
                else
                {
                    HelperClasses.Logger.Log("User does NOT want default ZIP Folder. Will not change it.");
                }
            }
            else
            {
                HelperClasses.Logger.Log("GTA V Path manually set, not asking user if he wants to move ZIP Folder, since its at the correct Location already.");
            }



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
            Retailers NewRetailer = (Retailers)System.Enum.Parse(typeof(Retailers), combox_Set_Retail.SelectedItem.ToString());
            Retailer = NewRetailer;
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
            RefreshGUI();
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
                            PopupWrapper.PopupError("Changing ZIP Path did not work. Probably non existing Path or same Path as before");
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
            PopupWrapper.PopupOk("This option is no longer available here.\n\nTo manually import a ZIP,\nclick the 'import' button inside the Componentmanager");
        }




        /// <summary>
        /// Button Click on Reset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            bool yesno = PopupWrapper.PopupYesNo("Do you want to reset settings?");
            if (yesno == true)
            {
                HelperClasses.Logger.Log("Resetting Settings STARTED, this will explain the following messages");
                Settings.ResetSettings();

                // Just checks if the GTAVInstallationPath is empty.
                while (String.IsNullOrEmpty(Settings.GTAVInstallationPath) || String.IsNullOrEmpty(Settings.ZIPExtractionPath))
                {
                    Settings.InitImportantSettings();
                }
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

            PopupWrapper.PopupOk(copiedFiles + " Files were imported from the selected Folder");
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


        private void RefreshLaunchLabel()
        {
            if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch)
            {
                if (SocialClubLaunchGameVersion == "124")
                {
                    lbl_LaunchWays.Content = "Launch - Method: 1.24 SocialClubLaunch";
                }
                else
                {
                    lbl_LaunchWays.Content = "Launch - Method: 1.27 SocialClubLaunch";
                }
            }
            else if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.DragonEmu)
            {
                if (DragonEmuGameVersion == "124")
                {
                    lbl_LaunchWays.Content = "Launch - Method: 1.24 Dragon Launcher";
                }
                else
                {
                    lbl_LaunchWays.Content = "Launch - Method: 1.27 Dragon Launcher";
                }
            }
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
            btn_Set_GTAVInstallationPath.ToolTip = Settings.GTAVInstallationPath;
            btn_Set_ZIPExtractionPath.Content = "Components Installation Path: " + Settings.ZIPExtractionPath;
            btn_Set_ZIPExtractionPath.ToolTip = Settings.ZIPExtractionPath;
            btn_Set_PathFPSLimiter.Content = "FPS Limiter Path: " + Settings.PathFPSLimiter;
            btn_Set_PathFPSLimiter.ToolTip = Settings.PathFPSLimiter;
            btn_Set_PathLiveSplit.Content = "LiveSplit Path: " + Settings.PathLiveSplit;
            btn_Set_PathLiveSplit.ToolTip = Settings.PathLiveSplit;
            btn_Set_PathStreamProgram.Content = "Stream Program Path: " + Settings.PathStreamProgram;
            btn_Set_PathStreamProgram.ToolTip = Settings.PathStreamProgram;
            btn_Set_PathNohboard.Content = "Nohboard Path: " + Settings.PathNohboard;
            btn_Set_PathNohboard.ToolTip = Settings.PathNohboard;

            btn_Import_Zip.Content = "Import ZIP Manually (Current Version: " + Globals.ZipVersion + ")";

            combox_Set_Retail.SelectedItem = Settings.Retailer;
            combox_Set_LanguageSelected.SelectedItem = Settings.LanguageSelected;
            combox_Set_ExitWays.SelectedItem = Settings.ExitWay;
            combox_Set_StartWays.SelectedItem = Settings.StartWay;
            combox_Set_SocialClubGameVersion.SelectedItem = Settings.SocialClubLaunchGameVersion;
            combox_Set_DragonEmuGameVersion.SelectedItem = Settings.DragonEmuGameVersion;
            combox_Set_PostMTLAction.SelectedItem = Settings.PostMTLAction;

            tb_Set_InGameName.Text = Settings.InGameName;

            // Needs to be here...
            // Settings.OverWriteGTACommandLineArgs returns command args alg comes up with, if its empty
            // if SCL is selected as launch option, or user is on dr490n-124, this will not come with -StutterFix
            // Appending so if user wants to overwrite gta command line args, user is a aware of that argument
            string tmp = Settings.OverWriteGTACommandLineArgs;
            if (!tmp.ToLower().Contains("-stutterfix")) { tmp += " -StutterFix"; }
            tb_OverWriteGTACommandLineArgs.Text = tmp;

            btn_Set_JumpScriptKey1.Content = Settings.JumpScriptKey1;
            btn_Set_JumpScriptKey2.Content = Settings.JumpScriptKey2;
            btn_Set_Special_Patcher_Key.Content = Settings.SpecialPatcherKey;

            ButtonMouseOverMagic(btn_Refresh);
            ButtonMouseOverMagic(btn_cb_Set_EnableLogging);
            ButtonMouseOverMagic(btn_cb_Set_CopyFilesInsteadOfHardlinking);
            ButtonMouseOverMagic(btn_cb_Set_EnableAlternativeLaunchForceCProgramFiles);
            ButtonMouseOverMagic(btn_cb_Set_EnableJumpscriptUseCustomScript);
            ButtonMouseOverMagic(btn_cb_Set_EnablePreOrderBonus);
            ButtonMouseOverMagic(btn_cb_Set_EnableReturningPlayer);
            ButtonMouseOverMagic(btn_cb_Set_EnableRunAsAdmin);
            ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartFPSLimiter);
            ButtonMouseOverMagic(btn_cb_Set_EnableScripthookOnDowngraded);
            ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartLiveSplit);
            ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartNohboard);
            ButtonMouseOverMagic(btn_cb_Set_EnableOverlay);
            ButtonMouseOverMagic(btn_cb_Set_EnableOverlayMultiMonitor);
            ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartStreamProgram);
            ButtonMouseOverMagic(btn_cb_Set_AutoSetHighPriority);
            ButtonMouseOverMagic(btn_cb_Set_OnlyAutoStartProgramsWhenDowngraded);
            ButtonMouseOverMagic(btn_cb_Set_EnableDontLaunchThroughSteam);
            ButtonMouseOverMagic(btn_cb_Set_EnableAutoStartJumpScript);
            ButtonMouseOverMagic(btn_cb_Set_EnableOverWriteGTACommandLineArgs);
            ButtonMouseOverMagic(btn_cb_Set_SlowCompare);
            ButtonMouseOverMagic(btn_cb_Set_AutoMTLAuthOnStartup);
            ButtonMouseOverMagic(btn_cb_Set_EnableCoreFix);
            ButtonMouseOverMagic(btn_cb_Set_EnableStutterFix);
            ButtonMouseOverMagic(btn_cb_EnableSpecialPatcher);
            ButtonMouseOverMagic(btn_cb_EnablePPTester);


            if (LauncherLogic.AuthWay == LauncherLogic.AuthWays.MTL)
            {
                btn_AuthMethod_LegacyAuth.Style = Application.Current.FindResource("btn_AuthWay") as Style;
                btn_AuthMethod_MTL.Style = Application.Current.FindResource("btn_AuthWay_Enabled") as Style;
                lbl_AuthWays.Content = "Auth - Method: MTL";
            }
            else
            {
                btn_AuthMethod_LegacyAuth.Style = Application.Current.FindResource("btn_AuthWay_Enabled") as Style;
                btn_AuthMethod_MTL.Style = Application.Current.FindResource("btn_AuthWay") as Style;
                lbl_AuthWays.Content = "Auth - Method: Legacy Auth";
            }



            Version myUpgradeVersion = HelperClasses.FileHandling.GetVersionFromFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\gta5.exe");
            Version myBackupVersion = HelperClasses.FileHandling.GetVersionFromFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\gta5.exe");
            btn_CreateBackup.Content = "Create BackupFiles for Upgrading" + BuildVersionTable.GetNiceGameVersionString(myUpgradeVersion);
            btn_UseBackup.Content = "Use BackupFiles for Upgrading" + BuildVersionTable.GetNiceGameVersionString(myBackupVersion);

            Refresh_SCL_EMU_Order();
        }


        /// <summary>
        /// Enum for all ReadMeStates
        /// </summary>
        public enum SettingsStates
        {
            General,
            GTA,
            Extra,
            Patcher
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
                switch (value)
                {
                    case SettingsStates.General:
                        sv_Settings_General.Visibility = Visibility.Visible;
                        sv_Settings_GTA.Visibility = Visibility.Hidden;
                        sv_Settings_Extra.Visibility = Visibility.Hidden;
                        sv_Settings_Secret.Visibility = Visibility.Hidden;

                        btn_SettingsGeneral.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;
                        btn_SettingsGTA.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
                        btn_SettingsExtra.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

                        lbl_SettingsHeader.Content = "General P127 Settings";
                        sv_Settings_General.ScrollToVerticalOffset(0);
                        break;

                    case SettingsStates.GTA:
                        sv_Settings_General.Visibility = Visibility.Hidden;
                        sv_Settings_GTA.Visibility = Visibility.Visible;
                        sv_Settings_Extra.Visibility = Visibility.Hidden;
                        sv_Settings_Secret.Visibility = Visibility.Hidden;

                        btn_SettingsGeneral.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
                        btn_SettingsGTA.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;
                        btn_SettingsExtra.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

                        lbl_SettingsHeader.Content = "GTA & Launch Settings";
                        sv_Settings_GTA.ScrollToVerticalOffset(0);

                        LauncherLogic.ResetReturningPlayerBonusSetting();
                        ButtonMouseOverMagic(btn_cb_Set_EnableReturningPlayer);
                        Refresh_SCL_EMU_Order();
                        break;

                    case SettingsStates.Extra:
                        sv_Settings_General.Visibility = Visibility.Hidden;
                        sv_Settings_GTA.Visibility = Visibility.Hidden;
                        sv_Settings_Extra.Visibility = Visibility.Visible;
                        sv_Settings_Secret.Visibility = Visibility.Hidden;

                        btn_SettingsGeneral.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
                        btn_SettingsGTA.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
                        btn_SettingsExtra.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

                        lbl_SettingsHeader.Content = "Settings of Extra Features";
                        sv_Settings_Extra.ScrollToVerticalOffset(0);
                        break;

                    case SettingsStates.Patcher:
                        sv_Settings_General.Visibility = Visibility.Hidden;
                        sv_Settings_GTA.Visibility = Visibility.Hidden;
                        sv_Settings_Extra.Visibility = Visibility.Hidden;
                        sv_Settings_Secret.Visibility = Visibility.Visible;


                        btn_SettingsGeneral.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
                        btn_SettingsGTA.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
                        btn_SettingsExtra.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
                        lbl_SettingsHeader.Content = "Patcher Settings";
                        sv_Settings_Extra.ScrollToVerticalOffset(0);

                        break;
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
                case "btn_cb_Set_EnableOverWriteGTACommandLineArgs":
                    SetCheckBoxBackground(myBtn, Settings.EnableOverWriteGTACommandLineArgs);
                    break;
                case "btn_cb_Set_EnableCoreFix":
                    SetCheckBoxBackground(myBtn, Settings.EnableCoreFix);
                    break;
                case "btn_cb_Set_EnableStutterFix":
                    SetCheckBoxBackground(myBtn, Settings.EnableStutterFix);
                    break;
                case "btn_cb_Set_EnableAlternativeLaunchForceCProgramFiles":
                    SetCheckBoxBackground(myBtn, Settings.EnableAlternativeLaunchForceCProgramFiles);
                    break;
                case "btn_cb_Set_EnableLogging":
                    SetCheckBoxBackground(myBtn, Settings.EnableLogging);
                    break;
                case "btn_cb_Set_SlowCompare":
                    SetCheckBoxBackground(myBtn, Settings.EnableSlowCompare);
                    break;
                case "btn_cb_Set_CopyFilesInsteadOfHardlinking":
                    SetCheckBoxBackground(myBtn, Settings.EnableCopyFilesInsteadOfHardlinking);
                    break;
                case "btn_cb_Set_EnableJumpscriptUseCustomScript":
                    SetCheckBoxBackground(myBtn, Settings.EnableJumpscriptUseCustomScript);
                    break;
                case "btn_cb_Set_EnablePreOrderBonus":
                    SetCheckBoxBackground(myBtn, Settings.EnablePreOrderBonus);
                    break;
                case "btn_cb_Set_EnableReturningPlayer":
                    SetCheckBoxBackground(myBtn, Settings.EnableReturningPlayer);
                    break;
                case "btn_cb_Set_EnableRunAsAdmin":
                    SetCheckBoxBackground(myBtn, Settings.EnableRunAsAdminDowngraded);
                    break;
                case "btn_cb_Set_AutoSetHighPriority":
                    SetCheckBoxBackground(myBtn, Settings.EnableAutoSetHighPriority);
                    break;
                case "btn_cb_Set_OnlyAutoStartProgramsWhenDowngraded":
                    SetCheckBoxBackground(myBtn, Settings.EnableOnlyAutoStartProgramsWhenDowngraded);
                    break;
                case "btn_cb_Set_AutoMTLAuthOnStartup":
                    SetCheckBoxBackground(myBtn, Settings.AutoMTLAuthOnStartup);
                    break;
                case "btn_cb_Set_EnableScripthookOnDowngraded":
                    SetCheckBoxBackground(myBtn, Settings.EnableScripthookOnDowngraded);
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
                case "btn_cb_EnableSpecialPatcher":
                    SetCheckBoxBackground(myBtn, Settings.SpecialPatcherEnabled);
                    break;
                case "btn_cb_EnablePPTester":
                    SetCheckBoxBackground(myBtn, Settings.PointerPathTesterEnabled);
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


        private void Refresh_SCL_EMU_Order()
        {
            Grid_Settings_GTA.RowDefinitions.RemoveAt(6);
            Grid_Settings_GTA.RowDefinitions.RemoveAt(5);
            RowDefinition Row_SCL_Options = new RowDefinition();
            Row_SCL_Options.Height = new GridLength(100);
            RowDefinition Row_DragonEmu_Options = new RowDefinition();
            Row_DragonEmu_Options.Height = new GridLength(480);

            if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch)
            {
                btn_LaunchWays_SCL.Style = Application.Current.FindResource("btn_LaunchWays_SCL_Enabled") as Style;
                btn_LaunchWays_DragonEmu.Style = Application.Current.FindResource("btn_LaunchWays_DragonEmu") as Style;
                //btn_LaunchWays_Base124.Style = Application.Current.FindResource("btn_LaunchWays_Base124") as Style;
                brdr_LaunchWays.BorderBrush = MyColors.MyColorSCL;
                lbl_LaunchWays.Foreground = MyColors.MyColorSCL;

                Grid_Settings_GTA.RowDefinitions.Add(Row_SCL_Options);
                Grid_Settings_GTA.RowDefinitions.Add(Row_DragonEmu_Options);

                Grid.SetRow(brdr_SCLOptions, 5);
                Grid.SetRow(brdr_DragonEmuOptions, 6);

                btn_HideSCLOptions.Visibility = Visibility.Hidden;
                btn_HideEmuOptions.Visibility = Visibility.Visible;

                Rect_HideOptions_HideFromSteam.Visibility = Visibility.Hidden;
                Rect_HideOptions_StutterFix.Visibility = Visibility.Hidden;
                Rect_HideOptions_AutoMTLOnStartup.Visibility = Visibility.Hidden;
            }
            else if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.DragonEmu)
            {
                btn_LaunchWays_SCL.Style = Application.Current.FindResource("btn_LaunchWays_SCL") as Style;
                btn_LaunchWays_DragonEmu.Style = Application.Current.FindResource("btn_LaunchWays_DragonEmu_Enabled") as Style;
                //btn_LaunchWays_Base124.Style = Application.Current.FindResource("btn_LaunchWays_Base124") as Style;
                brdr_LaunchWays.BorderBrush = MyColors.MyColorEmu;
                lbl_LaunchWays.Foreground = MyColors.MyColorEmu;

                Grid_Settings_GTA.RowDefinitions.Add(Row_DragonEmu_Options);
                Grid_Settings_GTA.RowDefinitions.Add(Row_SCL_Options);

                Grid.SetRow(brdr_DragonEmuOptions, 5);
                Grid.SetRow(brdr_SCLOptions, 6);

                btn_HideSCLOptions.Visibility = Visibility.Visible;
                btn_HideEmuOptions.Visibility = Visibility.Hidden;

                Rect_Background_DragonEmu_1.Visibility = Visibility.Visible;
                Rect_Background_DragonEmu_2.Visibility = Visibility.Visible;
                Rect_Background_DragonEmu_3.Visibility = Visibility.Visible;
                Rect_Background_DragonEmu_4.Visibility = Visibility.Visible;
                Rect_Background_DragonEmu_5.Visibility = Visibility.Visible;
                Rect_Background_SCL_1.Visibility = Visibility.Hidden;
            }
            RefreshLaunchLabel();

            RefreshIfOptionsHide_True();
        }

        private void RefreshIfOptionsHide_True()
        {
            if (Settings.EnableOverlay)
            {
                Rect_HideOption_OverlayMM.Visibility = Visibility.Hidden;
            }
            else
            {
                Rect_HideOption_OverlayMM.Visibility = Visibility.Visible;
            }

            if (Settings.EnableJumpscriptUseCustomScript)
            {
                Rect_HideOptions_JumpscriptKeys.Visibility = Visibility.Visible;
            }
            else
            {
                Rect_HideOptions_JumpscriptKeys.Visibility = Visibility.Hidden;
            }

            if (Settings.EnableOverWriteGTACommandLineArgs)
            {
                Rect_HideOptions_CommandLineArg.Visibility = Visibility.Hidden;
                Rect_HideOptions_AutoCoreFix.Visibility = Visibility.Visible;
                Rect_HideOptions_Language.Visibility = Visibility.Visible;
            }
            else
            {
                Rect_HideOptions_CommandLineArg.Visibility = Visibility.Visible;
                Rect_HideOptions_AutoCoreFix.Visibility = Visibility.Hidden;
                Rect_HideOptions_Language.Visibility = Visibility.Hidden;
            }

            if (Retailer == Retailers.Epic)
            {
                Rect_HideOptions_SCL_Launch.Visibility = Visibility.Visible;
            }
            else
            {
                Rect_HideOptions_SCL_Launch.Visibility = Visibility.Hidden;
            }





            if (btn_HideSCLOptions.Visibility == Visibility.Hidden)
            {
                Rect_Background_SCL_1.Visibility = Visibility.Visible;
            }
            else
            {
                Rect_Background_SCL_1.Visibility = Visibility.Hidden;
            }

            if (btn_HideEmuOptions.Visibility == Visibility.Hidden)
            {
                if (DragonEmuGameVersion == "127")
                {
                    Rect_HideOptions_StutterFix.Visibility = Visibility.Hidden;
                }
                else
                {
                    Rect_HideOptions_StutterFix.Visibility = Visibility.Visible;
                }

                if (Retailer == Retailers.Steam)
                {
                    Rect_HideOptions_HideFromSteam.Visibility = Visibility.Hidden;
                }
                else
                {
                    Rect_HideOptions_HideFromSteam.Visibility = Visibility.Visible;
                }

                if (EnableLegacyAuth)
                {
                    Rect_HideOptions_AutoMTLOnStartup.Visibility = Visibility.Visible;
                }
                else
                {
                    Rect_HideOptions_AutoMTLOnStartup.Visibility = Visibility.Hidden;
                }

                Rect_Background_DragonEmu_1.Visibility = Visibility.Visible;
                Rect_Background_DragonEmu_2.Visibility = Visibility.Visible;
                Rect_Background_DragonEmu_3.Visibility = Visibility.Visible;
                Rect_Background_DragonEmu_4.Visibility = Visibility.Visible;
                Rect_Background_DragonEmu_5.Visibility = Visibility.Visible;
            }
            else
            {
                Rect_HideOptions_StutterFix.Visibility = Visibility.Hidden;
                Rect_HideOptions_HideFromSteam.Visibility = Visibility.Hidden;
                Rect_HideOptions_AutoMTLOnStartup.Visibility = Visibility.Hidden;

                Rect_Background_DragonEmu_1.Visibility = Visibility.Hidden;
                Rect_Background_DragonEmu_2.Visibility = Visibility.Hidden;
                Rect_Background_DragonEmu_3.Visibility = Visibility.Hidden;
                Rect_Background_DragonEmu_4.Visibility = Visibility.Hidden;
                Rect_Background_DragonEmu_5.Visibility = Visibility.Hidden;
            }
        }

        private void btn_LaunchWays_SCL_Click(object sender, RoutedEventArgs e)
        {
            if (LauncherLogic.LaunchWay != LauncherLogic.LaunchWays.SocialClubLaunch)
            {
                LauncherLogic.LaunchWay = LauncherLogic.LaunchWays.SocialClubLaunch;
                RefreshGUI();
            }
        }

        private void btn_LaunchWays_DragonEmu_Click(object sender, RoutedEventArgs e)
        {
            if (LauncherLogic.LaunchWay != LauncherLogic.LaunchWays.DragonEmu)
            {
                LauncherLogic.LaunchWay = LauncherLogic.LaunchWays.DragonEmu;
                RefreshGUI();
            }
        }

        private void btn_AuthWays_MTL_Click(object sender, RoutedEventArgs e)
        {
            LauncherLogic.AuthWay = LauncherLogic.AuthWays.MTL;
            RefreshGUI();
        }

        private void btn_AuthWays_Legacy_Click(object sender, RoutedEventArgs e)
        {
            LauncherLogic.AuthWay = LauncherLogic.AuthWays.LegacyAuth;
            RefreshGUI();
        }

        private void btn_HideSCLOptions_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            btn_HideSCLOptions.Visibility = Visibility.Hidden;

            RefreshIfOptionsHide_True();
        }

        private void btn_HideEmuOptions_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            btn_HideEmuOptions.Visibility = Visibility.Hidden;

            RefreshIfOptionsHide_True();
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
                case "btn_cb_Set_SlowCompare":
                    Settings.EnableSlowCompare = !Settings.EnableSlowCompare;
                    break;
                case "btn_cb_Set_EnableOverWriteGTACommandLineArgs":
                    Settings.EnableOverWriteGTACommandLineArgs = !Settings.EnableOverWriteGTACommandLineArgs;
                    break;
                case "btn_cb_Set_EnableScripthookOnDowngraded":
                    Settings.EnableScripthookOnDowngraded = !Settings.EnableScripthookOnDowngraded;
                    break;
                case "btn_cb_Set_CopyFilesInsteadOfHardlinking":
                    Settings.EnableCopyFilesInsteadOfHardlinking = !Settings.EnableCopyFilesInsteadOfHardlinking;
                    SetDefaultEnableCopyingHardlinking();
                    break;
                case "btn_cb_Set_EnableJumpscriptUseCustomScript":
                    Settings.EnableJumpscriptUseCustomScript = !Settings.EnableJumpscriptUseCustomScript;
                    break;
                case "btn_cb_Set_AutoMTLAuthOnStartup":
                    Settings.AutoMTLAuthOnStartup = !Settings.AutoMTLAuthOnStartup;
                    break;
                case "btn_cb_Set_EnableAlternativeLaunchForceCProgramFiles":
                    Settings.EnableAlternativeLaunchForceCProgramFiles = !Settings.EnableAlternativeLaunchForceCProgramFiles;
                    break;
                case "btn_cb_Set_EnablePreOrderBonus":
                    Settings.EnablePreOrderBonus = !Settings.EnablePreOrderBonus;
                    break;
                case "btn_cb_Set_EnableReturningPlayer":
                    bool prevSetting = Settings.EnableReturningPlayer;
                    Settings.EnableReturningPlayer = !Settings.EnableReturningPlayer;
                    LauncherLogic.ResetReturningPlayerBonusSetting();
                    if (prevSetting == Settings.EnableReturningPlayer)
                    {
                        PopupWrapper.PopupOk("Cant change ReturningPlayerBonus Setting.\n\nThis is most likely due to non existing pc_settings.bin\nStart the game once to fix.");
                    }
                    break;
                case "btn_cb_Set_EnableRunAsAdmin":
                    Settings.EnableRunAsAdminDowngraded = !Settings.EnableRunAsAdminDowngraded;
                    break;
                case "btn_cb_Set_EnableCoreFix":
                    Settings.EnableCoreFix = !Settings.EnableCoreFix;
                    break;
                case "btn_cb_Set_EnableStutterFix":
                    Settings.EnableStutterFix = !Settings.EnableStutterFix;
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
                    break;
                case "btn_cb_Set_EnableOverlayMultiMonitor":
                    Settings.OverlayMultiMonitorMode = !Settings.OverlayMultiMonitorMode;
                    break;
                case "btn_cb_Set_EnableAutoStartJumpScript":
                    Settings.EnableAutoStartJumpScript = !Settings.EnableAutoStartJumpScript;
                    break;
                case "btn_cb_EnableSpecialPatcher":
                    Settings.SpecialPatcherEnabled = !Settings.SpecialPatcherEnabled;
                    break;
                case "btn_cb_EnablePPTester":
                    Settings.PointerPathTesterEnabled = !Settings.PointerPathTesterEnabled;
                    break;
            }
            ButtonMouseOverMagic(myBtn);
            RefreshIfOptionsHide_True();
        }

        /// <summary>
        /// Checking for Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            CheckForUpdateClick();
        }

        public static async Task CheckForUpdateClick(bool ThrowPopupAtEnd = true)
        {
            //Globals.CheckForBigThree();
            //Globals.CheckForZipUpdate();

            Task a = Globals.CheckForUpdate();
            Task b = Auth.DynamicMTLOffsets.SetUpMTLOffsets();
            Task c = Globals.SetUpDownloadManager();

            await Task.WhenAll(a, b, c).ConfigureAwait(continueOnCapturedContext: false);

            if (ThrowPopupAtEnd)
            {
                PopupWrapper.PopupOk("Update check complete. All updates handled.\nIf you want to re-download certain files,\nyou can do so in the 'Component Manager'");
            }
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
            bool yesno = PopupWrapper.PopupYesNo("This resets the whole Project 1.27 Client including:\nSettings\nInstalled Components\nBacked up GTA Files to upgrade when downgraded\nYOU NEED TO repair / verify GTA V through Steam / Epic / Rockstar AFTER THIS\n\nYou will be asked if you want to un-install P127 after this.");
            if (yesno == true)
            {
                PopupWrapper.PopupOk("This might take a while, just hit \"OK\" and wait a bit.\nProject 1.27 will Exit once its done.");

                HelperClasses.Logger.Log("Initiating a full Reset:");

                HelperClasses.Logger.Log("Making an Upgrade.");
                LauncherLogic.Upgrade();
                LaunchAlternative.SocialClubReset();
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

                HelperClasses.Logger.Log("Deleting all MTLOffsets.xml");
                HelperClasses.FileHandling.deleteFile(Globals.MTLOffsetsLocalFilepath);
                HelperClasses.Logger.Log("Done deleting MTLOffsets.xml.", 1);

                bool yesno2 = PopupWrapper.PopupYesNo("Do you want to uninstall Project 1.27?");
                if (yesno2 == true)
                {
                    HelperClasses.Logger.Log("User wants to uninstall...Gonna close this now I guess...cya");
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
                    HelperClasses.Logger.Log("User wants NO uninstall...Gonna close this now I guess...cya");
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
            RepairGTA_UserInteraction();
            MainWindow.MW.UpdateGUIDispatcherTimer();
        }


        public static void RepairGTA_UserInteraction()
        {
            if (!LauncherLogic.IsGTAVInstallationPathCorrect() && !LauncherLogic.GTAVInstallationIncorrectMessageThrownAlready)
            {
                HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");

                bool yesno2 = PopupWrapper.PopupYesNo("Error:\nGTA V Installation Path is not a valid Path.\nProceed?");
                if (yesno2 == true)
                {
                    HelperClasses.Logger.Log("User wants to force this Repair. Will not throw the WrongGTAVPathError again on this P127 instance.");
                    LauncherLogic.GTAVInstallationIncorrectMessageThrownAlready = true;
                }
                else
                {
                    HelperClasses.Logger.Log("User does not want to force this Repair. Will abondon.");
                    return;
                }
            }


            if (HelperClasses.BuildVersionTable.IsUpgradedGTA(LauncherLogic.UpgradeFilePathBackup))
            {
                string msg3 = "Found Backup for UpgradeFiles.\nDo you want to try that instead of Repairing?";
                bool yesno2 = PopupWrapper.PopupYesNo(msg3);
                if (yesno2 == true)
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
            msg2 += "\ndeep Repair:\n";
            msg2 += "You need to repair / verify your GTA via your Retailer";
            msg2 += "\n(Steam, Rockstar, Epic) AFTER this.";
            msg2 += "\nThis really removes all P127 Files and Traces\nfrom your GTA V Installation";
            //quick repair is true
            Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNoRepair, msg2);
            yesno.ShowDialog();
            if (yesno.DialogResult == true)
            {
                if (yesno.RtrnRepairMode == "DEEP")
                {
                    LauncherLogic.Repair();
                }
                else
                {
                    LauncherLogic.Repair(true);
                }
            }
        }

        /// <summary>
        /// Create Backup Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CreateBackup_Click(object sender, RoutedEventArgs e)
        {
            LauncherLogic.CreateBackup();
            RefreshGUI();
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
            if (e.ClickCount >= 3)
            {
                if (SettingsState == SettingsStates.General)
                {
                    PopupWrapper.PopupMode();
                }
                else if (SettingsState == SettingsStates.GTA)
                {
                    btn_SettingsGTA_MouseRightButtonDown(null, null);
                }
                else if (SettingsState == SettingsStates.Extra)
                {
                    SettingsState = SettingsStates.Patcher;
                }
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
            combox_Set_SocialClubGameVersion.SelectedItem = Settings.SocialClubLaunchGameVersion;
            RefreshLaunchLabel();
            RefreshIfOptionsHide_True();
        }

        /// <summary>
        /// Selection Changed of Version ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combox_Set_DragonEmuGameVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.DragonEmuGameVersion = combox_Set_DragonEmuGameVersion.SelectedItem.ToString();
            combox_Set_DragonEmuGameVersion.SelectedItem = Settings.DragonEmuGameVersion;
            RefreshLaunchLabel();
            RefreshIfOptionsHide_True();
        }


        private void combox_Set_PostMTLAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.PostMTLAction = (PostMTLActions)System.Enum.Parse(typeof(PostMTLActions), combox_Set_PostMTLAction.SelectedItem.ToString());
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
        /// Create Backup Rightclick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CreateBackup_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupCreateBackup.IsOpen = true;
        }


        /// <summary>
        /// Method gets called when Popup of CreateBackup is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopupCreateBackup_Opened(object sender, EventArgs e)
        {
            tb_Set_BackupName.Text = "BackupName";

            Version myUpgradeVersion = HelperClasses.FileHandling.GetVersionFromFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\gta5.exe");
            lbl_CreateBackupInPopup.Content = "Create Backup" + BuildVersionTable.GetNiceGameVersionString(myUpgradeVersion);
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
                    bool yesno = PopupWrapper.PopupYesNo("Backup with that name ('" + tb_Set_BackupName.Text + "') already exists.\nDo you want to delete it?");
                    if (yesno == true)
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
                    RefreshGUI();
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
            PopupJumpscriptAdditional.IsOpen = false;
            PopupGTACommandLineArgs.IsOpen = false;
            RefreshGUI();
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

                    Version myGameVersion = HelperClasses.FileHandling.GetVersionFromFile(MyFolder.TrimEnd('\\') + @"\gta5.exe");

                    combox_UseBackup.Items.Add(NiceName + BuildVersionTable.GetNiceGameVersionString(myGameVersion));
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

            string tmp = combox_UseBackup.SelectedItem.ToString();
            string Name = tmp.Substring(0, tmp.LastIndexOf('('));
            string Path = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles_Backup_" + Name;

            if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(Path).Length >= 2))
            {
                string prevName = btn_UseBackupName.Content.ToString();
                btn_UseBackupName.Content = "No Files in that Folder";
                await Task.Delay(750);
                btn_UseBackupName.Content = prevName;
                return;
            }

            LauncherLogic.UseBackup(Name);
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
            Overlay.Overlay_MultipleMonitor.ResetPosition();
        }

        /// <summary>
        /// Rightclick on General Subpage Button. MOde popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SettingsGeneral_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 3)
            {
                PopupWrapper.PopupMode();
            }
        }

        /// <summary>
        /// Check for Update rightclick ( getting specific build)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CheckForUpdate_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            string tb = PopupWrapper.PopupTextbox("Build Name:", "MyBuildName");
            // Check if we can Download the build from the branch we are currently on.

            if (HelperClasses.FileHandling.URLExists(tb))
            {
                HelperClasses.Logger.Log("Importing Build. Links: ");
                HelperClasses.Logger.Log("DDL: " + tb);
                Globals.ImportBuildFromUrl(tb);
            }
            else if (tb != "" && tb.ToLower() != "cancel")
            {
                string DLLinkBranch = "https://github.com/TwosHusbandS/Project-127/raw/" + Globals.P127Branch + "/Installer/Builds/" + tb.TrimEnd(".exe") + ".exe";
                string DLLinkMaster = "https://github.com/TwosHusbandS/Project-127/raw/Master" + "/Installer/Builds/" + tb.TrimEnd(".exe") + ".exe";
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

            PopupWrapper.PopupOk("Cant find that build online.");
        }


        private void btn_SettingsGTA_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (LauncherLogic.GameState == LauncherLogic.GameStates.NonRunning)
            {
                ROSCommunicationBackend.setFlag(ROSCommunicationBackend.Flags.indicateTheLessThanLegalProcurementOfMotorVehicles, true);
                Overlay.GTAOverlay.indicateTheLessThanLegalProcurementOfMotorVehicles = true;
                Settings.GTAWindowTitle = Overlay.GTAOverlay.targetWindowBorderlessEasterEgg;
                PopupWrapper.PopupOk("'" + Overlay.GTAOverlay.targetWindowBorderlessEasterEgg + "' activated.\nRestart P127 to disable.");
            }
            else
            {
                PopupWrapper.PopupOk("Cant do that while the game is running or Stuck.");
            }
        }

        private void btn_Jumpscript_Additional_Click(object sender, RoutedEventArgs e)
        {
            PopupJumpscriptAdditional.IsOpen = true;
        }

        private void btn_Import_AHK_Click(object sender, RoutedEventArgs e)
        {
            string AHKFileLocation = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Import AHK Jumpscript", Globals.ProjectInstallationPath, pFilter: "AHK Files|*.ahk*");
            if (HelperClasses.FileHandling.doesFileExist(AHKFileLocation))
            {
                string customPath = Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript_Custom.ahk";
                HelperClasses.FileHandling.copyFile(AHKFileLocation, customPath);
                Settings.EnableJumpscriptUseCustomScript = true;
                RefreshGUI();
            }
            else
            {
                PopupWrapper.PopupOk("No AHK File selected.");
            }
        }

        private void PopupJumpscriptAdditional_Opened(object sender, EventArgs e)
        {

        }

        private void PopupJumpscriptAdditional_Closed(object sender, EventArgs e)
        {

        }

        private void PopupEnableAlternativeLaunch_Closed(object sender, EventArgs e)
        {

        }

        private void PopupEnableAlternativeLaunch_Opened(object sender, EventArgs e)
        {

        }

        static bool RockstarDisableAutoUpdateThrownAlready = false;

        public static void TellRockstarUsersToDisableAutoUpdateIfNeeded()
        {
            if (Settings.Retailer == Retailers.Rockstar && LauncherLogic.AuthWay == LauncherLogic.AuthWays.MTL && LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.DragonEmu && !RockstarDisableAutoUpdateThrownAlready)
            {
                string msg = "You need to stop Rockstar Game Launcher\nfrom automatically Updating your GTA.\nOtherwise certain features might not work.\n\nTo do this:\nInside Rockstar Games Launcher,\nhead into Settings\n-> My Installed Games\n->Grand Theft Auto V\n-> uncheck the \"Enable automatic updates\" checkbox at the very top.";
                PopupWrapper.PopupOk(msg);
                RockstarDisableAutoUpdateThrownAlready = true;
            }
        }

        private void btn_AntivirusFix_Click(object sender, RoutedEventArgs e)
        {
            AntiVirusFix();
        }

        public static void AntiVirusFix()
        {
            string msg = "Exclude P127 Folders from Windows Anti Virus?\n\nSo Windows automatically deleting our Files got annoying really quick...\n P127 can automatically add an Exclusion of the following Folders:\n";
            msg += "\n- GTA Installation Directory\n- Project 1.27 Installation Directory\n- Project 1.27 Component Download Location\n";
            msg += "\nto the Windows Defender.\nWindows defender will STILL BE ACTIVE,\nbut it will not scan for Viruses in those folders.\n\nDo you want me to do that?";

            bool yesno = PopupWrapper.PopupYesNo(msg);
            if (yesno == true)
            {
                HelperClasses.Logger.Log("User wants the AntiVirus Fix.");

                // Done intentionally like this, so user is kinda aware that something is happening
                string command = "";
                command += "Add-MpPreference -ExclusionPath '" + Globals.ProjectInstallationPath + "'";
                command += "; Add-MpPreference -ExclusionPath '" + LauncherLogic.GTAVFilePath + "'";
                command += "; Add-MpPreference -ExclusionPath '" + LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\" + "'";

                string powershell = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
                string powershell2 = @"C:\Windows\System32\WindowsPowerShell\v2.0\powershell.exe";

                if (HelperClasses.FileHandling.doesFileExist(powershell))
                {
                    HelperClasses.ProcessHandler.StartProcess(powershell, "", command, true, true);
                    HelperClasses.Logger.Log("User should have the AntiVirus Fix.");
                }
                else if (HelperClasses.FileHandling.doesFileExist(powershell2))
                {
                    HelperClasses.ProcessHandler.StartProcess(powershell2, "", command, true, true);
                    HelperClasses.Logger.Log("User should have the AntiVirus Fix.");
                }
                else
                {
                    HelperClasses.Logger.Log("Cant find Powershell.exe.");
                }
            }
            else
            {
                HelperClasses.Logger.Log("User does not want the AntiVirus Fix.");
            }
        }


        private void btn_GTACommandLineArgs_Click(object sender, RoutedEventArgs e)
        {
            PopupGTACommandLineArgs.IsOpen = true;
        }

        private void PopupGTACommandLineArgs_Closed(object sender, EventArgs e)
        {

        }

        private void PopupGTACommandLineArgs_Opened(object sender, EventArgs e)
        {

        }

        private void tb_OverWriteGTACommandLineArgs_TextChanged(object sender, KeyEventArgs e)
        {
            tb_OverWriteGTACommandLineArgs_LostFocus(null, null);
        }

        private void tb_OverWriteGTACommandLineArgs_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb_OverWriteGTACommandLineArgs_LostFocus(null, null);
        }

        private void tb_OverWriteGTACommandLineArgs_LostFocus(object sender, RoutedEventArgs e)
        {
            string txt = tb_OverWriteGTACommandLineArgs.Text;
            if (String.IsNullOrWhiteSpace(txt)) { txt = LauncherLogic.GetStartCommandLineArgs(); }
            Settings.OverWriteGTACommandLineArgs = txt;
            tb_OverWriteGTACommandLineArgs.Text = txt;
        }

        private async void btn_Set_Special_Patcher_Key_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Content = "[Press new Key]";
            System.Windows.Forms.Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
            if (MyNewKey != System.Windows.Forms.Keys.None && MyNewKey != System.Windows.Forms.Keys.Escape)
            {
                Settings.SpecialPatcherKey = MyNewKey;
            }
            ((Button)sender).Content = Settings.SpecialPatcherKey;
        }

        private void btn_open_pptest_dialog_Click(object sender, RoutedEventArgs e)
        {
            PopupPPTesterSetup.IsOpen = true;
        }

        private void btn_open_patch_dialog_Click(object sender, RoutedEventArgs e)
        {
            PopupWrapper.PopupSpecialPatcher();
        }

        private void PopupPPTesterSetup_Opened(object sender, EventArgs e)
        {
            tb_pptestervars.Text = Settings.PointerPathTesterString;
        }

        private void PopupPPTesterSetup_Closed(object sender, EventArgs e)
        {
            Settings.PointerPathTesterString = tb_pptestervars.Text;
            Globals.preparsedPPs = ASPointerPath.pointerPathParse(Settings.PointerPathTesterString);
        }

        private void btn_SettingsExtra_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 3)
            {
                SettingsState = SettingsStates.Patcher;
            }
        }

        private void btn_OpenPatcher_Click(object sender, RoutedEventArgs e)
        {
            SettingsState = SettingsStates.Patcher;
        }

        private void btn_Set_Special_Patcher_Add_llfix_Click(object sender, RoutedEventArgs e)
        {
            string llpatch_json;
            Uri llpatch_json_uri = new Uri(@"HelperClasses\llpatch.json", UriKind.Relative);
            var llpatch_stream = System.Windows.Application.GetResourceStream(llpatch_json_uri);
            using (var reader = new System.IO.StreamReader(llpatch_stream.Stream))
            {
                llpatch_json = reader.ReadToEnd();
            }
            bool do_continute = PopupWrapper.PopupYesNo("Warning: Enabling the long load patch will invalidate your run and\n delete all of you existing patches. Would you like to continue?");
            if (do_continute)
            {
                SpecialPatchHandler.disableAll();
                SpecialPatchHandler.patch.clearCache();
                SpecialPatcherPatches = llpatch_json;
                SpecialPatchHandler.patch.reloadObservable();
                foreach (var p in SpecialPatchHandler.patch.GetPatches())
                {
                    if (p.DefaultEnabled)
                    {
                        p.Enabled = true;
                    }
                }
            }

        }

        /// <summary>
        /// Debloating GTA V
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DebloatV_Click(object sender, RoutedEventArgs e)
        {
            if (LauncherLogic.InstallationState != LauncherLogic.InstallationStates.Downgraded)
            {
                PopupWrapper.PopupOk("You need to be downgraded to do that.");
                return;
            }


            bool yesno = PopupWrapper.PopupYesNo("This will remove all DLCs (online and later GTA Versions),\nwhich are not needed for GTA Version 1.27.\nThis will save a few GB of storage.\n\nDo you want to continue?");
            if (yesno == true)
            {
                bool yesno2 = PopupWrapper.PopupYesNo("If you continue, you will save a bit of storage space\nbut you will need to verify gamefiles via your Retailer\n(Steam, Rockstar or EpicGames)\nif you want to update GTA or play online.\n\nAre you REALLY sure you want to continue?");
                if (yesno2 == true)
                {
                    HelperClasses.Logger.Log("Called DebloatV. Gonna QUICK Repair now. Only need to yeet upgradefiles and upgradebackup files to force people to verify gamefiles. Not allfileseverplaced.");
                    // rockstar processes are killed
                    LauncherLogic.Repair(true, true);


                    List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

                    // https://community.pcgamingwiki.com/topic/4837-gta-5-150-downgraded-to-127-to-save-on-size-which-files-are-safe-to-delete/#comment-14299
                    List<string> FoldersToKeep = new List<string>();
                    FoldersToKeep.Add("mpchristmas2");
                    FoldersToKeep.Add("mpheist");
                    FoldersToKeep.Add("mpluxe");
                    FoldersToKeep.Add("mppatchesng");
                    FoldersToKeep.Add("patchday1ng");
                    FoldersToKeep.Add("patchday2bng");
                    FoldersToKeep.Add("patchday2ng");
                    FoldersToKeep.Add("patchday3ng");

                    // as per specials request
                    string update2rpf_path = LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\update\update2.rpf";
                    MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, update2rpf_path, "", "Deleting '" + update2rpf_path + "'", 2, MyFileOperation.FileOrFolder.File));

                    HelperClasses.Logger.Log("DebloatV. Quick repair done. Deleting folders now.");

                    string[] Folders = HelperClasses.FileHandling.GetSubFolders(LauncherLogic.DebloatVPath);
                    foreach (string Folder in Folders)
                    {
                        string tmp = Folder.Substring(Folder.LastIndexOf('\\') + 1);
                        if (!FoldersToKeep.Contains(tmp))
                        {
                            MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, Folder, "", "Deleting '" + (tmp) + @"' from the $GTAVPath\update\x64\dlcpacks", 2, MyFileOperation.FileOrFolder.Folder));
                        }
                    }

                    PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing DebloatV", MyFileOperations);

                    HelperClasses.Logger.Log("DebloatV. Done.");


                    PopupWrapper.PopupOk("Successfully ran DebloatV");
                }
            }
        }


    } // End of Class
} // End of Namespace 
