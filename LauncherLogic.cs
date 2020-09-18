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
		/// Enum for GameState
		/// </summary>
		public enum GameState
		{
			Upgraded,
			Downgraded
		}

		public static GameState MyGameState = GameState.Upgraded;

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
		public static string UpgradeFilePath = Settings.InstallationPath.TrimEnd('\\') + @"\UpgradeFiles\";

		/// <summary>
		/// Property of often used variable. (DowngradeFilePath)
		/// </summary>
		public static string DowngradeFilePath = Settings.InstallationPath.TrimEnd('\\') + @"\DowngradeFiles\";

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

			// Those are WITH the "\" at the end
			string[] FilesInUpgradesFiles = Directory.GetFiles(UpgradeFilePath, "*", SearchOption.AllDirectories);
			string[] CorrespondingFilePathInGTALocation = new string[DowngradeFilePath.Length];

			HelperClasses.Logger.Log("Found " + FilesInUpgradesFiles.Length.ToString() + " Files in Upgrade Folder.");

			// Loop through all Files in Downgrade Files Folder
			for (int i = 0; i <= FilesInUpgradesFiles.Length - 1; i++)
			{
				// Build the Corresponding theoretical Filenames for Upgrade Folder and GTA V Installation Folder
				CorrespondingFilePathInGTALocation[i] = GTAVFilePath + FilesInUpgradesFiles[i].Substring(DowngradeFilePath.Length);


				// If the File exists in GTA V Installation Path
				if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInGTALocation[i]))
				{
					// Delete from GTA V Installation Path
					HelperClasses.FileHandling.deleteFile(CorrespondingFilePathInGTALocation[i]);
				}

				// Creates actual Symbolic Link
				HelperClasses.FileHandling.HardLinkFiles(CorrespondingFilePathInGTALocation[i], FilesInUpgradesFiles[i]);
			}

			// We dont need to mess with social club versions since the launch process doesnt depend on it
		}

		/// <summary>
		/// Method for "Repairing" our setup
		/// </summary>
		public static void Repair()
		{
			HelperClasses.Logger.Log("Initiating Repair.");

			KillRelevantProcesses();

			string[] FilesInUpgradeFiles = Directory.GetFiles(UpgradeFilePath, "*", SearchOption.AllDirectories);
			HelperClasses.Logger.Log("Found " + FilesInUpgradeFiles.Length.ToString() + " Files in Upgrade Folder. Will try to delete them",1);
			foreach (string myFileName in FilesInUpgradeFiles)
			{
				HelperClasses.FileHandling.deleteFile(myFileName);
			}
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

			// Those are WITH the "\" at the end
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
						HelperClasses.FileHandling.deleteFile(CorrespondingFilePathInGTALocation[i]);
					}
					else
					{
						// Move File from GTA V Installation Path to Upgrade Folder
						HelperClasses.FileHandling.moveFile(CorrespondingFilePathInGTALocation[i], CorrespondingFilePathInUpgradeFiles[i]);
					}
				}
				// Creates actual Symbolic Link
				HelperClasses.FileHandling.HardLinkFiles(CorrespondingFilePathInGTALocation[i], FilesInDowngradeFiles[i]);
			}

			// We dont need to mess with social club versions since the launch process doesnt depend on it
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

