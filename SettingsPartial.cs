using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project_127
{
	/// <summary>
	/// Partial Class for Settings Window. 
	/// Also Creates Properties for all Settings, which are easier to interact with than the Dictionary
	/// </summary>
	public partial class Settings : Window
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
					HelperClasses.Logger.Log("Writing '" + KVP.Key.ToString() + "' to the Registry (Value: '" + KVP.Value.ToString() + "') as a Part of Initiating Settings.", true, 2);
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
		/// Button Click to change the Path of ZIPExtractionPath which we use to use for all Contents of ZIP File etc.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_ZIPExtractionPath_Click(object sender, RoutedEventArgs e)
		{
			// Grabbing the new Path from FolderDialogThingy
			string _ZIPExtractionPath = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder where this Program will store its Data.", Settings.ZIPExtractionPath);
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
		/// Settings Retailer. Gets and Sets from Dictionary.
		/// </summary>
		public static Languages LanguageSelected
		{
			get
			{
				return (Languages)System.Enum.Parse(typeof(Languages), GetSetting("LanguageSelected"));
			}
			set
			{
				SetSetting("LanguageSelected", value.ToString());
			}
		}


		/// <summary>
		/// Enum for all Retailers
		/// </summary>
		public enum Retailers
		{
			Steam,
			Rockstar,
			Epic
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
				SetSetting("Retailer", value.ToString());
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
		public static bool EnableAutoStartJumpScript
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableAutoStartJumpScript"));
			}
			set
			{
				SetSetting("EnableAutoStartJumpScript", value.ToString());
			}
		}

		/// <summary>
		/// Settings JumpScriptKey1. Gets and Sets from the Dictionary.
		/// </summary>
		public static string JumpScriptKey1
		{
			get
			{
				return GetSetting("JumpScriptKey1");
			}
			set
			{
				SetSetting("JumpScriptKey1", value);
			}
		}

		/// <summary>
		/// Settings JumpScriptKey2. Gets and Sets from the Dictionary.
		/// </summary>
		public static string JumpScriptKey2
		{
			get
			{
				return GetSetting("JumpScriptKey2");
			}
			set
			{
				SetSetting("JumpScriptKey2", value);
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
		/// Settings Theme. Gets and Sets from the Dictionary.
		/// </summary>
		public static string Theme
		{
			get
			{
				return GetSetting("Theme");
			}
			set
			{
				SetSetting("Theme", value);
			}
		}

		/// <summary>
		/// Enabling the "Remember me" for the user when logging in. User cant change this
		/// </summary>
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
				}
			}
		}

	} // End of partial Class
} // End of Namespace
