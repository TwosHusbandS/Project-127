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
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using CefSharp;

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

		public static Version GameVersion
		{
			get
			{
				try
				{
					if (HelperClasses.FileHandling.doesFileExist(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\gta5.exe"))
					{
						FileVersionInfo myFVI = FileVersionInfo.GetVersionInfo(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\gta5.exe");
						return new Version(myFVI.FileVersion);
					}
				}
				catch
				{

				}
				return new Version("1.0.0.0");
			}
		}

		/// <summary>
		/// Download Location of Zip File
		/// </summary>
		public static string ZipFileDownloadLocation = Globals.ProjectInstallationPath + @"\NewZipFile.zip";

		/// <summary>
		/// We use this to launch after Auth automatically
		/// </summary>
		public static bool LaunchAfterAuth = false;

		/// <summary>
		/// Property if we are in Beta
		/// </summary>
		public static bool InternalMode
		{
			get
			{
				foreach (string tmp in Globals.CommandLineArgs)
				{
					if (tmp.ToLower().Contains("internal"))
					{
						return true;
					}
				}
				if (HelperClasses.FileHandling.doesFileExist(ProjectInstallationPath.TrimEnd('\\') + @"\internal.txt"))
				{
					return true;
				}
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
		public static string BuildInfo = "Build 1, Internal Testing";

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
		/// Property we use to keep track if we have already thrown one OfflineError Popup
		/// </summary>
		public static bool OfflineErrorThrown = false;

		/// <summary>
		/// Property of LogFile Location. Will always be in in the same folder as the executable, since we want to start logging before inititng regedit and loading settings
		/// </summary>
		public static string Logfile { get; private set; } = ProjectInstallationPath.TrimEnd('\\') + @"\AAA - Logfile.log";

		/// <summary>
		/// Property of the Registry Key we use for our Settings
		/// </summary>													
		public static RegistryKey MySettingsKey { get { return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("Project_127"); } }


		public static Dictionary<string, string> VersionTable { get; private set; } = new Dictionary<string, string>()
		{
			{"1.0.323.1", "1.24" },
			{"1.0.331.1", "1.24" },
			{"1.0.335.2", "1.24" },
			{"1.0.350.1", "1.26" },
			{"1.0.350.2", "1.26" },
			{"1.0.372.2", "1.27" },
			{"1.0.393.2", "1.28" },
			{"1.0.393.4", "1.28.01" },
			{"1.0.463.1", "1.29" },
			{"1.0.505.2", "1.30" },
			{"1.0.573.1", "1.31" },
			{"1.0.617.1", "1.32" },
			{"1.0.678.1", "1.33" },
			{"1.0.757.2", "1.34" },
			{"1.0.757.4", "1.34" },
			{"1.0.791.2", "1.35" },
			{"1.0.877.1", "1.36" },
			{"1.0.944.2", "1.37" },
			{"1.0.1011.1", "1.38" },
			{"1.0.1032.1", "1.39" },
			{"1.0.1103.2", "1.40" },
			{"1.0.1180.2", "1.41" },
			{"1.0.1290.1", "1.42" },
			{"1.0.1365.1", "1.43" },
			{"1.0.1493.0", "1.44" },
			{"1.0.1493.1", "1.44" },
			{"1.0.1604.0", "1.46" },
			{"1.0.1734.0", "1.47" },
			{"1.0.1737.0", "1.48" },
			{"1.0.1737.6", "1.48" },
			{"1.0.1868.0", "1.50" },
			{"1.0.2060.0", "1.51" },
			{"1.0.2060.1", "1.52" },
		};


		/// <summary>
		/// Property of our default Settings
		/// </summary>
		public static Dictionary<string, string> MyDefaultSettings { get; private set; } = new Dictionary<string, string>()
		{
			/*
			Previously Used Settings Variables, we cannot use anymore, since they are written,
			and we are not able to reset only them (for now...):
				- "FileFolder"
				- "EnableAutoSteamCoreFix"
			    - "EnableNohboardBurhac"
				- "Theme"
			*/

			// Internal Settings we dont show the user
			{"FirstLaunch", "True" },
			{"LastLaunchedVersion", Globals.ProjectVersion.ToString() },
			{"InstallationPath", Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\')) },
			{"EnableRememberMe", "False" },

			// Project 1.27 Settings
			{"GTAVInstallationPath", ""},
			{"ZIPExtractionPath", Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\')) },
			{"EnableLogging", "True"},
			{"EnableCopyFilesInsteadOfHardlinking", "False"},
			
			// GTA V Settings
			{"Retailer", "Steam"},
			{"LanguageSelected", "English"},
			{"InGameName", "HiMomImOnYoutube"},
			{"EnablePreOrderBonus", "False"},
			{"EnableDontLaunchThroughSteam", "False"},
   
			// Extra Features
			{"EnableOverlay", "False"},
			{"EnableAutoStartJumpScript", "True" },
			{"JumpScriptKey1", "32" },
			{"JumpScriptKey2", "76" },
			{"EnableAutoSetHighPriority", "True" },

			// Auto start Shit
			{"EnableOnlyAutoStartProgramsWhenDowngraded", "True"},
			{"EnableAutoStartLiveSplit", "True" },
			{"PathLiveSplit", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartStreamProgram", "True" },
			{"PathStreamProgram", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartFPSLimiter", "True" },
			{"PathFPSLimiter", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartNohboard", "True" },
			{"PathNohboard", @"C:\Some\Path\SomeFile.exe" },

			// Overlay shit
			{"KeyOverlayToggle", "163" },
			{"KeyOverlayScrollUp", "109" },
			{"KeyOverlayScrollDown", "107" },
			{"KeyOverlayScrollRight", "106" },
			{"KeyOverlayScrollLeft", "111" },
			{"KeyOverlayNoteNext", "103" },
			{"KeyOverlayNotePrev", "105" },


			{"OverlayBackground", "100,0,0,0" },
			{"OverlayForeground", "255,255,0,255" },
			{"OverlayLocation", "TopLeft" },
			{"OverlayMargin", "10" },
			{"OverlayWidth", "580" },
			{"OverlayHeight", "500" },
			{"OverlayTextFont", "Arial" },
			{"OverlayTextSize", "24" },

			{"OverlayNotesMain","Note1.txt;Note2.txt;Note3.txt;Note4.txt"},
			{"OverlayNotesPresetA",""},
			{"OverlayNotesPresetB",""},
			{"OverlayNotesPresetC",""},
			{"OverlayNotesPresetD",""},
			{"OverlayNotesPresetE",""},
			{"OverlayNotesPresetF",""},
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


				new Popup(Popup.PopupWindowTypes.PopupOk,
				"Project 1.27 is finally in fully released.\n" +
				"The published Product should work as expected.\n\n" +
				"No gurantees that this will not break your GTAV in any way, shape or form.\n\n" +
				"The 'Remember' Me function, is storing credentials\n" +
				"using the Windows Credential Manager.\n" +
				"You are using the it on your own risk.\n\n" +
				"If anything does not work as expected, \n" +
				"contact me on Discord. @thS#0305\n\n" +
				" - The Project 1.27 Team").ShowDialog();


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

				if (Settings.LastLaunchedVersion < new Version("1.1.0.0"))
				{
					Settings.JumpScriptKey1 = System.Windows.Forms.Keys.Space;
					Settings.JumpScriptKey2 = System.Windows.Forms.Keys.L;

					if (LauncherLogic.InstallationState != LauncherLogic.InstallationStates.Downgraded)
					{
						FileHandling.deleteFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\asmjit.dll");
						FileHandling.deleteFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\botan.dll");
						FileHandling.deleteFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\launc.dll");
						FileHandling.deleteFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\origi_socialclub.dll");
						FileHandling.deleteFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\Readme.txt");
						FileHandling.deleteFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\socialclub.dll");
						FileHandling.deleteFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\tinyxml2.dll");
					}

					FileHandling.deleteFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\asmjit.dll");
					FileHandling.deleteFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\botan.dll");
					FileHandling.deleteFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\launc.dll");
					FileHandling.deleteFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\origi_socialclub.dll");
					FileHandling.deleteFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\Readme.txt");
					FileHandling.deleteFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\socialclub.dll");
					FileHandling.deleteFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\tinyxml2.dll");

					string[] tmp = HelperClasses.FileHandling.GetSubFolders(ProjectInstallationPath);
					foreach (string temp in tmp)
					{
						HelperClasses.FileHandling.DeleteFolder(temp);
					}
				}

				Settings.LastLaunchedVersion = Globals.ProjectVersion;
			}


			// Deleting all Installer and ZIP Files from own Project Installation Path
			DeleteOldFiles();

			// Throw annoucements
			HandleAnnouncements();

			// Auto Updater
			CheckForUpdate();

			// Downloads the "big 3" gamefiles from github release
			CheckForBigThree();

			// Check whats the latest Version of the ZIP File in GITHUB
			CheckForZipUpdate();

			// Intepreting all Command Line shit
			CommandLineArgumentIntepretation();

			// Jumpscript
			if (Settings.EnableAutoStartJumpScript)
			{
				Jumpscript.InitJumpscript();
			}

			// Checks if Update hit
			LauncherLogic.HandleUpdates();

			// Rolling Log stuff
			HelperClasses.Logger.RollingLog();

			// CEF Initializing
			CEFInitialize();
			
			// Starting the Dispatcher Timer for the automatic updates of the GTA V Button
			MyDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			MyDispatcherTimer.Tick += new EventHandler(MainWindow.MW.UpdateGUIDispatcherTimer);
			MyDispatcherTimer.Interval = TimeSpan.FromMilliseconds(2500);
			MyDispatcherTimer.Start();
			MainWindow.MW.UpdateGUIDispatcherTimer();
		}

		public static string GetGameVersionOfBuildNumber(Version BuildNumber)
		{
			foreach (KeyValuePair<string, string> KVP in Globals.VersionTable)
			{
				if (KVP.Key == BuildNumber.ToString())
				{
					return KVP.Value;
				}
			}

			if (BuildNumber < new Version("1.0.2060.1"))
			{
				return "???";
			}
			else
			{
				return "> 1.52";
			}
		}


		/// <summary>
		/// CommandLineArgumentIntepretation(), currently used for Background Image
		/// </summary>
		private static void CommandLineArgumentIntepretation()
		{
			// Code for internal mode is in Globals.Internalmode Getter

			// Need to be in following Format
			// "-CommandLineArg:Value"
			foreach (string CommandLineArg in Globals.CommandLineArgs)
			{
				string Argument = "";
				string Value = "";
				try
				{
					Argument = CommandLineArg.Substring(0, CommandLineArg.IndexOf(':'));
					Value = CommandLineArg.Substring(CommandLineArg.IndexOf(':') + 1);
				}
				catch
				{
				}

				if (Argument == "-Background")
				{
					Globals.BackgroundImages Tmp = Globals.BackgroundImages.Main;
					try
					{
						Tmp = (Globals.BackgroundImages)System.Enum.Parse(typeof(Globals.BackgroundImages), Value);
						Globals.BackgroundImage = Tmp;
						MainWindow.MW.SetBackground(Globals.GetBackGroundPath());
					}
					catch { }
				}
			}
		}


		private static void HandleAnnouncements()
		{
			string MyAnnoucment = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "announcement");
			if (MyAnnoucment != "")
			{
				MyAnnoucment = MyAnnoucment.Replace(@"\n", "\n");
				new Popup(Popup.PopupWindowTypes.PopupOk, MyAnnoucment);
			}
		}

		/// <summary>
		/// Deleting all Old Files (Installer and ZIP Files) from the Installation Folder
		/// </summary>
		private static void DeleteOldFiles()
		{
			HelperClasses.Logger.Log("Checking if there is an old Installer or ZIP Files in the Project InstallationPath during startup procedure.");

			// Looping through all Files in the Installation Path
			foreach (string myFile in HelperClasses.FileHandling.GetFilesFromFolder(Globals.ProjectInstallationPath))
			{
				// If it contains the word installer, delete it
				if (myFile.ToLower().Contains("installer"))
				{
					HelperClasses.Logger.Log("Found old installer File ('" + HelperClasses.FileHandling.PathSplitUp(myFile)[1] + "') in the Directory. Will delete it.");
					HelperClasses.FileHandling.deleteFile(myFile);
				}
				// If it is the Name of the ZIP File we download, we delete it
				if (myFile == Globals.ZipFileDownloadLocation)
				{
					HelperClasses.Logger.Log("Found old ZIP File ('" + HelperClasses.FileHandling.PathSplitUp(myFile)[1] + "') in the Directory. Will delete it.");
					HelperClasses.FileHandling.deleteFile(myFile);
				}
			}
		}


		/// <summary>
		/// Method which does the UpdateCheck on Startup
		/// </summary>
		public static void CheckForUpdate()
		{
			// Check online File for Version.
			string MyVersionOnlineString = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "version");

			// If this is empty,  github returned ""
			if (!(String.IsNullOrEmpty(MyVersionOnlineString)))
			{
				// Building a Version out of the String
				Version MyVersionOnline = new Version(MyVersionOnlineString);

				// Logging some stuff
				HelperClasses.Logger.Log("Checking for Project 1.27 Update during start up procedure");
				HelperClasses.Logger.Log("MyVersionOnline = '" + MyVersionOnline.ToString() + "', Globals.ProjectVersion = '" + Globals.ProjectVersion + "'", 1);

				// If Online Version is "bigger" than our own local Version
				if (MyVersionOnline > Globals.ProjectVersion)
				{
					// Update Found.
					HelperClasses.Logger.Log("Update found", 1);
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Version: '" + MyVersionOnline.ToString() + "' found on the Server.\nVersion: '" + Globals.ProjectVersion.ToString() + "' found installed.\nDo you want to upgrade?");
					yesno.ShowDialog();
					// Asking User if he wants update.
					if (yesno.DialogResult == true)
					{
						// User wants Update
						HelperClasses.Logger.Log("Update found. User wants it", 1);
						string DLPath = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "url");
						string DLFilename = DLPath.Substring(DLPath.LastIndexOf('/') + 1);
						string LocalFileName = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\" + DLFilename;

						new PopupDownload(DLPath, LocalFileName, "Installer").ShowDialog();
						HelperClasses.ProcessHandler.StartProcess(LocalFileName);
						Environment.Exit(0);
					}
					else
					{
						// User doesnt want update
						HelperClasses.Logger.Log("Update found. User does not wants it", 1);
					}
				}
				else
				{
					// No update found
					HelperClasses.Logger.Log("No Update Found");
				}
			}
			else
			{
				// String return is fucked
				HelperClasses.Logger.Log("Did not get most up to date Project 1.27 Version from Github. Github offline or your PC offline. Probably. Lets hope so.");
			}
		}


		/// <summary>
		/// Checks Github for the big 3 files we need
		/// </summary>
		public static void CheckForBigThree()
		{
			HelperClasses.Logger.Log("Downloading the 'big three' files");

			string DLLinkG = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkG");
			string DLLinkGHash = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkGHash");
			string DLLinkU = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkU");
			string DLLinkUHash = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkUHash");
			string DLLinkX = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkX");
			string DLLinkXHash = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkXHash");

			HelperClasses.Logger.Log("Checking if gta5.exe exists locally", 1);
			if (HelperClasses.FileHandling.doesFileExist(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe"))
			{
				HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			}
			else
			{
				HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);
				new PopupDownload(DLLinkG, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe", "Needed Files (gta5.exe 1/3)").ShowDialog();

				if (!string.IsNullOrWhiteSpace(DLLinkGHash))
				{
					HelperClasses.Logger.Log("We do have a Hash for that file. Lets compare it:", 2);
					HelperClasses.Logger.Log("Hash we want: '" + DLLinkGHash + "'", 3);
					HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + "'", 3);
					while (HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") != DLLinkGHash)
					{
						HelperClasses.Logger.Log("Well..hashes dont match shit. Lets try again", 2);
						HelperClasses.FileHandling.deleteFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe");
						new PopupDownload(DLLinkG, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe", "Needed Files (gta5.exe 1/3)").ShowDialog();
						HelperClasses.Logger.Log("Hash we want: '" + DLLinkGHash + "'", 3);
						HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + "'", 3);
					}
				}
			}

			HelperClasses.Logger.Log("Checking if x64a.rpf exists locally", 1);
			if (HelperClasses.FileHandling.doesFileExist(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf"))
			{
				HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			}
			else
			{
				HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);
				new PopupDownload(DLLinkX, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf", "Needed Files (x64a.rpf, 2/3)").ShowDialog();

				if (!string.IsNullOrWhiteSpace(DLLinkXHash))
				{
					HelperClasses.Logger.Log("We do have a Hash for that file. Lets compare it:", 2);
					HelperClasses.Logger.Log("Hash we want: '" + DLLinkXHash + "'", 3);
					HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf") + "'", 3);
					while (HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf") != DLLinkXHash)
					{
						HelperClasses.Logger.Log("Well..hashes dont match shit. Lets try again", 2);
						HelperClasses.FileHandling.deleteFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf");
						new PopupDownload(DLLinkX, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf", "Needed Files (x64a.rpf, 2/3)").ShowDialog();
						HelperClasses.Logger.Log("Hash we want: '" + DLLinkXHash + "'", 3);
						HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf") + "'", 3);
					}
				}
			}

			HelperClasses.Logger.Log(@"Checking if update\update.rpf exists locally", 1);
			if (HelperClasses.FileHandling.doesFileExist(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf"))
			{
				HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			}
			else
			{
				HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);
				new PopupDownload(DLLinkU, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf", "Needed Files (Update.rpf, 3/3)").ShowDialog();

				if (!string.IsNullOrWhiteSpace(DLLinkUHash))
				{
					HelperClasses.Logger.Log("We do have a Hash for that file. Lets compare it:", 2);
					HelperClasses.Logger.Log("Hash we want: '" + DLLinkUHash + "'", 3);
					HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf") + "'", 3);
					while (HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf") != DLLinkUHash)
					{
						HelperClasses.Logger.Log("Well..hashes dont match shit. Lets try again", 2);
						HelperClasses.FileHandling.deleteFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf");
						new PopupDownload(DLLinkU, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf", "Needed Files (update.rpf, 3/3)").ShowDialog();
						HelperClasses.Logger.Log("Hash we want: '" + DLLinkUHash + "'", 3);
						HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf") + "'", 3);
					}
				}
			}
		}


		public static string GetDDL(string pLink)
		{
			string DDL = pLink;

			if (pLink.Contains("anonfiles"))
			{
				string NonDDL = pLink;

				//href = "https:\/\/cdn-[0-9]+\.anonfiles\.com\/[a-zA-Z0-9]+\/[a-zA-Z0-9]+-[a-zA-Z0-9]+\/[_\w]+\.zip">
				string RegexPattern = @"href=""https:\/\/cdn-[0-9]+\.anonfiles\.com\/[a-zA-Z0-9]+\/[a-zA-Z0-9]+-[a-zA-Z0-9]+\/[_\w]+\.zip"">";

				// Setting up some Webclient stuff. 
				WebClient webClient = new WebClient();
				string webSource = "";
				webSource = webClient.DownloadString(NonDDL);
				webSource.Replace(" ", "");
				webSource.Replace("\n", "");
				webSource.Replace("\r", "");
				webSource.Replace("\t", "");

				Regex MyRegex = new Regex(RegexPattern);
				Match MyMatch = MyRegex.Match(webSource);

				if (MyMatch.Success)
				{
					if (MyMatch.Groups.Count > 0)
					{
						DDL = MyMatch.Groups[0].ToString();
						int FirstIndexOfDoubleQuotes = DDL.IndexOf('"');
						int LastIndexOfDoubleQuotes = DDL.LastIndexOf('"');
						DDL = DDL.Substring(FirstIndexOfDoubleQuotes + 1, LastIndexOfDoubleQuotes - FirstIndexOfDoubleQuotes - 1);
					}
				}
			}
			return DDL;
		}

		/// <summary>
		/// Checks for Update of the ZIPFile and extracts it
		/// </summary>
		public static void CheckForZipUpdate()
		{
			// Check whats the latest Version of the ZIP File in GITHUB
			int ZipOnlineVersion = 0;
			Int32.TryParse(HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zipversion"), out ZipOnlineVersion);

			HelperClasses.Logger.Log("Checking for ZIP - Update");
			HelperClasses.Logger.Log("ZipVersion = '" + Globals.ZipVersion + "', ZipOnlineVersion = '" + ZipOnlineVersion + "'");

			// If Zip file from Server is newer
			if (ZipOnlineVersion > Globals.ZipVersion)
			{
				HelperClasses.Logger.Log("Update for ZIP found");
				Popup yesno;
				if (Globals.ZipVersion > 0)
				{
					yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "ZIP Version: '" + ZipOnlineVersion.ToString() + "' found on the Server.\nZIP Version: '" + Globals.ZipVersion.ToString() + "' found installed.\nDo you want to upgrade?");
				}
				else
				{
					yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "ZIP Version: '" + ZipOnlineVersion.ToString() + "' found on the Server.\nNo ZIP Version found installed.\nDo you want to install the ZIP?");
				}
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("User wants update for ZIP");

					// Getting the Hash of the new ZIPFile
					string hashNeeded = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zipmd5");
					HelperClasses.Logger.Log("HashNeeded: " + hashNeeded);

					// Looping 0 through 5
					for (int i = 0; i <= 5; i++)
					{
						// Getting DL Link of zip + i
						string pathOfNewZip = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zip" + i.ToString());
						HelperClasses.Logger.Log("Zip-Try: 'zip" + i.ToString() + "'");
						HelperClasses.Logger.Log("DL Link: '" + pathOfNewZip + "'");

						// Deleting old ZIPFile
						HelperClasses.FileHandling.deleteFile(Globals.ZipFileDownloadLocation);

						// Getting actual DDL
						pathOfNewZip = GetDDL(pathOfNewZip);

						// Downloading the ZIP File
						new PopupDownload(pathOfNewZip, Globals.ZipFileDownloadLocation, "ZIP-File").ShowDialog();

						// Checking the hash of the Download
						string HashOfDownload = HelperClasses.FileHandling.GetHashFromFile(Globals.ZipFileDownloadLocation);
						HelperClasses.Logger.Log("Download Done, Hash of Downloaded File: '" + HashOfDownload + "'");

						// If Hash looks good, we import it
						if (HashOfDownload == hashNeeded)
						{
							HelperClasses.Logger.Log("Hashes Match, will Import");
							LauncherLogic.ImportZip(Globals.ZipFileDownloadLocation, true);
							return;
						}
						HelperClasses.Logger.Log("Hashes dont match, will move on");
					}
					HelperClasses.Logger.Log("Error. Could not find a suitable ZIP File from a FileHoster. Program cannot download new ZIP at the moment.");
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Update of ZIP File failed (No Suitable ZIP Files Found).\nI suggest restarting the program and opting out of update.");
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
		}

		/// <summary>
		/// Proper Exit Method. EMPTY FOR NOW. Get called when closed (user and taskmgr) and when PC is shutdown. Not when process is killed or power ist lost.
		/// </summary>
		public static void ProperExit()
		{
			HelperClasses.Keyboard.KeyboardListener.Stop();
			WindowChangeListener.Stop();
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
			GTA,
			NoteOverlay
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

				if (value != PageStates.NoteOverlay)
				{
					NoteOverlay.DisposePreview();
				}

				MainWindow.MW.SetBackground(Globals.GetBackGroundPath());

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
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.SaveFileHandler:
						MainWindow.MW.Frame_Main.Content = new SaveFileHandler();
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.ReadMe:
						MainWindow.MW.Frame_Main.Content = new ReadMe();
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.NoteOverlay:
						MainWindow.MW.Frame_Main.Content = new Overlay.NoteOverlay();
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.Auth:
						if (Globals.LaunchAfterAuth)
						{
							Globals.LaunchAfterAuth = false;
							MainWindow.MW.Frame_Main.Content = new ROSIntegration(true);
						}
						else
						{
							MainWindow.MW.Frame_Main.Content = new ROSIntegration();
						}
						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.GTA:
						MainWindow.MW.Frame_Main.Content = new GTA_Page();

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
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
				MainWindow.MW.SetBackground(GetBackGroundPath());
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
				MainWindow.MW.SetBackground(Globals.GetBackGroundPath());


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


		/// <summary>
		/// Initialzes CEF settings
		/// </summary>
		public static void CEFInitialize()
		{
			HelperClasses.Logger.Log("Initializing CEF...");
			var s = new CefSharp.Wpf.CefSettings();
			s.CachePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\CEF_CacheFiles";
			s.BackgroundColor = 0;//0x13 << 16 | 0x15 << 8 | 0x18;
			s.DisableGpuAcceleration();
			s.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";
#if DEBUG
			s.RemoteDebuggingPort = 8088;
#endif
			Cef.Initialize(s);
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
		public static Brush MyColorOffBlack70 { get; private set; } = SetOpacity((Brush)new BrushConverter().ConvertFromString("#1a1a1a"), 70);
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
		public static Brush SFH_DGAlternateRowBackground { get; private set; } = SetOpacity(MyColorOffWhite, 20);
		public static Brush SFH_DGForeground { get; private set; } = MyColorWhite;
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

		public static Brush SE_RowBackground { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush SE_AlternateRowBackground { get; private set; } = SetOpacity(MyColorOffWhite, 20);
		public static Brush SE_BorderBrush_Inner { get; private set; } = MyColorWhite;

		public static Brush SE_Lbl_Header_Background { get; private set; } = MyColorOffWhite;
		public static Brush SE_Lbl_Header_Foreground { get; private set; } = MyColorOffBlack;


		// ReadMe Window

		public static Brush ReadME_Inner_Background { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush ReadME_Inner_BorderBrush { get; private set; } = MyColorWhite;
		public static Thickness ReadME_Inner_BorderThickness { get; private set; } = new Thickness(2);
		public static CornerRadius ReadME_Inner_CornerRadius { get; private set; } = new CornerRadius(10);

		// Using a lot of settings stuff (grid-background, grid second / alternative row color, button styles inside grid) on the noteoverlay...whatevs XD

		public static Brush NO_Slider_Track_Brush { get; private set; } = MyColorOffWhite;
		public static Brush NO_Slider_Thumb_Brush { get; private set; } = MyColorOffWhite;
		public static Brush NO_Slider_Thumb_MO_Brush { get; private set; } = MyColorOffBlack;


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
