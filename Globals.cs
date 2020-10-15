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
				if (Globals.CommandLineArgs.ToString().ToLower().Contains("internal")) { return true; }
				return false;
			}
		}

		/// <summary>
		/// Property if we are in Beta
		/// </summary>
		public static bool BetaMode = false;

		/// <summary>
		/// Property of other Buildinfo. Will be in the top message of logs
		/// </summary>
		public static string BuildInfo = "Build 1";

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
		public static RegistryKey MySettingsKey { get { return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("Project_127"); } }

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
		/// Value we use for PageState. Setter is Gucci :*
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

				if (value != PageStates.GTA)
				{
					HamburgerMenuState = HamburgerMenuStates.Visible;
				}

				MainWindow.MW.SetControlBackground(MainWindow.MW, GetBackGroundPath());

				// Switch Value
				switch (value)
				{
					// In Case: Settings
					case PageStates.Settings:

						// Set actual Frame_Main Content to the correct Page
						MainWindow.MW.Frame_Main.Content = new Settings();
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						// Call Mouse_Over false on other Buttons where a page is behind
						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.SaveFileHandler:
						MainWindow.MW.Frame_Main.Content = new SaveFileHandler();
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.ReadMe:
						MainWindow.MW.Frame_Main.Content = new ReadMe();
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.Auth:
						MainWindow.MW.Frame_Main.Content = new ROSIntegration();
						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.GTA:
						MainWindow.MW.Frame_Main.Content = new GTA_Page();

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
				}
			}
		}

		/// <summary>
		/// Enum for all BackgroundImages
		/// </summary>
		public enum BackgroundImages
		{
			Main,
			FourTwenty,
			XMas,
			Spooky
		}

		/// <summary>
		/// Internal Value for BackgroundImage
		/// </summary>
		private static BackgroundImages _BackgroundImage = BackgroundImages.Main;

		/// <summary>
		/// Value we use for BackgroundImage. Setter is Gucci :*
		/// </summary>
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

		/// <summary>
		/// Enum for all HamburgerMenuStates
		/// </summary>
		public enum HamburgerMenuStates
		{
			Visible,
			Hidden
		}

		/// <summary>
		/// Internal Value for HamburgerMenuState
		/// </summary>
		private static HamburgerMenuStates _HamburgerMenuState = HamburgerMenuStates.Hidden;

		/// <summary>
		/// Value we use for HamburgerMenuState. Setter is Gucci :*
		/// </summary>
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
		

		/// <summary>
		/// Gets Path to correct Background URI, based on the 3 States above
		/// </summary>
		/// <returns></returns>
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
		public static Brush MyColorOffBlack70 { get; private set; } = SetOpacity((Brush)new BrushConverter().ConvertFromString("#1a1a1a"),70);
		public static Brush MyColorOrange { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#E35627");
		public static Brush MyColorGreen { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#4cd213");

		/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// All other colors:
		/// App = App_Wide styles
		/// MW = MainWindow
		/// PU = Pop Up
		/// SE = Settings
		/// SFH = SafeFileHandler
		/// MO = Mouse Over
		/// DG = Data Grid
		/// </summary>
		/// 


		// App - Wide stuff
		public static Brush App_LabelForeground { get; private set; } = MyColorWhite;

		public static Thickness App_ButtonBorderThickness { get; private set; } = new Thickness(0);

		public static Brush App_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush App_ButtonBackground { get; private set; } = SetOpacity(MyColorBlack, 70);
		public static Brush App_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush App_ButtonMOBackground { get; private set; } = MyColorOffWhite;
		public static Brush App_ButtonMOForeground { get; private set; } = MyColorBlack;
		public static Brush App_ButtonMOBorderBrush { get; private set; } = MyColorWhite;

		public static Brush App_Submenu_Background { get; private set; } = SetOpacity(MyColorBlack, 65);

		public static Brush SM_ButtonBackground { get; private set; } = MyColorOffBlack;
		public static Brush SM_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush SM_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush SM_ButtonMOBackground { get; private set; } = MyColorOffWhite;
		public static Brush SM_ButtonMOForeground { get; private set; } = MyColorOffBlack;
		public static Brush SM_ButtonMOBorderBrush { get; private set; } = Brushes.Transparent;
		public static System.Windows.Thickness SM_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);


		public static Brush App_ScrollViewerForeground { get; private set; } = MyColorOffWhite;



		public static Thickness App_ButtonSmallBorderThickness { get; private set; } = new Thickness(0);
		public static Brush App_ButtonSmallBorderBrush { get; private set; } = MyColorWhite;


		// MainWindow
		public static Thickness MW_BorderThickness { get; private set; } = new System.Windows.Thickness(2);
		public static Brush MW_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush MW_HamburgerMenuGridBackground { get; private set; } = SetOpacity(MyColorBlack, 65);
		public static Brush MW_HamburgerMenuSeperatorBrush { get; private set; } = MyColorWhite;

		public static Thickness MW_ButtonHamburgerMenuBorderThickness { get; private set; } = new Thickness(0);
		public static Brush MW_ButtonHamburgerMenuBorderBrush { get; private set; } = App_ButtonBorderBrush;
		public static Brush MW_ButtonHamburgerMenuBackground { get; private set; } = App_ButtonBackground;
		public static Brush MW_ButtonHamburgerMenuForeground { get; private set; } = App_ButtonForeground;
		public static Brush MW_ButtonHamburgerMenuMOBackground { get; private set; } = App_ButtonMOBackground;
		public static Brush MW_ButtonHamburgerMenuMOForeground { get; private set; } = App_ButtonMOForeground;
		public static Brush MW_ButtonHamburgerMenuMOBorderBrush { get; private set; } = App_ButtonMOBorderBrush;


		// GTA Launch Button
		// Border Color will depend on game running or not running, so we will not set this here. I guess. 
		public static System.Windows.Thickness MW_ButtonGTABorderThickness { get; private set; } = new System.Windows.Thickness(5);
		public static Brush MW_ButtonGTAGameNotRunningBorderBrush { get; private set; } = MyColorWhite;
		public static Brush MW_ButtonGTAGameRunningBorderBrush { get; private set; } = MyColorGreen;
		public static Brush MW_ButtonGTABackground { get; private set; } = SetOpacity(MyColorBlack, 70);
		public static Brush MW_ButtonGTAForeground { get; private set; } = MyColorWhite;
		public static Brush MW_ButtonGTAMOBackground { get; private set; } = SetOpacity(MyColorOffWhite, 100);
		public static Brush MW_ButtonGTAMOForeground { get; private set; } = MyColorBlack;

		// GTA Label (Upgraded, Downgrad, Unsure etc.
		public static Brush MW_GTALabelDowngradedForeground { get; private set; } = MyColorGreen;
		public static Brush MW_GTALabelUpgradedForeground { get; private set; } = Brushes.White;
		public static Brush MW_GTALabelUnsureForeground { get; private set; } = Brushes.Red;





		// POPUP Window
		public static Brush PU_Background { get; private set; } = MyColorOffBlack;
		public static Brush PU_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush PU_LabelForeground { get; private set; } = MyColorWhite;

		public static Brush PU_ButtonBackground { get; private set; } = MyColorOffBlack;
		public static Brush PU_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush PU_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush PU_ButtonMOBackground { get; private set; } = MyColorOffWhite;
		public static Brush PU_ButtonMOForeground { get; private set; } = MyColorOffBlack;
		public static Brush PU_ButtonMOBorderBrush { get; private set; } = MyColorOffBlack;
		public static System.Windows.Thickness PU_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);

		public static Brush ProgressBarBackground { get; private set; } = MyColorOffBlack;
		public static Brush ProgressBarForeground { get; private set; } = MyColorOffWhite;
		public static Brush ProgressBarBorderBrush { get; private set; } = MyColorOffWhite;

		public static Brush DropDownBackground { get; private set; } = MyColorBlack;
		public static Brush DropDownForeground { get; private set; } = MyColorOffWhite;
		public static Brush DropDownPopDownBackground { get; private set; } = MyColorBlack;
		public static Brush DropDownBorderBrush { get; private set; } = MyColorWhite;


		// SaveFilerHandler Window


		public static Brush SFH_DGBorderBrush { get; private set; } = MyColorWhite;
		public static Thickness SFH_DGBorderThickness { get; private set; } = new Thickness(2);
		public static Brush SFH_DGHeaderBackground { get; private set; } = MyColorOffWhite;
		public static Brush SFH_DGHeaderForeground { get; private set; } = MyColorBlack;
		public static Brush SFH_DGBackground { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush SFH_DGRowBackground { get; private set; } = Brushes.Transparent;
		public static Brush SFH_DGAlternateRowBackground { get; private set; } = SetOpacity(MyColorOffWhite,20);
		public static Brush SFH_DGForeground { get; private set; } = MyColorOffWhite;
		public static Brush SFH_DGCellBorderBrush { get; private set; } = Brushes.Transparent;
		public static Thickness SFH_DGCellBorderThickness { get; private set; } = new Thickness(0);
		//public static Brush SFH_DGSelectedBackground { get; private set; } = Brushes.Transparent;
		//public static Brush SFH_DGSelectedForeground { get; private set; } = GetBrushHex("#76e412");
		//public static Brush SFH_DGSelectedBorderBrush { get; private set; } = GetBrushHex("#76e412");
		public static Brush SFH_DGSelectedBackground { get; private set; } = SetOpacity(MyColorWhite, 80);
		public static Brush SFH_DGSelectedForeground { get; private set; } = MyColorOffBlack;
		public static Brush SFH_DGSelectedBorderBrush { get; private set; } = MyColorOffWhite;
		public static Thickness SFH_DGSelectedBorderThickness { get; private set; } = new Thickness(2);

		//GetBrushRGB(226, 0, 116);

		// Settings Window

		public static Brush SE_RowBackground { get; private set; } = SetOpacity(MyColorBlack,50);
		public static Brush SE_AlternateRowBackground { get; private set; } = SetOpacity(MyColorOffWhite, 20);
		public static Brush SE_BorderBrush_Inner { get; private set; } = MyColorWhite;

		public static Brush SE_Lbl_Header_Background { get; private set; } = MyColorOffWhite;
		public static Brush SE_Lbl_Header_Foreground { get; private set; } = MyColorOffBlack;


		// ReadMe Window

		public static Brush ReadME_Inner_Background { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush ReadME_Inner_BorderBrush { get; private set; } = MyColorOffWhite;
		public static Thickness ReadME_Inner_BorderThickness { get; private set; } = new Thickness(2);
		public static CornerRadius ReadME_Inner_CornerRadius { get; private set; } = new CornerRadius(10);

		

		//public static Brush SE_LabelForeground { get; private set; } = MyColorWhite;
		//public static Brush SE_LabelSetForeground { get; private set; } = MyColorWhite;

		//public static Brush SE_ButtonBackground { get; private set; } = MyColorBlack;
		//public static Brush SE_ButtonForeground { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonBorderBrush { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonMOBackground { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonMOForeground { get; private set; } = MyColorBlack;
		//public static Brush SE_ButtonMOBorderBrush { get; private set; } = MyColorWhite;

		//public static Brush SE_ButtonSetBackground { get; private set; } = MyColorBlack;
		//public static Brush SE_ButtonSetForeground { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonSetBorderBrush { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonSetMOBackground { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonSetMOForeground { get; private set; } = MyColorBlack;
		//public static Brush SE_ButtonSetMOBorderBrush { get; private set; } = MyColorWhite;

		//public static Brush SE_SVBackground { get; private set; } = MyColorBlack;
		//public static Brush SE_SVForeground { get; private set; } = MyColorWhite;

		//public static System.Windows.Thickness SE_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);





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
