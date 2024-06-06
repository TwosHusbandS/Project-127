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
using CredentialManagement;
using Microsoft.Win32;
using GSF.Parsing;

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


			string MyCreationDate = HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName).ToString("yyyy-MM-ddTHH:mm:ss");

			HelperClasses.Logger.Log("-", true, 0);
			HelperClasses.Logger.Log("-", true, 0);
			HelperClasses.Logger.Log("-", true, 0);
			HelperClasses.Logger.Log(" === Project - 127 Started (Version: '" + Globals.ProjectVersion + "' BuildInfo: '" + Globals.BuildInfo + "' Built at: '" + MyCreationDate + "' Central European Time) ===", true, 0);
			HelperClasses.Logger.Log("    Time Now: '" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "'", true, 0);
			HelperClasses.Logger.Log("    Time Now UTC: '" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "'", true, 0);
			HelperClasses.Logger.Log("Logging initiated. Time to the left is local time and NOT UTC. See Debug for more Info", true, 0);
		}

		/// <summary>
		/// Main Method of Logging.cs which is called to log stuff.
		/// </summary>
		/// <param name="pLogMessage"></param>
		public static void Log(string pLogMessage, bool pSkipLogSetting, int pLogLevel)
		{
			mut.WaitOne();
			if (pSkipLogSetting && !String.IsNullOrWhiteSpace(pLogMessage))
			{
				string LogMessage = "[" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "] - ";

				// Yes this for loop is correct. If Log level 0, we dont add another "- "
				for (int i = 0; i <= pLogLevel - 1; i++)
				{
					LogMessage += "- ";
				}

				LogMessage += HelperClasses.FileHandling.AnonymizeUser(pLogMessage);

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
				Stopwatch tmpsw = new Stopwatch();
				tmpsw.Start();


				//Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion
				//RegisteredOwner

				RegistryKey myRK = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("Microsoft").CreateSubKey("Windows NT").CreateSubKey("CurrentVersion");
				string AdditionalDebug2 = HelperClasses.RegeditHandler.GetValue(myRK, "ProductName");
				string AdditionalDebug3 = HelperClasses.RegeditHandler.GetValue(myRK, "EditionID");
				string AdditionalDebug4 = HelperClasses.RegeditHandler.GetValue(myRK, "CurrentBuild");
				string AdditionalDebug5 = HelperClasses.RegeditHandler.GetValue(myRK, "CurrentBuildNumber");

				RegistryKey myRK2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("Microsoft").CreateSubKey("Cryptography");
				string AdditionalDebug6 = HelperClasses.RegeditHandler.GetValue(myRK2, "MachineGuid");

				string LauncDatPath = LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\launc.dat";

				// Debug Info users can give me easily...
				List<string> DebugMessage = new List<string>();

                DebugMessage.Add("Project 1.27 Version: '" + Globals.ProjectVersion + "'");
				DebugMessage.Add("BuildInfo: '" + Globals.BuildInfo + "'");
				DebugMessage.Add("    BuildCreated: '" + HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("    BuildCreatedUTC: '" + HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName, true).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("    BuildLastModified: '" + HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("    BuildLastModifiedUTC: '" + HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName, true).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("    Time Now: '" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("    Time Now UTC: '" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("ZIP Version: '" + Globals.ZipVersion + "'");
				DebugMessage.Add("Globals.P127Branch: '" + Globals.P127Branch + "'");
				DebugMessage.Add("Globals.DMBranch: '" + Globals.DMBranch + "'");
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
				DebugMessage.Add("Chosen LaunchWay: '" + LauncherLogic.LaunchWay + "'");
				DebugMessage.Add("Chosen AuthWay: '" + LauncherLogic.AuthWay + "'");
				DebugMessage.Add("Detected AuthState: '" + LauncherLogic.AuthState + "'");
				DebugMessage.Add("Detected GameState: '" + LauncherLogic.GameState + "'");
				DebugMessage.Add("Detected InstallationState: '" + LauncherLogic.InstallationState + "'");
                DebugMessage.Add("    Size of GTA5.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    Size of gtastub.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\gtastub.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles (Emu) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles (Emu) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles (Emu) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
                DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles (Base 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeBase124FilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeBase124FilePath.TrimEnd('\\') + @"\GTA5.exe"));
                DebugMessage.Add("    Size of update.rpf in DowngradeFiles (Base 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeBase124FilePath.TrimEnd('\\') + @"\update\update.rpf"));
                DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles (Base 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeBase124FilePath.TrimEnd('\\') + @"\playgtav.exe"));
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
				DebugMessage.Add("    Size of gtavlauncher.exe in DowngradeFiles (SocialClubLaunch Rockstar 124) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar124.TrimEnd('\\') + @"\gtavlauncher.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles (SocialClubLaunch Rockstar 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles (SocialClubLaunch Rockstar 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of gtavlauncher.exe in DowngradeFiles (SocialClubLaunch Rockstar 127) Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\gtavlauncher.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    Size of gtavlauncher.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\gtavlauncher.exe"));
				DebugMessage.Add("    Size of gtastub.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\gtastub.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    Size of gtavlauncher.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\gtavlauncher.exe"));
				DebugMessage.Add("    Size of gtastub.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\gtastub.exe"));
                DebugMessage.Add("Detected Social Club InstallationStates:");
				DebugMessage.Add("    Detect Social Club InstallationStates SC_INSTALLATION_PATH ('" + LaunchAlternative.SCL_SC_Installation + "'): " + LaunchAlternative.Get_SCL_InstallationState(LaunchAlternative.SCL_SC_Installation));
				DebugMessage.Add("    Detect Social Club InstallationStates SC_TEMP_BACKUP_PATH ('" + LaunchAlternative.SCL_SC_TEMP_BACKUP + "'): " + LaunchAlternative.Get_SCL_InstallationState(LaunchAlternative.SCL_SC_TEMP_BACKUP));
				DebugMessage.Add("    Detect Social Club InstallationStates SC_DOWNGRADED_PATH ('" + LaunchAlternative.SCL_SC_DOWNGRADED + "'): " + LaunchAlternative.Get_SCL_InstallationState(LaunchAlternative.SCL_SC_DOWNGRADED));
				DebugMessage.Add("    Detect Social Club InstallationStates SC_DOWNGRADED_CACHE_PATH ('" + LaunchAlternative.SCL_SC_DOWNGRADED_CACHE + "'): " + LaunchAlternative.Get_SCL_InstallationState(LaunchAlternative.SCL_SC_DOWNGRADED_CACHE));
                DebugMessage.Add("All possible CFG.dat");
				string GTAProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\Profiles\";
				string[] GTAProfiles = HelperClasses.FileHandling.GetSubFolders(GTAProfilePath);
				foreach (string GTAProfile in GTAProfiles)
				{
					string cfgdatPath = GTAProfile.TrimEnd('\\') + @"\cfg.dat";
					long size = HelperClasses.FileHandling.GetSizeOfFile(cfgdatPath);
					if (size > 0)
					{
						DebugMessage.Add("    '" + cfgdatPath + "', fileSize: '" + size + "'");
						DebugMessage.Add("          CreationTime: '" + HelperClasses.FileHandling.GetCreationDate(cfgdatPath).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
						DebugMessage.Add("          CreationTimeUTC: '" + HelperClasses.FileHandling.GetCreationDate(cfgdatPath, true).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
						DebugMessage.Add("          LastModified: '" + HelperClasses.FileHandling.GetLastWriteDate(cfgdatPath).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
						DebugMessage.Add("          LastModifiedUTC: '" + HelperClasses.FileHandling.GetLastWriteDate(cfgdatPath, true).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
						DebugMessage.Add("          FOLDER-LastModified: '" + HelperClasses.FileHandling.GetLastModifiedFolderDate(GTAProfile).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
						DebugMessage.Add("          FOLDER-LastModifiedUTC: '" + HelperClasses.FileHandling.GetLastModifiedFolderDate(GTAProfile, true).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
					}
					else
					{
						DebugMessage.Add("    '" + cfgdatPath + "', fileSize: '" + size + "' (doesnt exist)");
					}
				}

                DebugMessage.Add("    >>>   Dragons Logic (probably) detects and uses this cfg.dat: '" + HelperClasses.FileHandling.MostLikelyProfileFolder().TrimEnd('\\') + @"\cfg.dat" + "'");
				DebugMessage.Add("Launch.dat inside GTA Installation Directory:");
				DebugMessage.Add("       Path: '" + LauncDatPath + "'");
				DebugMessage.Add("       DoesExist: '" + HelperClasses.FileHandling.doesFileExist(LauncDatPath) + "'");
				DebugMessage.Add("       LastModified: '" + HelperClasses.FileHandling.GetLastWriteDate(LauncDatPath).ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("       LastModifiedUTC: '" + HelperClasses.FileHandling.GetLastWriteDate(LauncDatPath, true).ToString("yyyy-MM-ddTHH:mm:ss") + "'");

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
				try
				{
					// Better safe than story...(try catching cause i have no idea how that shit works)
					using (var creds = new Credential())
					{
						creds.Target = "Project127Login";
						if (!creds.Exists())
						{
							DebugMessage.Add("Stored Credentials (legacy Auth): NOTHING FOUND");
						}
						else
						{
							creds.Load();
							DebugMessage.Add("Stored Credentials (legacy Auth):");

                            string email = creds.Username;
                            if (email.IndexOf('@') > 4)
                            {
                                string endappend = email.Substring(email.IndexOf('@'));
                                string start = email.Substring(0, email.IndexOf('@') - 1);
                                email = start.Substring(0, 2) + "..." + start.Substring(start.Length - 2) + endappend;
                            }

                            DebugMessage.Add("        Email: '" + email + "'");
							DebugMessage.Add("        LastWriteTime: '" + creds.LastWriteTime.ToString("yyyy-MM-ddTHH:mm:ss") + "'");
							DebugMessage.Add("        LastWriteTimeUTC: '" + creds.LastWriteTimeUtc.ToString("yyyy-MM-ddTHH:mm:ss") + "'");
						}
					}
					DebugMessage.Add("Windows-ProductName: '" + AdditionalDebug2 + "'");
					DebugMessage.Add("Windows-EditionID: '" + AdditionalDebug3 + "'");
					DebugMessage.Add("Windows-CurrentBuild: '" + AdditionalDebug4 + "'");
					DebugMessage.Add("Windows-CurrentBuildNumber: '" + AdditionalDebug5 + "'");
					DebugMessage.Add("Windows-UUID: '" + AdditionalDebug6 + "'");
				}
				catch { }

                try
                {
					DebugMessage.Add("Current Dynamic MTL Offsets in use, ask user to provide file from same folder as this debug if needed");
                    List<string> tmp = new List<string>();
                    List<string> tmp2 = new List<string>();
                    for (int i = 0; i <= MainWindow.DMO.match_pattern.Length - 1; i++)
                    {
                        tmp.Add(String.Format("0x{0:X2}", MainWindow.DMO.match_pattern[i]));
						tmp2.Add(MainWindow.DMO.match_pattern[i].ToString());
                    }

                    DebugMessage.Add("\tmatch_pattern: '" + String.Join(", ", tmp) + "' (" + String.Join(", ", tmp2) + ")");
                    DebugMessage.Add("\tpattern_search_offset: '" + MainWindow.DMO.pattern_search_offset + "' (" + MainWindow.DMO.pattern_search_offset + ")");
                    DebugMessage.Add("\tblob_offset: '" + MainWindow.DMO.blob_offset + "' (" + MainWindow.DMO.blob_offset + ")");
                    DebugMessage.Add("\tblob_count: '" + MainWindow.DMO.blob_count + "' (" + MainWindow.DMO.blob_count + ")");
                    DebugMessage.Add("\trockstarId_offset: '" + String.Format("0x{0:X}", MainWindow.DMO.rockstarId_offset) + "' (" + MainWindow.DMO.rockstarId_offset + ")");
                    DebugMessage.Add("\trockstarId_take: '" + MainWindow.DMO.rockstarId_take + "' (" + MainWindow.DMO.rockstarId_take + ")");
                    DebugMessage.Add("\tsessKey_offset: '" + String.Format("0x{0:X}", MainWindow.DMO.sessKey_offset) + "' (" + MainWindow.DMO.sessKey_offset + ")");
                    DebugMessage.Add("\tsessKey_take: '" + MainWindow.DMO.sessKey_take + "' (" + MainWindow.DMO.sessKey_take + ")");
                    DebugMessage.Add("\tticket_offset: '" + String.Format("0x{0:X}", MainWindow.DMO.ticket_offset) + "' (" + MainWindow.DMO.ticket_offset + ")");
                    DebugMessage.Add("\tsessTicket_offset: '" + String.Format("0x{0:X}", MainWindow.DMO.sessTicket_offset) + "' (" + MainWindow.DMO.sessTicket_offset + ")");
                    DebugMessage.Add("\trockstarNick_offset: '" + String.Format("0x{0:X}", MainWindow.DMO.rockstarNick_offset) + "' (" + MainWindow.DMO.rockstarNick_offset + ")");
                    DebugMessage.Add("\tcountryCode_offset: '" + String.Format("0x{0:X}", MainWindow.DMO.countryCode_offset) + "' (" + MainWindow.DMO.countryCode_offset + ")");
                    DebugMessage.Add("\tIDSegment_bitwiseXOR: '" + String.Format("0x{0:X}", MainWindow.DMO.IDSegment_bitwiseXOR) + "' (" + MainWindow.DMO.IDSegment_bitwiseXOR + ")");

				}
				catch { }



				//string tmpPath = @"C:\Users\ingow\Downloads\TMP\";
				//string URLBASE = @"http://gtav.anushk.net";
				//string[] AllFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(tmpPath);
				//foreach (string File in AllFiles )
				//{
				//	DebugMessage.Add("");
				//	DebugMessage.Add(File.Replace(tmpPath,""));
				//	DebugMessage.Add(File.Replace(tmpPath, URLBASE).Replace(@"\","/"));
				//	DebugMessage.Add(HelperClasses.FileHandling.GetHashFromFile(File));
				//	DebugMessage.Add("");
				//}


                for (int i = 0; i <= DebugMessage.Count - 1; i++)
				{
					DebugMessage[i] = HelperClasses.FileHandling.AnonymizeUser(DebugMessage[i]);
				}

				tmpsw.Stop();
				DebugMessage.Add("Generating DebugFile took " + tmpsw.ElapsedMilliseconds + " ms.");

				// Building DebugPath

				// Deletes File, Creates File, Adds to it


				HelperClasses.FileHandling.WriteStringToFileOverwrite(Globals.DebugFile, DebugMessage.ToArray());

				HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: Globals.ProjectInstallationPath);
			});
		}





	} // End of Class
} // End of NameSpace
