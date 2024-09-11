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
            NonRunning,
            Stuck
        }

        /// <summary>
        /// If we have thrown the userpopup due to polling gamestate already
        /// </summary>
        public static bool StuckGTAErrorThrownAlready = false;

        /// <summary>
        /// If we are currently in FileOperationWrapperLoop
        /// </summary>
        public static bool InFileOperationWrapperLoop = false;
        
        /// <summary>
        /// Property of our GameState. Gets polled every 2.5 seconds
        /// </summary>
        public static GameStates GameState
        {
            get
            {
                // Check if GTA V is running
                Process[] Procs = HelperClasses.ProcessHandler.GetProcesses("gta5.exe");
                if (Procs.Length > 0)
                {
                    Process Proc = Procs.FirstOrDefault();

                    if (String.IsNullOrEmpty(Proc.GetMainModuleFileName()))
                    {
                        return GameStates.Stuck;
                    }
                    else
                    {
                        return GameStates.Running;
                    }
                }
                else
                {
                    return GameStates.NonRunning;
                }
            }
        }

        private static int GameStateStuckCounter = 0;
        private static GameStates LastGameState = GameStates.NonRunning;

        public static GameStates PollGameState()
        {
            GameStates currGameState = GameState;

            if (currGameState == GameStates.Running)
            {
                GameStateStuckCounter = 0;

                WindowChangeHander.WindowChangeEvent(WindowChangeListener.GetActiveWindowTitle());

                if (LastGameState == GameStates.NonRunning)
                {
                    GTAStarted();
                }
            }
            else if (currGameState == GameStates.NonRunning)
            {
                GameStateStuckCounter = 0;

                if (LastGameState != GameStates.NonRunning)
                {
                    GTAClosed();
                }
            }
            else
            {
                GameStateStuckCounter += 1;
                if (GameStateStuckCounter > 5)
                {
                    GameStateStuckCounter = 0;
                    HandleStuckGTA();
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

            GetGTACommandLineArgs();

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
                //Popup yn = PopupWrapper.PopupYesNo("Upgrade SocialClub after game exited.");
                //yn.ShowDialog();
                //if (yn.DialogResult == true)
                //{
                    LaunchAlternative.SocialClubUpgrade(2000);
                //}
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

        public static bool RockstarFuckedUsErrorThrownAlready = false;

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

                long SizeOfDowngradeBase124GTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeBase124FilePath.TrimEnd('\\') + @"\GTA5.exe");
                long SizeOfDowngradeBase124Update = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeBase124FilePath.TrimEnd('\\') + @"\update\update.rpf");
                long SizeOfDowngradeBase124PlayGTAV = HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeBase124FilePath.TrimEnd('\\') + @"\playgtav.exe");

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
                    if (SizeOfGTAV == SizeOfDowngradeBase124GTAV && SizeOfUpdate == SizeOfDowngradeBase124Update && SizeOfPlayGTAV == SizeOfDowngradeBase124PlayGTAV)
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
                        Settings.EnableAlternativeLaunch = false;
                        return;
                        // dont allow changing to social club
                        //PopupWrapper.PopupOk("LaunchWay did not change.").ShowDialog();


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
                    //PopupWrapper.PopupOk("LaunchWay was not changed.").ShowDialog();
                }

                MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Auth);
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
                return AuthWays.MTL;
                // Old
                /*
                if (Settings.EnableLegacyAuth)
                {
                    return AuthWays.LegacyAuth;
                }
                else
                {
                    return AuthWays.MTL;
                }
                */
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
                Settings.TellRockstarUsersToDisableAutoUpdateIfNeeded();
            }
        }


        #endregion

        #region Properties for often used Stuff

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
                    if (Settings.DragonEmuGameVersion == "127")
                    {
                        return DowngradeEmuFilePath;
                    }
                    else
                    {
                        return DowngradeBase124FilePath;
                    }
                }
            }
        }







        /// <summary>
        /// Property of often used variable. (DowngradeEmuFilePath)
        /// </summary>
        public static string DowngradeEmuFilePath { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles\"; } }

        /// <summary>
        /// Property of often used variable. (DowngradeBase124)
        /// </summary>
        public static string DowngradeBase124FilePath { get { return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles124\"; } }

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
        /// Property of often used variable. (DebloatVPath)
        /// </summary>
        public static string DebloatVPath { get { return LauncherLogic.GTAVFilePath + @"update\x64\dlcpacks"; } }


        /// <summary>
        /// Property of often used variable. (EmuCfgPath)
        /// </summary>
        public static string EmuCfgPath { get { return Settings.GTAVInstallationPath.TrimEnd('\\') + @"\scemu.cfg"; } }

        #endregion


        public static void AuthClick(bool StartGameImmediatelyAfter = false)
        {
            if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch)
            {
                if (PopupWrapper.PopupYesNo("You do not need to auth,\nbased on your current settings.\n\nDo you still want to auth?") == false)
                {
                    return;
                }
            }



            if (!MySettings.Settings.EnableLegacyAuth)
            {
                if (LauncherLogic.AuthState == LauncherLogic.AuthStates.NotAuth)
                {
                    Auth.ROSIntegration.MTLAuth(StartGameImmediatelyAfter);
                }
                else
                {
                    PopupWrapper.PopupOk("You are already authenticated.");
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
                        PopupWrapper.PopupOk("You are already authenticated.");
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
        public static void Upgrade(bool IgnoreNewFiles = false, bool IgnoreUninstalledComponenets = false)
        {
            if (!LauncherLogic.IsGTAVInstallationPathCorrect() && !LauncherLogic.GTAVInstallationIncorrectMessageThrownAlready)
            {
                HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");

                if (PopupWrapper.PopupYesNo("Error:\nGTA V Installation Path is not a valid Path.\nDo you want to force this Upgrade?") == true)
                {
                    HelperClasses.Logger.Log("User wants to force this Upgrade. Will not throw the WrongGTAVPathError again on this P127 instance.");
                    LauncherLogic.GTAVInstallationIncorrectMessageThrownAlready = true;
                }
                else
                {
                    HelperClasses.Logger.Log("User does not want to force this Upgrade. Will abondon.");
                    return;
                }
            }

            // DebloatV check
            if (HelperClasses.FileHandling.GetSubFolders(LauncherLogic.DebloatVPath).Length < 12)
            {
                HelperClasses.Logger.Log("DebloatV check inside Upgrade hit. Throwing User popup and canceling update.", 1);
                PopupWrapper.PopupError("Your GTA Folder is missing some critical files.\nYou cannot play updated GTA.\n\nYou probably ran DebloatV at some point.\n\nRepair your game via Steam / Rockstar / Epic.");
                return;
            }

            if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
            {
                HelperClasses.Logger.Log("Gamestate looks OK (Downgraded). Will Proceed to try to Upgrade.", 1);
            }
            else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
            {
                HelperClasses.Logger.Log("This program THINKS you are already Upgraded. Update procedure will be called anyways since it shouldnt break things.", 1);
            }
            else
            {
                HelperClasses.Logger.Log("Installation State Broken.", 1);
                PopupWrapper.PopupError("Installation State is broken. I suggest trying to repair.\nWill try to Upgrade anyways.");
            }

            IgnoreNewFilesWhileUpgradeDowngradeLogic = IgnoreNewFiles;

            if (!IgnoreUninstalledComponenets)
            {
                if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
                {
                    PopupWrapper.PopupError("Error:\nCant do that because of because of missing Components");
                    return;
                }


                if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(DowngradeFilePath).Length >= 2 && HelperClasses.BuildVersionTable.IsDowngradedGTA(DowngradeFilePath)))
                {
                    PopupWrapper.PopupError("Found no DowngradeFiles. Please make sure the required components are installed.");
                    return;
                }
            }


            // Cancel any stuff when we have no files in upgrade files...simple right?
            if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(UpgradeFilePath).Length >= 2 && HelperClasses.BuildVersionTable.IsUpgradedGTA(UpgradeFilePath)))
            {
                // NO FILES TO UPGRADE
                PopupWrapper.PopupOk("Found no Files to Upgrade with. I suggest verifying Files through steam\nor clicking \"Use Backup Files\" in Settings.\nWill abort Upgrade.");
                return;
            }

            List<HelperClasses.MyFileOperation> tmpRtrnMyFileOperations = PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.Upgrade, "");
            // Actually executing the File Operations
            PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing an Upgrade", tmpRtrnMyFileOperations);

            // We dont need to mess with social club versions since the launch process doesnt depend on it

            if (InstallationState != InstallationStates.Upgraded)
            {
                PopupWrapper.PopupOk("We just did an Upgrade but the detected InstallationState is not Upgraded.\nI suggest reading the \"Help\" Part of the Information Page");
            }


            IgnoreNewFilesWhileUpgradeDowngradeLogic = false;

            HelperClasses.Logger.Log("Done Upgrading");

            HandleUnsureInstallationState();
            HandleRockstarFuckingUs();
        }

        public static bool IsUpgradedGTA(string MyUpgradePath)
        {
            if (HelperClasses.FileHandling.doesPathExist(MyUpgradePath))
            {
                string GTA5Exe = MyUpgradePath.TrimEnd('\\') + @"\gta5.exe";
                string Updaterpf = MyUpgradePath.TrimEnd('\\') + @"\update\update.rpf";
                string launcher1 = MyUpgradePath.TrimEnd('\\') + @"\playgtav.exe";
                string launcher2 = MyUpgradePath.TrimEnd('\\') + @"\gtavlauncher.exe";
                if (HelperClasses.BuildVersionTable.GetGameVersionOfBuild(HelperClasses.FileHandling.GetVersionFromFile(GTA5Exe)) > new Version(1, 30))
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


        public static bool IsDowngradedGTA(string MyDowngradePath)
        {
            if (HelperClasses.FileHandling.doesPathExist(MyDowngradePath))
            {
                string GTA5Exe = MyDowngradePath.TrimEnd('\\') + @"\gta5.exe";
                string Updaterpf = MyDowngradePath.TrimEnd('\\') + @"\update\update.rpf";
                string launcher1 = MyDowngradePath.TrimEnd('\\') + @"\playgtav.exe";
                string launcher2 = MyDowngradePath.TrimEnd('\\') + @"\gtavlauncher.exe";
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
            if (!LauncherLogic.IsGTAVInstallationPathCorrect() && !LauncherLogic.GTAVInstallationIncorrectMessageThrownAlready)
            {
                HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");

                if (PopupWrapper.PopupYesNo("Error:\nGTA V Installation Path is not a valid Path.\nDo you want to force this Downgrade?") == true)
                {
                    HelperClasses.Logger.Log("User wants to force this Downgrade. Will not throw the WrongGTAVPathError again on this P127 instance.");
                    LauncherLogic.GTAVInstallationIncorrectMessageThrownAlready = true;
                }
                else
                {
                    HelperClasses.Logger.Log("User does not want to force this Downgrade. Will abondon.");
                    return;
                }
            }

            if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
            {
                HelperClasses.Logger.Log("Gamestate looks OK (Upgraded). Will Proceed to try to Downgrade.", 1);
            }
            else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
            {
                HelperClasses.Logger.Log("This program THINKS you are already Downgraded. Downgrade procedure will be called anyways since it shouldnt break things.", 1);
            }
            else
            {
                HelperClasses.Logger.Log("Installation State Broken.", 1);
                PopupWrapper.PopupError("Installation State is broken. I suggest trying to repair.\nWill try to Downgrade anyways.");
            }

            IgnoreNewFilesWhileUpgradeDowngradeLogic = IgnoreNewFiles;

            if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
            {
                PopupWrapper.PopupOk("Cant do that because of because of missing Components");
                return;
            }

            if (!(HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(DowngradeFilePath).Length >= 2 && HelperClasses.BuildVersionTable.IsDowngradedGTA(DowngradeFilePath)))
            {
                PopupWrapper.PopupOk("Found no DowngradeFiles. Please make sure the required components are installed.");
                return;
            }

            List<HelperClasses.MyFileOperation> tmpRtrnMyFileOperations = PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.Downgrade, "");

            // Actually executing the File Operations
            PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing a Downgrade", tmpRtrnMyFileOperations);

            // We dont need to mess with social club versions since the launch process doesnt depend on it

            if (InstallationState != InstallationStates.Downgraded)
            {
                PopupWrapper.PopupOk("We just did an Downgraded but the detected InstallationState is not Downgraded.\nI suggest reading the \"Help\" Part of the Information Page");
            }

            IgnoreNewFilesWhileUpgradeDowngradeLogic = false;

            HelperClasses.Logger.Log("Done Downgrading");

            HandleUnsureInstallationState();
            HandleRockstarFuckingUs();
        }


        /// <summary>
        /// Method for "Repairing" our setup
        /// </summary>
        public static void Repair(bool quickRepair = false, bool skipUpdate = false)
        {
            // Saving all the File Operations I want to do, executing this at the end of this Method
            List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();

            if (skipUpdate)
            {
                HelperClasses.Logger.Log("Initiating Repair. Lets do an Upgrade first.", 0);
            }
            else
            {
                HelperClasses.Logger.Log("Initiating Repair. Lets do an Upgrade first.", 0);
                LauncherLogic.Upgrade();
                HelperClasses.Logger.Log("Initiating Repair. Done with Upgrade.", 0);
            }

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
            PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Performing a Repair", MyFileOperations);

            // We dont need to mess with social club versions since the launch process doesnt depend on it

            HelperClasses.Logger.Log("Repair is done. Files in Upgrade Folder deleted.");
        }

        public static string GetFullCommandLineArgsForStarting()
        {
            bool viaSteam = (Settings.Retailer == Settings.Retailers.Steam && !Settings.EnableDontLaunchThroughSteam && LaunchWay == LaunchWays.DragonEmu);

            string tmp = "";


            if (viaSteam)
            {
                tmp += @"/c cd /d " + "\"" + Globals.SteamInstallPath.TrimEnd('\\') + @"\" + "\"" + @" && ";
            }
            else
            {
                tmp += @"/c cd /d " + "\"" + LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\" + "\"" + @" && ";
            }


            if (Settings.EnableOverWriteGTACommandLineArgs && InstallationState == InstallationStates.Downgraded)
            {
                tmp += Settings.OverWriteGTACommandLineArgs;
            }
            else
            {
                tmp += GetStartCommandLineArgs();
            }

            // thanks for nothing windows
            // the && exit part is not needed, cmd window closes anyways
            // and with it, command line args have a trailing spacebar. Yes. Really.
            //tmp += " && exit";

            if (viaSteam)
            {
                tmp = tmp.Replace("gta_p127.exe", "steam.exe -applaunch 271590");
            }
            else
            {
                if (InstallationState == InstallationStates.Downgraded)
                {
                    if (LaunchWay == LaunchWays.DragonEmu)
                    {
                        tmp = tmp.Replace("gta_p127.exe", "playgtav.exe");
                    }
                    
                    else if (LaunchWay == LaunchWays.SocialClubLaunch)
                    {
                        if (Settings.Retailer == Settings.Retailers.Steam)
                        {
                            tmp = tmp.Replace("gta_p127.exe", "playgtav.exe -scOfflineOnly");
                        }
                        else if (Settings.Retailer == Settings.Retailers.Rockstar)
                        {
                            tmp = tmp.Replace("gta_p127.exe", "gtavlauncher.exe -scOfflineOnly");
                        }
                    }
                }
                else
                {
                    // So this is called when upgraded
                    // means upgraded rockstar
                    tmp = tmp.Replace("gta_p127.exe", "gtavlauncher.exe");
                }
            }

            HelperClasses.Logger.Log("Command Args the alg came up with: #" + tmp + "#, launching Game");

            return tmp;
        }

        public static string GetStartCommandLineArgs()
        {
            string rtrn = "start ";

            if (Settings.EnableCoreFix)
            {
                int AmountOfCores = Environment.ProcessorCount;
                if (AmountOfCores < 4)
                {
                    AmountOfCores = 4;
                }
                else if (AmountOfCores > 23)
                {
                    AmountOfCores = 23;
                }
                UInt64 Possibilities = (UInt64)Math.Pow(2, AmountOfCores);
                string MyHex = (Possibilities - 1).ToString("X");

                rtrn += "/affinity " + MyHex + " ";
            }

            rtrn += "gta_p127.exe -uilanguage " + Settings.ToMyLanguageString(Settings.LanguageSelected).ToLower();

            if (Settings.EnableStutterFix && LaunchWay == LaunchWays.DragonEmu && Settings.DragonEmuGameVersion=="127" && InstallationState == InstallationStates.Downgraded)
            {
                rtrn += " -StutterFix";
            }

            return rtrn;
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
                    LaunchAlternative.SocialClubUpgrade(0);

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
                        // Launch through Non Retail
                        HelperClasses.ProcessHandler.StartDowngradedGame();
                    }
                }
                else if (LauncherLogic.InstallationState == InstallationStates.Downgraded)
                {
                    if (!ComponentManager.CheckIfRequiredComponentsAreInstalled(true))
                    {
                        PopupWrapper.PopupOk("Cant do that because of because of missing Components");
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
                            try
                            {
                                await ROSCommunicationBackend.GenLaunchToken();
                            }
                            catch (Exception ex)
                            {
                                HelperClasses.Logger.Log("Unable to connect to the server. Probably best to restart P127.");
                                PopupWrapper.PopupError("Unable to connect to the server. Probably best to restart P127.");
                                return;
                            }
                        }

                        HelperClasses.FileHandling.deleteFile(EmuCfgPath);
                        if (UseEmuConfigFile)
                        {
                            string[] LaunchOptions = new string[5];
                            LaunchOptions[0] = "PreOrderBonus: \"" + Settings.EnablePreOrderBonus.ToString() + "\"";
                            LaunchOptions[1] = "InGameName: \"" + Settings.InGameName + "\"";
                            LaunchOptions[2] = "SavePath: \"" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4" + "\"";
                            LaunchOptions[3] = "WindowTitleTomfoolery: \"" + Overlay.GTAOverlay.targetWindowBorderless + "\"";
                            LaunchOptions[4] = "EnableScripthookOnDowngraded: \"" + Settings.EnableScripthookOnDowngraded.ToString() + "\"";

                            HelperClasses.FileHandling.WriteStringToFileOverwrite(EmuCfgPath, LaunchOptions);
                        }

                        if (Settings.Retailer == Settings.Retailers.Steam && !Settings.EnableDontLaunchThroughSteam && LaunchWay == LaunchWays.DragonEmu)
                        {
                            var steamprocs = Process.GetProcessesByName("steam");
                            if (steamprocs.Length > 0)
                            {
                                var steamproc = steamprocs[0];
                                Int64 coreaffinity = steamproc.ProcessorAffinity.ToInt64();
                                int corecount = 0;
                                for (int i = 0; i < 64; i++)
                                {
                                    corecount += (coreaffinity & ((Int64)1 << i)) != 0 ? 1 : 0;
                                }
                                HelperClasses.Logger.Log("Current core affinity for steam is " + coreaffinity.ToString("X") + " (" + corecount + " cores)");
                                if (corecount > 16)
                                {
                                    HelperClasses.Logger.Log("Settings steam's core affinity to FFFF (16 cores)");
                                    Int64 NewAffinity = 0xFFFF;
                                    steamproc.ProcessorAffinity = (IntPtr)NewAffinity;
                                }
                            }

                        }

                        HelperClasses.ProcessHandler.StartDowngradedGame();

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

                PopupWrapper.PopupError("Installation State is broken for some reason. Try to repair.");
                    return;
                }




                HelperClasses.Logger.Log("Game should be launched");


            PostLaunchEvents();
        }





        public static void HandleUnsureInstallationState()
        {
            if (InstallationState == InstallationStates.Unsure)
            {
                string msg = "Installation State is Unsure.";
                msg += "\n\n";
                msg += "Potential Fixes:\n";
                msg += "- Repairing / Verifying your GTA V Installation via Steam / Rockstar / Epic\n";
                msg += "- Settings -> General P127 Settings -> Repair GTA V Installation\n";
                msg += "- Settings -> General P127 Settings -> Re-Installing all Componenets inside the Componenet Manager\n";
                msg += "- Settings -> General P127 Settings -> Reset Everything\n";
                msg += "- Settings -> General P127 Settings -> Enable \"Slow but stable Method for Upgrading / Downgrading\n";
                msg += "(specifically applies if your GTA is on an external HDD)";

                PopupWrapper.PopupOk(msg);
            }
        }


        public static bool HandleStuckGTA(bool IgnoreAlreadyThrownError = false, string msg = "")
        {
            // if we ignore error and throw anywas
            // or if we havent thrown error yet and we are not in file operation loop
            if ((!StuckGTAErrorThrownAlready && !InFileOperationWrapperLoop) || IgnoreAlreadyThrownError)
            {
                StuckGTAErrorThrownAlready = true;

                Logger.Log("Stuck GTA detected and need to tell user about it");

                if (string.IsNullOrEmpty(msg))
                {
                    msg = "We have a 'stuck' GTA V Process,\nas we have tried to kill it, waited, and its still running.\n\nThe only fix is to FULLY restart your computer.\nIf you manually do it, you have to hold SHIFT while clicking the restart button.\nDo you want P127 to restart your PC for you?";
                }

                Logger.Log("Asking if User wants a restart");
                if (PopupWrapper.PopupYesNo(msg) == true)
                {
                    Logger.Log("User wants a restart");
                    PopupWrapper.PopupOk("Close all Files and Programs that need saving,\nand hit 'ok' to restart your PC.");
                    Logger.Log("Goodnight");
                    Process.Start("shutdown.exe", "/r /f /t 0");
                }
                else
                {
                    Logger.Log("User does NOT want a restart");

                    return false;
                }
            }
            return true;
        }

        public static void HandleRockstarFuckingUs()
        {
            // DETECTING IF ROCKSTAR FUCKED US
            if (InstallationState == InstallationStates.Downgraded)
            {
                if (BuildVersionTable.GetGameVersionOfBuild(Globals.GTABuild) > new Version(1, 30))
                {
                    if (!RockstarFuckedUsErrorThrownAlready)
                    {
                        RockstarFuckedUsErrorThrownAlready = true;
                        bool yesno = PopupWrapper.PopupYesNo("It appears like Rockstar (or Steam and Epic although unlikely) messed up Project 1.27 Files.\nDo you want to correct them?");
                        if (yesno == true)
                        {
                            Upgrade(false, true);

                            if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.DragonEmu || Settings.Retailer == Settings.Retailers.Epic)
                            {
                                if (Settings.DragonEmuGameVersion == "124")
                                {
                                    ComponentManager.Components.Base124.ReInstall();
                                }
                                else
                                {
                                    ComponentManager.Components.Base.ReInstall();
                                }
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
                    }
                }
            }
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
                PopupWrapper.PopupOk("No Upgrade Files available to back up.");
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
                        bool yesno = PopupWrapper.PopupYesNo("Backup Files already exist.\nOverwrite existing Backup Files?");
                        if (yesno == true)
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

                PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Creating Backup", MyFileOperations);

                PopupWrapper.PopupOk("Files are now backed up.");
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
                PopupWrapper.PopupOk("No Backup Files available.");
                return;
            }
            else
            {
                InstallationStates OldInstallationState = InstallationState;
                List<MyFileOperation> MyFileOperations = new List<MyFileOperation>();


                bool yesno = PopupWrapper.PopupYesNo("Do you want to back up your current \"Upgrade-Files\"\nbefore applying the backup and overwriting it?");
                if (yesno == true)
                {
                    bool exitWhileLoop = false;
                    string input = "";

                    while (!exitWhileLoop)
                    {
                        // Asking for Name 
                        input = PopupWrapper.PopupTextbox("Enter the Name of the Backup:", "MyNewBackupName");
                        if (!string.IsNullOrWhiteSpace(input) && input.ToLower() != "cancel")
                        {
                            // Getting the Name chosen
                            string newPath = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"_Backup_" + input;
                            if (HelperClasses.FileHandling.doesPathExist(newPath))
                            {
                                bool yesno2 = PopupWrapper.PopupYesNo("Backup with that name ('" + input + "') already exists.\nDo you want to delete it?");
                                if (yesno2 == true)
                                {
                                    HelperClasses.FileHandling.DeleteFolder(newPath);
                                    MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, newPath, "", "Deleting Path: '" + (newPath) + "'", 2, MyFileOperation.FileOrFolder.Folder));
                                    MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, LauncherLogic.UpgradeFilePath, newPath, "Moving Path: '" + LauncherLogic.UpgradeFilePath + "', to: '" + (newPath) + "'", 2, MyFileOperation.FileOrFolder.Folder));
                                    exitWhileLoop = true;
                                    break;
                                }
                            }
                            else
                            {
                                MyFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, LauncherLogic.UpgradeFilePath, newPath, "Moving Path: '" + LauncherLogic.UpgradeFilePath + "', to: '" + (newPath) + "'", 2, MyFileOperation.FileOrFolder.Folder));
                                exitWhileLoop = true;
                                break;
                            }
                        }
                        else
                        {
                            exitWhileLoop = true;
                            break;
                        }
                    }
                }


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

                PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Applying Backup", MyFileOperations);

                PopupWrapper.PopupOk("Using backup files now.");

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
                            path = path.TrimStart('"').TrimEnd('"');
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
            try
            {
                HelperClasses.Logger.Log("GTAV Path Magic by Rockstar", 2);
                RegistryKey myRK2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{5EFC6C07-6B87-43FC-9524-F9E967241741}");
                return HelperClasses.RegeditHandler.GetValue(myRK2, "InstallLocation");
            }
            catch
            {
                return "";
            }
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
                Regex MyRegex = new Regex("\"path\"\"[a-zA-Z]:\\\\");
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



        public static bool GTAVInstallationIncorrectMessageThrownAlready = false;

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
            GetGTACommandLineArgs();

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


        public static void GetGTACommandLineArgs()
        {
            HelperClasses.Logger.Log("Trying to get GTAV Process Commandline");
            try
            {
                Process[] processes = HelperClasses.ProcessHandler.GetProcesses("gta5");
                if (processes.Length > 0)
                {
                    string tmp = processes[0].GetCommandLine();
                    HelperClasses.Logger.Log("Found launched GTAV with Commandline: '" + tmp + "'");
                }
            }
            catch
            {
                HelperClasses.Logger.Log("Failed to get GTA5 Process CommandLine...");
            }
        }


        #endregion




        /// <summary>
        /// Sets the ReturningPlayerBonus Setting based on actual contents of file
        /// </summary>
        public static bool SetReturningPlayerBonusSetting(bool WriteToSettings = true)
        {
            HelperClasses.Logger.Log("Reading pc_settings.bin to determine if setting should be set to true or false");
            try
            {
                var settingsPath = @"\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4\pc_settings.bin";
                var fullSettingsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + settingsPath;
                byte[] settings_bytes = File.ReadAllBytes(fullSettingsPath);
                byte[] bytes_to_remove = { 98, 3, 0, 0, 1, 0, 0, 0 }; // this sequence of bytes enables the returning player content

                // definitely not just copied from method below
                for (int i = 0; i < settings_bytes.Length; i++)
                {
                    if (settings_bytes.Skip(i).Take(bytes_to_remove.Length).SequenceEqual(bytes_to_remove))
                    {
                        if (WriteToSettings)
                        {
                            Settings.EnableReturningPlayer = true;
                        }
                        return true;
                    }
                }
                if (WriteToSettings)
                {
                    Settings.EnableReturningPlayer = false;
                }
                return false;
            }
            catch
            {
                if (WriteToSettings)
                {
                    Settings.EnableReturningPlayer = false;
                }
                return false;
            }
        }


        /// <summary>
        /// Enables or Disables the Returning Player Bonus by writing to the file.
        /// </summary>
        /// <param name="IsEnabled"></param>
        public static void SetReturningPlayerBonus(bool IsEnabled)
        {
            // Main code by Special For and Hoxi, slightly adapted (try/catch), List instead of Array, checking what the file contains

            // If file already does what user wants, dont write
            if (SetReturningPlayerBonusSetting(false) == IsEnabled)
            {
                return;
            }

            var settingsPath = @"\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4\pc_settings.bin";
            var fullSettingsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + settingsPath;
            if (File.Exists(fullSettingsPath))
            {
                if (IsEnabled)
                {

                    FileStream writeStream;
                    try
                    {
                        writeStream = new FileStream(fullSettingsPath, FileMode.Append, FileAccess.Write);
                        BinaryWriter writeBinary = new BinaryWriter(writeStream);
                        writeBinary.Write(0x00000362);
                        writeBinary.Write(0x00000001);
                        writeBinary.Close();
                    }
                    catch (Exception ex)
                    {
                        HelperClasses.Logger.Log("Returning Player Bonus Enabler: Error enabling returning player bonus: " + ex.ToString());
                    }
                    HelperClasses.Logger.Log("Returning Player Bonus Enabler: Returning Player Bonus Enabled");
                }
                else
                {
                    try
                    {
                        byte[] settings_bytes = File.ReadAllBytes(fullSettingsPath);
                        byte[] bytes_to_remove = { 98, 3, 0, 0, 1, 0, 0, 0 }; // this sequence of bytes enables the returning player content


                        // definitely not just copied from https://stackoverflow.com/a/40284498 xdd
                        //byte[] result = new byte[settings_bytes.Length];
                        List<byte> result = new List<byte>();
                        int k = 0;
                        for (int i = 0; i < settings_bytes.Length;)
                        {
                            if (settings_bytes.Skip(i).Take(bytes_to_remove.Length).SequenceEqual(bytes_to_remove))
                            {

                                i += bytes_to_remove.Length;
                            }
                            else
                            {
                                //result[k] = settings_bytes[i];
                                result.Add(settings_bytes[i]);
                                i++;
                                k++;
                            }
                        }
                        File.WriteAllBytes(fullSettingsPath, result.ToArray()); // write new array of bytes to settings file
                        HelperClasses.Logger.Log("Returning Player Bonus Enabler: Returning Player Bonus Disabled");
                    }
                    catch (Exception ex)
                    {
                        HelperClasses.Logger.Log("Returning Player Bonus Enabler: Error enabling returning player bonus: " + ex.ToString());
                    }
                }
            }
            else
            {
                HelperClasses.Logger.Log("Returning Player Bonus Enabler: Settings file not found!");
            }
        }

    } // End of Class
} // End of NameSpace

