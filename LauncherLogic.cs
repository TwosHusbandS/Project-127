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
		/// Enum we use to change the Auth Button Image (lock)
		/// </summary>
		public enum Authstates
		{
			NotAuth = 0,
			Auth = 1
		}

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

		private static Authstates _AuthState = Authstates.NotAuth;

		public static Authstates AuthState
		{
			get
			{
				return _AuthState;
			}
			set
			{
				_AuthState = value;
				// Call Something
			}
		}

		/// <summary>
		/// Property of our GameState
		/// </summary>
		public static GameStates GameState
		{
			get
			{
				if (HelperClasses.ProcessHandler.IsGtaRunning())
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
		/// Path of where the ZIP File is extracted
		/// </summary>
		public static string ZIPFilePath { get { return Settings.ZIPExtractionPath.TrimEnd('\\') + @"\"; } }

		/// <summary>
		/// Property of often used variable. (UpgradeFilePath)
		/// </summary>
		public static string UpgradeFilePath = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles\";

		/// <summary>
		/// Property of often used variable. (DowngradeFilePath)
		/// </summary>
		public static string DowngradeFilePath = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles\";

		/// <summary>
		/// Property of often used variable. (SupportFilePath)
		/// </summary>
		public static string SupportFilePath = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\";

		/// <summary>
		/// Property of often used variable. (GTAVFilePath)
		/// </summary>
		public static string GTAVFilePath = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\";

		/// <summary>
		/// Method for Upgrading the Game back to latest Version
		/// </summary>
		public static void Upgrade()
		{
			// Saving all the File Operations I want to do, executing this at the end of this Method
			List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

			KillRelevantProcesses();

			// Creates Hardlink Link in GTAV Installation Folder to all the files of Upgrade Folder
			// If they exist in GTAV Installation Folder,  we delete them from GTAV Installation folder

			HelperClasses.Logger.Log("Initiating Upgrade", 0);
			HelperClasses.Logger.Log("GTAV Installation Path: " + GTAVFilePath, 1);
			HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
			HelperClasses.Logger.Log("ZIP File Location: " + LauncherLogic.ZIPFilePath, 1);
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
					MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "File found in GTA V Installation Path and the Upgrade Folder. Will delete '" + CorrespondingFilePathInGTALocation[i] + "'", 1));
				}

				// Creates actual Hard Link (this will further down check if we should copy based on settings)
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Hardlink, FilesInUpgradesFiles[i], CorrespondingFilePathInGTALocation[i], "Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + FilesInUpgradesFiles[i] + "'", 1));
			}

			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Upgrade", MyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			HelperClasses.Logger.Log("Done Upgrading");
		}

		/// <summary>
		/// Method for "Repairing" our setup
		/// </summary>
		public static void Repair()
		{
			// Saving all the File Operations I want to do, executing this at the end of this Method
			List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

			HelperClasses.Logger.Log("Initiating Repair.", 0);
			HelperClasses.Logger.Log("GTAV Installation Path: " + GTAVFilePath, 1);
			HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
			HelperClasses.Logger.Log("ZIP File Location: " + LauncherLogic.ZIPFilePath, 1);
			HelperClasses.Logger.Log("DowngradeFilePath: " + DowngradeFilePath, 1);
			HelperClasses.Logger.Log("UpgradeFilePath: " + UpgradeFilePath, 1);

			KillRelevantProcesses();

			string[] FilesInUpgradeFiles = Directory.GetFiles(UpgradeFilePath, "*", SearchOption.AllDirectories);
			HelperClasses.Logger.Log("Found " + FilesInUpgradeFiles.Length.ToString() + " Files in Upgrade Folder. Will try to delete them", 1);
			foreach (string myFileName in FilesInUpgradeFiles)
			{
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, myFileName, "", "Deleting '" + (myFileName) + "' from the $UpgradeFolder", 2));
			}

			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Repair", MyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			HelperClasses.Logger.Log("Repair is done. Files in Upgrade Folder deleted.");
		}

		/// <summary>
		/// Method for Downgrading
		/// </summary>
		public static void Downgrade()
		{
			// Saving all the File Operations I want to do, executing this at the end of this Method
			List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

			KillRelevantProcesses();

			// Creates Hardlink Link in GTAV Installation Folder to all the files of Downgrade Folder
			// If they exist in GTAV Installation Folder, and in Upgrade Folder, we delete them from GTAV Installation folder
			// If they exist in GTAV Installation Folder, and NOT in Upgrade Folder, we move them there

			HelperClasses.Logger.Log("Initiating Downgrade", 0);
			HelperClasses.Logger.Log("GTAV Installation Path: " + GTAVFilePath, 1);
			HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
			HelperClasses.Logger.Log("ZIP File Location: " + LauncherLogic.ZIPFilePath, 1);
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
						MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and $UpgradeFiles. Will delelte from GTA V Installation", 1));
					}
					else
					{
						// Move File from GTA V Installation Path to Upgrade Folder
						MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, CorrespondingFilePathInGTALocation[i], CorrespondingFilePathInUpgradeFiles[i], "Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and NOT in $UpgradeFiles. Will move it from GTA V Installation to $UpgradeFiles", 1));
					}
				}


				// Creates actual Hard Link (this will further down check if we should copy based on settings)
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Hardlink, FilesInDowngradeFiles[i], CorrespondingFilePathInGTALocation[i], "Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + FilesInDowngradeFiles[i] + "'", 1));
			}

			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Downgrade", MyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			HelperClasses.Logger.Log("Done Downgrading");
		}

		/// <summary>
		/// Guessing Game with GTA V Paths. Have 2 ways of detecting it.
		/// </summary>
		public static void GTAVPathGuessingGame()
		{
			HelperClasses.Logger.Log("Playing the GTAV Guessing Game");

			// Try to find GTA V installation Path
			string potentialGTAVInstallationPath = Globals.ProjectInstallationPath.TrimEnd('\\').Substring(0, Globals.ProjectInstallationPath.LastIndexOf('\\'));
			HelperClasses.Logger.Log("First GTAV Location Guess (based on Project 1.27 Installation Folder) is: '" + potentialGTAVInstallationPath + "'");

			// If our Guess is valid
			if (LauncherLogic.IsGTAVInstallationPathCorrect(potentialGTAVInstallationPath, false))
			{
				HelperClasses.Logger.Log("First GTAV Location Guess is theoretical valid");

				// Ask the User if its the right path
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Is: '" + potentialGTAVInstallationPath + "' your GTA V Installation Path?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					Settings.GTAVInstallationPath = potentialGTAVInstallationPath;
					HelperClasses.Logger.Log("First GTAV guess was picked by User");
					return;
				}
				HelperClasses.Logger.Log("First GTAV guess was NOT picked by User");
			}
			else
			{
				HelperClasses.Logger.Log("First GTAV Location Guess is NOT theoretical valid");
			}

			// If Setting is not correct, we need to guess more. Settings would have been set my code above.
			if (!(LauncherLogic.IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath, false)))
			{
				HelperClasses.Logger.Log("Settings.GTAVInstallationPath is not a valid GTA V Installation Path. Will do second guess now.");

				// Doing some Magic
				string newPotentialGTAVInstallationPath = LauncherLogic.GetGTAVPathMagic();
				HelperClasses.Logger.Log("Second GTAV Location Guess (via Steam Regedit and Files) is: '" + newPotentialGTAVInstallationPath + "'");

				// If that path is correct
				if (LauncherLogic.IsGTAVInstallationPathCorrect(newPotentialGTAVInstallationPath, false))
				{
					// Ask the User if its the right path
					HelperClasses.Logger.Log("Second GTAV Location Guess is theoretical valid");
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Is: '" + newPotentialGTAVInstallationPath + "' your GTA V Installation Path?");
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						Settings.GTAVInstallationPath = newPotentialGTAVInstallationPath;
						HelperClasses.Logger.Log("Second GTAV guess was picked by User");
						return;
					}
					HelperClasses.Logger.Log("Second GTAV guess was NOT picked by User");
				}
				else
				{
					HelperClasses.Logger.Log("Second GTAV Location Guess is NOT theoretical valid");
				}
			}

			// If Setting is not correct, we need to guess more. Settings would have been set my code above.
			if (!(LauncherLogic.IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath, false)))
			{
				HelperClasses.Logger.Log("Settings.GTAVInstallationPath is not a valid GTA V Installation Path. Will do third guess now.");


				string newnewPotentialGTAVInstallationPath = ZIPFilePath.TrimEnd('\\').Substring(0, Globals.ProjectInstallationPath.LastIndexOf('\\'));
				HelperClasses.Logger.Log("Third GTAV Location Guess (based on ZIP File Location) is: '" + newnewPotentialGTAVInstallationPath + "'");

				// If that path is correct
				if (LauncherLogic.IsGTAVInstallationPathCorrect(newnewPotentialGTAVInstallationPath, false))
				{
					// Ask the User if its the right path
					HelperClasses.Logger.Log("Third GTAV Location Guess is theoretical valid");
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Is: '" + newnewPotentialGTAVInstallationPath + "' your GTA V Installation Path?");
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						Settings.GTAVInstallationPath = newnewPotentialGTAVInstallationPath;
						HelperClasses.Logger.Log("Third GTAV guess was picked by User");
						return;
					}
					HelperClasses.Logger.Log("Third GTAV guess was NOT picked by User");
				}
				else
				{
					HelperClasses.Logger.Log("Third GTAV Location Guess is NOT theoretical valid");
				}
			}

			// If Setting is STILL not correct
			if (!(LauncherLogic.IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath, false)))
			{
				// Log
				HelperClasses.Logger.Log("After three guesses we still dont have the correct GTAVInstallationPath. User has to do it manually now.");

				// Ask User for Path
				SetGTAVPathManually();
			}

			HelperClasses.Logger.Log("End of GTAVPathGuessingGame");
		}

		/// <summary>
		/// Method to set the GTA V Path manually
		/// </summary>
		public static void SetGTAVPathManually()
		{
			HelperClasses.Logger.Log("Asking User for GTA V Installation path");
			string GTAVInstallationPathUserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTAV.exe", @"C:\");
			HelperClasses.Logger.Log("Users picked path is: '" + GTAVInstallationPathUserChoice + "'");
			while (!(LauncherLogic.IsGTAVInstallationPathCorrect(GTAVInstallationPathUserChoice, false)))
			{
				HelperClasses.Logger.Log("Users picked path detected to be faulty. Asking user to try again");
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "GTA V Path detected to be not correct. Are you sure?\nForce '" + GTAVInstallationPathUserChoice + "' as your GTAV Installation Location?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("Will force the Path that user picked even tho Algorithm think its faulty.");
					Settings.GTAVInstallationPath = GTAVInstallationPathUserChoice;
					return;
				}
				GTAVInstallationPathUserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTAV.exe", @"C:\");
				HelperClasses.Logger.Log("New Users picked path is: '" + GTAVInstallationPathUserChoice + "'");
			}
			HelperClasses.Logger.Log("Picked path '" + GTAVInstallationPathUserChoice + "'´is valid and will be set as Settings.GTAVInstallationPath.");
			Settings.GTAVInstallationPath = GTAVInstallationPathUserChoice;
		}

		/// <summary>
		/// "Cleanest" way of getting the GTA V Path automatically
		/// </summary>
		/// <returns></returns>
		public static string GetGTAVPathMagic()
		{
			HelperClasses.Logger.Log("GTAV Path Magic by steam", 2);

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
							HelperClasses.Logger.Log("GTAV Path Magic by steam detected to be: '" + MyLines[i] + "'", 3);
							return MyLines[i];
						}
					}
				}
			}
			HelperClasses.Logger.Log("GTAV Path Magic by steam didnt work", 3);
			return "";
		}

		/// <summary>
		/// This actually launches the game
		/// </summary>
		public static void Launch()
		{
			HelperClasses.Logger.Log("Trying to Launch the game.");
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

				// TO DO, Clean this Up, move to ProcessHandler HelperClass
				HelperClasses.Logger.Log("Launching Downgraded", 1);
				Process p = new Process();
				p.StartInfo.FileName = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\PlayGTAV.exe";
				p.StartInfo.WorkingDirectory = Settings.GTAVInstallationPath.TrimEnd('\\');
				p.Start();
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
		public static bool IsGTAVInstallationPathCorrect(string pPath, bool pLogThis = true)
		{
			if (pLogThis) { HelperClasses.Logger.Log("Trying to see if GTAV Installation Path ('" + pPath + "') is a theoretical valid Path", 3); }
			if (HelperClasses.FileHandling.doesFileExist(pPath.TrimEnd('\\') + @"\x64b.rpf"))
			{
				if (pLogThis) { HelperClasses.Logger.Log("It is", 4); }
				return true;
			}
			else
			{
				if (pLogThis) { HelperClasses.Logger.Log("It is not", 4); }
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

