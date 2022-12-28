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
using Project_127.SaveFileHandlerStuff;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using CefSharp;
using System.IO;
using System.Timers;

namespace Project_127
{
	/// <summary>
	/// Class for Global / Central Place
	/// </summary>
	public static class Globals
	{
		// Properties with benefit or logic below

		#region Properties with benefit or logic


		/// <summary>
		/// Property of our own Installation Path
		/// </summary>
		public static string ProjectInstallationPath
		{
			get
			{
				return (Directory.GetParent(ProjectInstallationPathBinary).ToString());
			}
		}

		/// <summary>
		/// Property of our own Installation Path Binary
		/// </summary>
		public static string ProjectInstallationPathBinary { get { return Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\')); } }

		/// <summary>
		/// Property of the ZIP Version currently installed
		/// </summary>
		public static int ZipVersion
		{
			get
			{
				return ComponentManager.Components.Base.GetInstalledVersion().Minor;
			}
		}

		/// <summary>
		/// Property of our own Project Version
		/// </summary>
		public static Version ProjectVersion = Assembly.GetExecutingAssembly().GetName().Version;



		/// <summary>
		/// XML for AutoUpdaterFile
		/// </summary>
		public static string XML_AutoUpdate
		{
			get
			{
				string masterURL = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/master/Installer/Update.xml";
				string modeURL = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/" + P127Branch.ToLower() + "/Installer/Update.xml";

				string modeXML = HelperClasses.FileHandling.GetStringFromURL(modeURL, true);

				if (!String.IsNullOrWhiteSpace(modeXML))
				{
					return modeXML;
				}
				else
				{
					return HelperClasses.FileHandling.GetStringFromURL(masterURL);
				}
			}
		}


		/// <summary>
		/// XML for DownloadManagerXML
		/// </summary>
		public static string XML_DownloadManager
		{
			get
			{
				string masterURL = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/master/Installer/DownloadManager.xml";
				string modeURL = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/" + DMBranch + "/Installer/DownloadManager.xml";

				string modeXML = HelperClasses.FileHandling.GetStringFromURL(modeURL, true);
				if (!String.IsNullOrWhiteSpace(modeXML))
				{
					return modeXML;
				}
				else
				{
					return HelperClasses.FileHandling.GetStringFromURL(masterURL, true);
				}
			}
		}

		/// <summary>
		/// URL for DownloadManagerXML
		/// </summary>
		public static string URL_DownloadManager
		{
			get
			{
				string masterURL = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/master/Installer/DownloadManager.xml";
				string modeURL = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/" + DMBranch + "/Installer/DownloadManager.xml";
				if (String.IsNullOrWhiteSpace(HelperClasses.FileHandling.GetStringFromURL(modeURL, true)))
				{
					return masterURL;
				}
				else
				{
					return modeURL;
				}
			}
		}

		/// Gets the Branch we are in as actual branch.
		/// </summary>
		public static string P127Branch
		{
			get
			{
				if (InternalMode)
				{
					return "internal";
				}
				if (Project_127.MySettings.Settings.P127Mode.ToLower() == "default")
				{
					return "master";
				}
				return Project_127.MySettings.Settings.P127Mode.ToLower();
			}
		}

		/// Gets the Branch we are in as actual branch.
		/// </summary>
		public static string DMBranch
		{
			get
			{
				if (InternalMode)
				{
					return "internal";
				}
				if (Project_127.MySettings.Settings.DMMode.ToLower() == "default")
				{
					return "master";
				}
				return Project_127.MySettings.Settings.DMMode.ToLower();
			}
		}

		/// <summary>
		/// Gets the Version (BuildVersion) of our GTA5.exe
		/// </summary>
		public static Version GTABuild
		{
			get
			{
				return HelperClasses.FileHandling.GetVersionFromFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\gta5.exe");
			}
		}

		/// <summary>
		/// Contains the verion info of running GTA
		/// </summary>
		public static Version RunningGTABuild = new Version(0, 0);

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
		public static string BuildInfo = "1.2.5.0 - Build 3";


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
				try
				{
					RegistryKey myRK = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("WOW6432Node").CreateSubKey("Valve").CreateSubKey("Steam");
					return HelperClasses.RegeditHandler.GetValue(myRK, "InstallPath");
				}
				catch
				{
					return LauncherLogic.GTAVFilePath;
				}
			}
		}



		/// <summary>
		/// Property we use to keep track if we have already thrown one OfflineError Popup
		/// </summary>
		public static bool OfflineErrorThrown = false;

		/// <summary>
		/// Property of LogFile Location. Will always be in in the same folder as the executable, since we want to start logging before inititng regedit and loading settings
		/// </summary>
		public static string Logfile { get; private set; } = ProjectInstallationPath.TrimEnd('\\') + @"\AAA - Logfile.log";


		public static IPCPipeServer pipeServer = new IPCPipeServer("Project127Launcher");



		#endregion

		// Properties with benefit / Logic above

		// Settings stuff below

		#region Settings stuff

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
				- "EnableAutoSteamCoreFix"
			    - "EnableNohboardBurhac"
				- "Theme"
				- "EnableCopyFilesInsteadOfSyslinking_SocialClub"
				- "TeasingFeatures"
			*/

