using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Class of our own Logger
	/// </summary>
	public static class Logger
	{
		// We should probably use a logging libary / framework now that I think about it...whatevs
		// Actually implementing this probably took less time than googling "Logging class c#", and we have more control over it

		/// <summary>
		/// Init Function which gets called once at the start.
		/// </summary>
		public static void Init()
		{
			// Since the createFile Method will override an existing file
			if (!FileHandling.doesFileExist(Globals.Logfile))
			{
				HelperClasses.FileHandling.createFile(Globals.Logfile);
			}


			string MyCreationDate = HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName);

			HelperClasses.Logger.Log("", true, 0);
			HelperClasses.Logger.Log("", true, 0);
			HelperClasses.Logger.Log(" === Project - 127 Started (Version: '" + Globals.ProjectVersion + "' BuildInfo: '" + Globals.BuildInfo + "' Built at: '" + MyCreationDate + "' Central European Time) ===", true, 0);
			HelperClasses.Logger.Log("Logging initiated", true, 0);
		}

		/// <summary>
		/// Main Method of Logging.cs which is called to log stuff.
		/// </summary>
		/// <param name="pLogMessage"></param>
		public static void Log(string pLogMessage, bool pSkipLogSetting, int pLogLevel)
		{
			if (pSkipLogSetting)
			{
				string LogMessage = "[" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "] - ";

				// Yes this for loop is correct. If Log level 0, we dont add another "- "
				for (int i = 0; i <= pLogLevel - 1; i++)
				{
					LogMessage += "- ";
				}

				LogMessage += pLogMessage;

				HelperClasses.FileHandling.AddToLog(Globals.Logfile, LogMessage);
			}
		}

		/// <summary>
		/// Overloaded / Underloaded Logging Method
		/// </summary>
		/// <param name="pLogMessage"></param>
		public static void Log(string pLogMessage)
		{
			Log(pLogMessage, Settings.EnableLogging, 0);
		}

		/// <summary>
		/// Overloaded / Underloaded Logging Method
		/// </summary>
		/// <param name="pLogMessage"></param>
		/// <param name="pLogLevel"></param>
		public static void Log(string pLogMessage, int pLogLevel)
		{
			Log(pLogMessage, Settings.EnableLogging, pLogLevel);
		}

		/// <summary>
		/// Rolling Log. Gets called on P127 start. Only keeps the latest 2500 lines, everything before that will get deleted
		/// </summary>
		public static void RollingLog()
		{
			string[] Logs = HelperClasses.FileHandling.ReadFileEachLine(Globals.Logfile);
			if (Logs.Length > 2500)
			{
				List<string> myNewLog = new List<string>();
				int i = Logs.Length - 2490;
				while (i <= Logs.Length - 1)
				{
					myNewLog.Add(Logs[i]);
					i++;
				}
				string[] tmp = myNewLog.ToArray();
				HelperClasses.FileHandling.WriteStringToFileOverwrite(Globals.Logfile, tmp);
			}
		}



		/// <summary>
		/// Generating Debug File
		/// </summary>
		public static async void GenerateDebug()
		{
			await Task.Run(() =>
			{
				string MyCreationDate = HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName);

				// Debug Info users can give me easily...
				List<string> DebugMessage = new List<string>();

				DebugMessage.Add("Project 1.27 Version: '" + Globals.ProjectVersion + "'");
				DebugMessage.Add("BuildInfo: '" + Globals.BuildInfo + "'");
				DebugMessage.Add("BuildTime: '" + MyCreationDate + "'");
				DebugMessage.Add("Time Now: '" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("ZIP Version: '" + Globals.ZipVersion + "'");
				DebugMessage.Add("Globals.Branch: '" + Globals.Branch + "'");
				DebugMessage.Add("InternalMode (Overwites, mode / branch): '" + Globals.InternalMode + "'");
				DebugMessage.Add("Project 1.27 Installation Path '" + Globals.ProjectInstallationPath + "'");
				DebugMessage.Add("Project 1.27 Installation Path Binary '" + Globals.ProjectInstallationPathBinary + "'");
				DebugMessage.Add("ZIP Extraction Path '" + LauncherLogic.ZIPFilePath + "'");
				DebugMessage.Add("LauncherLogic.GTAVFilePath: '" + LauncherLogic.GTAVFilePath + "'");
				DebugMessage.Add("LauncherLogic.UpgradeFilePath: '" + LauncherLogic.UpgradeFilePath + "'");
				DebugMessage.Add("LauncherLogic.DowngradeFilePath: '" + LauncherLogic.DowngradeFilePath + "'");
				DebugMessage.Add("LauncherLogic.SupportFilePath: '" + LauncherLogic.SupportFilePath + "'");
				DebugMessage.Add("Detected AuthState: '" + LauncherLogic.AuthState + "'");
				DebugMessage.Add("Detected GameState: '" + LauncherLogic.GameState + "'");
				DebugMessage.Add("Detected InstallationState: '" + LauncherLogic.InstallationState + "'");
				DebugMessage.Add("    Size of GTA5.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("Settings: ");
				foreach (KeyValuePair<string, string> KVP in Globals.MySettings)
				{
					DebugMessage.Add("    " + KVP.Key + ": '" + KVP.Value + "'");
				}

				// Building DebugPath
				string DebugFile = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\AAA - DEBUG.txt";

				// Deletes File, Creates File, Adds to it

				string[] currContents = HelperClasses.FileHandling.ReadFileEachLine(DebugFile);

				if (currContents.Length > DebugMessage.Count + 1)
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "The file we are trying to overwrite contains more Lines than we want to write it it.\nBy overwriting it, we might lose information in the debugfile.\nDo you want to overwrite?");
					yesno.ShowDialog();
					if (yesno.DialogResult == false)
					{
						return;
					}
				}

				HelperClasses.FileHandling.WriteStringToFileOverwrite(DebugFile, DebugMessage.ToArray());

				HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: Globals.ProjectInstallationPath);
			});
		}





	} // End of Class
} // End of NameSpace
