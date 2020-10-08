using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Project_127
{
	/// <summary>
	/// Class for Global / Central Place
	/// </summary>
	public static class Globals
	{
		/// <summary>
		/// Property of our own Installation Path
		/// </summary>
		public static string ProjectInstallationPath { get { return Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\')); } }

		/// <summary>
		/// Property of our ProjectName (for Folders, Regedit, etc.)
		/// </summary>
		public static string ProjectName = "Project_127";

		/// <summary>
		/// Property of our ProjectNiceName (for GUI)
		/// </summary>
		public static string ProjectNiceName = "Project 127";

		/// <summary>
		/// Property of the ZIP Version currently installed
		/// </summary>
		public static int ZipVersion
		{
			get
			{
				int _ZipVersion = 0;
				Int32.TryParse(HelperClasses.FileHandling.ReadContentOfFile(LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\Version.txt"), out _ZipVersion);
				return _ZipVersion;
			}
		}

		/// <summary>
		/// Property of our own Project Version
		/// </summary>
		public static Version ProjectVersion = Assembly.GetExecutingAssembly().GetName().Version;

		/// <summary>
		/// URL for AutoUpdaterFile
		/// </summary>
		public static string URL_AutoUpdate
		{
			get
			{
				if (InternalMode)
				{
					return "https://raw.githubusercontent.com/TwosHusbandS/Project-127/internal/Installer/Update.xml";
				}
				else
				{
					return "https://raw.githubusercontent.com/TwosHusbandS/Project-127/master/Installer/Update.xml";
				}
			}
		}

		/// <summary>
		/// Download Location of Zip File
		/// </summary>
		public static string ZipFileDownloadLocation = Globals.ProjectInstallationPath + @"\NewZipFile.zip";

		/// <summary>
		/// Property if we are in Beta
		/// </summary>
		public static bool InternalMode
		{
			get
			{
				if (HelperClasses.FileHandling.doesFileExist(Settings.InstallationPath.TrimEnd('\\') + @"\internal.txt")) { return true; }
				return false;
			}
		}

		/// <summary>
		/// Property if we are in Beta
		/// </summary>
		public static bool BetaMode = true;

		/// <summary>
		/// Property of other Buildinfo. Will be in the top message of logs
		/// </summary>
		public static string BuildInfo = "You should not see this at all...ever";

		/// <summary>
		/// Returns all Command Line Args as StringArray
		/// </summary>
		/// <returns></returns>
		public static string[] CommandLineArgs { get { return Environment.GetCommandLineArgs(); } }

		/// <summary>
		/// String of Steam Install Path
		/// </summary>
		public static string SteamInstallPath
		{
			get
			{
				RegistryKey myRK = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("WOW6432Node").CreateSubKey("Valve").CreateSubKey("Steam");
				return HelperClasses.RegeditHandler.GetValue(myRK, "InstallPath");
			}
		}

		/// <summary>
		/// Property of the Dispatcher Timer we use to keep track of GameState
		/// </summary>
		public static DispatcherTimer MyDispatcherTimer;


		/// <summary>
		/// Property of LogFile Location. Will always be in in the same folder as the executable, since we want to start logging before inititng regedit and loading settings
		/// </summary>
		public static string Logfile { get; private set; } = ProjectInstallationPath.TrimEnd('\\') + @"\AAA - Logfile.log";

		/// <summary>
		/// Property of the Registry Key we use for our Settings
		/// </summary>													
		public static RegistryKey MySettingsKey { get { return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey(ProjectName); } }

		/// <summary>
		/// Property of our default Settings
		/// </summary>
		public static Dictionary<string, string> MyDefaultSettings { get; private set; } = new Dictionary<string, string>()
		{
			/*
			Previously Used Settings Variables, we cannot use anymore, since they are written,
			and we are not able to reset only them (for now...):
				- "FileFolder"
			*/

			{"FirstLaunch", "True" },
			{"LastLaunchedVersion", "0.0.0.1" },
			{"InstallationPath", Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\')) },
			{"GTAVInstallationPath", ""},
			{"ZIPExtractionPath", Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\')) },
			{"EnableLogging", "True"},
			{"EnableCopyFilesInsteadOfHardlinking", "False"},
			{"EnablePreOrderBonus", "False"},
			{"EnableOnlyAutoStartProgramsWhenDowngraded", "True"},
			{"Retailer", "Steam"},
			{"LanguageSelected", "English"},
			{"EnableDontLaunchThroughSteam", "false"},
			{"InGameName", "HiMomImOnYoutube"},
			{"EnableAutoSetHighPriority", "True" },
			{"EnableAutoSteamCoreFix", "True" },
			{"EnableAutoStartLiveSplit", "True" },
			{"PathLiveSplit", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartStreamProgram", "True" },
			{"PathStreamProgram", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartFPSLimiter", "True" },
			{"PathFPSLimiter", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartJumpScript", "True" },
			{"JumpScriptKey1", "A" },
			{"JumpScriptKey2", "A" },
			{"EnableAutoStartNohboard", "True" },
			{"EnableNohboardBurhac", "True" },
			{"PathNohboard", @"C:\Some\Path\SomeFile.exe" },
			{"Theme", @"Empty" },
			{"EnableRememberMe", "False" }
		};

		/// <summary>
		/// Property of our Settings (Dictionary). Gets the default values on initiating the program. Our Settings will get read from registry on the Init functions.
		/// </summary>
		public static Dictionary<string, string> MySettings { get; private set; } = MyDefaultSettings.ToDictionary(entry => entry.Key, entry => entry.Value); // https://stackoverflow.com/a/139626

		/// <summary>
		/// Init function which gets called at the very beginning
		/// </summary>
		public static void Init(MainWindow pMW)
		{
			// Initiates Logging
			// This is also responsible for the intial first few messages on startup.
			HelperClasses.Logger.Init();

			// Initiates the Settings
			// Writes Settings Dictionary [copy of default settings at this point] in the registry if the value doesnt already exist
			// then reads the Regedit Values in the Settings Dictionary
			Settings.Init();

			// Checks if we are doing first Launch.
			if (Settings.FirstLaunch)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk,
				"This software is unfinished, there may be bugs and we are in no way guaranteeing\n" +
				"that this does not break your PC or GTA V Installation.\n" +
				"The UI too, is still very unfinished, and not all planned features are implemented at this point.\n" +
				"We thought it would be a better choice to release this now, even with the potential Issues,\n" +
				"unimplemented features, and ugly UI, so that people can actually play.\n" +
				"An update will be pushed as soon as possible to provide more features,\n" +
				"a more stable client, and make it not look like a \"hányadék\".\n\n" +
				" - The Project 1.27 Team").ShowDialog();

				// Set Own Installation Path in Regedit Settings
				HelperClasses.Logger.Log("FirstLaunch Procedure Started");
				HelperClasses.Logger.Log("Setting Installation Path to '" + ProjectInstallationPath + "'", 1);
				Settings.SetSetting("InstallationPath", ProjectInstallationPath);

				// Calling this to get the Path automatically
				Settings.InitImportantSettings();

				// Set FirstLaunch to false
				Settings.FirstLaunch = false;

				HelperClasses.Logger.Log("FirstLaunch Procedure Ended");
			}


			// Just checks if the GTAVInstallationPath is empty.
			// So we dont have to "Force" the path every startup...
			if (String.IsNullOrEmpty(Settings.GTAVInstallationPath) || String.IsNullOrEmpty(Settings.ZIPExtractionPath))
			{
				// Calling this to get the Path automatically
				Settings.InitImportantSettings();
			}

			// Writing ProjectInstallationPath to Registry.
			Settings.InstallationPath = Globals.ProjectInstallationPath;

			// Last Launched Version Cleanup
			if (Settings.LastLaunchedVersion < Globals.ProjectVersion)
			{
				// Do things we want to do
				Version GiveWarningMessageVersion = new Version("0.0.3.1");

				if (Settings.LastLaunchedVersion < new Version("0.0.3.1"))
				{
					new Popup(Popup.PopupWindowTypes.PopupOk,
					"Project 1.27 is finally in OPEN beta\n" +
					"The published Product is still very much unfinished,\n" +
					"and we very much rely on User Feedback to improve things.\n" +
					"Please do not hesitate to contact us with ANYTHING.\n\n" +
					"Once again:\n" +
					"No gurantees that this will not break your GTAV in any way, shape or form.\n" +
					" - The Project 1.27 Team").ShowDialog();
				}

				if (Settings.LastLaunchedVersion < new Version("0.0.4.0"))
				{
					new Popup(Popup.PopupWindowTypes.PopupOk,
					"The 'Remember' Me function, is storing credentials\n" +
					"using the Windows Credential Manager.\n" +
					"You are using the it on your own risk.\n\n" +
					" - The Project 1.27 Team").ShowDialog();
				}

				Settings.LastLaunchedVersion = Globals.ProjectVersion;
			}

			// Starting the Dispatcher Timer for the automatic updates of the GTA V Button
			MyDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			MyDispatcherTimer.Tick += new EventHandler(pMW.UpdateGUIDispatcherTimer);
			MyDispatcherTimer.Interval = TimeSpan.FromMilliseconds(5000);
			MyDispatcherTimer.Start();
			pMW.UpdateGUIDispatcherTimer();
		}



		/// <summary>
		/// Proper Exit Method. EMPTY FOR NOW. Get called when closed (user and taskmgr) and when PC is shutdown. Not when process is killed or power ist lost.
		/// </summary>
		public static void ProperExit()
		{
			HelperClasses.Logger.Log("Program closed. Proper Exit. Ended normally");
		}




		/// <summary>
		/// DebugPopup Method. Just opens Messagebox with pMsg
		/// </summary>
		/// <param name="pMsg"></param>
		public static void DebugPopup(string pMsg)
		{
			System.Windows.Forms.MessageBox.Show(pMsg);
		}


		/// <summary>
		/// Enum for potential Loaded Pages
		/// </summary>
		public enum PageStates
		{
			Settings,
			SaveFileHandler,
			Auth,
			ReadMe,
			GTA
		}

		/// <summary>
		/// Internal Value for PageState
		/// </summary>
		private static PageStates _PageState = PageStates.GTA;


		/// <summary>
		/// Value we use for PageState
		/// </summary>
		public static PageStates PageState
		{
			get
			{
				return _PageState;
			}
			set
			{
				// Setting actual Enum to the correct Value
				_PageState = value;
				MainWindow.MW.SetControlBackground(MainWindow.MW, GetBackGroundPath());

				// Switch Value
				switch (value)
				{
					// In Case: Settings
					case PageStates.Settings:

						// Set actual Frame_Main Content to the correct Page
						MainWindow.MW.Frame_Main.Content = new Settings();

						// Call Mouse_Over false on other Buttons where a page is behind
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Auth, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_SaveFiles, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_ReadMe, false);
						break;
					case PageStates.SaveFileHandler:
						MainWindow.MW.Frame_Main.Content = new SaveFileHandler();

						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Auth, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Settings, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_ReadMe, false);
						break;
					case PageStates.ReadMe:
						MainWindow.MW.Frame_Main.Content = new ReadMe();

						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Auth, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Settings, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_SaveFiles, false);
						break;
					case PageStates.Auth:
						MainWindow.MW.Frame_Main.Content = new ROSIntegration();

						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_ReadMe, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Settings, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_SaveFiles, false);
						break;
					case PageStates.GTA:
						MainWindow.MW.Frame_Main.Content = new GTA_Page();

						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Settings, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_SaveFiles, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Auth, false);
						MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_ReadMe, false);
						break;
				}
			}
		}


		public enum BackgroundImages
		{
			Main,
			FourTwenty,
			XMas,
			Spooky
		}

		private static BackgroundImages _BackgroundImage = BackgroundImages.Main;

		public static BackgroundImages BackgroundImage
		{
			get
			{
				return _BackgroundImage;
			}
			set
			{
				_BackgroundImage = value;
				MainWindow.MW.SetControlBackground(MainWindow.MW, GetBackGroundPath());
			}
		}

		public enum HamburgerMenuStates
		{
			Visible,
			Hidden
		}

		private static HamburgerMenuStates _HamburgerMenuState = HamburgerMenuStates.Hidden;

		public static HamburgerMenuStates HamburgerMenuState
		{
			get
			{
				return _HamburgerMenuState;
			}
			set
			{
				_HamburgerMenuState = value;
				MainWindow.MW.SetControlBackground(MainWindow.MW, GetBackGroundPath());

				if (value == HamburgerMenuStates.Visible)
				{
					// Make invisible
					MainWindow.MW.GridHamburgerOuter.Visibility = Visibility.Visible;
					MainWindow.MW.GridHamburgerOuterSeperator.Visibility = Visibility.Visible;
				}
				// If is not visible
				else
				{
					// Make visible
					MainWindow.MW.GridHamburgerOuter.Visibility = Visibility.Hidden;
					MainWindow.MW.GridHamburgerOuterSeperator.Visibility = Visibility.Hidden;
					PageState = PageStates.GTA;
				}
			}
		}
		

		public static string GetBackGroundPath()
		{
			string URL_Path = @"Artwork\bg_";

			switch (BackgroundImage)
			{
				case BackgroundImages.Main:
					URL_Path += "main";
					break;
				case BackgroundImages.FourTwenty:
					URL_Path += "420";
					break;
				case BackgroundImages.XMas:
					URL_Path += "xmas";
					break;
				case BackgroundImages.Spooky:
					URL_Path += "spooky";
					break;
			}

			if (HamburgerMenuState == HamburgerMenuStates.Hidden)
			{
				URL_Path += ".png";
			}
			else if (HamburgerMenuState == HamburgerMenuStates.Visible)
			{
				if (PageState == PageStates.GTA)
				{
					URL_Path += "_hb.png";
				}
				else
				{
					URL_Path += "_blur.png";
				}
			}
			// HamburgerMenuState and also PageState

			return URL_Path;
		}

		/// COLOR STUFF

		/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Actual Main colors we use:
		/// </summary>
		public static Brush MyColorWhite { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
		public static Brush MyColorOffWhite { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#c1ced1");
		public static Brush MyColorBlack { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#000000");
		public static Brush MyColorOffBlack { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#1a1a1a");
		public static Brush MyColorOrange { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#E35627");
		public static Brush MyColorGreen { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#4cd213");

		/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// All other colors:
		/// MW = MainWindow
		/// PU = Pop Up
		/// SE = Settings
		/// SFH = SafeFileHandler
		/// MO = Mouse Over
		/// DG = Data Grid
		/// </summary>


		// Colors and stuff which is referenced in all of the XAML. 
		// AFAIK, no Color is hardcoded.
		// Some of thickness, corner radius, margins are semi-hardcoded in XAML Styles.

		// Border of MainWindow
		public static Brush MW_Border { get; private set; } = MyColorWhite;
		// All the HamburgerButton Items, Backgrounds, etc.
		public static System.Windows.Thickness MW_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(0);
		public static Brush MW_HamburgerMenuGridBackground { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush MW_ButtonBackground { get; private set; } = SetOpacity(MyColorBlack, 70);
		public static Brush MW_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush MW_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush MW_ButtonMOBackground { get; private set; } = MyColorOffWhite;
		public static Brush MW_ButtonMOForeground { get; private set; } = MyColorBlack;
		public static Brush MW_ButtonMOBorderBrush { get; private set; } = MyColorWhite;

		public static Brush MW_GTALabelDowngradedForeground { get; private set; } = MyColorGreen;
		public static Brush MW_GTALabelUpgradedForeground { get; private set; } = Brushes.White;
		public static Brush MW_GTALabelBrokenForeground { get; private set; } = Brushes.Red;

		// Hamburger Button and "X"
		// These have no effect since these are all Icons now...
		//public static Brush MW_ButtonSmallBackground { get; private set; } = SetOpacity(MyColorBlack, 70);
		//public static Brush MW_ButtonSmallForeground { get; private set; } = MyColorWhite;
		//public static Brush MW_ButtonSmallBorderBrush { get; private set; } = MyColorWhite;
		//public static Brush MW_ButtonSmallMOBackground { get; private set; } = SetOpacity(MyColorWhite, 70);
		//public static Brush MW_ButtonSmallMOForeground { get; private set; } = MyColorBlack;
		//public static Brush MW_ButtonSmallMOBorderBrush { get; private set; } = MyColorWhite;
		public static System.Windows.Thickness MW_ButtonSmallBorderThickness { get; private set; } = new System.Windows.Thickness(0);

		// GTA Launch Button
		// Border Color will depend on game running or not running, so we will not set this here. I guess. 
		public static Brush MW_ButtonGTAGameNotRunningBorderBrush { get; private set; } = MyColorWhite;
		public static Brush MW_ButtonGTAGameRunningBorderBrush { get; private set; } = MyColorGreen;

		public static Brush MW_ButtonGTABackground { get; private set; } = SetOpacity(MyColorBlack, 70);
		public static Brush MW_ButtonGTAForeground { get; private set; } = MyColorWhite;
		public static Brush MW_ButtonGTAMOBackground { get; private set; } = SetOpacity(MyColorOffWhite, 100);
		public static Brush MW_ButtonGTAMOForeground { get; private set; } = MyColorBlack;

		public static System.Windows.Thickness MW_ButtonGTABorderThickness { get; private set; } = new System.Windows.Thickness(5);

		// POPUP Window
		public static Brush PU_Background { get; private set; } = MyColorOffBlack;

		public static Brush PU_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush PU_BorderBrush_Inner { get; private set; } = MyColorWhite;

		public static Brush PU_ButtonBackground { get; private set; } = MyColorOffBlack;
		public static Brush PU_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush PU_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush PU_ButtonMOBackground { get; private set; } = MyColorOffWhite;
		public static Brush PU_ButtonMOForeground { get; private set; } = MyColorOffBlack;
		public static Brush PU_ButtonMOBorderBrush { get; private set; } = MyColorOffBlack;

		public static Brush PU_ProgressBarBackground { get; private set; } = MyColorOffBlack;
		public static Brush PU_ProgressBarForeground { get; private set; } = MyColorOffWhite;
		public static Brush PU_ProgressBarBorderBrush { get; private set; } = MyColorOffBlack;
		public static System.Windows.Thickness PU_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);
		public static Brush PU_LabelForeground { get; private set; } = MyColorWhite;

		// SaveFilerHandler Window
		public static Brush SFH_Background { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush SFH_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush SFH_BorderBrush_Inner { get; private set; } = MyColorWhite;
		public static Brush SFH_LabelForeground { get; private set; } = MyColorWhite;

		public static Brush SFH_ButtonBackground { get; private set; } = MyColorBlack;
		public static Brush SFH_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush SFH_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush SFH_ButtonMOBackground { get; private set; } = MyColorWhite;
		public static Brush SFH_ButtonMOForeground { get; private set; } = MyColorBlack;
		public static Brush SFH_ButtonMOBorderBrush { get; private set; } = MyColorWhite;

		public static Brush SFH_SVBackground { get; private set; } = MyColorBlack;
		public static Brush SFH_SVForeground { get; private set; } = MyColorWhite;

		public static Brush SFH_DGBackground { get; private set; } = MyColorBlack;
		public static Brush SFH_DGForeground { get; private set; } = MyColorWhite;
		public static Brush SFH_DGCellBackground { get; private set; } = MyColorBlack;
		public static Brush SFH_DGCellForeground { get; private set; } = MyColorWhite;
		public static Brush SFH_DGCellSelectedBackground { get; private set; } = MyColorOrange;
		public static Brush SFH_DGCellSelectedForeground { get; private set; } = MyColorWhite;

		public static System.Windows.Thickness SFH_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);


		// Settings Window
		public static Brush SE_Background { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush SE_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush SE_BorderBrush_Inner { get; private set; } = MyColorWhite;

		public static Brush SE_LabelForeground { get; private set; } = MyColorWhite;
		public static Brush SE_LabelSetForeground { get; private set; } = MyColorWhite;

		public static Brush SE_ButtonBackground { get; private set; } = MyColorBlack;
		public static Brush SE_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush SE_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush SE_ButtonMOBackground { get; private set; } = MyColorWhite;
		public static Brush SE_ButtonMOForeground { get; private set; } = MyColorBlack;
		public static Brush SE_ButtonMOBorderBrush { get; private set; } = MyColorWhite;

		public static Brush SE_ButtonSetBackground { get; private set; } = MyColorBlack;
		public static Brush SE_ButtonSetForeground { get; private set; } = MyColorWhite;
		public static Brush SE_ButtonSetBorderBrush { get; private set; } = MyColorWhite;
		public static Brush SE_ButtonSetMOBackground { get; private set; } = MyColorWhite;
		public static Brush SE_ButtonSetMOForeground { get; private set; } = MyColorBlack;
		public static Brush SE_ButtonSetMOBorderBrush { get; private set; } = MyColorWhite;

		public static Brush SE_SVBackground { get; private set; } = MyColorBlack;
		public static Brush SE_SVForeground { get; private set; } = MyColorWhite;

		public static System.Windows.Thickness SE_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);



		// InitSettings Window. For FirstLaunch and Resetting
		public static Brush IS_Background { get; private set; } = MyColorBlack;
		public static Brush IS_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush IS_BorderBrush_Inner { get; private set; } = MyColorWhite;

		public static Brush IS_LabelForeground { get; private set; } = MyColorWhite;
		public static Brush IS_LabelSetForeground { get; private set; } = MyColorWhite;

		public static Brush IS_ButtonBackground { get; private set; } = MyColorBlack;
		public static Brush IS_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush IS_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush IS_ButtonMOBackground { get; private set; } = MyColorWhite;
		public static Brush IS_ButtonMOForeground { get; private set; } = MyColorBlack;
		public static Brush IS_ButtonMOBorderBrush { get; private set; } = MyColorWhite;

		public static Brush IS_ButtonSetBackground { get; private set; } = MyColorBlack;
		public static Brush IS_ButtonSetForeground { get; private set; } = MyColorWhite;
		public static Brush IS_ButtonSetBorderBrush { get; private set; } = MyColorWhite;
		public static Brush IS_ButtonSetMOBackground { get; private set; } = MyColorWhite;
		public static Brush IS_ButtonSetMOForeground { get; private set; } = MyColorBlack;
		public static Brush IS_ButtonSetMOBorderBrush { get; private set; } = MyColorWhite;

		public static Brush IS_SVBackground { get; private set; } = MyColorBlack;
		public static Brush IS_SVForeground { get; private set; } = MyColorWhite;

		public static System.Windows.Thickness IS_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Takes a Brush and a Opacity (0-100) and returns a Brush.
		/// </summary>
		/// <param name="pBrush"></param>
		/// <param name="pOpacity"></param>
		/// <returns></returns>
		private static Brush SetOpacity(Brush pBrush, int pOpacity)
		{
			double dOpacity = (((double)pOpacity) / 100);
			Brush NewBrush = pBrush.Clone();
			NewBrush.Opacity = dOpacity;
			return NewBrush;
		}


		/// <summary>
		/// Returns a Brush from a Hex String
		/// </summary>
		/// <param name="pString"></param>
		/// <returns></returns>
		private static Brush GetBrushHex(string pString)
		{
			return (GetBrushHex(pString, 100));
		}


		/// <summary>
		/// Returns a Brush from a Hex String and an Opacity (0-100)
		/// </summary>
		/// <param name="pString"></param>
		/// <param name="pOpacity"></param>
		/// <returns></returns>
		private static Brush GetBrushHex(string pString, int pOpacity)
		{
			Brush rtrn = (Brush)new BrushConverter().ConvertFromString("#" + pString.TrimStart('#'));
			return SetOpacity(rtrn, pOpacity);
		}


		/// <summary>
		/// Returns a Brush from RGB integers
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static Brush GetBrushRGB(int r, int g, int b)
		{
			return GetBrushRGB(r, g, b, 100);
		}


		/// <summary>
		/// Replacing substring with other substring, ignores cases. Used for replacing hardlink with copy in some logs when needed
		/// </summary>
		/// <param name="input"></param>
		/// <param name="search"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		public static string ReplaceCaseInsensitive(string input, string search, string replacement)
		{
			string result = Regex.Replace(
				input,
				Regex.Escape(search),
				replacement.Replace("$", "$$"),
				RegexOptions.IgnoreCase
			);
			return result;
		}


		/// <summary>
		/// Returns a Brush from RGB integers, and an Opacity (0-100)
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <param name="pOpacity"></param>
		/// <returns></returns>
		// yeye this ugly like yo mama but its just for internal testing. Wont be called in production
		private static Brush GetBrushRGB(int r, int g, int b, int pOpacity)
		{
			try
			{
				string hex = string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);

				return GetBrushHex(hex, pOpacity);
			}
			catch
			{
				System.Windows.Forms.MessageBox.Show("this shouldnt have happened. Error in RGB / Hex conversion");
				Environment.Exit(1);
				return null;
			}
		}



	} // End of Class
} // End of Namespace
