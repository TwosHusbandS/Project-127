using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				HelperClasses.Logger.Log("Failed to SettingsPartial.cs SetSetting(" + pKey + ", " + pValue  + ")");
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
		/// Settings FirstLaunch. Gets from the Dictionary. NO SET.
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
		/// Settings FileFolder. Gets and Sets from the Dictionary.
		/// </summary>
		public static string FileFolder
		{
			get
			{
				return GetSetting("FileFolder");
			}
			set
			{
				SetSetting("FileFolder", value);
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
		/// Settings EnableTmpFixSteamLaunch. Gets and Sets from the Dictionary.
		/// </summary>
		public static bool EnableTempFixSteamLaunch
		{
			get
			{
				return GetBoolFromString(GetSetting("EnableTempFixSteamLaunch"));
			}
			set
			{
				SetSetting("EnableTempFixSteamLaunch", value.ToString());
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

	} // End of partial Class
} // End of Namespace
