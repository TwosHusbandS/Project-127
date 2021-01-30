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
		#region States (enums) Auth, Installation, Game, GameStarted() GameExited()

		/// <summary>
		/// Enum for GameStates
		/// </summary>
		public enum GameStates
		{
			Running,
			NonRunning
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


		public static async void GTAStarted()
		{

			Globals.RunningGTABuild = Globals.GTABuild;

			await Task.Delay(5000);

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

			if (Settings.EnableOverlay && Settings.OverlayMultiMonitorMode)
			{
				if (MainWindow.OL_MM != null)
				{
					if (MainWindow.OL_MM.IsDisplayed() == true)
					{
						NoteOverlay.OverlaySettingsChanged(true);
						return;
					}
				}
			}


			NoteOverlay.OverlaySettingsChanged();

		}


		public static bool UpgradeSocialClubAfterGame = false;

		public static void GTAClosed()
		{
			Jumpscript.StopJumpscript();

			Globals.RunningGTABuild = new Version(0, 0);

			if (!GTAOverlay.DebugMode && GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.Borderless)
			{
				NoteOverlay.DisposeGTAOverlay();
				HelperClasses.Keyboard.KeyboardListener.Stop();
				HelperClasses.WindowChangeListener.Stop();
			}

			if (UpgradeSocialClubAfterGame)
			{
				LaunchAlternative.SocialClubDowngrade(7500);
				UpgradeSocialClubAfterGame = false;
			}
		}



		/// <summary>
		/// Enum we use to change the Auth Button Image (lock)
		/// </summary>
		public enum AuthStates
		{
			NotAuth = 0,
			Auth = 1
		}

		/// <summary>
		/// Overwrites the AuthState. Enable to test some UI, and UX choices.
		/// </summary>
		public static bool AuthStateOverWrite = false;

		/// <summary>
		/// Uses a config file for the emu. To debug emu without it taking P127 Settings Settings.
		/// </summary>
		public static bool UseEmuConfigFile = false;

		/// <summary>
		/// AuthState Property
		/// </summary>
		public static AuthStates AuthState
		{
			get
			{
				if (AuthStateOverWrite)
				{
					return AuthStates.Auth;
				}

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
		/// Enum for InstallationStates
		/// </summary>
		public enum InstallationStates
		{
			Upgraded,
			Downgraded,
			Unsure
		}

		public static bool RockstarFuckedUsErrorThrownAlread = false;

		/// <summary>
		/// Property of what InstallationState we are in. I want to access this from here
		/// </summary>
		public static InstallationStates InstallationState
		{
			get
			{
				InstallationStates rtrn = InstallationStates.Unsure;

				long SizeOfGTAV = HelperClasses.FileHandling.GetSizeOfFile(GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfUpdate = HelperClasses.FileHandling.GetSizeOfFile(GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfPlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(GTAVFilePath.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfUpgradedGTAV = HelperClasses.FileHandling.GetSizeOfFile(UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfUpgradedUpdate = HelperClasses.FileHandling.GetSizeOfFile(UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfUpgradedPlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfDowngradeEmuGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfDowngradeEmuUpdate = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfDowngradeEmuPlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeEmuFilePath.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfDowngradeAlternativeSteam127GTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfDowngradeAlternativeSteam127Update = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfDowngradeAlternativeSteam127PlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam127.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfDowngradeAlternativeRockstar127GTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfDowngradeAlternativeRockstar127Update = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfDowngradeAlternativeRockstar127PlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar127.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfDowngradeAlternativeSteam124GTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam124.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfDowngradeAlternativeSteam124Update = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam124.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfDowngradeAlternativeSteam124PlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathSteam124.TrimEnd('\\') + @"\playgtav.exe");

				long SizeOfDowngradeAlternativeRockstar124GTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar124.TrimEnd('\\') + @"\GTA5.exe");
				long SizeOfDowngradeAlternativeRockstar124Update = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar124.TrimEnd('\\') + @"\update\update.rpf");
				long SizeOfDowngradeAlternativeRockstar124PlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeAlternativeFilePathRockstar124.TrimEnd('\\') + @"\playgtav.exe");

				// if both Files in the GTA V Install Path exist
				if (SizeOfGTAV > 0 && SizeOfUpdate > 0)
				{
					// if Sizes in GTA V Installation Path match what files we use from ZIP for downgrading
					if (SizeOfGTAV == SizeOfDowngradeEmuGTAV && SizeOfUpdate == SizeOfDowngradeEmuUpdate && SizeOfPlayGTAV == SizeOfDowngradeEmuPlayGTAV)
					{
						rtrn = InstallationStates.Downgraded;
					}
					else if (SizeOfGTAV == SizeOfDowngradeAlternativeSteam127GTAV && SizeOfUpdate == SizeOfDowngradeAlternativeSteam127Update)
					{
						rtrn = InstallationStates.Downgraded;
					}
					else if (SizeOfGTAV == SizeOfDowngradeAlternativeRockstar127GTAV && SizeOfUpdate == SizeOfDowngradeAlternativeRockstar127Update)
					{
						rtrn = InstallationStates.Downgraded;
					}
					else if (SizeOfGTAV == SizeOfDowngradeAlternativeSteam124GTAV && SizeOfUpdate == SizeOfDowngradeAlternativeSteam124Update)
					{
						rtrn = InstallationStates.Downgraded;
					}
					else if (SizeOfGTAV == SizeOfDowngradeAlternativeRockstar124GTAV && SizeOfUpdate == SizeOfDowngradeAlternativeRockstar124Update)
					{
						rtrn = InstallationStates.Downgraded;
					}
					// if not downgraded
					else
					{
						if (SizeOfGTAV > 0 && SizeOfUpdate > 0 && SizeOfPlayGTAV > 0)
						{
							if (BuildVersionTable.GetGameVersionOfBuild(Globals.GTABuild) > new Version(1, 30))
							{
								rtrn = InstallationStates.Upgraded;
							}
						}
					}
				}


				// DETECTING IF ROCKSTAR FUCKED US
				if (rtrn == InstallationStates.Downgraded)
				{
					if (BuildVersionTable.GetGameVersionOfBuild(Globals.GTABuild) > new Version(1, 30))
					{
						if (!ThrewUpdateDetectedMessageAlready)
						{
							Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "It appears like Rockstar (or Steam and Epic although unlikely) messed up Project 1.27 Files.\nDo you want to correct them?");
							yesno.ShowDialog();
							if (yesno.DialogResult == true)
							{
								Upgrade();

								if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch || Settings.Retailer == Settings.Retailers.Epic)
								{
									ComponentManager.Components.Base.ReInstall();
								}
								else
								{
									if (Settings.Retailer == Settings.Retailers.Rockstar)
									{
										if (Settings.SocialClubLaunchGameVersion == "124")
										{
											ComponentManager.Components.SCLRockstar124.ReInstall();

										}
										else
										{
											ComponentManager.Components.SCLRockstar127.ReInstall();
										}
									}
									else if (Settings.Retailer == Settings.Retailers.Steam)
									{
										if (Settings.SocialClubLaunchGameVersion == "124")
										{
											ComponentManager.Components.SCLSteam124.ReInstall();

										}
										else
										{
											ComponentManager.Components.SCLSteam127.ReInstall();
										}
									}
								}
							}
							ThrewUpdateDetectedMessageAlready = true;
						}
					}
				}

				return rtrn;
			}
		}


		/// <summary>
		/// Enum for LaunchWays
		/// </summary>
		public enum LaunchWays
		{
			DragonEmu,
			SocialClubLaunch
		}

		public static LaunchWays LaunchWay
		{
			get
			{
				if (Settings.EnableAlternativeLaunch)
				{
					return LaunchWays.SocialClubLaunch;
				}
				else
				{
					return LaunchWays.DragonEmu;
				}
			}
			set
			{
				if (Settings.Retailer == Settings.Retailers.Epic)
				{
					if (value == LaunchWays.SocialClubLaunch)
					{
						// dont allow changing to social club
						new Popup(Popup.PopupWindowTypes.PopupOk, "LaunchWay was not changed.").ShowDialog();
						return;
					}
				}

				if (ComponentManager.RecommendUpgradedGTA())
				{
					if (value == LaunchWays.SocialClubLaunch)
					{
						Settings.EnableAlternativeLaunch = true;
					}
					else
					{
						Settings.EnableAlternativeLaunch = false;
					}
				}
				else
				{
					new Popup(Popup.PopupWindowTypes.PopupOk, "LaunchWay was not changed.").ShowDialog();
				}

				Settings.TellRockstarUsersToDisableAutoUpdateIfNeeded();
			}
		}



		/// <summary>
		/// Enum for AuthWays
		/// </summary>
		public enum AuthWays
		{
			MTL,
			LegacyAuth
		}

		public static AuthWays AuthWay
		{
			get
			{
				if (Settings.EnableLegacyAuth)
				{
					return AuthWays.LegacyAuth;
				}
				else
				{
					return AuthWays.MTL;
				}
			}
			set
			{
				if (value == AuthWays.LegacyAuth)
				{
					Settings.EnableLegacyAuth = true;
				}
				else
				{
					Settings.EnableLegacyAuth = false;
				}
			}
		}


		#endregion

		#region Properties for often used Stuff

		/// <summary>
		/// Using this to keep track if we have shown the user one detected Upgrade Message per P127 Launch
		/// </summary>
		public static bool ThrewUpdateDetectedMessageAlready = false;


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
		public static string DowngradeFilePath
		{
			get
			{
				if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch)
				{
					if (Settings.Retailer == Settings.Retailers.Steam)
					{
						if (Settings.SocialClubLaunchGameVersion == "124")
						{
							return DowngradeAlternativeFilePathSteam124;
						}
						else
						{
							return DowngradeAlternativeFilePathSteam127;
						}
					}
					else if (Settings.Retailer == Settings.Retailers.Rockstar)
					{
						if (Settings.SocialClubLaunchGameVersion == "124")
						{
							return DowngradeAlternativeFilePathRockstar124;
						}
						else
						{
							return DowngradeAlternativeFilePathRockstar127;
						}
					}
					else
					{
						return DowngradeEmuFilePath;
					}
				}
				else
				{
					return DowngradeEmuFilePath;
				}
			}
		}

		/// <summary>
		/// Property of often used variable. (DowngradeEmuFilePath)
		/// </summary>
		public static string DowngradeEmuFilePath { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles\"; } }

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
		/// Property of often used variable. (EmuCfgPath)
		/// </summary>
		public static string EmuCfgPath { get { return Settings.GTAVInstallationPath.TrimEnd('\\') + @"\scemu.cfg"; } }

		#endregion


		public static void AuthClick(bool StartGameImmediatelyAfter = false)
		{
			if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch)
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "You do not need to auth,\nbased on your current settings.\n\nDo you still want to auth?");
				yesno.ShowDialog();
				if (yesno.DialogResult == false)
				{
					return;
				}
			}


			Auth.ROSIntegration.AuthErrorMessageThrownAlready = false;

			if (!MySettings.Settings.EnableLegacyAuth)
			{
				if (LauncherLogic.AuthState == LauncherLogic.AuthStates.NotAuth)
				{
					Auth.ROSIntegration.MTLAuth(StartGameImmediatelyAfter);
				}
				else
				{
					new Popup(Popup.PopupWindowTypes.PopupOk, "You are already authenticated.").ShowDialog();
				}
			}
			else
			{
				if (Globals.PageState != Globals.PageStates.Auth)
				{
					if (LauncherLogic.AuthState == LauncherLogic.AuthStates.NotAuth)
					{
						Globals.LaunchAfterAuth = StartGameImmediatelyAfter;
						Globals.PageState = Globals.PageStates.Auth;
					}
					else
					{
						new Popup(Popup.PopupWindowTypes.PopupOk, "You are already authenticated.").ShowDialog();
					}
				}
				else
				{
					Globals.PageState = Globals.PageStates.GTA;
				}
			}

		}






		#region Upgrade / Downgrade / Repair / Launch

		public static bool IgnoreNewFilesWhileUpgradeDowngradeLogic = false;

		/// <summary>
		/// Method for Upgrading the Game back to latest Version
		/// </summary>
		public static void Upgrade(bool IgnoreNewFiles = false)
		{
			HelperClasses.ProcessHandler.KillRockstarProcesses();

			if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
			{
				new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOkError, "Error:\nCant do that because of because of missing Components").ShowDialog();
				return;
			}

			IgnoreNewFilesWhileUpgradeDowngradeLogic = IgnoreNewFiles;

			// Cancel any stuff when we have no files in upgrade files...simple right?
			if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(UpgradeFilePath).Length >= 2 && HelperClasses.BuildVersionTable.IsUpgradedGTA(UpgradeFilePath)))
			{
				// NO FILES TO UPGRADE
				new Popup(Popup.PopupWindowTypes.PopupOk, "Found no Files to Upgrade with. I suggest verifying Files through steam\nor clicking \"Use Backup Files\" in Settings.\nWill abort Upgrade.").ShowDialog();
				return;
			}

			HelperClasses.ProcessHandler.KillRockstarProcesses();

			if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(DowngradeFilePath).Length >= 2 && HelperClasses.BuildVersionTable.IsDowngradedGTA(DowngradeFilePath)))
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "Found no DowngradeFiles. Please make sure the required components are installed.").ShowDialog();
				return;
			}


			PopupProgress tmp = new PopupProgress(PopupProgress.ProgressTypes.Upgrade, "");
			tmp.ShowDialog();
			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing an Upgrade", tmp.RtrnMyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			if (InstallationState != InstallationStates.Upgraded)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "We just did an Upgrade but the detected InstallationState is not Upgraded.\nI suggest reading the \"Help\" Part of the Information Page");
			}


			IgnoreNewFilesWhileUpgradeDowngradeLogic = false;

			HelperClasses.Logger.Log("Done Upgrading");
		}


		public static bool IsDowngradedGTA(string MyDowngradePath)
		{
			if (HelperClasses.FileHandling.doesPathExist(MyDowngradePath))
			{
				string GTA5Exe = MyDowngradePath.TrimEnd('\\') + @"\gta5.exe";
				string Updaterpf = MyDowngradePath.TrimEnd('\\') + @"\update\update.rpf";
				string launcher1 = MyDowngradePath.TrimEnd('\\') + @"\playgtav.exe";
				string launcher2 = MyDowngradePath.TrimEnd('\\') + @"\gtastub.exe";
				if (HelperClasses.BuildVersionTable.GetGameVersionOfBuild(HelperClasses.FileHandling.GetVersionFromFile(GTA5Exe)) < new Version(1, 30))
				{
					if (HelperClasses.FileHandling.GetSizeOfFile(Updaterpf) > 1000)
					{
						if ((HelperClasses.FileHandling.GetSizeOfFile(launcher1) > 50) ||
							(HelperClasses.FileHandling.GetSizeOfFile(launcher2) > 50))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Method for Downgrading
		/// </summary>
		public static void Downgrade(bool IgnoreNewFiles = false)
		{
			HelperClasses.ProcessHandler.KillRockstarProcesses();

			IgnoreNewFilesWhileUpgradeDowngradeLogic = IgnoreNewFiles;

			if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
			{
				new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOk, "Cant do that because of because of missing Components").ShowDialog();
				return;
			}

			if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(DowngradeFilePath).Length >= 2 && HelperClasses.BuildVersionTable.IsDowngradedGTA(DowngradeFilePath)))
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "Found no DowngradeFiles. Please make sure the required components are installed.").ShowDialog();
				return;
			}

			PopupProgress tmp = new PopupProgress(PopupProgress.ProgressTypes.Downgrade, "");
			tmp.ShowDialog();

			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing a Downgrade", tmp.RtrnMyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			if (InstallationState != InstallationStates.Downgraded)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "We just did an Downgraded but the detected InstallationState is not Downgraded.\nI suggest reading the \"Help\" Part of the Information Page");
			}

			IgnoreNewFilesWhileUpgradeDowngradeLogic = false;

			HelperClasses.Logger.Log("Done Downgrading");
		}


		/// <summary>
		/// Method for "Repairing" our setup
		/// </summary>
		public static void Repair(bool quickRepair = false)
		{

			// Saving all the File Operations I want to do, executing this at the end of this Method
			List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

			HelperClasses.Logger.Log("Initiating Repair. Lets do an Upgrade first.", 0);
			LauncherLogic.Upgrade();
			HelperClasses.Logger.Log("Initiating Repair. Done with Upgrade.", 0);
			HelperClasses.Logger.Log("GTAV Installation Path: " + GTAVFilePath, 1);
			HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
			HelperClasses.Logger.Log("ZIP File Location: " + LauncherLogic.ZIPFilePath, 1);
			HelperClasses.Logger.Log("DowngradeFilePath: " + DowngradeFilePath, 1);
			HelperClasses.Logger.Log("UpgradeFilePath: " + UpgradeFilePath, 1);

			HelperClasses.ProcessHandler.KillRockstarProcesses();

			if (quickRepair)
			{
				HelperClasses.Logger.Log("RepairMode quick.", 1);
			}
			else
			{
				HelperClasses.Logger.Log("RepairMode deep.", 1);
				HelperClasses.Logger.Log("Deleting every File we ever placed inside GTA", 1);
				foreach (string tmp in Settings.AllFilesEverPlacedInsideGTA)
				{
					MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, GTAVFilePath.TrimEnd('\\') + @"\" + tmp, "", "Deleting '" + (GTAVFilePath.TrimEnd('\\') + @"\" + tmp) + "' from the GTA_INSTALLATION_PATH", 2));

				}
			}



			string[] FilesInUpgradeFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(UpgradeFilePath);
			HelperClasses.Logger.Log("Found " + FilesInUpgradeFiles.Length.ToString() + " Files in Upgrade Folder. Will try to delete them", 1);
			foreach (string myFileName in FilesInUpgradeFiles)
			{
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, myFileName, "", "Deleting '" + (myFileName) + "' from the $UpgradeFolder", 2));
			}

			string[] FilesInUpgradeBackupFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(UpgradeFilePathBackup);
			HelperClasses.Logger.Log("Found " + FilesInUpgradeBackupFiles.Length.ToString() + " Files in Upgrade BACKUP Folder. Will try to delete them", 1);
			foreach (string myFileName in FilesInUpgradeBackupFiles)
			{
				MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, myFileName, "", "Deleting '" + (myFileName) + "' from the $UpgradeBackupFolder", 2));
			}

			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing a Repair", MyFileOperations).ShowDialog();

			// We dont need to mess with social club versions since the launch process doesnt depend on it

			HelperClasses.Logger.Log("Repair is done. Files in Upgrade Folder deleted.");
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

				// Checking if we can Upgrade Social Club before launchin Upgraded
				LaunchAlternative.SocialClubUpgrade();

				// If Steam
				if (Settings.Retailer == Settings.Retailers.Steam)
				{
					HelperClasses.Logger.Log("Trying to start Game normally through Steam.", 1);
					// Launch through steam
					HelperClasses.ProcessHandler.StartProcess(Globals.SteamInstallPath.TrimEnd('\\') + @"\steam.exe", pCommandLineArguments: "-applaunch 271590 -uilanguage " + Settings.ToMyLanguageString(Settings.LanguageSelected).ToLower());
				}
				// If Epic Games
				else if (Settings.Retailer == Settings.Retailers.Epic)
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
				if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
				{
					new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOk, "Cant do that because of because of missing Components").ShowDialog();
					return;
				}

				HelperClasses.Logger.Log("Installation State Downgraded Detected.", 1);

				if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.DragonEmu)
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

						AuthClick(true);
						return;
					}

					// Generates Token needed to Launch Downgraded GTAV

					if (!AuthStateOverWrite)
					{
						HelperClasses.Logger.Log("Letting Dragon work his magic");
						await ROSCommunicationBackend.GenLaunchToken();
					}

					HelperClasses.FileHandling.deleteFile(EmuCfgPath);
					if (UseEmuConfigFile)
					{
						string[] LaunchOptions = new string[4];
						LaunchOptions[0] = "PreOrderBonus: \"" + Settings.EnablePreOrderBonus.ToString() + "\"";
						LaunchOptions[1] = "InGameName: \"" + Settings.InGameName + "\"";
						LaunchOptions[2] = "SavePath: \"" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4" + "\"";
						LaunchOptions[3] = "EnableScripthookOnDowngraded: \"" + Settings.EnableScripthookOnDowngraded.ToString() + "\"";
						
	  
						LaunchOptions[3] = "WindowTitleTomfoolery: \"" + Overlay.GTAOverlay.targetWindowBorderless + "\"";
						HelperClasses.FileHandling.WriteStringToFileOverwrite(EmuCfgPath, LaunchOptions);
					}

					// If Steam
					if (Settings.Retailer == Settings.Retailers.Steam && !Settings.EnableDontLaunchThroughSteam)
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
				HelperClasses.Logger.Log("    Size of GTA5.exe in Downgrade Files Folder: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				HelperClasses.Logger.Log("    Size of update.rpf in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				HelperClasses.Logger.Log("    Size of update.rpf in Downgrade Files Folder: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf"));

				new Popup(Popup.PopupWindowTypes.PopupOkError, "Installation State is broken for some reason. Try to repair.");
				return;
			}




			HelperClasses.Logger.Log("Game should be launched");

			PostLaunchEvents();
		}


		#endregion

		#region Backup (UpgradeFiles) stuff

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

			if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(UpgradeFilePath).Length >= 2 && HelperClasses.BuildVersionTable.IsUpgradedGTA(UpgradeFilePath)))
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "No Upgrade Files available to back up.").ShowDialog();
				return;
			}
			else
			{
				long combinedSize = 0;
				foreach (string myFile in FileHandling.GetFilesFromFolderAndSubFolder(NewPath))
				{
					combinedSize += FileHandling.GetSizeOfFile(myFile);
					if (combinedSize > 5000)
					{
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Backup Files already exist.\nOverwrite existing Backup Files?");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
						{
							break;
						}
						else
						{
							return;
						}
					}
				}

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

			if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(UpgradeFilePathBackup).Length >= 2 && HelperClasses.BuildVersionTable.IsUpgradedGTA(UpgradeFilePathBackup)))
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "No Backup Files available.").ShowDialog();
				return;
			}
			else
			{
				InstallationStates OldInstallationState = InstallationState;

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

				if (OldInstallationState == InstallationStates.Upgraded)
				{
					Upgrade(true);
				}
				else
				{
					Downgrade(true);
				}
			}

		}

		#endregion

		#region zipstuff

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

			ComponentManager.ZIPVersionSwitcheroo();

			new Popup(Popup.PopupWindowTypes.PopupOk, "ZIP File imported (Version: '" + Globals.ZipVersion + "')").ShowDialog();
		}

		#endregion

		#region GTAV Path Magic and IsCorrect

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


		#endregion



		#region PostLaunch, PostLaunchHelpers

		/// <summary>
		/// Method which gets called after Starting GTAV
		/// </summary>
		public async static void PostLaunchEvents()
		{
			HelperClasses.Logger.Log("Post Launch Events started");
			await Task.Delay(2500);
			HelperClasses.Logger.Log("Waited a good bit");

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
				HelperClasses.Logger.Log("Trying to Set GTAV Process Priority to High");
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



		#endregion


	} // End of Class
} // End of NameSpace