			// Internal Settings we dont show the user
			{"DownloadManagerInstalledSubassemblies", "" },
			{"FirstLaunch", "True" },
			{"LastLaunchedVersion", Globals.ProjectVersion.ToString() },
			{"InstallationPath", Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\')) },
			{"EnableRememberMe", "False" },
			{"AllFilesEverPlacedInsideGTA", @"bink2w64.dll;GFSDK_ShadowLib.win64.dll;GTA5.exe;launc.dll;orig_socialclub.dll;PlayGTAV.exe;ROSCrypto.dll;socialclub.dll;x64a.rpf;update\update.rpf;scemu.cfg;launc.dat;version.txt;P127_ASMPATCHER_SCRIPTHOOK.dll" },

			// Project 1.27 Settings
			{"GTAVInstallationPath", ""},
			{"ZIPExtractionPath", Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\')) },
			{"EnableLogging", "True"},
			{"EnableAlternativeLaunch", "False"},
			{"EnableAlternativeLaunchForceCProgramFiles", "False"},
			{"EnableCopyFilesInsteadOfHardlinking", "False"},
			{"AutoMTLAuthOnStartup", "True"},
			{"PostMTLAction", "MinimizeRGL"},
			{"EnableSlowCompare", "False"},
			{"EnableLegacyAuth", "False"},
			{"GTAWindowTitle", "Grand Theft Auto V"},
			{"Version", "127"},
			//{"EnableCopyFilesInsteadOfSyslinking_SocialClub", "False"},
			{"ExitWay", "Close"},
			{"StartWay", "Maximized"},
			{"Mode", "default"},
			{"DMMode", "default"},
	
			// GTA V Settings
			{"Retailer", "Steam"},
			{"LanguageSelected", "English"},
			{"InGameName", "HiMomImOnYoutube"},
			{"EnablePreOrderBonus", "False"},
            {"EnableReturningPlayer", "False"},
            {"EnableDontLaunchThroughSteam", "False"},
			{"EnableScripthookOnDowngraded", "False"},
			{"EnableOverWriteGTACommandLineArgs", "False"},
			{"EnableCoreFix", "True"},
			{"EnableRunAsAdminDowngraded", "False"},
			{"OverWriteGTACommandLineArgs", ""},
   
			// Extra Features
			{"EnableOverlay", "False"},
			{"EnableAutoStartJumpScript", "False" },
			{"JumpScriptKey1", "32" },
			{"JumpScriptKey2", "76" },
			{"EnableJumpscriptUseCustomScript", "False" },
			{"EnableAutoSetHighPriority", "True" },

			// Auto start Shit
			{"EnableOnlyAutoStartProgramsWhenDowngraded", "True"},
			{"EnableAutoStartLiveSplit", "False" },
			{"PathLiveSplit", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartStreamProgram", "False" },
			{"PathStreamProgram", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartFPSLimiter", "False" },
			{"PathFPSLimiter", @"C:\Some\Path\SomeFile.exe" },
			{"EnableAutoStartNohboard", "False" },
			{"PathNohboard", @"C:\Some\Path\SomeFile.exe" },

			// Overlay shit
			{"KeyOverlayToggle", "163" },
			{"KeyOverlayScrollUp", "109" },
			{"KeyOverlayScrollDown", "107" },
			{"KeyOverlayScrollRight", "106" },
			{"KeyOverlayScrollLeft", "111" },
			{"KeyOverlayNoteNext", "105" },
			{"KeyOverlayNotePrev", "103" },

			{"OverlayMultiMonitorMode", "False" },
			{"OverlayBackground", "100,0,0,0" },
			{"OverlayForeground", "255,255,0,255" },
			{"OverlayLocation", "TopLeft" },
			{"OverlayMarginX", "10" },
			{"OverlayMarginY", "10" },
			{"OverlayWidth", "580" },
			{"OverlayHeight", "500" },
			{"OverlayTextFont", "Arial" },
			{"OverlayTextSize", "24" },
			{"OL_MM_Left", "0" },
			{"OL_MM_Top", "0" },

			{"OverlayBackgroundImages", ""},
			{"OverlaySelectedBackground", ""},

			{"OverlayNotesMain","Note1.txt;Note2.txt;Note3.txt;Note4.txt"},
			{"OverlayNotesPresetA",""},
			{"OverlayNotesPresetB",""},
			{"OverlayNotesPresetC",""},
			{"OverlayNotesPresetD",""},
			{"OverlayNotesPresetE",""},
			{"OverlayNotesPresetF",""},

			{"SpecialPatcherEnabledSetting","False"},
			// {"SpecialPatcherToggleKey","F14"}, // gonna use the same way of storing Keys like all other Keybinds. New Regidit value is called SpecialPatcherKey, ToggleKey is NOT used anymore
			{"SpecialPatcherKey","112"},
			{"SpecialPatcherPatches", "{}"},
			{"PointerPathTesterEnabled", "False" },
			{"PointerPathTesterString", "" },
		};

		/// <summary>
		/// Property of our Settings (Dictionary). Gets the default values on initiating the program. Our Settings will get read from registry on the Init functions.
		/// </summary>
		public static Dictionary<string, string> MySettings { get; private set; } = MyDefaultSettings.ToDictionary(entry => entry.Key, entry => entry.Value); // https://stackoverflow.com/a/139626

		#endregion

		// Settings stuff above

		// Init and exit Code below

		#region Init and Exit

		/// <summary>
		/// Init function which gets called at the very beginning
		/// </summary>
		public static void Init()
		{
			// Initiates Logging
			// This is also responsible for the intial first few messages on startup.
			HelperClasses.Logger.Init();

			// Initiates the Settings
			// Writes Settings Dictionary [copy of default settings at this point] in the registry if the value doesnt already exist
			// then reads the Regedit Values in the Settings Dictionary
			Settings.Init();

			// Rolling Log stuff
			HelperClasses.Logger.RollingLog();

			// Checks if we are doing first Launch.
			if (Settings.FirstLaunch)
			{
				string msg = "Legal Disclaimer:\nWe (and Project 1.27) are not responsible for anything that happens to:\nYour Windows, your hardware, your PC,\nyour GTA, your Social Club account etc. The \"Remember Me\" function (for legacy auth) is storing your credentials\nusing the Windows Credential Manager.\n\nBy clicking 'OK' you agree to those terms.\n\n- The Project 1.27 Team";

				new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();

				// Set Own Installation Path in Regedit Settings
				HelperClasses.Logger.Log("FirstLaunch Procedure Started");
				HelperClasses.Logger.Log("Setting Installation Path to '" + ProjectInstallationPath + "'", 1);
				Settings.SetSetting("InstallationPath", ProjectInstallationPath);

				// Calling this to get the Path automatically
				Settings.InitImportantSettings();

				Settings.AntiVirusFix();

				// Set FirstLaunch to false
				Settings.FirstLaunch = false;

				HelperClasses.Logger.Log("FirstLaunch Procedure Ended");
			}


			// Just checks if the GTAVInstallationPath is empty.
			// So we dont have to "Force" the path every startup...
			while (String.IsNullOrEmpty(Settings.GTAVInstallationPath) || String.IsNullOrEmpty(Settings.ZIPExtractionPath))
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

					Globals.SetUpDownloadManager(false);
					ComponentManager.ZIPVersionSwitcheroo();

					Settings.P127Mode = "default";

					HelperClasses.FileHandling.createPath(MySaveFile.BackupSavesPath.TrimEnd('\\') + @"\New Folder");
					HelperClasses.FileHandling.createPath(MySaveFile.BackupSavesPath.TrimEnd('\\') + @"\YouCanRightclick");

					string[] Files = HelperClasses.FileHandling.GetFilesFromFolder(ProjectInstallationPath);
					foreach (string file in Files)
					{
						if (HelperClasses.FileHandling.PathSplitUp(file)[1].Contains("internal"))
						{
							HelperClasses.FileHandling.deleteFile(file);
						}
					}
					HelperClasses.RegeditHandler.SetValue("TeasingFeatures", "True");
				}


				if (Settings.LastLaunchedVersion < new Version("1.2.0.0"))
				{
					Settings.P127Mode = "default";
					Settings.DMMode = "default";

					string msg = "Legal Disclaimer:\nWe (and Project 1.27) are not responsible for anything that happens to:\nYour Windows, your harware, your PC,\nyour GTA, your Social Club account etc.\nBy clicking 'OK' you agree to those terms.\n\n- The Project 1.27 Team";

					new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();

					HelperClasses.RegeditHandler.DeleteValue("EnableCopyFilesInsteadOfSyslinking_SocialClub");
					HelperClasses.RegeditHandler.DeleteValue("TeasingFeatures");

					if (Settings.EnableLegacyAuth)
					{
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "The captcha-free-Authentication (MTL) has been improved,\nand should be working for everyone on this version.\nWould you like to enable it?");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
						{
							Settings.EnableLegacyAuth = false;
						}
					}

					Settings.TellRockstarUsersToDisableAutoUpdateIfNeeded();

					Settings.AllFilesEverPlacedInsideGTAMyAdd("asmjit.dll");
					Settings.AllFilesEverPlacedInsideGTAMyAdd("botan.dll");
					Settings.AllFilesEverPlacedInsideGTAMyAdd("launc.dll");
					Settings.AllFilesEverPlacedInsideGTAMyAdd("origi_socialclub.dll");
					Settings.AllFilesEverPlacedInsideGTAMyAdd("Readme.txt");
					Settings.AllFilesEverPlacedInsideGTAMyAdd("socialclub.dll");
					Settings.AllFilesEverPlacedInsideGTAMyAdd("tinyxml2.dll");

					List<string> tmp = new List<string>();
					tmp.AddRange(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder("").ToList<string>());
					tmp = tmp.Distinct().ToList();

					string[] tmp1 = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeEmuFilePath);
					foreach (string tmp_i in tmp1)
					{
						Settings.AllFilesEverPlacedInsideGTAMyAdd(tmp_i.Substring(LauncherLogic.DowngradeEmuFilePath.Length).TrimStart('\\'));
					}

					string[] tmp2 = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeAlternativeFilePathRockstar124);
					foreach (string tmp_i in tmp2)
					{
						Settings.AllFilesEverPlacedInsideGTAMyAdd(tmp_i.Substring(LauncherLogic.DowngradeAlternativeFilePathRockstar124.Length).TrimStart('\\'));
					}

					string[] tmp3 = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeAlternativeFilePathRockstar127);
					foreach (string tmp_i in tmp3)
					{
						Settings.AllFilesEverPlacedInsideGTAMyAdd(tmp_i.Substring(LauncherLogic.DowngradeAlternativeFilePathRockstar127.Length).TrimStart('\\'));
					}

