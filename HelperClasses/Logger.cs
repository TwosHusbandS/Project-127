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
using System.Threading;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Class of our own Logger
	/// </summary>
	public static class Logger
	{
		// We should probably use a logging libary / framework now that I think about it...whatevs
		// Actually implementing this probably took less time than googling "Logging class c#", and we have more control over it

		private static Mutex mut = new Mutex();

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
			mut.WaitOne();
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
			mut.ReleaseMutex();
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
				foreach (ComponentManager.Components myComponent in ComponentManager.AllComponents)
				{
					DebugMessage.Add("    Component: '" + myComponent.ToString() + "', Installed: '" + myComponent.IsInstalled() + "'. Version: '" + myComponent.GetInstalledVersion() + "'");
				}
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
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles (Emu) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles (Emu) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles (Emu) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles (SocialClubLaunch Steam 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam124.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeAlternativeFilePathSteam124.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles (SocialClubLaunch Steam 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam124.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles (SocialClubLaunch Steam 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam124.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles (SocialClubLaunch Steam 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles (SocialClubLaunch Steam 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles (SocialClubLaunch Steam 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles (SocialClubLaunch Rockstar 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar124.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeAlternativeFilePathRockstar124.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles (SocialClubLaunch Rockstar 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar124.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles (SocialClubLaunch Rockstar 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar124.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles (SocialClubLaunch Rockstar 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles (SocialClubLaunch Rockstar 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles (SocialClubLaunch Rockstar 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("Files I ever placed inside GTA: ");
				foreach (string tmp in Settings.AllFilesEverPlacedInsideGTA)
				{
					DebugMessage.Add("    '" + tmp + "'");
				}
				DebugMessage.Add("Settings: ");
				foreach (KeyValuePair<string, string> KVP in Globals.MySettings)
				{
					DebugMessage.Add("    " + KVP.Key + ": '" + KVP.Value + "'");
				}

				// Building DebugPath
				string DebugFile = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\AAA - DEBUG.txt";

				// Deletes File, Creates File, Adds to it



				HelperClasses.FileHandling.WriteStringToFileOverwrite(DebugFile, DebugMessage.ToArray());

				HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: Globals.ProjectInstallationPath);
			});
		}





	} // End of Class
} // End of NameSpace
