using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Project_127.MySettings
{
	/// <summary>
	/// Partial Class for Settings Window. 
	/// Also Creates Properties for all Settings, which are easier to interact with than the Dictionary
	/// </summary>
	public partial class Settings : Page
	{

		/*
		Properties of all Settings Values
		Some bigger functions needed for Settings Management
		
		*/


		/// <summary>
		/// Initial Function. Gets Called from Globals.Init which gets called from the Contructor of MainWindow
		/// </summary>
		public static void Init()
		{
			HelperClasses.Logger.Log("Initiating Settings", true, 0);
			HelperClasses.Logger.Log("Initiating Regedit Setup Part of Settings", true, 1);

			// Writes our Settings (Copy of DefaultSettings) to Registry if the Value does not exist.
			HelperClasses.Logger.Log("Writing MySettings (at this point a copy of MyDefaultSettings to the Regedit if the Value doesnt exist", true, 1);
			foreach (KeyValuePair<string, string> KVP in Globals.MySettings)
			{
				if (!(HelperClasses.RegeditHandler.DoesValueExists(KVP.Key)))
				{
					HelperClasses.Logger.Log("Writing '" + KVP.Key.ToString() + "' to the Registry (Value: '" + KVP.Value.ToString() + "') on Startup of P127, because it doesnt exist or is empty", true, 2);
					HelperClasses.RegeditHandler.SetValue(KVP.Key, KVP.Value);
				}
			}

			HelperClasses.Logger.Log("Done Initiating Regedit Part of Settings", true, 1);

			// Read the Registry Values in the Settings Dictionary
			LoadSettings();

			if (Settings.EnableLogging)
			{
				HelperClasses.Logger.Log("Settings initiated and loaded. Will continue Logging.", true, 0);
			}
			else
			{
				HelperClasses.Logger.Log("Settings initiated and loaded. Will stop Logging", true, 0);
			}
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
		/// Method which checks what Setting it recommends (Hardlinking or Copying) and asks user if he wants it
		/// </summary>
		public static void SetDefaultEnableCopyingHardlinking()
		{
			bool currentSetting = Settings.EnableCopyFilesInsteadOfHardlinking;
			bool recommendSetting = !(Settings.ZIPExtractionPath[0] == Settings.GTAVInstallationPath[0]);

			HelperClasses.Logger.Log("Checking to see if Settings.EnableCopyFilesInsteadOfHardlinking is on recommended value");
			HelperClasses.Logger.Log("Settings.ZIPExtractionPath: '" + Settings.ZIPExtractionPath + "'");
			HelperClasses.Logger.Log("Settings.GTAVInstallationPath: '" + Settings.GTAVInstallationPath + "'");
			HelperClasses.Logger.Log("Setting: EnableCopyFilesInsteadOfHardlinking");
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
		/// Gets a specific Setting from the Dictionary. Does not query from Registry.
		/// </summary>
		/// <param name="pKey"></param>
		/// <returns></returns>
		public static string GetSetting(string pKey)
		{
			return Globals.MySettings[pKey];
		}

		/// <summary>
		/// Sets a specific Setting both in the Dictionary and in the Registry
		/// </summary>
		/// <param name="pKey"></param>
		/// <param name="pValue"></param>
		public static void SetSetting(string pKey, string pValue)
		{
			HelperClasses.Logger.Log("Changing Setting '" + pKey + "' to '" + pValue + "'");
			try
			{
				HelperClasses.RegeditHandler.SetValue(pKey, pValue);
				Globals.MySettings[pKey] = pValue;
			}
			catch
			{
				HelperClasses.Logger.Log("Failed to SettingsPartial.cs SetSetting(" + pKey + ", " + pValue + ")");
			}
		}

		/// <summary>
		/// Loads all the Settings from the Registry into the Dictionary
		/// </summary>
		public static void LoadSettings()
		{
			HelperClasses.Logger.Log("Loading Settings from Regedit", true, 1);
			foreach (KeyValuePair<string, string> SingleSetting in Globals.MyDefaultSettings)
			{
				Globals.MySettings[SingleSetting.Key] = HelperClasses.RegeditHandler.GetValue(SingleSetting.Key);
			}
			HelperClasses.Logger.Log("Loaded Settings from Regedit", true, 1);

		}

		/// <summary>
		/// Resets all Settings to Default Settings 
		/// </summary>
		private static void ResetSettings()
		{
			HelperClasses.Logger.Log("Resetting Settings from Regedit", true, 1);
			foreach (KeyValuePair<string, string> SingleDefaultSetting in Globals.MyDefaultSettings)
			{
				SetSetting(SingleDefaultSetting.Key, SingleDefaultSetting.Value);
			}
			HelperClasses.Logger.Log("Resetted Settings from Regedit", true, 1);
		}

		/// <summary>
		/// Gets Bool from String
		/// </summary>
		/// <param name="pString"></param>
		/// <returns></returns>
		private static bool GetBoolFromString(string pString)
		{
			bool tmpBool;
			bool.TryParse(pString, out tmpBool);
			return tmpBool;
		}


		// Below are Properties for all Settings, which is easier to Interact with than the Dictionary


		/// <summary>
		/// Settings FirstLaunch. Gets from the Dictionary.
		/// </summary>
		public static bool FirstLaunch
		{
			get
			{
				return GetBoolFromString(GetSetting("FirstLaunch"));
			}
			set
			{
				SetSetting("FirstLaunch", value.ToString());
			}
		}

		/// <summary>
		/// Settings InstallationPath. Gets and Sets from Dictionary.
		/// </summary>
		public static string InstallationPath
		{
			get
			{
				return GetSetting("InstallationPath");
			}
			set
			{
				SetSetting("InstallationPath", value);
			}
		}

		/// <summary>
		/// Settings LastLaunchedVersion. Gets and Sets from Dictionary.
		/// </summary>
		public static Version LastLaunchedVersion
		{
			get
			{
				return new Version(GetSetting("LastLaunchedVersion"));
			}
			set
			{
				SetSetting("LastLaunchedVersion", value.ToString());
			}
		}

		/// <summary>
		/// Settings GTAVInstallationPath. Gets and Sets from the Dictionary.
		/// </summary>
		public static string GTAVInstallationPath
		{
			get
			{
				return GetSetting("GTAVInstallationPath");
			}
			set
			{
				SetSetting("GTAVInstallationPath", value);
			}
		}

		/// <summary>
		/// Settings ZIPExtractionPath. Gets and Sets from the Dictionary.
		/// </summary>
		public static string ZIPExtractionPath
		{
			get
			{
				return GetSetting("ZIPExtractionPath");
			}
			set
			{
				SetSetting("ZIPExtractionPath", value);
			}
		}

		/// <summary>
		/// Settings P127Mode. Mode / Branch for Update.xml Gets and Sets from the Dictionary.
		/// </summary>
		public static string P127Mode
		{
			get
			{
				return GetSetting("Mode");
			}
			set
			{
				SetSetting("Mode", value);

				if (value.ToLower() != "default")
				{
					MainWindow.MW.btn_lbl_Mode.Content = "Curr P127 Mode: '" + value.ToLower() + "'";
					MainWindow.MW.btn_lbl_Mode.Visibility = Visibility.Visible;
				}
				else
				{
					MainWindow.MW.btn_lbl_Mode.Content = "";
					MainWindow.MW.btn_lbl_Mode.Visibility = Visibility.Hidden;
				}
				MainWindow.MW.btn_lbl_Mode.ToolTip = MainWindow.MW.btn_lbl_Mode.Content;

				Globals.CheckForUpdate();
			}
		}


		/// <summary>
		/// Settings DMMode. Mode / Branch for DownloadManager.xml Gets and Sets from the Dictionary.
		/// </summary>
		public static string DMMode
		{
			get
			{
				return GetSetting("DMMode");
			}
			set
			{
				SetSetting("DMMode", value);

				ComponentManager.SetMode(value.ToLower());

				Globals.SetUpDownloadManager();
			}
		}

		/// <summary>
		/// Settings EnableOnlyAutoStartProgramsWhenDowngraded. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableOnlyAutoStartProgramsWhenDowngraded
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableOnlyAutoStartProgramsWhenDowngraded"));
			}
			set
			{
				SetSetting("EnableOnlyAutoStartProgramsWhenDowngraded", value.ToString());
			}
		}

		/// <summary>
		/// Settings EnableLogging. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableLogging
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableLogging"));
			}
			set
			{
				SetSetting("EnableLogging", value.ToString());
			}
		}

		/// <summary>
		/// Settings EnableSlowCompare. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableSlowCompare
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableSlowCompare"));
			}
			set
			{
				SetSetting("EnableSlowCompare", value.ToString());
			}
		}

		/// <summary>
		/// Settings EnableCopyFilesInsteadOfHardlinking. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableCopyFilesInsteadOfHardlinking
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableCopyFilesInsteadOfHardlinking"));
			}
			set
			{
				SetSetting("EnableCopyFilesInsteadOfHardlinking", value.ToString());
			}
		}


		///// <summary>
		///// Settings EnableCopyFilesInsteadOfSyslinking_SocialClub. Gets and Sets from the Dictionary.
		///// </summary>
		//public static bool EnableCopyFilesInsteadOfSyslinking_SocialClub
		//{
		//	get
		//	{
		//		return GetBoolFromString(GetSetting("EnableCopyFilesInsteadOfSyslinking_SocialClub"));
		//	}
		//	set
		//	{
		//		SetSetting("EnableCopyFilesInsteadOfSyslinking_SocialClub", value.ToString());
		//	}
		//}



		/// <summary>
		/// Settings EnableJumpscriptUseCustomScript. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableJumpscriptUseCustomScript
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableJumpscriptUseCustomScript"));
			}
			set
			{
				if (value != EnableJumpscriptUseCustomScript)
				{
					SetSetting("EnableJumpscriptUseCustomScript", value.ToString());
				}
			}
		}

		/// <summary>
		/// Settings EnableAlternativeLaunch. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableAlternativeLaunch
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAlternativeLaunch"));
			}
			set
			{
				if (ComponentManager.RecommendUpgradedGTA())
				{
					SetSetting("EnableAlternativeLaunch", value.ToString());
					if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
					{
						SetSetting("EnableAlternativeLaunch", (!value).ToString());
						return;
					}
				}
				else
				{
					new Popup(Popup.PopupWindowTypes.PopupOk, "Setting was not changed.");
					return;
				}
				Settings.TellRockstarUsersToDisableAutoUpdateIfNeeded();
			}
		}


		/// <summary>
		/// Settings EnableAlternativeLaunchForceCProgramFiles. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableAlternativeLaunchForceCProgramFiles
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAlternativeLaunchForceCProgramFiles"));
			}
			set
			{
				if (value != EnableAlternativeLaunchForceCProgramFiles)
				{
					SetSetting("EnableAlternativeLaunchForceCProgramFiles", value.ToString());
					LaunchAlternative.SetUpSocialClubRegistryThing();
				}
			}
		}

		/// <summary>
		/// Settings EnablePreOrderBonus. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnablePreOrderBonus
		{
			get
			{
				return GetBoolFromString(GetSetting("EnablePreOrderBonus"));
			}
			set
			{
				SetSetting("EnablePreOrderBonus", value.ToString());
			}
		}


		/// <summary>
		/// Settings InGameName. Gets and Sets from the Dictionary.
		/// </summary>
		public static string InGameName
		{
			get
			{
				string rtrn = Regex.Replace(GetSetting("InGameName"), @"[^0-9A-Za-z_]", @"");
				if (String.IsNullOrEmpty(rtrn)) { rtrn = "HiMomImOnYoutube"; }
				if (rtrn.Length < 3) { rtrn = "HiMomImOnYoutube"; }
				if (rtrn.Length > 16) { rtrn = rtrn.Substring(0, 16); }
				return rtrn;
			}
			set
			{
				SetSetting("InGameName", value);
			}
		}



		public static string ToMyLanguageString(Languages pLanguage)
		{
			if (pLanguage == Languages.English)
			{
				return "american";
			}
			return pLanguage.ToString();
		}


		/// <summary>
		/// Enum for all Languages
		/// </summary>
		public enum Languages
		{
			English,
			Chinese,
			French,
			German,
			Italian,
			Japanese,
			Korean,
			Mexican,
			Polish,
			Portuguese,
			Russian,
			Spanish
		}


		/// <summary>
		/// Enum for all Ways to Start
		/// </summary>
		public enum StartWays
		{
			Maximized,
			Tray
		}

		/// <summary>
		/// Enum for all Ways to Exit
		/// </summary>
		public enum ExitWays
		{
			Minimize,
			HideInTray,
			Close
		}

		/// <summary>
		/// Settings LanguageSelected. Gets and Sets from Dictionary.
		/// </summary>
		public static Languages LanguageSelected
		{
			get
			{
				return (Languages)System.Enum.Parse(typeof(Languages), GetSetting("LanguageSelected"));
			}
			set
			{
				if (value != LanguageSelected)
				{
					SetSetting("LanguageSelected", value.ToString());
				}
			}
		}


		/// <summary>
		/// Enum for all Retailers
		/// </summary>
		public enum Retailers
		{
			// THESE NEED TO BE IN THAT SPELLING AND UPPERCASING
			Steam,
			Rockstar,
			Epic
		}

		/// <summary>
		/// Version of Social Club Launch. Either "127" or "124"
		/// </summary>
		public static string SocialClubLaunchGameVersion
		{
			get
			{
				if (GetSetting("Version") == "124")
				{
					return "124";
				}
				else
				{
					return "127";
				}
			}
			set
			{
				if (value != SocialClubLaunchGameVersion)
				{
					if (ComponentManager.RecommendUpgradedGTA())
					{
						SetSetting("Version", value);
						if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
						{
							if (value == "124")
							{
								SetSetting("Version", "127");
							}
							else
							{
								SetSetting("Version", "124");
							}
							return;
						}
					}
					else
					{
						new Popup(Popup.PopupWindowTypes.PopupOk, "Setting was not changed.");
						return;
					}
				}
			}
		}



		/// <summary>
		/// Settings Retailer. Gets and Sets from Dictionary.
		/// </summary>
		public static Retailers Retailer
		{
			get
			{
				return (Retailers)System.Enum.Parse(typeof(Retailers), GetSetting("Retailer"));
			}
			set
			{
				if (value != Retailer)
				{
					if (Settings.EnableAlternativeLaunch)
					{
						Retailers OldRetailer = Retailer;
						if (ComponentManager.RecommendUpgradedGTA())
						{
							SetSetting("Retailer", value.ToString());
							if (value == Retailers.Epic)
							{
								Settings.EnableAlternativeLaunch = false;
							}
							if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
							{
								SetSetting("Retailer", OldRetailer.ToString());
								return;
							}
						}
						else
						{
							new Popup(Popup.PopupWindowTypes.PopupOk, "Retailer was not changed.");
							return;
						}

					}
					else
					{
						SetSetting("Retailer", value.ToString());
					}
					Settings.TellRockstarUsersToDisableAutoUpdateIfNeeded();
				}
			}
		}

		/// <summary>
		/// Settings StartWay. Gets and Sets from Dictionary.
		/// </summary>
		public static StartWays StartWay
		{
			get
			{
				return (StartWays)System.Enum.Parse(typeof(StartWays), GetSetting("StartWay"));
			}
			set
			{
				if (value != StartWay)
				{
					SetSetting("StartWay", value.ToString());
				}
			}
		}

		/// <summary>
		/// Settings ExitWay. Gets and Sets from Dictionary.
		/// </summary>
		public static ExitWays ExitWay
		{
			get
			{
				return (ExitWays)System.Enum.Parse(typeof(ExitWays), GetSetting("ExitWay"));
			}
			set
			{
				if (value != ExitWay)
				{
					SetSetting("ExitWay", value.ToString());
				}
			}
		}


		/// <summary>
		/// Settings EnableAutoSetHighPriority. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableAutoSetHighPriority
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAutoSetHighPriority"));
			}
			set
			{
				SetSetting("EnableAutoSetHighPriority", value.ToString());
			}
		}



		/// <summary>
		/// Settings EnableScripthookOnDowngraded. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableScripthookOnDowngraded
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableScripthookOnDowngraded"));
			}
			set
			{
				SetSetting("EnableScripthookOnDowngraded", value.ToString());
			}
		}

		/// <summary>
		/// Settings EnableAutoStartLiveSplit. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableAutoStartLiveSplit
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAutoStartLiveSplit"));
			}
			set
			{
				SetSetting("EnableAutoStartLiveSplit", value.ToString());
			}
		}

		/// <summary>
		/// Settings PathLiveSplit. Gets and Sets from the Dictionary.
		/// </summary>
		public static string PathLiveSplit
		{
			get
			{
				return GetSetting("PathLiveSplit");
			}
			set
			{
				SetSetting("PathLiveSplit", value);
			}
		}

		/// <summary>
		/// Settings EnableAutoStartStreamProgram. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableAutoStartStreamProgram
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAutoStartStreamProgram"));
			}
			set
			{
				SetSetting("EnableAutoStartStreamProgram", value.ToString());
			}
		}

		/// <summary>
		/// Settings PathStreamProgram. Gets and Sets from the Dictionary.
		/// </summary>
		public static string PathStreamProgram
		{
			get
			{
				return GetSetting("PathStreamProgram");
			}
			set
			{
				SetSetting("PathStreamProgram", value);
			}
		}

		/// <summary>
		/// Settings EnableAutoStartFPSLimiter. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableAutoStartFPSLimiter
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAutoStartFPSLimiter"));
			}
			set
			{
				SetSetting("EnableAutoStartFPSLimiter", value.ToString());
			}
		}

		/// <summary>
		/// Settings PathFPSLimiter. Gets and Sets from the Dictionary.
		/// </summary>
		public static string PathFPSLimiter
		{
			get
			{
				return GetSetting("PathFPSLimiter");
			}
			set
			{
				SetSetting("PathFPSLimiter", value);
			}
		}

		/// <summary>
		/// Settings EnableAutoStartJumpScript. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableOverlay
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableOverlay"));
			}
			set
			{
				SetSetting("EnableOverlay", value.ToString());
				NoteOverlay.OverlaySettingsChanged();
			}
		}

		/// <summary>
		/// Settings OverlayMultiMonitorMode. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool OverlayMultiMonitorMode
		{
			get
			{
				return GetBoolFromString(GetSetting("OverlayMultiMonitorMode"));
			}
			set
			{
				SetSetting("OverlayMultiMonitorMode", value.ToString());
				NoteOverlay.OverlaySettingsChanged();
			}
		}

		/// <summary>
		/// Settings EnableAutoStartJumpScript. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableAutoStartJumpScript
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAutoStartJumpScript"));
			}
			set
			{
				SetSetting("EnableAutoStartJumpScript", value.ToString());

				if (value)
				{
					if (LauncherLogic.GameState == LauncherLogic.GameStates.Running)
					{
						if (Settings.EnableOnlyAutoStartProgramsWhenDowngraded)
						{
							if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
							{
								Jumpscript.StartJumpscript();
							}
						}
						else
						{
							Jumpscript.StartJumpscript();
						}
					}
				}
				else
				{
					Jumpscript.StopJumpscript();
				}
			}
		}


		/// <summary>
		/// Settings EnableLegacyAuth. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableLegacyAuth
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableLegacyAuth"));
			}
			set
			{
				SetSetting("EnableLegacyAuth", value.ToString());
			}
		}


		/// <summary>
		/// Getting a Key from the registry
		/// </summary>
		/// <param name="pString"></param>
		/// <returns></returns>
		public static Keys GetKeyFromString(string pString)
		{
			Keys myReturnKey;
			try
			{
				myReturnKey = (Keys)(Int32.Parse(pString));
			}
			catch
			{
				new Popups.Popup(Popup.PopupWindowTypes.PopupOkError, "Something broke while getting the Hotkeys from the Settings.\n Try Resetting Settings").ShowDialog();
				myReturnKey = Keys.None;
			}
			return myReturnKey;
		}

		/// <summary>
		/// Settings JumpScriptKey1. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys JumpScriptKey1
		{
			get
			{
				return GetKeyFromString(GetSetting("JumpScriptKey1"));
			}
			set
			{
				SetSetting("JumpScriptKey1", ((int)value).ToString());
				if (Jumpscript.IsRunning)
				{
					Jumpscript.StartJumpscript();
				}
			}
		}

		/// <summary>
		/// Settings JumpScriptKey2. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys JumpScriptKey2
		{
			get
			{
				return GetKeyFromString(GetSetting("JumpScriptKey2"));
			}
			set
			{
				SetSetting("JumpScriptKey2", ((int)value).ToString());
				if (Jumpscript.IsRunning)
				{
					Jumpscript.StartJumpscript();
				}
			}
		}

		/// <summary>
		/// Settings KeyOverlayToggle. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys KeyOverlayToggle
		{
			get
			{
				return GetKeyFromString(GetSetting("KeyOverlayToggle"));
			}
			set
			{
				SetSetting("KeyOverlayToggle", ((int)value).ToString());
			}
		}


		/// <summary>
		/// Settings KeyOverlayScrollLeft. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys KeyOverlayScrollLeft
		{
			get
			{
				return GetKeyFromString(GetSetting("KeyOverlayScrollLeft"));
			}
			set
			{
				SetSetting("KeyOverlayScrollLeft", ((int)value).ToString());
			}
		}

		/// <summary>
		/// Settings KeyOverlayScrollRight. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys KeyOverlayScrollRight
		{
			get
			{
				return GetKeyFromString(GetSetting("KeyOverlayScrollRight"));
			}
			set
			{
				SetSetting("KeyOverlayScrollRight", ((int)value).ToString());
			}
		}

		/// <summary>
		/// Settings KeyOverlayScrollUp. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys KeyOverlayScrollUp
		{
			get
			{
				return GetKeyFromString(GetSetting("KeyOverlayScrollUp"));
			}
			set
			{
				SetSetting("KeyOverlayScrollUp", ((int)value).ToString());
			}
		}

		/// <summary>
		/// Settings KeyOverlayScrollDown. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys KeyOverlayScrollDown
		{
			get
			{
				return GetKeyFromString(GetSetting("KeyOverlayScrollDown"));
			}
			set
			{
				SetSetting("KeyOverlayScrollDown", ((int)value).ToString());
			}
		}


		/// <summary>
		/// Settings KeyOverlayNoteNext. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys KeyOverlayNoteNext
		{
			get
			{
				return GetKeyFromString(GetSetting("KeyOverlayNoteNext"));
			}
			set
			{
				SetSetting("KeyOverlayNoteNext", ((int)value).ToString());
			}
		}

		/// <summary>
		/// Settings KeyOverlayNotePrev. Gets and Sets from the Dictionary.
		/// </summary>
		public static Keys KeyOverlayNotePrev
		{
			get
			{
				return GetKeyFromString(GetSetting("KeyOverlayNotePrev"));
			}
			set
			{
				SetSetting("KeyOverlayNotePrev", ((int)value).ToString());
			}
		}



		/// <summary>
		/// Setting for the OvererlayBackground. Gets and Sets from Dictionary
		/// </summary>
		public static System.Drawing.Color OverlayBackground
		{
			get
			{
				return GetColorFromString(GetSetting("OverlayBackground"));
			}
			set
			{
				SetSetting("OverlayBackground", GetStringFromColor(value));
			}
		}

		/// <summary>
		/// Setting for the OverlayForeground. Gets and Sets from Dictionary
		/// </summary>
		public static System.Drawing.Color OverlayForeground
		{
			get
			{
				return GetColorFromString(GetSetting("OverlayForeground"));
			}
			set
			{
				SetSetting("OverlayForeground", GetStringFromColor(value));
			}
		}

		/// <summary>
		/// Setting for the OverlayLocation. Gets and Sets from Dictionary
		/// </summary>
		public static GTAOverlay.Positions OverlayLocation
		{
			get
			{
				return (GTAOverlay.Positions)System.Enum.Parse(typeof(GTAOverlay.Positions), GetSetting("OverlayLocation"));
			}
			set
			{
				SetSetting("OverlayLocation", value.ToString());
			}
		}

		/// <summary>
		/// Setting for the OverlayTextFont. Gets and Sets from Dictionary
		/// </summary>
		public static string OverlayTextFont
		{
			get
			{
				return GetSetting("OverlayTextFont");
			}
			set
			{
				SetSetting("OverlayTextFont", value.ToString());
			}
		}




		public static double OL_MM_Left
		{
			get
			{
				return GetDoubleFromString(GetSetting("OL_MM_Left"));
			}
			set
			{
				SetSetting("OL_MM_Left", ((int)value).ToString());
			}
		}

		public static double OL_MM_Top
		{
			get
			{
				return GetDoubleFromString(GetSetting("OL_MM_Top"));
			}
			set
			{
				SetSetting("OL_MM_Top", ((int)value).ToString());
			}
		}

		public static int OverlayMarginX
		{
			get
			{
				return GetIntFromString(GetSetting("OverlayMarginX"));
			}
			set
			{
				SetSetting("OverlayMarginX", value.ToString());
			}
		}


		public static int OverlayMarginY
		{
			get
			{
				return GetIntFromString(GetSetting("OverlayMarginY"));
			}
			set
			{
				SetSetting("OverlayMarginY", value.ToString());
			}
		}

		public static int OverlayHeight
		{
			get
			{
				return GetIntFromString(GetSetting("OverlayHeight"));
			}
			set
			{
				SetSetting("OverlayHeight", value.ToString());
			}
		}

		public static int OverlayWidth
		{
			get
			{
				return GetIntFromString(GetSetting("OverlayWidth"));
			}
			set
			{
				SetSetting("OverlayWidth", value.ToString());
			}
		}

		public static int OverlayTextSize
		{
			get
			{
				return GetIntFromString(GetSetting("OverlayTextSize"));
			}
			set
			{
				SetSetting("OverlayTextSize", value.ToString());
			}
		}



		/// <summary>
		/// Settings EnableAutoStartNohboard. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableAutoStartNohboard
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAutoStartNohboard"));
			}
			set
			{
				SetSetting("EnableAutoStartNohboard", value.ToString());
			}
		}

		/// <summary>
		/// Settings EnableNohboardBurhac. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableNohboardBurhac
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableNohboardBurhac"));
			}
			set
			{
				SetSetting("EnableNohboardBurhac", value.ToString());
			}
		}


		/// <summary>
		/// Settings EnableDontLaunchThroughSteam. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableDontLaunchThroughSteam
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableDontLaunchThroughSteam"));
			}
			set
			{
				SetSetting("EnableDontLaunchThroughSteam", value.ToString());
			}
		}

		/// <summary>
		/// Settings PathNohboard. Gets and Sets from the Dictionary.
		/// </summary>
		public static string PathNohboard
		{
			get
			{
				return GetSetting("PathNohboard");
			}
			set
			{
				SetSetting("PathNohboard", value);
			}
		}

		/// <summary>
		/// Setting: AllFilesEverPlacedInsideGTA. Gets and Sets from Dictionary
		/// </summary>
		public static List<string> AllFilesEverPlacedInsideGTA
		{
			get
			{
				return GetStringListFromString(GetSetting("AllFilesEverPlacedInsideGTA"), ';');
			}
			set
			{
				SetSetting("AllFilesEverPlacedInsideGTA", String.Join(";", value.ToArray()));
			}
		}

		public static void AllFilesEverPlacedInsideGTAMyAdd(string filename)
		{
			List<string> tmp = AllFilesEverPlacedInsideGTA;
			if (!tmp.Contains(filename))
			{
				tmp.Add(filename);
				AllFilesEverPlacedInsideGTA = tmp;
			}
		}



		/// <summary>
		/// Setting: OverlayNotesPresetA. Gets and Sets from Dictionary
		/// </summary>
		public static List<string> OverlayNotesPresetA
		{
			get
			{
				return GetStringListFromString(GetSetting("OverlayNotesPresetA"), ';');
			}
			set
			{
				SetSetting("OverlayNotesPresetA", String.Join(";", value.ToArray()));
			}
		}

		/// <summary>
		/// Setting: OverlayNotesPresetB. Gets and Sets from Dictionary
		/// </summary>
		public static List<string> OverlayNotesPresetB
		{
			get
			{
				return GetStringListFromString(GetSetting("OverlayNotesPresetB"), ';');
			}
			set
			{
				SetSetting("OverlayNotesPresetB", String.Join(";", value.ToArray()));
			}
		}

		/// <summary>
		/// Setting: OverlayNotesPresetC. Gets and Sets from Dictionary
		/// </summary>
		public static List<string> OverlayNotesPresetC
		{
			get
			{
				return GetStringListFromString(GetSetting("OverlayNotesPresetC"), ';');
			}
			set
			{
				SetSetting("OverlayNotesPresetC", String.Join(";", value.ToArray()));
			}
		}

		/// <summary>
		/// Setting: OverlayNotesPresetD. Gets and Sets from Dictionary
		/// </summary>
		public static List<string> OverlayNotesPresetD
		{
			get
			{
				return GetStringListFromString(GetSetting("OverlayNotesPresetD"), ';');
			}
			set
			{
				SetSetting("OverlayNotesPresetD", String.Join(";", value.ToArray()));
			}
		}

		/// <summary>
		/// Setting: OverlayNotesPresetE. Gets and Sets from Dictionary
		/// </summary>
		public static List<string> OverlayNotesPresetE
		{
			get
			{
				return GetStringListFromString(GetSetting("OverlayNotesPresetE"), ';');

			}
			set
			{
				SetSetting("OverlayNotesPresetE", String.Join(";", value.ToArray()));
			}
		}

		/// <summary>
		/// Setting: OverlayNotesPresetF. Gets and Sets from Dictionary
		/// </summary>
		public static List<string> OverlayNotesPresetF
		{
			get
			{
				return GetStringListFromString(GetSetting("OverlayNotesPresetF"), ';');
			}
			set
			{
				SetSetting("OverlayNotesPresetF", String.Join(";", value.ToArray()));
			}
		}

		/// <summary>
		/// Setting: OverlayNotesMain. Gets and Sets from Dictionary
		/// </summary>
		public static List<string> OverlayNotesMain
		{
			get
			{
				return GetStringListFromString(GetSetting("OverlayNotesMain"), ';');
			}
			set
			{
				SetSetting("OverlayNotesMain", String.Join(";", value.ToArray()));
			}
		}


		/// <summary>
		/// Settings EnableRememberMe. Gets and Sets from the Dictionary.
		/// <summary>
		public static bool EnableRememberMe
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableRememberMe"));
			}
			set
			{
				if (GetBoolFromString(GetSetting("EnableRememberMe")) != value)
				{
					SetSetting("EnableRememberMe", value.ToString());
					if (!value)
					{
						using (var creds = new CredentialManagement.Credential())
						{
							creds.Target = "Project127Login";
							if (!creds.Exists())
							{
								return;
							}
							creds.Load();
							creds.Delete();
						}
					}
				}
			}
		}


		/// <summary>
		/// Gets StringList from a single String
		/// </summary>
		/// <param name="pString"></param>
		/// <param name="Deliminiter"></param>
		/// <returns></returns>
		public static List<string> GetStringListFromString(string pString, char Deliminiter)
		{
			List<string> rtrn = new List<string>(pString.Split(Deliminiter));

			if (rtrn.Count == 1)
			{
				if (String.IsNullOrWhiteSpace(rtrn[0]))
				{
					rtrn = new List<string>();
				}
			}

			return rtrn;
		}



		/// <summary>
		/// Gets Int from String
		/// </summary>
		public static int GetIntFromString(string pString)
		{
			int rtrn = 0;
			try
			{
				rtrn = Int32.Parse(pString);
			}
			catch
			{
				new Popups.Popup(Popup.PopupWindowTypes.PopupOkError, "Something broke while getting the a Number from the Settings.\n Try Resetting Settings").ShowDialog();
			}
			return rtrn;
		}

		/// <summary>
		/// Gets Double from String
		/// </summary>
		public static double GetDoubleFromString(string pString)
		{
			return GetIntFromString(pString);
		}

		/// <summary>
		/// Gets String from a Color
		/// </summary>
		/// <param name="pColor"></param>
		/// <returns></returns>
		public static string GetStringFromColor(System.Drawing.Color pColor)
		{
			string rtrn = pColor.A.ToString() + "," + pColor.R.ToString() + "," + pColor.G.ToString() + "," + pColor.B.ToString();
			return rtrn;
		}

		/// <summary>
		/// Gets Color from a String
		/// </summary>
		/// <param name="pColor"></param>
		/// <returns></returns>
		public static System.Drawing.Color GetColorFromString(string pColor)
		{
			System.Drawing.Color rtrn = System.Drawing.Color.Black;
			try
			{
				string[] values_s = pColor.Split(',');
				int[] values_i = new int[values_s.Length];

				for (int i = 0; i <= values_s.Length - 1; i++)
				{
					values_i[i] = Int32.Parse(values_s[i]);
				}

				rtrn = System.Drawing.Color.FromArgb(values_i[0], values_i[1], values_i[2], values_i[3]);
			}
			catch
			{
				new Popups.Popup(Popup.PopupWindowTypes.PopupOkError, "Something broke while getting the Colors from the Settings.\n Try Resetting Settings").ShowDialog();
			}
			return rtrn;
		}


	} // End of partial Class
} // End of Namespace
