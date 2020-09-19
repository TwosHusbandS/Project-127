using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project_127
{
	/// <summary>
	/// Class for the launching
	/// </summary>
    public static class LauncherLogic
    {
		/// <summary>
		/// Enum for InstallationStates
		/// </summary>
		public enum InstallationStates
		{
			Upgraded,
			Downgraded
		}

		/// <summary>
		/// Enum for GameStates
		/// </summary>
		public enum GameStates
		{
			Running,
			NonRunning
		}

		/// <summary>
		/// Property of our GameState
		/// </summary>
		public static GameStates GameState
		{
			get
			{
				Process[] pname = Process.GetProcessesByName("GTAV.exe");
				if (pname.Length > 0)
				{
					return GameStates.Running;
				}
				else
				{
					return GameStates.NonRunning;
				}
			}
		}

		/// <summary>
		/// Property of what InstallationState we are in. I want to access this from here
		/// </summary>
		public static InstallationStates InstallationState
		{
			get
			{
				return Settings.InstallationState;
			}
			set
			{
				Settings.InstallationState = value;
			}
		}

		/// <summary>
		/// Reference to the Windows Function
		/// </summary>
		/// <param name="lpSymlinkFileName"></param>
		/// <param name="lpTargetFileName"></param>
		/// <param name="dwFlags"></param>
		/// <returns></returns>
		[DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode)]
		public static extern int CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

		/// <summary>
		/// Property of often used variable. (UpgradeFilePath)
		/// </summary>
		public static string UpgradeFilePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles\";

		/// <summary>
		/// Property of often used variable. (DowngradeFilePath)
		/// </summary>
		public static string DowngradeFilePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles\";
		
		/// <summary>
		/// Property of often used variable. (SupportFilePath)
		/// </summary>
		public static string SupportFilePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\";

		/// <summary>
		/// Property of often used variable. (GTAVFilePath)
		/// </summary>
		public static string GTAVFilePath = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\";

		/// <summary>
		/// Method for Upgrading the Game back to latest Version
		/// </summary>
		public static void Upgrade()
		{
			KillRelevantProcesses();

			// Creates Symbolic Link in GTAV Installation Folder to all the files of Upgrade Folder
			// If they exist in GTAV Installation Folder,  we delete them from GTAV Installation folder

			HelperClasses.Logger.Log("GTAV Installation Path: " + GTAVFilePath, 1);
			HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
			HelperClasses.Logger.Log("DowngradeFilePath: " + DowngradeFilePath, 1);
			HelperClasses.Logger.Log("UpgradeFilePath: " + UpgradeFilePath, 1);
			
			// Those are WITH the "\" at the end
			string[] FilesInUpgradesFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(UpgradeFilePath);
			string[] CorrespondingFilePathInGTALocation = new string[DowngradeFilePath.Length];


			HelperClasses.Logger.Log("Found " + FilesInUpgradesFiles.Length.ToString() + " Files in Upgrade Folder.");

			// Loop through all Files in Upgrade Files Folder
			for (int i = 0; i <= FilesInUpgradesFiles.Length - 1; i++)
			{
				// Build the Corresponding theoretical Filenames for Upgrade Folder and GTA V Installation Folder
				CorrespondingFilePathInGTALocation[i] = GTAVFilePath + FilesInUpgradesFiles[i].Substring(UpgradeFilePath.Length);


				// If the File exists in GTA V Installation Path
				if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInGTALocation[i]))
				{
					// Delete from GTA V Installation Path
					HelperClasses.Logger.Log("File found in GTA V Installation Path and the Upgrade Folder. Will delete '" + CorrespondingFilePathInGTALocation[i] + "'",1);
					HelperClasses.FileHandling.deleteFile(CorrespondingFilePathInGTALocation[i]);
				}

				// Creates actual Symbolic Link
				HelperClasses.Logger.Log("Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + FilesInUpgradesFiles[i] + "'", 1);
				HelperClasses.FileHandling.HardLinkFiles(CorrespondingFilePathInGTALocation[i], FilesInUpgradesFiles[i]);
			}

			// CTRLF TODO - JUST TEMPORARY
			if (Settings.EnableTempFixSteamLaunch)
			{
				HelperClasses.Logger.Log("EnableTempFixSteamLaunch detected. Will Uninstall Old and Install New Social Club");
				Process.Start(SupportFilePath.TrimEnd('\\') + @"\SocialClubOldUninstaller.exe").WaitForExit();
				Process.Start(SupportFilePath.TrimEnd('\\') + @"\SocialClubNewInstaller.exe").WaitForExit();
			}

			HelperClasses.Logger.Log("Done Upgrading");
		}

		/// <summary>
		/// Method for "Repairing" our setup
		/// </summary>
		public static void Repair()
		{
			HelperClasses.Logger.Log("Initiating Repair.");

			HelperClasses.Logger.Log("GTAV Installation Path: " + GTAVFilePath, 1);
			HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
			HelperClasses.Logger.Log("DowngradeFilePath: " + DowngradeFilePath, 1);
			HelperClasses.Logger.Log("UpgradeFilePath: " + UpgradeFilePath, 1);

			KillRelevantProcesses();

			string[] FilesInUpgradeFiles = Directory.GetFiles(UpgradeFilePath, "*", SearchOption.AllDirectories);
			HelperClasses.Logger.Log("Found " + FilesInUpgradeFiles.Length.ToString() + " Files in Upgrade Folder. Will try to delete them",1);
			foreach (string myFileName in FilesInUpgradeFiles)
			{
				HelperClasses.FileHandling.deleteFile(myFileName);
			}

			// CTRLF TODO - JUST TEMPORARY
			if (Settings.EnableTempFixSteamLaunch)
			{
				HelperClasses.Logger.Log("EnableTempFixSteamLaunch detected. Will Uninstall Old and Install New Social Club");
				Process.Start(SupportFilePath.TrimEnd('\\') + @"\SocialClubOldUninstaller.exe").WaitForExit();
				Process.Start(SupportFilePath.TrimEnd('\\') + @"\SocialClubNewInstaller.exe").WaitForExit();
			}

			LauncherLogic.InstallationState = LauncherLogic.InstallationStates.Upgraded;
			HelperClasses.Logger.Log("Repair is done. Files in Upgrade Folder deleted. GameState set to Upgraded");
		}

		/// <summary>
		/// Method for Downgrading
		/// </summary>
		public static void Downgrade()
		{
			KillRelevantProcesses();

			// Creates Symbolic Link in GTAV Installation Folder to all the files of Downgrade Folder
			// If they exist in GTAV Installation Folder, and in Upgrade Folder, we delete them from GTAV Installation folder
			// If they exist in GTAV Installation Folder, and NOT in Upgrade Folder, we move them there

			HelperClasses.Logger.Log("GTAV Installation Path: " + GTAVFilePath, 1);
			HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
			HelperClasses.Logger.Log("DowngradeFilePath: " + DowngradeFilePath, 1);
			HelperClasses.Logger.Log("UpgradeFilePath: " + UpgradeFilePath, 1);

			// Those are WITH the "\" at the end
			Globals.DebugPopup("Debug:\n\nGTAV Installation Path: '" + GTAVFilePath + "'\n\nInstallationLocation: '" + Globals.ProjectInstallationPath + "'\n\nDowngradeFilePath: '" + DowngradeFilePath + "'\n\nUpgradeFilePath: '" + UpgradeFilePath + "'");
			string[] FilesInDowngradeFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(DowngradeFilePath);
			string[] CorrespondingFilePathInGTALocation = new string[DowngradeFilePath.Length];
			string[] CorrespondingFilePathInUpgradeFiles = new string[DowngradeFilePath.Length];

			HelperClasses.Logger.Log("Found " + FilesInDowngradeFiles.Length.ToString() + " Files in Downgrade Folder.");

			// Loop through all Files in Downgrade Files Folder
			for (int i = 0; i <= FilesInDowngradeFiles.Length - 1; i++)
			{
				// Build the Corresponding theoretical Filenames for Upgrade Folder and GTA V Installation Folder
				CorrespondingFilePathInGTALocation[i] = GTAVFilePath + FilesInDowngradeFiles[i].Substring(DowngradeFilePath.Length);
				CorrespondingFilePathInUpgradeFiles[i] = UpgradeFilePath + FilesInDowngradeFiles[i].Substring(DowngradeFilePath.Length);

				// If the File exists in GTA V Installation Path
				if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInGTALocation[i]))
				{
					// If the File Exists in Upgrade Folder
					if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInUpgradeFiles[i]))
					{
						// Delete from GTA V Installation Path
						HelperClasses.Logger.Log("Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and $UpgradeFiles. Will delelte from GTA V Installation",1);
						HelperClasses.FileHandling.deleteFile(CorrespondingFilePathInGTALocation[i]);
					}
					else
					{
						// Move File from GTA V Installation Path to Upgrade Folder
						HelperClasses.Logger.Log("Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and NOT in $UpgradeFiles. Will move it from GTA V Installation to $UpgradeFiles", 1);
						HelperClasses.FileHandling.moveFile(CorrespondingFilePathInGTALocation[i], CorrespondingFilePathInUpgradeFiles[i]);
					}
				}
				// Creates actual Symbolic Link
				HelperClasses.Logger.Log("Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + FilesInDowngradeFiles[i] + "'", 1);
				HelperClasses.FileHandling.HardLinkFiles(CorrespondingFilePathInGTALocation[i], FilesInDowngradeFiles[i]);
			}

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			if (Settings.EnableTempFixSteamLaunch)
			{
				HelperClasses.Logger.Log("EnableTempFixSteamLaunch detected. Will Uninstall New and Install Old Social Club");
				Process.Start(SupportFilePath.TrimEnd('\\') + @"\SocialClubNewUninstaller.exe").WaitForExit();
				Process.Start(SupportFilePath.TrimEnd('\\') + @"\SocialClubOldInstaller.exe").WaitForExit();
			}

			HelperClasses.Logger.Log("Done Downgrading");
		}

		public static void Launch()
		{
			HelperClasses.Logger.Log("Trying to Launch the game.");
			if (LauncherLogic.GameState == GameStates.Running)
			{
				HelperClasses.Logger.Log("Game deteced running.",1);
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Game is detected as running.\nDo you want to close it\nand run it again?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("Killing all Processes.", 1);
					KillRelevantProcesses();
				}
				else
				{
					HelperClasses.Logger.Log("Not wanting to kill all Processes, im aborting Launch function", 1);
					return;
				}
			}
			if (LauncherLogic.InstallationState == InstallationStates.Upgraded)
			{
				HelperClasses.Logger.Log("Installation State Upgraded Detected", 1);
				
				Process gtav = new Process();
				gtav.StartInfo.FileName = Globals.SteamInstallPath.TrimEnd('\\') + @"\steam.exe";
				gtav.StartInfo.Arguments = "-applaunch 271590 -StraightIntoFreemode";
				gtav.Start(); 
			}
			else
			{
				HelperClasses.Logger.Log("Installation State Downgraded Detected", 1);

				if (Settings.EnableTempFixSteamLaunch)
				{
					Process gtav = new Process();
					gtav.StartInfo.FileName = Globals.SteamInstallPath.TrimEnd('\\') + @"\steam.exe";
					gtav.StartInfo.Arguments = "-applaunch 271590 -scOfflineOnly";
					gtav.Start();
				}
			}
		}
	

		/// <summary>
		/// Helper Method which kills all Rockstar / GTA / Social Club Processes
		/// </summary>
		public static void KillRelevantProcesses()
		{
			HelperClasses.Logger.Log("Trying to kill all relevant Processes");

			Process[] allProcesses = Process.GetProcesses();

			foreach (Process myProc in allProcesses)
			{
				// WIP Implementation.
				// TODO CTRLF
				if (myProc.ProcessName.ToString().ToLower().Contains("gta") || myProc.ProcessName.ToString().ToLower().Contains("subprocess"))
				{
					HelperClasses.Logger.Log("Found: '" + myProc.ProcessName.ToString() + "'. Will try to kill it.",1);
					try
					{
						myProc.Kill();
					}
					catch (Exception e)
					{
						HelperClasses.Logger.Log("Failed to kill: '" + myProc.ProcessName.ToString() + "'. Error Message: " + e.ToString(), 1);
					}
				}
			}
		}

		/// <summary>
		/// Checks if Parameter Path is a correct GTA V Installation Path
		/// </summary>
		/// <param name="pPath"></param>
		/// <returns></returns>
		public static bool IsGTAVInstallationPathCorrect(string pPath)
		{
			if (HelperClasses.FileHandling.doesFileExist(pPath.TrimEnd('\\') + @"\x64a.rpf"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Checks if Settings.GTAVInstallationPath is a correct GTA V Installation Path
		/// </summary>
		/// <returns></returns>
		public static bool IsGTAVInstallationPathCorrect()
		{
			return IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath);
		}

		// Havent looked at this yet, but its probably useful.

		//TODO: do checks for user errors, if uninitialized or half state, or whatever, and fix them
		//case 1: check against 1.27 if every filename of 1.27 folder is a symlink in gtav root, else uninitialized / half state
		//case 2: check against 1.27 if every filename of 1.27 folder exists in gtav root, else missing symlinks, or nonexistent files
		//case 3: support files not present in support folder inside data folder, well nothing we can do unless downloading from thing but notify users
		//if uninitialized probably just reset settings and restart and let the autosetup do the job, but not for half state
		//also implement autosetup in checks, what we need to do is: get list of file names from downgrade folder, get files with those names in the gtav root and move them to upgrade folder
		public static void PerformChecksZCRI()
        {
            // CHANGE LOG Log.Information("Checking for user error.");
            //For now it just checks if it needs the initial setup, wont be handling user errors now i guess those users better be smart and not fuck their files up (unless its my fault, then, actually nevermind its still their fault)
            bool needsUpgrade = true;
            foreach (string filePath in Directory.GetFiles($"{Settings.FileFolder}\\Downgrade"))
            {
                string fileName = Path.GetFileName(filePath);
                if (File.Exists($"{Settings.GTAVInstallationPath}\\{fileName}") && (File.GetAttributes($"{Settings.GTAVInstallationPath}\\{fileName}") & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    // CHANGE LOG Log.Information("File {fileName} needs to be moved, moving it.", fileName);
                    File.Move($"{Settings.GTAVInstallationPath}\\{fileName}", $"{Settings.FileFolder}\\Upgrade\\{fileName}");
                    needsUpgrade = true;
                }
            }

            foreach (string filePath in Directory.GetFiles($"{Settings.FileFolder}\\Downgrade\\update"))
            {
                string fileName = Path.GetFileName(filePath);
                if (File.Exists($"{Settings.GTAVInstallationPath}\\update\\{fileName}") && (File.GetAttributes($"{Settings.GTAVInstallationPath}\\update\\{fileName}") & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    // CHANGE LOG Log.Information("File {fileName} needs to be moved, moving it.", fileName);
                    File.Move($"{Settings.GTAVInstallationPath}\\update\\{fileName}", $"{Settings.FileFolder}\\Upgrade\\update\\{fileName}");
                    needsUpgrade = true;
                }
            }
            if (needsUpgrade) Upgrade();
        }

	} // End of Class
} // End of NameSpace

