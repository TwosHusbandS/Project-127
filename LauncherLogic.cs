using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
				long SizeOfGTAV = HelperClasses.FileHandling.GetSizeOfFile(GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfUpdate = HelperClasses.FileHandling.GetSizeOfFile(GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf");

				if (SizeOfGTAV == SizeOfDowngradedGTAV && SizeOfUpdate == SizeOfDowngradedUPDATE)
				{
					return InstallationStates.Downgraded;
				}
				else
				{
					return InstallationStates.Upgraded;
				}
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
		/// Size of Downgraded GTAV.exe. So I can detect the InstallationState (Upgrade / Downgrade)
		/// </summary>
		public static long SizeOfDowngradedGTAV { get { return HelperClasses.FileHandling.GetSizeOfFile(DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe"); } }

		/// <summary>
		/// Size of Downgraded upgrade\upgrade.rpf . So I can detect the InstallationState (Upgrade / Downgrade)
		/// </summary>
		public static long SizeOfDowngradedUPDATE { get { return HelperClasses.FileHandling.GetSizeOfFile(DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf"); } }

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

			// Creates Hardlink Link in GTAV Installation Folder to all the files of Upgrade Folder
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
					HelperClasses.Logger.Log("File found in GTA V Installation Path and the Upgrade Folder. Will delete '" + CorrespondingFilePathInGTALocation[i] + "'", 1);
					HelperClasses.FileHandling.deleteFile(CorrespondingFilePathInGTALocation[i]);
				}

				// Creates actual Symbolic Link
				HelperClasses.Logger.Log("Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + FilesInUpgradesFiles[i] + "'", 1);
				HelperClasses.FileHandling.HardLinkFiles(CorrespondingFilePathInGTALocation[i], FilesInUpgradesFiles[i]);
			}

			// We dont need to mess with social club versions since the launch process doesnt depend on it

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
			HelperClasses.Logger.Log("Found " + FilesInUpgradeFiles.Length.ToString() + " Files in Upgrade Folder. Will try to delete them", 1);
			foreach (string myFileName in FilesInUpgradeFiles)
			{
				HelperClasses.FileHandling.deleteFile(myFileName);
			}

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			HelperClasses.Logger.Log("Repair is done. Files in Upgrade Folder deleted.");
		}

		/// <summary>
		/// Method for Downgrading
		/// </summary>
		public static void Downgrade()
		{
			KillRelevantProcesses();

			// Creates Hardlink Link in GTAV Installation Folder to all the files of Downgrade Folder
			// If they exist in GTAV Installation Folder, and in Upgrade Folder, we delete them from GTAV Installation folder
			// If they exist in GTAV Installation Folder, and NOT in Upgrade Folder, we move them there

			HelperClasses.Logger.Log("GTAV Installation Path: " + GTAVFilePath, 1);
			HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
			HelperClasses.Logger.Log("DowngradeFilePath: " + DowngradeFilePath, 1);
			HelperClasses.Logger.Log("UpgradeFilePath: " + UpgradeFilePath, 1);

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
						HelperClasses.Logger.Log("Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and $UpgradeFiles. Will delelte from GTA V Installation", 1);
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
				HelperClasses.Logger.Log("We are in TempFixSteamLaunch and will Mess with social Club installations");
				HelperClasses.ProcessHandler.StartProcess(SupportFilePath.TrimEnd('\\') + @"\SocialClubNewUninstaller.exe", "", true, true);
				new Popup(Popup.PopupWindowTypes.PopupOk, "Started the Uninstaller of new Social Club.\nClick 'OK' once the Uninstall progress is done").ShowDialog();
				HelperClasses.ProcessHandler.StartProcess(SupportFilePath.TrimEnd('\\') + @"\SocialClubOldInstaller.exe", "", true, true);
				new Popup(Popup.PopupWindowTypes.PopupOk, "Started the Installation of old Social Club.\nClick 'OK' once the Install progress is done").ShowDialog();
			}
			HelperClasses.Logger.Log("Done Downgrading");
		}

		/// <summary>
		/// Guessing Game with GTA V Paths. Have 2 ways of detecting it.
		/// </summary>
		public static void GTAVPathGuessingGame()
		{
			HelperClasses.Logger.Log("Trying to guess GTAV Path");

			// Try to find GTA V installation Path
			string potentialGTAVInstallationPath = Globals.ProjectInstallationPath.TrimEnd('\\').Substring(0, Globals.ProjectInstallationPath.LastIndexOf('\\'));
			HelperClasses.Logger.Log("First GTAV Location Guess is: '" + potentialGTAVInstallationPath + "'");

			// If our Guess is valid
			if (LauncherLogic.IsGTAVInstallationPathCorrect(potentialGTAVInstallationPath))
			{
				HelperClasses.Logger.Log("First GTAV Location Guess is valid");

				// Ask the User if its the right path
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Is: '" + potentialGTAVInstallationPath + "' your GTA V Installation Path?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					Settings.GTAVInstallationPath = potentialGTAVInstallationPath;
					HelperClasses.Logger.Log("First GTAV guess was chosen by User");
					return;
				}
				HelperClasses.Logger.Log("First GTAV guess was denied by user");
			}

			// If Setting is not correct
			if (!(LauncherLogic.IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath)))
			{
				// Doing some Magic
				string newPotentialGTAVInstallationPath = LauncherLogic.GetGTAVPathMagic();
				HelperClasses.Logger.Log("Second GTAV Location Guess is: '" + newPotentialGTAVInstallationPath + "'");

				// If that path is correct
				if (LauncherLogic.IsGTAVInstallationPathCorrect(newPotentialGTAVInstallationPath))
				{
					// Ask the User if its the right path
					HelperClasses.Logger.Log("Second GTAV Location Guess is valid");
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Is: '" + newPotentialGTAVInstallationPath + "' your GTA V Installation Path?");
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						Settings.GTAVInstallationPath = newPotentialGTAVInstallationPath;
						HelperClasses.Logger.Log("Second GTAV guess was chosen by User");
						return;
					}
					HelperClasses.Logger.Log("Second GTAV guess was denied by user");
				}
			}

			// If Setting is STILL not correct
			if (!(LauncherLogic.IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath)))
			{
				// Ask User for Path
				SetGTAVPathManually();
			}
		}

		/// <summary>
		/// Method to set the GTA V Path manually
		/// </summary>
		public static void SetGTAVPathManually()
		{
			HelperClasses.Logger.Log("Asking User for GTA V Installation path");
			string GTAVInstallationPath = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTAV.exe", @"C:\");
			while (!(LauncherLogic.IsGTAVInstallationPathCorrect(GTAVInstallationPath)))
			{
				HelperClasses.Logger.Log("GTA V installation path detected to be faulty. Asking user to try again");
				new Popup(Popup.PopupWindowTypes.PopupOk, "GTA V Path detected to be faulty. Try again");
				GTAVInstallationPath = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTAV.exe", @"C:\");
			}
			Settings.GTAVInstallationPath = GTAVInstallationPath;
		}

		/// <summary>
		/// "Cleanest" way of getting the GTA V Path automatically
		/// </summary>
		/// <returns></returns>
		public static string GetGTAVPathMagic()
		{
			HelperClasses.Logger.Log("GTAV Path Magic by steam");

			// Get all Lines of that File
			string[] MyLines = HelperClasses.FileHandling.ReadFileEachLine(Globals.SteamInstallPath.TrimEnd('\\') + @"\steamapps\libraryfolders.vdf");
			
			// Loop through those Lines
			for (int i = 0; i <= MyLines.Length - 1; i++)
			{
				// Clear them of Tabs and Spaces
				MyLines[i] = MyLines[i].Replace("\t", "").Replace(" ", "");

				// String from Regex: #"\d{1,4}""[a-zA-Z\\:]*"# (yes we are matching ", I used # as semicolons for string beginnign and end
				Regex MyRegex = new Regex("\"\\d{1,4}\"\"[a-zA-Z\\\\:]*\"");
				Match MyMatch = MyRegex.Match(MyLines[i]);

				// Regex Match them to see if we like them
				if (MyMatch.Success)
				{
					// Do some other stuff to get the actual path from it
					MyLines[i] = MyLines[i].TrimEnd('"');
					MyLines[i] = MyLines[i].Substring(MyLines[i].LastIndexOf('"') + 1);
					MyLines[i] = MyLines[i].Replace(@"\\", @"\");
					
					// If the Path contains this file, it is the GTA V Path
					if (HelperClasses.FileHandling.doesFileExist(MyLines[i].TrimEnd('\\') + @"\steamapps\appmanifest_271590.acf"))
					{
						// Build the Path to GTAV
						MyLines[i] = MyLines[i].TrimEnd('\\') + @"\steamapps\common\Grand Theft Auto V\";
						
						// Check if we can find a file from the game
						if (IsGTAVInstallationPathCorrect(MyLines[i]))
						{
							HelperClasses.Logger.Log("GTAV Path Magic by steam detected to be: '" + MyLines[i] + "'", 2);
							return MyLines[i];
						}
					}
				}
			}
			HelperClasses.Logger.Log("GTAV Path Magic by steam didnt work", 2);
			return "";
		}

		/// <summary>
		/// This actually launches the game
		/// </summary>
		public static void Launch()
		{
			HelperClasses.Logger.Log("Trying to Launch the game.");
			if (LauncherLogic.GameState == GameStates.Running)
			{
				HelperClasses.Logger.Log("Game deteced running.", 1);
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
				HelperClasses.Logger.Log("Installation State Upgraded Detected.", 1);
				HelperClasses.Logger.Log("Trying to start Game normally through Steam.", 1);
				Process gtav = new Process();
				gtav.StartInfo.FileName = Globals.SteamInstallPath.TrimEnd('\\') + @"\steam.exe";
				gtav.StartInfo.Arguments = "-applaunch 271590";
				gtav.Start();
			}
			else if (LauncherLogic.InstallationState == InstallationStates.Downgraded)
			{
				HelperClasses.Logger.Log("Installation State Downgraded Detected", 1);

				if (Settings.EnableTempFixSteamLaunch)
				{
					new Popup(Popup.PopupWindowTypes.PopupOk, "TempFixSteamLaunch launches this game through steam when downgraded\nThis only works if you have a working steam downgrade\nThis also will not be in the final release, its just to test the Client,\nwhich does not contain the actual Fixed Launch yet.\n\nClick 'OK' once steam is running and in OFFLINE mode\nOr if you wanna try this with Steam online (-scOfflineOnly is set anyways)\nYou probably wont need to do this in the next upgrade.").ShowDialog();

					HelperClasses.Logger.Log("Running game in TempFixSteamLauch way through Steam.", 1);

					HelperClasses.Logger.Log("Trying to start Game Offline.", 1);
					Process gtav = new Process();
					gtav.StartInfo.FileName = Globals.SteamInstallPath.TrimEnd('\\') + @"\steam.exe";
					gtav.StartInfo.Arguments = "-applaunch 271590 -scOfflineOnly";
					gtav.Start();
				}
				else
				{
					// TO DO, Clean this Up, move to ProcessHandler HelperClass
					HelperClasses.Logger.Log("Launching Downgraded", 1);
					new Popup(Popup.PopupWindowTypes.PopupOk, "This is where we need your help. Try to find out which Launch Method works for you. There is:\nPlayGTAV.exe\nTest3Fixed.bat\nTest9.bat\nTest15.bat\nTest17.bat\nand even more. Try all of them with and without starting as admin.\n Then report back on:\n").ShowDialog();
					Process.Start("explorer.exe", GTAVFilePath);
				}
			}
		}


		/// <summary>
		/// Helper Method which kills all Rockstar / GTA / Social Club Processes
		/// </summary>
		public static void KillRelevantProcesses()
		{
			HelperClasses.Logger.Log("Trying to kill all Rockstar related Processes");
			HelperClasses.ProcessHandler.KillRockstarProcesses();
		}

		/// <summary>
		/// Checks if Parameter Path is a correct GTA V Installation Path
		/// </summary>
		/// <param name="pPath"></param>
		/// <returns></returns>
		public static bool IsGTAVInstallationPathCorrect(string pPath)
		{
			HelperClasses.Logger.Log("Trying to see if GTAV Installation Path ('" + pPath + "')",1);
			if (HelperClasses.FileHandling.doesFileExist(pPath.TrimEnd('\\') + @"\x64a.rpf"))
			{
				HelperClasses.Logger.Log("Looks good", 2);
				return true;
			}
			else
			{
				HelperClasses.Logger.Log("Looks bad", 2);
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


	} // End of Class
} // End of NameSpace

