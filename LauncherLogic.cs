using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using GSF;

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
		public enum AuthStates
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
			Downgraded,
			Unsure
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
		/// AuthState Property
		/// </summary>
		public static AuthStates AuthState
		{
			get
			{
				if (ROSCommunicationBackend.SessionValid)
				{
					return AuthStates.Auth;
				}
				else
				{
					return AuthStates.NotAuth;
				}
			}
		}


		/// <summary>
		/// Property of our GameState. Gets polled every 2.5 seconds
		/// </summary>
		public static GameStates GameState
		{
			// Shit is commented out, because we dont handle the Overlay and the Keyboard Listener automatically here
			// because we use TeamSpeak 3 for testing the overlay, instead of GTA V
			get
			{
				// Check if GTA V is running
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

		private static GameStates LastGameState = GameStates.NonRunning;

		public static GameStates PollGameState()
		{
			GameStates currGameState = GameState;

			if (currGameState == GameStates.Running)
			{
				WindowChangeHander.WindowChangeEvent(WindowChangeListener.GetActiveWindowTitle());

				if (LastGameState == GameStates.NonRunning)
				{
					GTAStarted();
				}
			}
			else
			{
				if (LastGameState == GameStates.Running)
				{
					GTAClosed();
				}
			}

			LastGameState = currGameState;

			return currGameState;
		}


		public static void GTAStarted()
		{
			SetGTAProcessPriority();

			// Start Jumpscript
			if (Settings.EnableAutoStartJumpScript)
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


			NoteOverlay.OverlaySettingsChanged();

			//// If one of the Settings which require Hotkeys are enabled
			//if (Settings.EnableOverlay)
			//{
			//	// Only Start Stop shit here when the overlay is not in debugmode
			//	if (!GTAOverlay.DebugMode && GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.Borderless)
			//	{
			//		NoteOverlay.InitGTAOverlay();
			//		HelperClasses.WindowChangeListener.Start();
			//	}
			//	//else if (GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.MultiMonitor)
			//	//{
			//	//	if (HelperClasses.Keyboard.KeyboardListener.IsRunning)
			//	//	{
			//	//		HelperClasses.Keyboard.KeyboardListener.Stop();
			//	//		HelperClasses.Keyboard.KeyboardListener.Start();
			//	//	}
			//	//}
			//}
		}

		public static void GTAClosed()
		{
			Jumpscript.StopJumpscript();

			if (!GTAOverlay.DebugMode && GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.Borderless)
			{
				NoteOverlay.DisposeGTAOverlay();
				HelperClasses.Keyboard.KeyboardListener.Stop();
				HelperClasses.WindowChangeListener.Stop();
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
				long SizeOfPlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(GTAVFilePath.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfUpgradedGTAV = HelperClasses.FileHandling.GetSizeOfFile(UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfUpgradedUpdate = HelperClasses.FileHandling.GetSizeOfFile(UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfUpgradedPlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfDowngradeAlternativeSteamGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfDowngradeAlternativeSteamUpdate = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfDowngradeAlternativeSteamPlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfDowngradeAlternativeRockstarGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfDowngradeAlternativeRockstarUpdate = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfDowngradeAlternativeRockstarPlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\playgtav.exe");

				//string Message = "GTAV: '" + SizeOfGTAV + "'\nUpdate: '" + SizeOfUpdate + "'\nPlayGTAV: '" + SizeOfPlayGTAV + "'";
				//Globals.DebugPopup(Message);


				// if both Files in the GTA V Install Path exist
				if (SizeOfGTAV > 0 && SizeOfUpdate > 0 && SizeOfPlayGTAV > 0)
				{
					// if Sizes in GTA V Installation Path match what files we use from ZIP for downgrading
					if (SizeOfGTAV == SizeOfDowngradedGTAV && SizeOfUpdate == SizeOfDowngradedUPDATE && SizeOfPlayGTAV == SizeOfDowngradedPlayGTAV)
					{
						return InstallationStates.Downgraded;
					}
					else if (SizeOfGTAV == SizeOfDowngradeAlternativeSteamGTAV && SizeOfUpdate == SizeOfDowngradeAlternativeSteamUpdate && SizeOfPlayGTAV == SizeOfDowngradeAlternativeSteamPlayGTAV)
					{
						return InstallationStates.Downgraded;
					}
					else if (SizeOfGTAV == SizeOfDowngradeAlternativeRockstarGTAV && SizeOfUpdate == SizeOfDowngradeAlternativeRockstarUpdate && SizeOfPlayGTAV == SizeOfDowngradeAlternativeRockstarPlayGTAV)
					{
						return InstallationStates.Downgraded;
					}
					// if not downgraded
					else
					{
						//// If upgrade files exist
						//if (SizeOfUpgradedGTAV > 0 && SizeOfUpgradedUpdate > 0 && SizeOfUpgradedPlayGTAV > 0)
						//{
						//	// If both are NOT downgrad
						//	if (SizeOfGTAV == SizeOfUpgradedGTAV && SizeOfUpdate == SizeOfUpgradedUpdate && SizeOfPlayGTAV == SizeOfUpgradedPlayGTAV)
						//	{
						//		return InstallationStates.Upgraded;
						//	}
						//	else
						//	{
						//		return InstallationStates.Upgraded;
						//	}
						//}
						//else
						//{
						//	return InstallationStates.Upgraded;
						//}


						if (SizeOfGTAV == SizeOfDowngradedGTAV || SizeOfGTAV == SizeOfDowngradeAlternativeSteamGTAV || SizeOfGTAV == SizeOfDowngradeAlternativeRockstarGTAV)
						{
							return InstallationStates.Unsure;
						}
						else if (SizeOfGTAV == SizeOfDowngradedGTAV || SizeOfGTAV == SizeOfDowngradeAlternativeSteamGTAV || SizeOfGTAV == SizeOfDowngradeAlternativeRockstarGTAV)
						{
							return InstallationStates.Unsure;
						}
						else if (SizeOfGTAV == SizeOfDowngradedGTAV || SizeOfGTAV == SizeOfDowngradeAlternativeSteamGTAV || SizeOfGTAV == SizeOfDowngradeAlternativeRockstarGTAV)
						{
							return InstallationStates.Unsure;
						}
						else
						{
							if (SizeOfUpgradedGTAV > 0 && SizeOfUpgradedUpdate > 0 && SizeOfUpgradedPlayGTAV > 0)
							{
								return InstallationStates.Upgraded;
							}
							else
							{
								return InstallationStates.Unsure;
							}
						}
					}
				}
				else
				{
					return InstallationStates.Unsure;
				}
			}
		}

		/// <summary>
		/// Using this to keep track if we have shown the user one detected Upgrade Message per P127 Launch
		/// </summary>
		public static bool ThrewUpdateDetectedMessageAlready = false;

		/// <summary>
		/// Just a reference to the GameVersion we are running. GameVersion as in Retailer
		/// </summary>
		public static Settings.Retailers GameVersion { get { return Settings.Retailer; } }

		/// <summary>
		/// Size of Downgraded GTAV.exe. So I can detect the InstallationState (Upgrade / Downgrade)
		/// </summary>
		public static long SizeOfDowngradedGTAV { get { return HelperClasses.FileHandling.GetSizeOfFile(DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe"); } }

		/// <summary>
		/// Size of Downgraded upgrade\upgrade.rpf . So I can detect the InstallationState (Upgrade / Downgrade)
		/// </summary>
		public static long SizeOfDowngradedUPDATE { get { return HelperClasses.FileHandling.GetSizeOfFile(DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf"); } }

		/// <summary>
		/// Size of Downgraded upgrade\upgrade.rpf . So I can detect the InstallationState (Upgrade / Downgrade)
		/// </summary>
		public static long SizeOfDowngradedPlayGTAV { get { return HelperClasses.FileHandling.GetSizeOfFile(DowngradeFilePath.TrimEnd('\\') + @"\playgtav.exe"); } }

		/// <summary>
		/// Path of where the ZIP File is extracted
		/// </summary>
		public static string ZIPFilePath { get { return Settings.ZIPExtractionPath.TrimEnd('\\') + @"\"; } }

		/// <summary>
		/// Property of often used variable. (UpgradeFilePath)
		/// </summary>
		public static string UpgradeFilePath { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles\"; } }

		/// <summary>
		/// Property of often used variable. (UpgradeFilePathBackup)
		/// </summary>
		public static string UpgradeFilePathBackup { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles_Backup\"; } }

		/// <summary>
		/// Property of often used variable. (DowngradeFilePath)
		/// </summary>
		public static string DowngradeFilePath { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles\"; } }

		/// <summary>
		/// Property of often used variable. (DowngradeAlternativeFilePathSteam127)
		/// </summary>
		public static string DowngradeAlternativeFilePathSteam127 { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\steam\127\"; } }

		/// <summary>
		/// Property of often used variable. (DowngradeAlternativeFilePathRockstar127)
		/// </summary>
		public static string DowngradeAlternativeFilePathRockstar127 { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\rockstar\127\"; } }


		/// <summary>
		/// Property of often used variable. (DowngradeAlternativeFilePathSteam124)
		/// </summary>
		public static string DowngradeAlternativeFilePathSteam124 { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\steam\124\"; } }

		/// <summary>
		/// Property of often used variable. (DowngradeAlternativeFilePathRockstar124)
		/// </summary>
		public static string DowngradeAlternativeFilePathRockstar124 { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\rockstar\124\"; } }


		/// <summary>
		/// Property of often used variable. (DowngradedSocialClub)
		/// </summary>
		public static string DowngradedSocialClub { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\DowngradedSocialClub\"; } }



		/// <summary>
		/// Property of often used variable. (SupportFilePath)
		/// </summary>
		public static string SupportFilePath { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\"; } }

		/// <summary>
		/// Property of often used variable. (SupportFilePath)
		/// </summary>
		public static string SaveFilesPath { get { return LauncherLogic.SupportFilePath.TrimEnd('\\') + @"\SaveFiles\"; } }

		/// <summary>
		/// Property of often used variable. (GTAVFilePath)
		/// </summary>
		public static string GTAVFilePath { get { return Settings.GTAVInstallationPath.TrimEnd('\\') + @"\"; } }

		/// <summary>
		/// Method for Upgrading the Game back to latest Version
		/// </summary>
		public static void Upgrade()
		{
			if (HandleUpdates())
			{
				return;
			}


			// Cancel any stuff when we have no files in upgrade files...simple right?
			if (HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(UpgradeFilePath).Length <= 1)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "Found no Files to Upgrade with. I suggest verifying Files through steam\nor clicking \"Use Backup Files\" in Settings.\nWill abort Upgrade.").ShowDialog();
				return;
			}

			KillRelevantProcesses();


			PopupProgress tmp = new PopupProgress(PopupProgress.ProgressTypes.Upgrade, "");
			tmp.ShowDialog();
			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing an Upgrade", tmp.RtrnMyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			if (InstallationState != InstallationStates.Upgraded)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "We just did an Upgrade but the detected InstallationState is not Upgraded.\nI suggest reading the \"Help\" Part of the Information Page");
			}

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
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing a Repair", MyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			HelperClasses.Logger.Log("Repair is done. Files in Upgrade Folder deleted.");
		}

		/// <summary>
		/// Method for Downgrading
		/// </summary>
		public static void Downgrade()
		{
			if (HandleUpdates())
			{
				return;
			}

			KillRelevantProcesses();

			PopupProgress tmp = new PopupProgress(PopupProgress.ProgressTypes.Downgrade, "");
			tmp.ShowDialog();

			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing a Downgrade", tmp.RtrnMyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			if (InstallationState != InstallationStates.Downgraded)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "We just did an Downgraded but the detected InstallationState is not Downgraded.\nI suggest reading the \"Help\" Part of the Information Page");
			}


			HelperClasses.Logger.Log("Done Downgrading");
		}


		/// <summary>
		/// Checks if update hit, asks User, handles User interaction. Returns if it handled an update.
		/// </summary>
		public static bool HandleUpdates()
		{
			HelperClasses.Logger.Log("Checking if an Update hit");
			if (HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.UpgradeFilePath).Length > 1)
			{
				if (DidUpdateHit())
				{
					if (ThrewUpdateDetectedMessageAlready == false)
					{
						ThrewUpdateDetectedMessageAlready = true;

						HelperClasses.Logger.Log("Apparently it did. Lets see if the user wants a repair");
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Detected an automatic Update of GTA.\nDo you want to use your current state of GTA V\nas your new \"Upgraded\" Files?\nI recommend \"Yes\"\nThis will create a Backup of the Files P127 uses for Upgrading");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
						{
							HelperClasses.Logger.Log("User does want it. Initiating CreateBackup()");

							KillRelevantProcesses();

							LauncherLogic.CreateBackup();

							// Dont repair, so we still have UpgradeFiles folder from before backup. We need it.
							//LauncherLogic.Repair();

							return true;
						}
						else
						{
							HelperClasses.Logger.Log("User doesnt want it. Alright then");
						}
					}
					else
					{
						HelperClasses.Logger.Log("Update detected but we threw a popup already");
					}
				}
				else
				{
					HelperClasses.Logger.Log("No update detected");
				}
			}
			else
			{
				HelperClasses.Logger.Log("No Files in $Upgrade_Files, so im not even checking if update hit");
			}
			return false;
		}

		/// <summary>
		///  Returns Bool whether or not we think that an update hit.
		/// </summary>
		/// <returns></returns>
		public static bool DidUpdateHit()
		{
			if (1 == 2)
			{
				PopupProgress tmp = new PopupProgress(PopupProgress.ProgressTypes.DidUpdateHit, "");
				tmp.ShowDialog();
				return tmp.RtrnBool;
			}
			return false;
		}


		/// <summary>
		/// This actually launches the game
		/// </summary>
		public static async void Launch()
		{
			HelperClasses.Logger.Log("Trying to Launch the game.");

			// If Upgraded
			if (LauncherLogic.InstallationState == InstallationStates.Upgraded)
			{
				HelperClasses.Logger.Log("Installation State Upgraded Detected.", 1);

				// If Steam
				if (GameVersion == Settings.Retailers.Steam)
				{
					HelperClasses.Logger.Log("Trying to start Game normally through Steam.", 1);
					// Launch through steam
					HelperClasses.ProcessHandler.StartProcess(Globals.SteamInstallPath.TrimEnd('\\') + @"\steam.exe", pCommandLineArguments: "-applaunch 271590 -uilanguage " + Settings.ToMyLanguageString(Settings.LanguageSelected).ToLower());
				}
				// If Epic Games
				else if (GameVersion == Settings.Retailers.Epic)
				{

					HelperClasses.Logger.Log("Trying to start Game normally through EpicGames.", 1);

					// This does not work with custom wrapper StartProcess in ProcessHandler...i guess this is fine
					Process.Start(@"com.epicgames.launcher://apps/9d2d0eb64d5c44529cece33fe2a46482?action=launch&silent=true");
				}
				// If Rockstar
				else
				{
					// Launch through Non Retail re
					HelperClasses.ProcessHandler.StartGameNonRetail();
				}
			}
			else if (LauncherLogic.InstallationState == InstallationStates.Downgraded)
			{
				HelperClasses.Logger.Log("Installation State Downgraded Detected.", 1);

				if (!Settings.EnableAlternativeLaunch)
				{
					// If already Authed
					if (AuthState == AuthStates.Auth)
					{
						HelperClasses.Logger.Log("You are already Authenticated. Will Launch Game Now");
					}

					// If not Authed
					else
					{
						HelperClasses.Logger.Log("You are NOT already Authenticated. Throwing up Window now.");

						// Trying to Auth User
						Globals.LaunchAfterAuth = true;
						Globals.PageState = Globals.PageStates.Auth;
						return;
					}

					// Generates Token needed to Launch Downgraded GTAV
					HelperClasses.Logger.Log("Letting Dragon work his magic");
					await ROSCommunicationBackend.GenToken();


					// If Steam
					if (GameVersion == Settings.Retailers.Steam && !Settings.EnableDontLaunchThroughSteam)
					{
						HelperClasses.Logger.Log("Trying to start Game normally through Steam.", 1);
						// Launch through steam
						HelperClasses.ProcessHandler.StartProcess(Globals.SteamInstallPath.TrimEnd('\\') + @"\steam.exe", pCommandLineArguments: "-applaunch 271590 -uilanguage " + Settings.ToMyLanguageString(Settings.LanguageSelected).ToLower());

					}
					else
					{
						HelperClasses.Logger.Log("Trying to start Game normally non retail.", 1);
						// Launch through Non Retail re
						HelperClasses.ProcessHandler.StartGameNonRetail();
					}
				}
				else
				{
					LaunchAlternative.Launch();
				}

			}
			else
			{
				HelperClasses.Logger.Log("Installation State Broken");
				HelperClasses.Logger.Log("    Size of GTA5.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				HelperClasses.Logger.Log("    Size of GTA5.exe in Downgrade Files Folder: " + LauncherLogic.SizeOfDowngradedGTAV);
				HelperClasses.Logger.Log("    Size of update.rpf in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				HelperClasses.Logger.Log("    Size of update.rpf in Downgrade Files Folder: " + LauncherLogic.SizeOfDowngradedUPDATE);

				new Popup(Popup.PopupWindowTypes.PopupOkError, "Installation State is broken for some reason. Try to repair.");
				return;
			}




			HelperClasses.Logger.Log("Game should be launched");

			PostLaunchEvents();
		}




		/// <summary>
		/// Method which gets called after Starting GTAV
		/// </summary>
		public async static void PostLaunchEvents()
		{
			HelperClasses.Logger.Log("Post Launch Events started");
			await Task.Delay(2500);
			HelperClasses.Logger.Log("Waited a good bit");

			HelperClasses.Logger.Log("Trying to Set GTAV Process Priority to High");
			SetGTAProcessPriority();

			// If we DONT only auto start when downgraded OR if we are downgraded
			if (Settings.EnableOnlyAutoStartProgramsWhenDowngraded == false || LauncherLogic.InstallationState == InstallationStates.Downgraded)
			{
				HelperClasses.Logger.Log("Either we are Downgraded or EnableOnlyAutoStartProgramsWhenDowngraded is set to false");
				if (Settings.EnableAutoStartFPSLimiter)
				{
					HelperClasses.Logger.Log("We are trying to auto Start FPS Limiter: '" + Settings.PathFPSLimiter + "'");
					string ProcessName = HelperClasses.FileHandling.PathSplitUp(Settings.PathFPSLimiter)[1];
					if (!HelperClasses.ProcessHandler.IsProcessRunning(ProcessName))
					{
						HelperClasses.Logger.Log("Process is not already running...", 1);
						if (HelperClasses.FileHandling.doesFileExist(Settings.PathFPSLimiter))
						{
							HelperClasses.Logger.Log("File does exist, lets start it...", 1);
							try
							{
								string[] Stufferino = HelperClasses.FileHandling.PathSplitUp(Settings.PathFPSLimiter);
								HelperClasses.ProcessHandler.StartProcess(Settings.PathFPSLimiter, Stufferino[0]);
							}
							catch { }
						}
						else
						{
							HelperClasses.Logger.Log("Path (File) seems to not exist.", 1);
						}
					}
					else
					{
						HelperClasses.Logger.Log("Seems to be running already", 1);
					}
				}
				if (Settings.EnableAutoStartLiveSplit)
				{
					HelperClasses.Logger.Log("We are trying to auto Start LiveSplit: '" + Settings.PathLiveSplit + "'");
					string ProcessName = HelperClasses.FileHandling.PathSplitUp(Settings.PathLiveSplit)[1];
					if (!HelperClasses.ProcessHandler.IsProcessRunning(ProcessName))
					{
						HelperClasses.Logger.Log("Process is not already running...", 1);
						if (HelperClasses.FileHandling.doesFileExist(Settings.PathLiveSplit))
						{
							HelperClasses.Logger.Log("File does exist, lets start it...", 1);
							try
							{
								string[] Stufferino = HelperClasses.FileHandling.PathSplitUp(Settings.PathLiveSplit);
								HelperClasses.ProcessHandler.StartProcess(Settings.PathLiveSplit, Stufferino[0]);
							}
							catch { }
						}
						else
						{
							HelperClasses.Logger.Log("Path (File) seems to not exist.", 1);
						}
					}
					else
					{
						HelperClasses.Logger.Log("Seems to be running already", 1);
					}
				}
				if (Settings.EnableAutoStartStreamProgram)
				{
					HelperClasses.Logger.Log("We are trying to auto Start Stream Program: '" + Settings.PathStreamProgram + "'");
					string ProcessName = HelperClasses.FileHandling.PathSplitUp(Settings.PathStreamProgram)[1];
					if (!HelperClasses.ProcessHandler.IsProcessRunning(ProcessName))
					{
						HelperClasses.Logger.Log("Process is not already running...", 1);
						if (HelperClasses.FileHandling.doesFileExist(Settings.PathStreamProgram))
						{
							HelperClasses.Logger.Log("File does exist, lets start it...", 1);
							try
							{
								string[] Stufferino = HelperClasses.FileHandling.PathSplitUp(Settings.PathStreamProgram);
								HelperClasses.ProcessHandler.StartProcess(Settings.PathStreamProgram, Stufferino[0]);
							}
							catch { }
						}
						else
						{
							HelperClasses.Logger.Log("Path (File) seems to not exist.", 1);
						}
					}
					else
					{
						HelperClasses.Logger.Log("Seems to be running already", 1);
					}
				}
				if (Settings.EnableAutoStartNohboard)
				{
					HelperClasses.Logger.Log("We are trying to auto Start Nohboard: '" + Settings.PathNohboard + "'");
					string ProcessName = HelperClasses.FileHandling.PathSplitUp(Settings.PathNohboard)[1];
					if (!HelperClasses.ProcessHandler.IsProcessRunning(ProcessName))
					{
						HelperClasses.Logger.Log("Process is not already running...", 1);
						if (HelperClasses.FileHandling.doesFileExist(Settings.PathNohboard))
						{
							HelperClasses.Logger.Log("File does exist, lets start it...", 1);
							try
							{
								string[] Stufferino = HelperClasses.FileHandling.PathSplitUp(Settings.PathNohboard);
								HelperClasses.ProcessHandler.StartProcess(Settings.PathNohboard, Stufferino[0]);
							}
							catch { }
						}
						else
						{
							HelperClasses.Logger.Log("Path (File) seems to not exist.", 1);
						}
					}
					else
					{
						HelperClasses.Logger.Log("Seems to be running already", 1);
					}
				}
			}


		}

		public static void SetGTAProcessPriority()
		{
			if (Settings.EnableAutoSetHighPriority)
			{
				try
				{
					Process[] processes = HelperClasses.ProcessHandler.GetProcesses("gta5");
					if (processes.Length > 0)
					{
						if (processes[0].PriorityClass != ProcessPriorityClass.High)
						{
							processes[0].PriorityClass = ProcessPriorityClass.High;
							HelperClasses.Logger.Log("Set GTA5 Process Priority to High");
						}

					}
				}
				catch
				{
					HelperClasses.Logger.Log("Failed to get GTA5 Process...");
				}
			}
		}



		public static void CreateBackup(string NewPath = "")
		{
			string OrigPath = LauncherLogic.UpgradeFilePath.TrimEnd('\\');
			if (NewPath == "")
			{
				NewPath = Directory.GetParent(OrigPath).ToString().TrimEnd('\\') + @"\UpgradeFiles_Backup";
			}
			else
			{
				NewPath = Directory.GetParent(OrigPath).ToString().TrimEnd('\\') + @"\UpgradeFiles_Backup_" + NewPath.TrimEnd('\\');
			}

			if (HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(OrigPath).Length <= 1)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "No Upgrade Files available to back up.").ShowDialog();
				return;
			}
			else
			{
				List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, NewPath, "", "Deleting Path: '" + (NewPath) + "'", 2, MyFileOperation.FileOrFolder.Folder));
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Create, NewPath, "", "Creating Path: '" + (NewPath) + "'", 2, MyFileOperation.FileOrFolder.Folder));
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Create, NewPath + @"\update", "", "Creating Path: '" + (NewPath + @"\update") + "'", 2, MyFileOperation.FileOrFolder.Folder));

				string[] FilesFromOrigPath = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(OrigPath);
				string[] CorrespondingFilesFromNewPath = new string[FilesFromOrigPath.Length];


				for (int i = 0; i <= FilesFromOrigPath.Length - 1; i++)
				{
					CorrespondingFilesFromNewPath[i] = NewPath + FilesFromOrigPath[i].Substring(OrigPath.Length);
					MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, FilesFromOrigPath[i], CorrespondingFilesFromNewPath[i], "Copying: '" + (FilesFromOrigPath[i]) + "' to '" + CorrespondingFilesFromNewPath[i] + "'", 2, MyFileOperation.FileOrFolder.File));
				}

				new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Creating Backup", MyFileOperations).ShowDialog();

				new Popup(Popup.PopupWindowTypes.PopupOk, "Files are now backed up.").ShowDialog();
			}
		}

		public static void UseBackup(string NewPath = "")
		{
			string OrigPath = LauncherLogic.UpgradeFilePath.TrimEnd('\\');
			if (NewPath == "")
			{
				NewPath = Directory.GetParent(OrigPath).ToString().TrimEnd('\\') + @"\UpgradeFiles_Backup";
			}
			else
			{
				NewPath = Directory.GetParent(OrigPath).ToString().TrimEnd('\\') + @"\UpgradeFiles_Backup_" + NewPath.TrimEnd('\\');
			}

			if (HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(NewPath).Length <= 1)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "No Backup Files available.").ShowDialog();
				return;
			}
			else
			{
				List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, OrigPath, "", "Deleting Path: '" + (OrigPath) + "'", 2, MyFileOperation.FileOrFolder.Folder));
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Create, OrigPath, "", "Creating Path: '" + (OrigPath) + "'", 2, MyFileOperation.FileOrFolder.Folder));
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Create, OrigPath + @"\update", "", "Creating Path: '" + (OrigPath + @"\update") + "'", 2, MyFileOperation.FileOrFolder.Folder));

				string[] FilesFromNewPath = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(NewPath);
				string[] CorrespondingFilesFromOrigPath = new string[FilesFromNewPath.Length];

				for (int i = 0; i <= FilesFromNewPath.Length - 1; i++)
				{
					CorrespondingFilesFromOrigPath[i] = OrigPath + FilesFromNewPath[i].Substring(NewPath.Length);
					MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, FilesFromNewPath[i], CorrespondingFilesFromOrigPath[i], "Copying: '" + (FilesFromNewPath[i]) + "' to '" + CorrespondingFilesFromOrigPath[i] + "'", 2, MyFileOperation.FileOrFolder.File));
				}

				new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Applying Backup", MyFileOperations).ShowDialog();

				new Popup(Popup.PopupWindowTypes.PopupOk, "Using backup files now.").ShowDialog();

				// Keep this here...
				string asdf = InstallationState.ToString();
				if (asdf.Length > 10000)
				{
					Globals.DebugPopup(asdf);
				}
			}

		}




		/// <summary>
		/// Method to import Zip File
		/// </summary>
		public static void ImportZip(string pZipFileLocation, bool deleteFileAfter = false)
		{
			if (deleteFileAfter == false)
			{
				HelperClasses.Logger.Log("Importing ZIP File manually");

				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "You are manually importing a ZIP File.\nProject 1.27 cannot gurantee the integrity of the ZIP File.\nThis is the case even if you got the Download Link through Project 1.27 Help Page\nThe person hosting this file or the Person you got the Link from could have altered the Files inside to include malicious files.\nDo you still want to import the ZIP File?");
				yesno.ShowDialog();
				if (yesno.DialogResult == false)
				{
					HelperClasses.Logger.Log("User does NOT trust the ZIP File. Will abort.");
					return;
				}
				else
				{
					HelperClasses.Logger.Log("User DOES trust the ZIP File. Will continue.");
				}
			}

			// Creating all needed Folders
			HelperClasses.FileHandling.CreateAllZIPPaths(Settings.ZIPExtractionPath);

			// Getting some Info of the current Installation
			LauncherLogic.InstallationStates OldInstallationState = LauncherLogic.InstallationState;
			string OldHash = HelperClasses.FileHandling.CreateDirectoryMd5(LauncherLogic.DowngradeFilePath);

			HelperClasses.Logger.Log("Importing ZIP File: '" + pZipFileLocation + "'");
			HelperClasses.Logger.Log("Old ZIP File Version: '" + Globals.ZipVersion + "'");
			HelperClasses.Logger.Log("Old Installation State: '" + OldInstallationState + "'");
			HelperClasses.Logger.Log("Old Hash of Downgrade Folder: '" + OldHash + "'");
			HelperClasses.Logger.Log("Settings.ZIPPath: '" + Settings.ZIPExtractionPath + "'");


			string[] myFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files");

			// Dont need this for now, lets keep it in case its needed again
			//foreach (string myFile in myFiles)
			//{
			//	if (!myFile.Contains("UpgradeFiles"))
			//	{
			//		// Deleting all Files which are NOT in the UpgradeFiles Folder of the ZIP Extract Path
			//		HelperClasses.FileHandling.deleteFile(myFile);
			//	}
			//}

			// Actually Extracting the ZIP File
			HelperClasses.Logger.Log("Extracting ZIP File: '" + pZipFileLocation + "' to the path: '" + LauncherLogic.ZIPFilePath + "'");
			new PopupProgress(PopupProgress.ProgressTypes.ZIPFile, pZipFileLocation).ShowDialog();


			// Deleting the ZIP File
			if (deleteFileAfter)
			{
				HelperClasses.Logger.Log("Deleting ZIP File: '" + pZipFileLocation + "'");
				HelperClasses.FileHandling.deleteFile(pZipFileLocation);
			}

			LauncherLogic.InstallationStates NewInstallationState = LauncherLogic.InstallationState;
			string NewHash = HelperClasses.FileHandling.CreateDirectoryMd5(LauncherLogic.DowngradeFilePath);

			HelperClasses.Logger.Log("Done Importing ZIP File: '" + pZipFileLocation + "'");
			HelperClasses.Logger.Log("New ZIP File Version: '" + Globals.ZipVersion + "'");
			HelperClasses.Logger.Log("New Installation State: '" + NewInstallationState + "'");
			HelperClasses.Logger.Log("New Hash of Downgrade Folder: '" + NewHash + "'");


			// If the state was Downgraded before Importing ZIP-File
			if (OldInstallationState == LauncherLogic.InstallationStates.Downgraded)
			{
				// If old and new Hash (of downgrade folder) dont match
				if (OldHash != NewHash)
				{
					// Downgrade again
					LauncherLogic.Downgrade();
				}
			}

			new Popup(Popup.PopupWindowTypes.PopupOk, "ZIP File imported (Version: '" + Globals.ZipVersion + "')").ShowDialog();
		}



		/// <summary>
		/// "Cleanest" way of getting the GTA V Path automatically
		/// </summary>
		/// <returns></returns>
		public static string GetGTAVPathMagicEpic()
		{
			HelperClasses.Logger.Log("GTAV Path Magic by epic", 2);

			string[] MyFiles = HelperClasses.FileHandling.GetFilesFromFolder(@"C:\ProgramData\Epic\EpicGamesLauncher\Data\Manifests");

			foreach (string MyFile in MyFiles)
			{
				Regex MyRegex = new Regex(@"C:\\ProgramData\\Epic\\EpicGamesLauncher\\Data\\Manifests\\[0-9A-F]*.item");
				Match MyMatch = MyRegex.Match(MyFile);

				// Regex Match them to see if we like them
				if (MyMatch.Success)
				{
					// Get all Lines of that File
					string[] MyLines = HelperClasses.FileHandling.ReadFileEachLine(MyFile);

					// Loop through those Lines
					for (int i = 0; i <= MyLines.Length - 1; i++)
					{
						// Clear them of Tabs and Spaces
						MyLines[i] = MyLines[i].Replace("\t", "").Replace(" ", "");
						MyLines[i] = MyLines[i].TrimEnd(',').TrimEnd('"');

						// if DisplayName is something else, lets exit
						if (MyLines[i].Contains("\"DisplayName\":"))
						{
							if (!MyLines[i].Contains("GrandTheftAutoV"))
							{
								break;
							}
						}


						if (MyLines[i].Contains("\"InstallLocation\":"))
						{
							string path = MyLines[i].Substring(MyLines[i].LastIndexOf('"')).Replace(@"\\", @"\");
							HelperClasses.Logger.Log("GTAV Path Magic by Epic detected to be: '" + path + "'", 3);
							return path;
						}
					}
				}
			}
			HelperClasses.Logger.Log("GTAV Path Magic by Epic didnt work", 3);
			return "";
		}


		/// <summary>
		/// "Cleanest" way of getting the GTA V Path automatically
		/// </summary>
		/// <returns></returns>
		public static string GetGTAVPathMagicRockstar()
		{
			HelperClasses.Logger.Log("GTAV Path Magic by Rockstar", 2);
			RegistryKey myRK2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{5EFC6C07-6B87-43FC-9524-F9E967241741}");
			return HelperClasses.RegeditHandler.GetValue(myRK2, "InstallLocation");
		}


		/// <summary>
		/// "Cleanest" way of getting the GTA V Path automatically
		/// </summary>
		/// <returns></returns>
		public static string GetGTAVPathMagicSteam()
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
		public static bool IsGTAVInstallationPathCorrect(bool LogAttempt = true)
		{
			return IsGTAVInstallationPathCorrect(Settings.GTAVInstallationPath, LogAttempt);
		}


	} // End of Class
} // End of NameSpace