					string[] tmp4 = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeAlternativeFilePathSteam127);
					foreach (string tmp_i in tmp4)
					{
						Settings.AllFilesEverPlacedInsideGTAMyAdd(tmp_i.Substring(LauncherLogic.DowngradeAlternativeFilePathSteam127.Length).TrimStart('\\'));
					}

					string[] tmp5 = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeAlternativeFilePathSteam124);
					foreach (string tmp_i in tmp5)
					{
						Settings.AllFilesEverPlacedInsideGTAMyAdd(tmp_i.Substring(LauncherLogic.DowngradeAlternativeFilePathSteam124.Length).TrimStart('\\'));
					}

					string[] tmp6 = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.UpgradeFilePath);
					foreach (string tmp_i in tmp6)
					{
						Settings.AllFilesEverPlacedInsideGTAMyAdd(tmp_i.Substring(LauncherLogic.UpgradeFilePath.Length).TrimStart('\\'));
					}

					string[] tmp7 = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.UpgradeFilePathBackup);
					foreach (string tmp_i in tmp7)
					{
						Settings.AllFilesEverPlacedInsideGTAMyAdd(tmp_i.Substring(LauncherLogic.UpgradeFilePathBackup.Length).TrimStart('\\'));
					}

					Settings.AntiVirusFix();
				}

				if (Settings.LastLaunchedVersion < new Version("1.2.2.0"))
				{
					HelperClasses.FileHandling.deleteFile(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\LICENSE_JUMPSCRIPT");
					HelperClasses.FileHandling.deleteFile(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\LICENSE");
					HelperClasses.FileHandling.deleteFile(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\LICENSE_JUMPSCRIPT");
					HelperClasses.FileHandling.deleteFile(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\LICENSE");
				}

				if (Settings.LastLaunchedVersion < new Version("1.2.3.0"))
				{
					Settings.AllFilesEverPlacedInsideGTAMyAdd("P127_ASMPATCHER_SCRIPTHOOK.dll");
				}

				 Settings.LastLaunchedVersion = Globals.ProjectVersion;
			}


			// Deleting all Installer and ZIP Files from own Project Installation Path
			DeleteOldFiles();

			// Throw annoucements
			HandleAnnouncements();

			// Auto Updater
			CheckForUpdate();

			// Loading Info for Version stuff.
			HelperClasses.BuildVersionTable.ReadFromGithub();


			// SetUpDownloadManager
			SetUpDownloadManager();

			// reading Social club install dir from registry
			LaunchAlternative.SetUpSocialClubRegistryThing();
			LaunchAlternative.SocialClubUpgrade();

			// OUTDATED
			// Downloads the "big 3" gamefiles from github release
			//CheckForBigThree();

			// OUTDATED
			// Check whats the latest Version of the ZIP File in GITHUB
			// CheckForZipUpdate();



			// Called on Window Loaded from MainWindow, since this shows Overlay_MM WPF Window
			// this makes its parent window show super early, which is ugly.
			// NoteOverlay.OverlaySettingsChanged();

			// INIT the Inter-Process-Communication via Named Pipes. Shoutout to dr490n

			initIPC();

			//INIT Special Patcher
			initGamePatches();

			// Read Settings of Emu Profile for ReturningPlayerContent
			LauncherLogic.SetReturningPlayerBonusSetting();

			//Init pointer-path tester
			preparsedPPs = ASPointerPath.pointerPathParse(Settings.PointerPathTesterString);


				Settings.GTAWindowTitle = GTAOverlay.targetWindowBorderlessDefault;

			// INIT the dynamic text handler for the overlay
			initDynamicTextGetters();


			MainWindow.MW.StartMTLDispatcherTimer();

			HelperClasses.Logger.Log("Only CEF Init to go...");

			Auth.ROSIntegration.CEFInitialize();

			if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.DragonEmu)
			{
				if (LauncherLogic.AuthWay == LauncherLogic.AuthWays.MTL)
				{
					if (Settings.AutoMTLAuthOnStartup)
					{
						ROSIntegration.MTLAuth(false, true);
					}
				}
			}
		}



		/// <summary>
		/// Sets up the IPC server
		/// </summary>
		private static void initIPC()
		{
			pipeServer.registerEndpoint("messageBox", (a) =>
			{
				System.Windows.Forms.MessageBox.Show(Encoding.UTF8.GetString(a));
				return a;
			});

			pipeServer.registerEndpoint("getBaseToken", Auth.ROSCommunicationBackend.GenLaunchBase);

			pipeServer.registerEndpoint("log", a =>
			{
				HelperClasses.Logger.Log(Encoding.UTF8.GetString(a).TrimEnd('\0'));
				return a.Take(1).ToArray();
			});

			pipeServer.registerEndpoint("pleaseshow", a =>
			{
				MainWindow.MW.Dispatcher.Invoke(() =>
				{
					MainWindow.MW.menuItem_Show_Click(null, null);
				});
				return new byte[] { Convert.ToByte(true) };
			});

			
			pipeServer.registerEndpoint("getActivePatches", () =>
			{
				return Settings.SpecialPatcherEnabled ? SpecialPatchHandler.patchBlob : BitConverter.GetBytes(0);
			});

			
			

			pipeServer.run();

			//var pc = new IPCPipeClient("Project127Launcher");
			//pc.call("test", Encoding.UTF8.GetBytes("Hi"));
		}

		/// <summary>
		/// Sets up the special game patcher
		/// </summary>
		private static void initGamePatches()
        {
            foreach (var p in HelperClasses.SpecialPatchHandler.patch.GetPatches())
            {
				if (p.DefaultEnabled)
                {
					p.Enabled = true;
                }
            }
			HelperClasses.SpecialPatchHandler.checkCopyScripthook();
        }

		/// <summary>
		/// Handles pointer path interpretation similarly to AutoSplit
		/// </summary>
		public static ASPointerPath GTAPointerPathHandler = new ASPointerPath("GTA5");

		/// <summary>
		/// Sets up the overlay dynamic text handler
		/// </summary>
		private static void initDynamicTextGetters()
		{
			/*
			 * Currently Supported Variables:
			 * $missions
			 * $sandf
			 * $usj
			 * $bridges
			 * $randevs
			 * $hobbies
			 * $cutscene
			 * $script
			 * $scriptPretty
			 * $percent
			 * $golfhole
			 */

			Func<Func<string>, string> baseHandler = a =>
			 {
				 if (RunningGTABuild == new Version(0, 0))
				 {
					 return "[NF]";
				 }
				 else if (RunningGTABuild != new Version(1, 0, 372, 2))
				 {
					 return "[N/A]";
				 }
				 try
				 {
					 return a();
				 }
				 catch
				 {
					 return "[ERR]";
				 }
			 };
			DynamicText.registerVarGetter("ctime", () => DateTime.Now.ToString());
			DynamicText.registerVarGetter("missions", () => baseHandler(
				() => GTAPointerPathHandler.EvalPointerPath_I32(
					stateVarsCurrent["missionCounter"]
					).ToString()
				)
			);
			DynamicText.registerVarGetter("sandf", () => baseHandler(
				() => GTAPointerPathHandler.EvalPointerPath_I32(
					stateVarsCurrent["sAndFCounter"]
					).ToString()
				)
			);
			DynamicText.registerVarGetter("usj", () => baseHandler(
				 () => GTAPointerPathHandler.EvalPointerPath_I32(
					 stateVarsCurrent["uniqueStuntJumps"]
					 ).ToString()
				 )
			);
			DynamicText.registerVarGetter("bridges", () => baseHandler(
				 () => GTAPointerPathHandler.EvalPointerPath_I32(
					 stateVarsCurrent["bridgeCounter"]
					 ).ToString()
				 )
			);
			DynamicText.registerVarGetter("randevs", () => baseHandler(
				 () => GTAPointerPathHandler.EvalPointerPath_I32(
					 stateVarsCurrent["randEvCounter"]
					 ).ToString()
				 )
			);
			DynamicText.registerVarGetter("hobbies", () => baseHandler(
				 () => GTAPointerPathHandler.EvalPointerPath_I32(
					 stateVarsCurrent["hobbies"]
					 ).ToString()
				 )
			);
			DynamicText.registerVarGetter("cutscene", () => baseHandler(
				 () =>
				 {
					 var cc = GTAPointerPathHandler.EvalPointerPath(255,
						 stateVarsCurrent["currCutscene"]);
					 if (cc == null)
					 {
						 return "NONE";
					 }
					 else
					 {
						 var s = Encoding.UTF8.GetString(cc.TakeWhile(a => a != '\0').ToArray());
						 return (s == "") ? "NONE" : s;
					 }
				 }
				 )
			);
			DynamicText.registerVarGetter("script", () => baseHandler(
				 () =>
				 {
					 var cs = GTAPointerPathHandler.EvalPointerPath(255,
						 stateVarsCurrent["currScript"]);
					 if (cs == null)
					 {
						 return "NONE";
					 }
					 else
					 {
						 var s = Encoding.UTF8.GetString(cs.TakeWhile(a => a != '\0').ToArray());
						 return (s == "") ? "NONE" : s;
					 }
				 }
				 )
			);
			DynamicText.registerVarGetter("scriptPretty", () => baseHandler(
				 () =>
				 {
					 var cc = GTAPointerPathHandler.EvalPointerPath(255,
						 stateVarsCurrent["currScript"]);
					 if (cc == null)
					 {
						 return "NONE";
					 }
					 else
					 {
						 var s = Encoding.UTF8.GetString(cc.TakeWhile(a => a != '\0').ToArray());
						 return (s == "") ? "NONE" : MissionIdentifiers.getMissionInfo(s).Item1;
					 }
				 }
				 )
			);
			DynamicText.registerVarGetter("percent", () => baseHandler(
				 () => GTAPointerPathHandler.EvalPointerPath_fp32(
					 stateVarsCurrent["percent"]
					 ).ToString("##0.00")
				 )
			);
			DynamicText.registerVarGetter("golfhole", () => baseHandler(
				 () => GTAPointerPathHandler.EvalPointerPath_I32(
					 stateVarsCurrent["golfHole"]
					 ).ToString()
				 )
			);
			DynamicText.registerVarGetter("spEnabled", () => SpecialPatchHandler.PatcherEnabled.ToString());

			

			DynamicText.registerVarGetter("ppTester", () =>
			{
				if (!Settings.PointerPathTesterEnabled)
				{
					return "";
				}
				try
                {
					string output = "";
					foreach(var pp in preparsedPPs)
                    {
						output += pp.Name + ":";

						try
						{
							var ret = pp.evaluate();
							if (ret.Item1 != typeof(Byte[]))
                            {
								output += ret.Item2.ToString() + '\n';
							}
                            else
                            {
								output += BitConverter.ToString((Byte[])ret.Item2).Replace("-", "").ToLower();
                            }
						}
                        catch
                        {
							output += "[ERR]\n";
                        }
                    }
					return output;
                }
                catch
                {
					return "Eval Error!";
                }
				
			});
		}

		/// <summary>
		/// Pre-parsed pointer paths
		/// </summary>
		public static List<ASPointerPath.pointerPath> preparsedPPs;

		/// <summary>
		/// Dictionary with all pointer paths for 1.27 vars (steam)
		/// </summary>
		private static Dictionary<string, int[]> stateVarsSteam = new Dictionary<string, int[]>
		{
			{"missionCounter", new int[]{ 0x2A0D4B0, 0xBDA08 } },
			{"sAndFCounter", new int[]{ 0x2A0D4B0, 0xBDA20 } },
			{"uniqueStuntJumps", new int[]{ 0x2193E58, 0x10378 } },
			{"bridgeCounter", new int[]{ 0x2A0D4B0, 0x30318 } },
			{"randEvCounter", new int[]{ 0x2A0D4B0, 0xBDA28 } },
			{"hobbies", new int[]{ 0x2A0D4B0, 0xBDA10 } },
			{"currCutscene", new int[]{ 0x01CB8530, 0xB70 } },
			{"currScript", new int[]{ 0x1CB8710 } },
			//{"loading", new int[]{ 0x2157FA0 } },
			{"percent", new int[]{ 0x0218FAD8, 0x18068 } },
			{"golfHole", new int[]{ 0x1DDC004 } }

		};

		/// <summary>
		/// Dictionary with all pointer paths for 1.27 vars (RGL)
		/// </summary>
		private static Dictionary<string, int[]> stateVarsRGL = new Dictionary<string, int[]>
		{
			{"missionCounter", new int[]{ 0x2A07E70, 0xBDA08 } },
			{"sAndFCounter", new int[]{ 0x2A07E70, 0xBDA20 } },
			{"uniqueStuntJumps", new int[]{ 0x2A07E70, 0xCE5C0 } },
			{"bridgeCounter", new int[]{ 0x2A07EC8, 0x40318 } },
			{"randEvCounter", new int[]{ 0x2A07E70, 0xBDA28 } },
			{"hobbies", new int[]{ 0x2A07E70, 0xBDA10 } },
			{"currCutscene", new int[]{ 0x01CB44A0, 0xB70 } },
			{"currScript", new int[]{ 0x1CB4340 } },
			//{"loading", new int[]{ 0x2153C30 } },
			{"percent", new int[]{ 0x0218FAD8, 0x18068 } },
			{"golfHole", new int[]{ 0x1DE3970 } }

		};

		/// <summary>
		/// Dictionary with all pointer paths for 1.27 vars
		/// </summary>
		public static Dictionary<string, int[]> stateVarsCurrent
		{
			get
			{
				if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch &&
					Settings.Retailer == Settings.Retailers.Steam)
				{
					return stateVarsSteam;
				}
				else
				{
					return stateVarsRGL;
				}
			}
		}

		/// <summary>
		/// Proper Exit Method. EMPTY FOR NOW. Get called when closed (user and taskmgr) and when PC is shutdown. Not when process is killed or power ist lost.
		/// </summary>
		public static void ProperExit()
		{
			NoteOverlay.DisposeAllOverlayStuff();
			Jumpscript.StopJumpscript();
			MainWindow.MyDispatcherTimer.Stop();
			MainWindow.MTLAuthTimer.Stop();
			MainWindow.myMutex.ReleaseMutex();
			MainWindow.MW.notifyIcon.Visible = false;
			MainWindow.MW.notifyIcon.Dispose();
			HelperClasses.Logger.Log("Program closed. Proper Exit. Ended normally");
			try
			{
				MainWindow.MW.Close();
			}
			catch { }
			Application.Current.Shutdown();
			Environment.Exit(0);
		}



		#endregion

		// Init and exit stuff above

		// Update Stuff below

		#region UpdateShit


		public static HelperClasses.DownloadManager MyDM;

		public static void SetUpDownloadManager(bool StartupCheck = true)
		{
			MyDM = new HelperClasses.DownloadManager(Globals.XML_DownloadManager);
			if (StartupCheck)
			{
				ComponentManager.StartupCheck();
			}
		}



		/// <summary>
		/// Method which does the UpdateCheck on Startup
		/// </summary>
		public static void CheckForUpdate(string XML_Autoupdate_Temp = "")
		{
			if (XML_Autoupdate_Temp == "")
			{
				XML_Autoupdate_Temp = XML_AutoUpdate;
			}


			// Check online File for Version.
			string MyVersionOnlineString = HelperClasses.FileHandling.GetXMLTagContent(XML_Autoupdate_Temp, "version");

			// Just so we have one big code snippet we can exit at any point we want.
			while (true)
			{
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
						HelperClasses.Logger.Log("Update found (Version Check returning true).", 1);
						HelperClasses.Logger.Log("Checking if URL is reachable.", 1);

						string DLPath = HelperClasses.FileHandling.GetXMLTagContent(XML_Autoupdate_Temp, "url");
						string DLFilename = DLPath.Substring(DLPath.LastIndexOf('/') + 1);
						string LocalFileName = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\" + DLFilename;

						if (!HelperClasses.FileHandling.URLExists(DLPath, 2500))
						{
							HelperClasses.Logger.Log("Cant reach URL, will throw choice");
							Popup yesno2 = new Popup(Popup.PopupWindowTypes.PopupYesNo, "There is an Update, but P127 cant seem to reach it. Do you want to try to get the Update anyways?");
							yesno2.ShowDialog();
							if (yesno2.DialogResult == true)
							{
								HelperClasses.Logger.Log("Cant reach URL, Choice thrown, will try to get Update anyways.");
							}
							else
							{
								// Do last Lines of this function anyway. Cant hurt.
								HelperClasses.Logger.Log("Cant reach URL, Choice thrown, will NOT try to update.");
								break;
							}
						}

						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Version: '" + MyVersionOnline.ToString() + "' found on the Server.\nVersion: '" + Globals.ProjectVersion.ToString() + "' found installed.\nDo you want to upgrade?");
						yesno.ShowDialog();
						// Asking User if he wants update.
						if (yesno.DialogResult == true)
						{
							// User wants Update
							HelperClasses.Logger.Log("Presented Update Choice to User. User wants it", 1);

							HelperClasses.FileHandling.deleteFile(LocalFileName);
							new PopupDownload(DLPath, LocalFileName, "Installer").ShowDialog();

							if (HelperClasses.FileHandling.GetSizeOfFile(LocalFileName) > 50000)
							{
								HelperClasses.Logger.Log("Installer on Disk looks good, Starting Installer, Exits this now.", 1);
								HelperClasses.ProcessHandler.StartProcess(LocalFileName);
								Globals.ProperExit();
							}
							else
							{
								HelperClasses.Logger.Log("Installer on Disk does not look good. FileSize: '" + HelperClasses.FileHandling.GetSizeOfFile(LocalFileName) + "'. Will not start it. Will not exit this.", 1);
							}
						}
						else
						{
							// User doesnt want update
							HelperClasses.Logger.Log("User does not wants update", 1);
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
				break;
			}

			HelperClasses.BuildVersionTable.ReadFromGithub(XML_Autoupdate_Temp);
			ReadMe.DynamicLinksMethod(XML_Autoupdate_Temp);
		}


		/// <summary>
		/// Checks Github for the big 3 files we need
		/// </summary>
		public static void CheckForBigThree()
		{
			//HelperClasses.Logger.Log("Downloading the 'big three' files");

			//bool PopupThrownAlready = false;

			//string UpdateXML = XML_AutoUpdate;

			//string DLLinkG = HelperClasses.FileHandling.GetXMLTagContent(UpdateXML, "DLLinkG");
			//string DLLinkGHash = HelperClasses.FileHandling.GetXMLTagContent(UpdateXML, "DLLinkGHash");
			//string DLLinkU = HelperClasses.FileHandling.GetXMLTagContent(UpdateXML, "DLLinkU");
			//string DLLinkUHash = HelperClasses.FileHandling.GetXMLTagContent(UpdateXML, "DLLinkUHash");
			//string DLLinkX = HelperClasses.FileHandling.GetXMLTagContent(UpdateXML, "DLLinkX");
			//string DLLinkXHash = HelperClasses.FileHandling.GetXMLTagContent(UpdateXML, "DLLinkXHash");

			//HelperClasses.Logger.Log("Checking if gta5.exe exists locally", 1);
			//if (HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") > 100)
			//{
			//	HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			//}
			//else
			//{
			//	HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);

			//	if (PopupThrownAlready == false)
			//	{
			//		Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "You are missing Files required for Downgrading.\nDo you want to download these now?\nI recommend clicking 'Yes'");
			//		yesno.ShowDialog();
			//		PopupThrownAlready = true;
			//		if (yesno.DialogResult == false)
			//		{
			//			HelperClasses.Logger.Log("Well user doesnt want to Download Files...alright then");
			//			return;
			//		}
			//	}

			//	new PopupDownload(DLLinkG, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe", "Needed Files (gta5.exe 1/3)").ShowDialog();

			//	if (!string.IsNullOrWhiteSpace(DLLinkGHash))
			//	{
			//		HelperClasses.Logger.Log("We do have a Hash for that file. Lets compare it:", 2);
			//		HelperClasses.Logger.Log("Hash we want: '" + DLLinkGHash + "'", 3);
			//		HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + "'", 3);
			//		while (HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") != DLLinkGHash)
			//		{
			//			HelperClasses.Logger.Log("Well..hashes dont match shit. Lets try again", 2);
			//			HelperClasses.FileHandling.deleteFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe");
			//			new PopupDownload(DLLinkG, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe", "Needed Files (gta5.exe 1/3)").ShowDialog();
			//			HelperClasses.Logger.Log("Hash we want: '" + DLLinkGHash + "'", 3);
			//			HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + "'", 3);
			//		}
			//	}
			//}

			//HelperClasses.Logger.Log("Checking if x64a.rpf exists locally", 1);
			//if (HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf") > 100)
			//{
			//	HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			//}
			//else
			//{
			//	HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);

			//	if (PopupThrownAlready == false)
			//	{
			//		Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "You are missing Files required for Downgrading.\nDo you want to download these now?\nI recommend clicking 'Yes'");
			//		yesno.ShowDialog();
			//		PopupThrownAlready = true;
			//		if (yesno.DialogResult == false)
			//		{
			//			HelperClasses.Logger.Log("Well user doesnt want to Download Files...alright then");
			//			return;
			//		}
			//	}

			//	new PopupDownload(DLLinkX, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf", "Needed Files (x64a.rpf, 2/3)").ShowDialog();

			//	if (!string.IsNullOrWhiteSpace(DLLinkXHash))
			//	{
			//		HelperClasses.Logger.Log("We do have a Hash for that file. Lets compare it:", 2);
			//		HelperClasses.Logger.Log("Hash we want: '" + DLLinkXHash + "'", 3);
			//		HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf") + "'", 3);
			//		while (HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf") != DLLinkXHash)
			//		{
			//			HelperClasses.Logger.Log("Well..hashes dont match shit. Lets try again", 2);
			//			HelperClasses.FileHandling.deleteFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf");
			//			new PopupDownload(DLLinkX, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf", "Needed Files (x64a.rpf, 2/3)").ShowDialog();
			//			HelperClasses.Logger.Log("Hash we want: '" + DLLinkXHash + "'", 3);
			//			HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf") + "'", 3);
			//		}
			//	}
			//}

			//HelperClasses.Logger.Log(@"Checking if update\update.rpf exists locally", 1);
			//if (HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf") > 100)
			//{
			//	HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			//}
			//else
			//{
			//	HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);

			//	if (PopupThrownAlready == false)
			//	{
			//		Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "You are missing Files required for Downgrading.\nDo you want to download these now?\nI recommend clicking 'Yes'");
			//		yesno.ShowDialog();
			//		PopupThrownAlready = true;
			//		if (yesno.DialogResult == false)
			//		{
			//			HelperClasses.Logger.Log("Well user doesnt want to Download Files...alright then");
			//			return;
			//		}
			//	}

			//	new PopupDownload(DLLinkU, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf", "Needed Files (Update.rpf, 3/3)").ShowDialog();

			//	if (!string.IsNullOrWhiteSpace(DLLinkUHash))
			//	{
			//		HelperClasses.Logger.Log("We do have a Hash for that file. Lets compare it:", 2);
			//		HelperClasses.Logger.Log("Hash we want: '" + DLLinkUHash + "'", 3);
			//		HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf") + "'", 3);
			//		while (HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf") != DLLinkUHash)
			//		{
			//			HelperClasses.Logger.Log("Well..hashes dont match shit. Lets try again", 2);
			//			HelperClasses.FileHandling.deleteFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf");
			//			new PopupDownload(DLLinkU, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf", "Needed Files (update.rpf, 3/3)").ShowDialog();
			//			HelperClasses.Logger.Log("Hash we want: '" + DLLinkUHash + "'", 3);
			//			HelperClasses.Logger.Log("Hash we have: '" + HelperClasses.FileHandling.GetHashFromFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf") + "'", 3);
			//		}
			//	}
			//}
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
			//// Check whats the latest Version of the ZIP File in GITHUB
			//int ZipOnlineVersion = 0;
			//Int32.TryParse(HelperClasses.FileHandling.GetXMLTagContent(XML_AutoUpdate, "zipversion"), out ZipOnlineVersion);

			//HelperClasses.Logger.Log("Checking for ZIP - Update");
			//HelperClasses.Logger.Log("ZipVersion = '" + Globals.ZipVersion + "', ZipOnlineVersion = '" + ZipOnlineVersion + "'");

			//// If Zip file from Server is newer
			//if (ZipOnlineVersion > Globals.ZipVersion)
			//{
			//	HelperClasses.Logger.Log("Update for ZIP found");
			//	Popup yesno;
			//	if (Globals.ZipVersion > 0)
			//	{
			//		yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "ZIP Version: '" + ZipOnlineVersion.ToString() + "' found on the Server.\nZIP Version: '" + Globals.ZipVersion.ToString() + "' found installed.\nDo you want to upgrade?");
			//	}
			//	else
			//	{
			//		yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "ZIP Version: '" + ZipOnlineVersion.ToString() + "' found on the Server.\nNo ZIP Version found installed.\nDo you want to install the ZIP?");
			//	}
			//	yesno.ShowDialog();
			//	if (yesno.DialogResult == true)
			//	{
			//		HelperClasses.Logger.Log("User wants update for ZIP");

			//		ZipUpdate();
			//	}
			//	else
			//	{
			//		HelperClasses.Logger.Log("User does not want update for ZIP");
			//	}
			//}
			//else
			//{
			//	HelperClasses.Logger.Log("NO Update for ZIP found");
			//}
		}


		public static void ZipUpdate()
		{
			//string TMP_AutoupdateXML = Globals.XML_AutoUpdate;

			//// Getting the Hash of the new ZIPFile
			//string hashNeeded = HelperClasses.FileHandling.GetXMLTagContent(TMP_AutoupdateXML, "zipmd5");
			//HelperClasses.Logger.Log("HashNeeded: " + hashNeeded);

			//// Looping 0 through 5
			//for (int i = 0; i <= 5; i++)
			//{
			//	// Getting DL Link of zip + i
			//	string pathOfNewZip = HelperClasses.FileHandling.GetXMLTagContent(TMP_AutoupdateXML, "zip" + i.ToString());
			//	HelperClasses.Logger.Log("Zip-Try: 'zip" + i.ToString() + "'");
			//	HelperClasses.Logger.Log("DL Link: '" + pathOfNewZip + "'");

			//	// Deleting old ZIPFile
			//	HelperClasses.FileHandling.deleteFile(Globals.ZipFileDownloadLocation);

			//	// Getting actual DDL
			//	pathOfNewZip = GetDDL(pathOfNewZip);

			//	// Downloading the ZIP File
			//	new PopupDownload(pathOfNewZip, Globals.ZipFileDownloadLocation, "ZIP-File").ShowDialog();

			//	// Checking the hash of the Download
			//	string HashOfDownload = HelperClasses.FileHandling.GetHashFromFile(Globals.ZipFileDownloadLocation);
			//	HelperClasses.Logger.Log("Download Done, Hash of Downloaded File: '" + HashOfDownload + "'");

			//	// If Hash looks good, we import it
			//	if (HashOfDownload == hashNeeded)
			//	{
			//		HelperClasses.Logger.Log("Hashes Match, will Import");
			//		LauncherLogic.ImportZip(Globals.ZipFileDownloadLocation, true);
			//		return;
			//	}
			//	HelperClasses.Logger.Log("Hashes dont match, will move on");
			//}
			//HelperClasses.Logger.Log("Error. Could not find a suitable ZIP File from a FileHoster. Program cannot download new ZIP at the moment.");
			//new Popup(Popup.PopupWindowTypes.PopupOkError, "Update of ZIP File failed (No Suitable ZIP Files Found).\nI suggest restarting the program.");
		}

		#endregion

		// Update Stuff above

		// UI-States (enums) below

		#region UI-States (enums)



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
			NoteOverlay,
			ComponentManager
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

				MainWindow.MW.SetWindowBackgroundBlur();

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
						MainWindow.MW.btn_ComponentManager.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.SaveFileHandler:
						MainWindow.MW.Frame_Main.Content = new SaveFileHandler();
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ComponentManager.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.ReadMe:
						MainWindow.MW.Frame_Main.Content = new ReadMe();
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ComponentManager.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.NoteOverlay:
						MainWindow.MW.Frame_Main.Content = new Overlay.NoteOverlay();
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ComponentManager.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.ComponentManager:
						MainWindow.MW.Frame_Main.Content = new ComponentManager();
						MainWindow.MW.btn_ComponentManager.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.Auth:
						Auth.ROSIntegration.AuthErrorMessageThrownAlready = false;
						Auth.ROSIntegration.MinimizedAlready = false;

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
						MainWindow.MW.btn_ComponentManager.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case PageStates.GTA:
						MainWindow.MW.Frame_Main.Content = new GTA_Page();

						MainWindow.MW.btn_Auth.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ReadMe.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_Settings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_SaveFiles.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_NoteOverlay.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						MainWindow.MW.btn_ComponentManager.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
				}
			}
		}

		/// <summary>
		/// Enum for all BackgroundImages
		/// </summary>
		public enum BackgroundImages
		{
			Default,
			FourTwenty,
			Winter,
			Spooky,
			Valentine,
			Germania,
			Turkey,
			Murica,
			Cat

			// If you touch this, please also change Installer/Info/AdvancedUser.md under #CommandLineOptions accordingly.
		}

		/// <summary>
		/// Internal Value for BackgroundImage
		/// </summary>
		private static BackgroundImages _BackgroundImage = BackgroundImages.Default;

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
				MainWindow.MW.SetWindowBackgroundImage();
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
				MainWindow.MW.SetWindowBackgroundBlur();

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




		#endregion

		// UI-States (enums) above

		// Random shit below

		#region random shit

		public static void ImportBuildFromUrl(string pUrl)
		{
			HelperClasses.Logger.Log("Importing Build from '" + pUrl + "'");

			string pDownloadLocation = ProjectInstallationPath.TrimEnd('\\') + @"\NewBuild.exe";

			new PopupDownload(pUrl, pDownloadLocation, "Custom Build").ShowDialog();

			Process p = new Process();
			p.StartInfo.FileName = ProjectInstallationPath.TrimEnd('\\') + @"\Project 127 Launcher.exe";
			p.StartInfo.WorkingDirectory = ProjectInstallationPath;
			p.StartInfo.Arguments = "-ImportBuild " + "\"" + pDownloadLocation + "\"";
			p.Start();

			Globals.ProperExit();
		}


		/// <summary>
		/// CommandLineArgumentIntepretation(), currently used for Background Image
		/// </summary>
		public static void CommandLineArgumentIntepretation()
		{
			// Code for internal mode is in Globals.Internalmode Getter

			// Need to be in following Format
			// #-examplearg exampleparam#
			// or
			// #-examplearg#
			// ATM NO SUPPORT FOR #-examplearg "exampleparam"#

			string[] args = Globals.CommandLineArgs;


			for (int i = 0; i <= args.Length - 1; i++)
			{
				// i+1 exists
				if (i < args.Length - 1)
				{
					// FOR THIS: 
					// #-examplearg exampleparam#

					// Checks with "2 - part" command line like: "-name test"
					if (args[i].ToLower() == "-background")
					{
						Globals.BackgroundImages Tmp = Globals.BackgroundImages.Default;
						try
						{
							Tmp = (Globals.BackgroundImages)System.Enum.Parse(typeof(Globals.BackgroundImages), args[i + 1], true);
							Globals.BackgroundImage = Tmp;
						}
						catch (Exception e)
						{
							new Popup(Popup.PopupWindowTypes.PopupOkError, "Error converting Command Line Argument to Background Image.\n" + e.ToString()).ShowDialog();
						}
					}
					else if (args[i].ToLower() == "-authstateoverwrite")
					{
						if (args[i + 1].ToLower().StartsWith("-"))
						{
							LauncherLogic.AuthStateOverWrite = true;
						}
						else
						{
							if (args[i + 1].ToLower() == "true")
							{
								LauncherLogic.AuthStateOverWrite = true;
							}
							else
							{
								LauncherLogic.AuthStateOverWrite = false;
							}
						}
					}
					else if (args[i].ToLower() == "-useemudebugfile")
					{
						if (args[i + 1].ToLower().StartsWith("-"))
						{
							LauncherLogic.UseEmuConfigFile = true;
						}
						else
						{
							if (args[i + 1].ToLower() == "true")
							{
								LauncherLogic.UseEmuConfigFile = true;
							}
							else
							{
								LauncherLogic.UseEmuConfigFile = false;
							}
						}
					}
					else if (args[i].ToLower() == "-disablerichard")
					{
						if (args[i + 1].ToLower().StartsWith("-"))
						{
							GTAOverlay.DisableRichard = true;
						}
						else
						{
							if (args[i + 1].ToLower() == "true")
							{
								GTAOverlay.DisableRichard = true;
							}
							else
							{
								GTAOverlay.DisableRichard = false;
							}
						}
					}

					continue;
				}

				if (args[i].ToLower() == "-reset")
				{
					RegeditHandler.DeleteKey();

					string tmp = HelperClasses.RegeditHandler.GetValue("ZIPExtractionPath").TrimEnd('\\') + @"\Project_127_Files\";
					HelperClasses.FileHandling.DeleteFolder(tmp);

					Globals.ProperExit();
				}
				else if (args[i].ToLower() == "-uninstall")
				{
					RegeditHandler.DeleteKey();

					string tmp = HelperClasses.RegeditHandler.GetValue("ZIPExtractionPath").TrimEnd('\\') + @"\Project_127_Files\";
					HelperClasses.FileHandling.DeleteFolder(tmp);

					// run uninstaller
					string FilePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\unins000.exe";
					ProcessHandler.StartProcess(FilePath, Globals.ProjectInstallationPath);

					Globals.ProperExit();
				}
				else if (args[i].ToLower() == "-authstateoverwrite")
				{
					LauncherLogic.AuthStateOverWrite = true;
				}
				else if (args[i].ToLower() == "-useemudebugfile")
				{
					LauncherLogic.UseEmuConfigFile = true;
				}
				else if (args[i].ToLower() == "-disablerichard")
				{
					GTAOverlay.DisableRichard = true;
				}



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
				if (myFile.ToLower().Contains("pleaseshow"))
				{
					HelperClasses.Logger.Log("Found pleaseshow File in the Directory. Will delete it.");
					HelperClasses.FileHandling.deleteFile(myFile);
				}
				if (myFile.ToLower().Contains("Project 1.27.exe" + ".BACKUP"))
				{
					HelperClasses.Logger.Log("Found old build ('.BACKUP'). Will delete it.");
					HelperClasses.FileHandling.deleteFile(myFile);
				}
				if (myFile.ToLower().Contains("dl.zip"))
				{
					HelperClasses.Logger.Log("Found zip File ('DL.ZIP'). Will delete it.");
					HelperClasses.FileHandling.deleteFile(myFile);
				}
			}
		}


		private static void HandleAnnouncements()
		{
			string MyAnnoucment = HelperClasses.FileHandling.GetXMLTagContent(XML_AutoUpdate, "announcement");
			if (MyAnnoucment != "")
			{
				MyAnnoucment = MyAnnoucment.Replace(@"\n", "\n");
				new Popup(Popup.PopupWindowTypes.PopupOk, MyAnnoucment);
			}
		}




		public static string GetGameInfoForDebug(string pFilePath)
		{
			Version tmp = HelperClasses.FileHandling.GetVersionFromFile(pFilePath);
			if (tmp != new Version("0.0.0.1"))
			{
				string rtrn = " [" + tmp.ToString();

				try
				{
					rtrn += " - " + BuildVersionTable.GetNiceGameVersionString(tmp) + "]";
				}
				catch
				{
					rtrn += "]";
				}

				return rtrn;

			}
			return "";
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
		/// DebugPopup Method. Just opens Messagebox with pMsg
		/// </summary>
		/// <param name="pMsg"></param>
		public static void DebugPopup(string pMsg)
		{
			System.Windows.Forms.MessageBox.Show(pMsg);
		}





		#endregion




	} // End of Class
} // End of Namespace
