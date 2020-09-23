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
using System.Threading.Tasks;
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
		/// Property of our own Project Version
		/// </summary>
		public static Version ProjectVersion = Assembly.GetExecutingAssembly().GetName().Version;

		/// <summary>
		/// URL for AutoUpdaterFile
		/// </summary>
		public static string URL_AutoUpdate = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/master/Installer/Update.xml";

		/// <summary>
		/// URL for AuthUserFile
		/// </summary>
		public static string URL_AuthUser = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "authuser");

		/// <summary>
		/// Download Location of Zip File
		/// </summary>
		public static string ZipFileDownloadLocation = Globals.ProjectInstallationPath + @"\NewZipFile.zip";

		/// <summary>
		/// Property if we are in Beta
		/// </summary>
		public static bool BetaMode = true;

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
		/// String[] of CommandLineArguments
		/// </summary>
		public static string[] CommandLineArguments;

		/// <summary>
		/// Property of LogFile Location. Will always be in in the same folder as the executable, since we want to start logging before inititng regedit and loading settings
		/// </summary>
		public static string Logfile { get; private set; } = ProjectInstallationPath.TrimEnd('\\') + @"\AAA - Logfile.log";

		/// <summary>
		/// Property of the Registry Key we use for our Settings
		/// </summary>
		public static RegistryKey MySettingsKey { get; private set; } = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey(ProjectName);

		/// <summary>
		/// Property of our default Settings
		/// </summary>
		public static Dictionary<string, string> MyDefaultSettings { get; private set; } = new Dictionary<string, string>()
		{
			{"FirstLaunch", "True" },
			{"InstallationPath", ProjectInstallationPath },
			{"GTAVInstallationPath", ""},
			{"FileFolder", Environment.ExpandEnvironmentVariables(@"%ALLUSERSPROFILE%\" + ProjectName)},
			{"EnableLogging", "True"},
			{"EnableTempFixSteamLaunch", "False"},
			{"EnablePreOrderBonus", "False"},
			{"EnableAutoSetHighPriority", "True" },
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
			{"Theme", @"Empty" }
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

			// Warning Message if first Launch or if we are in BetaMode
			if (Settings.FirstLaunch || Globals.BetaMode)
			{
				(new Popup(Popup.PopupWindowTypes.PopupOk, "This shit, eh i mean software, is shit, ehm I mean Beta.\nMay break your GTA V Installation.\nDelete your Save Files.\nUninstall Everything.\nUpload your Browsing History to Facebook.\nAnd Set your PC on fire.\n\nWe aint responsible.")).ShowDialog();
			}

			// Checks if we are doing first Launch.
			if (Settings.FirstLaunch)
			{
				// Set Own Installation Path in Regedit Settings
				HelperClasses.Logger.Log("FirstLaunch Procedure Started");
				HelperClasses.Logger.Log("Setting Installation Path to '" + ProjectInstallationPath + "'",1);
				Settings.SetSetting("InstallationPath", ProjectInstallationPath);

				// Calling this to get the Path automatically
				LauncherLogic.GTAVPathGuessingGame();

				// Set FirstLaunch to false
				Settings.FirstLaunch = false;

				HelperClasses.Logger.Log("FirstLaunch Procedure Ended");
			}

			// Check if GTA V Folder is correct
			HelperClasses.Logger.Log("Intitial start-up to make sure GTAV Path is valid (NOT FIRSTLAUNCH)");
			if (!LauncherLogic.IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath))
			{
				HelperClasses.Logger.Log("Settings.GTAVInstallationPath detected to be wrong calling LauncherLogic.SetGTAVPathManually()");
				LauncherLogic.SetGTAVPathManually();
			}

			// Check our version of the ZIP File
			int ZipVersion = 0;
			Int32.TryParse(HelperClasses.FileHandling.ReadContentOfFile(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\Project_127_Files\Version.txt"), out ZipVersion);

			// Check whats the latest Version of the ZIP File in GITHUB
			int ZipOnlineVersion = 0;
			Int32.TryParse(HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zipversion"), out ZipOnlineVersion);

			HelperClasses.Logger.Log("Checking for ZIP - Update");
			HelperClasses.Logger.Log("ZipVersion = '" + ZipVersion.ToString() + "', ZipOnlineVersion = '" + ZipOnlineVersion + "'");

			// If Zip file from Server is newer
			if (ZipOnlineVersion > ZipVersion)
			{
				HelperClasses.Logger.Log("Update for ZIP found");
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "ZIP Version: '" + ZipOnlineVersion.ToString() + "' found on the Server.\nZIP Version: '" + ZipVersion.ToString() + "' found installed.\nDo you want to upgrade?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("User wants update for ZIP");
					string pathOfNewZip = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zip");
					new PopupProgress(PopupDownloadTypes.ZIP, pathOfNewZip, ZipFileDownloadLocation).ShowDialog();
				}
				else
				{
					HelperClasses.Logger.Log("User does not want update for ZIP");
				}
			}
			else
			{
				HelperClasses.Logger.Log("NO Update for ZIP found");
			}

			// Starting the Dispatcher Timer for the automatic updates of the GTA V Button
			MyDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			MyDispatcherTimer.Tick += new EventHandler(pMW.SetGTAVButtonBasedOnGameAndInstallationState);
			MyDispatcherTimer.Interval = TimeSpan.FromMilliseconds(5000);
			MyDispatcherTimer.Start();
			pMW.SetGTAVButtonBasedOnGameAndInstallationState(null, null);
		}

		/// <summary>
		/// Proper Exit Method. EMPTY FOR NOW. Get called when closed (user and taskmgr) and when PC is shutdown. Not when process is killed or power ist lost.
		/// </summary>
		public static void ProperExit()
		{

		}

		/// <summary>
		/// Method to import Zip File
		/// </summary>
		public static void ImportZip(string pZipFileLocation, bool deleteFileAfter = false)
		{
			HelperClasses.Logger.Log("Importing ZIP File: '" + pZipFileLocation + "'");

			string[] myFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\Project_127_Files");
			foreach (string myFile in myFiles)
			{
				if (!myFile.Contains("UpgradeFiles"))
				{
				HelperClasses.FileHandling.deleteFile(myFile);
				}
			}

			HelperClasses.Logger.Log("Extracting ZIP File: '" + pZipFileLocation + "' to the path: '" + Globals.ProjectInstallationPath + "'");
			ZipFile.ExtractToDirectory(pZipFileLocation, Globals.ProjectInstallationPath);


			if (deleteFileAfter)
			{
				HelperClasses.Logger.Log("Deleting ZIP File: '" + pZipFileLocation + "'");
				HelperClasses.FileHandling.deleteFile(pZipFileLocation);
			}

			new Popup(Popup.PopupWindowTypes.PopupOk, "Done importing ZIP File").ShowDialog();
		}


		/// <summary>
		/// DebugPopup Method. Just opens Messagebox with pMsg
		/// </summary>
		/// <param name="pMsg"></param>
		public static void DebugPopup(string pMsg)
		{
			System.Windows.Forms.MessageBox.Show(pMsg);
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
		public static Brush SFH_Background { get; private set; } = MyColorBlack;
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

		public static Brush SFH_DGCellSelectedBackground { get; private set; } = MyColorWhite;
		public static Brush SFH_DGCellSelectedForeground { get; private set; } = MyColorBlack;

		public static System.Windows.Thickness SFH_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);


		// Settings Window
		public static Brush SE_Background { get; private set; } = MyColorBlack;
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
