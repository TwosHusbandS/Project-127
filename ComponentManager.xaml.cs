﻿using CefSharp.DevTools.IndexedDB;
using GSF.IO;
using GSF.Parsing;
using Project_127.HelperClasses;
using Project_127.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_127
{
    /// <summary>
    /// Interaction logic for ComponentManager.xaml
    /// </summary>
    public partial class ComponentManager : Page
    {
        private static ComponentManager MyDirtyProgramming;

        public enum Components
        {
            Base,
            Base124,
            Base129,
            SCLSteam127,
            SCLSteam124,
            SCLSteam129,
            SCLRockstar127,
            SCLRockstar124,
            SCLRockstar129,
            DowngradedSC,
            AdditionalSaveFiles
        }


        public static Array AllComponents
        {
            get
            {
                return Enum.GetValues(typeof(Components));
            }
        }

        public static List<Components> InstalledComponents
        {
            get
            {
                List<Components> tmp = new List<Components>();
                foreach (Components myComponent in AllComponents)
                {
                    if (myComponent.IsInstalled())
                    {
                        tmp.Add(myComponent);
                    }
                }
                return tmp;
            }
        }


        public static List<Components> RequiredComponentsBasedOnSettings
        {
            get
            {
                List<Components> tmp = new List<Components>();
                tmp.Add(Components.Base);
                if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch && MySettings.Settings.Retailer != MySettings.Settings.Retailers.Epic)
                {
                    if (MySettings.Settings.SocialClubLaunchGameVersion == "124")
                    {
                        if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Steam)
                        {
                            tmp.Add(Components.SCLSteam124);
                        }
                        else if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Rockstar)
                        {
                            tmp.Add(Components.SCLRockstar124);
                        }
                    }
                    else if (MySettings.Settings.SocialClubLaunchGameVersion == "129")
                    {
                        if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Steam)
                        {
                            tmp.Add(Components.SCLSteam129);
                        }
                        else if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Rockstar)
                        {
                            tmp.Add(Components.SCLRockstar129);
                        }
                    }
                    else
                    {
                        if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Steam)
                        {
                            tmp.Add(Components.SCLSteam127);
                        }
                        else if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Rockstar)
                        {
                            tmp.Add(Components.SCLRockstar127);
                        }
                    }
                    tmp.Add(Components.DowngradedSC);
                }
                if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.DragonEmu)
                {
                    if (MySettings.Settings.DragonEmuGameVersion == "124")
                    {
                        tmp.Add(Components.Base124);
                    }
                    else if (MySettings.Settings.DragonEmuGameVersion == "129")
                    {
                        tmp.Add(Components.Base129);
                    }
                }
                return tmp;
            }
        }


        public static bool RecommendUpgradedGTA()
        {
            HelperClasses.Logger.Log("ComponentMngr - Recommending an Upgraded GTA before Uninstall / Updating Componenets.");
            if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
            {
                HelperClasses.Logger.Log("ComponentMngr - GTA is Downgraded. Asking User", 1);

                bool yesno = PopupWrapper.PopupYesNo("We need to Upgrade before doing that.\nAfter that you can Downgrade again.\nDo you want to Upgrade now?");
                if (yesno == true)
                {
                    HelperClasses.Logger.Log("ComponentMngr - User wants Upgrade, lets do it.", 1);
                    LauncherLogic.Upgrade();

                    if (LauncherLogic.IsUpgradedGTA(LauncherLogic.GTAVFilePath))
                    {
                        HelperClasses.Logger.Log("ComponentMngr - Upgrade successfully, returning true.", 1);
                        return true;
                    }
                    else
                    {
                        HelperClasses.Logger.Log("ComponentMngr - Upgrade failed, returning false.", 1);
                        return false;
                    }
                }
                else
                {
                    HelperClasses.Logger.Log("ComponentMngr - User does NOT want to Upgrade, lets return false.", 1);

                    return false;
                }
            }
            else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Unsure)
            {
                HelperClasses.Logger.Log("ComponentMngr - GTA is Unsure. Asking User", 1);

                bool continueanyways = PopupWrapper.PopupYesNo("Current installation state is UNSURE.\nThis can be fixed by going into Settings -> General P127 Settings -> Repair GTA V Installation.\n\nIt is recommend to fix it, before continuing.\n\nContinue anyways?");
                if (continueanyways == true)
                {
                    HelperClasses.Logger.Log("ComponentMngr - User wants to continue anyways.", 1);
                    return true;
                }
                else
                {
                    HelperClasses.Logger.Log("ComponentMngr - User does NOT want to continue anyways.", 1);
                    return false;
                }
            }
            else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
            {
                HelperClasses.Logger.Log("ComponentMngr - Already Upgraded GTA,", 1);
                return true;
            }
            else
            {
                HelperClasses.Logger.Log("ComponentMngr - In else, we should never be here,", 1);
                return true;
            }
        }


        public static bool CheckIfRequiredComponentsAreInstalled(bool AskUser = false)
        {
            bool rtrn = true;
            HelperClasses.Logger.Log("ComponentMngr - Checking if all required Components are installed. AskUser: '" + AskUser + "'");

            for (int i = 0; i <= RequiredComponentsBasedOnSettings.Count - 1; i++)
            {
                Components myComponent = RequiredComponentsBasedOnSettings[i];
                if (!myComponent.IsInstalled())
                {
                    if (AskUser)
                    {
                        HelperClasses.Logger.Log("ComponentMngr - Component: '" + myComponent + "' not installed, but needed, will ask user.", 1);

                        bool yesno = PopupWrapper.PopupYesNo("Component:\n'" + myComponent.GetNiceName() + "'\nmissing but needed.\nDo you want to install it now?");
                        if (yesno == true)
                        {
                            HelperClasses.Logger.Log("ComponentMngr - User wants it, installing. Will log if install failed.", 2);

                            if (!myComponent.Install())
                            {
                                HelperClasses.Logger.Log("ComponentMngr - Install failed, will return false.", 2);

                                rtrn = false;
                            }
                        }
                        else
                        {
                            HelperClasses.Logger.Log("ComponentMngr - User does NOT want it, will return false.", 2);

                            rtrn = false;
                        }
                    }
                    else
                    {
                        HelperClasses.Logger.Log("ComponentMngr - Component: '" + myComponent + "' not installed, but needed, will install. Will log if install failed.", 1);

                        PopupWrapper.PopupOk("Component:\n'" + myComponent.GetNiceName() + "'\nmissing but needed.\nIt will be downloaded and installed now.");
                        if (!myComponent.Install())
                        {
                            HelperClasses.Logger.Log("ComponentMngr - Install failed, will return false.", 2);

                            rtrn = false;
                        }
                    }
                }
                else
                {
                    if (!myComponent.IsOnDisk())
                    {
                        if (AskUser)
                        {
                            HelperClasses.Logger.Log("ComponentMngr - Component: '" + myComponent + "' installed but NOT on disk, but needed, ask user. Will log if install failed.", 1);

                            bool yesno = PopupWrapper.PopupYesNo("Component:\n'" + myComponent.GetNiceName() + "'\nnot found on Disk but needed.\nDo you want to install it?\n(Clicking no might result in Upgrading / Downgrading / Launching being disabled.)");
                            if (yesno == true)
                            {
                                HelperClasses.Logger.Log("ComponentMngr - User wants it, installing. Will log if install failed.", 2);

                                if (!myComponent.ReInstall())
                                {
                                    HelperClasses.Logger.Log("ComponentMngr - Install failed, will return false.", 2);

                                    rtrn = false;
                                }
                            }
                            else
                            {
                                HelperClasses.Logger.Log("ComponentMngr - User does NOT want it, will return false.", 2);

                                rtrn = false;
                            }
                        }
                        else
                        {
                            HelperClasses.Logger.Log("ComponentMngr - Component: '" + myComponent + "' installed but NOT on disk, but needed, will install. Will log if install failed.", 1);

                            PopupWrapper.PopupOk("Component:\n'" + myComponent.GetNiceName() + "'\nmissing but needed.\nIt will be downloaded and installed now.");
                            if (!myComponent.ReInstall())
                            {
                                HelperClasses.Logger.Log("ComponentMngr - Install failed, will return false.", 2);

                                rtrn = false;
                            }
                        }
                    }
                    else
                    {
                        HelperClasses.Logger.Log("ComponentMngr - Component: '" + myComponent + "' installed AND on disk", 1);
                    }
                }
            }
            return rtrn;
        }

        public static Components[] RequireCSLSocialClub = new Components[] { Components.SCLRockstar124, Components.SCLRockstar127, Components.SCLSteam124, Components.SCLSteam127 };

        public ComponentManager()
        {
            InitializeComponent();
            // MyRefresh(false); this is already on load event
            ButtonMouseOverMagic(btn_Refresh);
            MyDirtyProgramming = this;
            SetMode(MySettings.Settings.DMMode);
        }

        public static void SetMode(string Mode)
        {
            if (MyDirtyProgramming != null)
            {
                if (String.IsNullOrEmpty(Mode) || Mode == "default")
                {
                    MyDirtyProgramming.lbl_ComponentManager_Mode.Content = "";
                    MyDirtyProgramming.lbl_ComponentManager_Mode.Visibility = Visibility.Hidden;
                    MyDirtyProgramming.btn_lbl_Mode.Visibility = Visibility.Hidden;
                }
                else
                {
                    MyDirtyProgramming.lbl_ComponentManager_Mode.Content = "Curr DM Mode: " + Mode;
                    MyDirtyProgramming.lbl_ComponentManager_Mode.Visibility = Visibility.Visible;
                    MyDirtyProgramming.btn_lbl_Mode.Visibility = Visibility.Visible;
                }
                MyDirtyProgramming.lbl_ComponentManager_Mode.ToolTip = MyDirtyProgramming.lbl_ComponentManager_Mode.Content;
            }
        }

        public static void MyRefreshStatic()
        {
            if (MyDirtyProgramming != null)
            {
                MyDirtyProgramming.MyRefresh(false, false);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.MyRefresh(false, false);
        }


        private void btn_Install_Click(object sender, RoutedEventArgs e)
        {
            Components MyComponent = GetComponentFromButton((Button)sender);
            if (MyComponent.IsInstalled())
            {
                bool yesno = PopupWrapper.PopupYesNo("Do you want to Re-Install the following Component:\n'" + MyComponent.GetNiceName() + "'");
                if (yesno == true)
                {
                    if (MyComponent == Components.DowngradedSC)
                    {
                        LaunchAlternative.SocialClubReset();
                    }

                    bool tmp = MyComponent.ReInstall();
                    if (tmp)
                    {
                        PopupWrapper.PopupOk("Done Installing:\n'" + MyComponent.GetNiceName() + "'");
                        if (MyComponent == Components.AdditionalSaveFiles)
                        {
                            ThrowShoutout();
                        }
                    }
                    else
                    {
                        PopupWrapper.PopupOk("Install failed. Try again:\n'" + MyComponent.GetNiceName() + "'");
                    }
                }
            }
            else
            {
                bool tmp = MyComponent.Install();
                if (tmp)
                {
                    PopupWrapper.PopupOk("Done Installing:\n'" + MyComponent.GetNiceName() + "'");
                    if (MyComponent == Components.AdditionalSaveFiles)
                    {
                        ThrowShoutout();
                    }
                }
                else
                {
                    PopupWrapper.PopupOk("Install failed. Try again:\n'" + MyComponent.GetNiceName() + "'");
                }
            }
        }

        public static void ZIPVersionSwitcheroo()
        {
            int _ZipVersion = 0;
            string VersionTxt = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\Version.txt";
            if (HelperClasses.FileHandling.doesFileExist(VersionTxt))
            {
                Int32.TryParse(HelperClasses.FileHandling.ReadContentOfFile(VersionTxt), out _ZipVersion);
                if (_ZipVersion > 0)
                {
                    HelperClasses.FileHandling.deleteFile(VersionTxt);
                    Components.Base.ForceSetInstalled(new Version(1, _ZipVersion));
                }
            }

        }

        public static void ThrowShoutout()
        {
            string msg = "";
            msg += "Editors Note: Massive Shoutout to @AntherXx for going through the trouble and manual labor" + "\n";
            msg += "of renaming the SaveFiles and collecting them.Thanks a lot mate." + "\n";
            msg += "" + "\n";
            msg += "This is a compilation of save files to practice each mission as you would in a full run, instead of in a Mission Replay." + "\n";
            msg += "All save files are made immediately following the end of the previous mission, so that you can practice the drive to the mission" + "\n";
            msg += "The mission order is based on the current Classic % route." + "\n";
            msg += "" + "\n";
            msg += "Some save files have a duplicate, for example Lamar Down, where you finish The Wrap Up as Michael at the gas station(save file #1)," + "\n";
            msg += "but switch to Franklin and save warp to start Lamar Down. (save file #2 is made right after the switch to Franklin)" + "\n";
            msg += "" + "\n";
            msg += "Some mission save files are missing, such as the tow truck and garbage truck for Blitz Play. This is simply because there are routes" + "\n";
            msg += "that differ so much from run to run that it would be unfair to decide one particular way to make a save file for it." + "\n";
            msg += "" + "\n";
            msg += "This should NOT be used to start segment runs, there should be save files already within your Project 1.27 launcher that has those," + "\n";
            msg += "if not you can download them from speedrun.com/ gtav" + "\n";
            msg += "" + "\n";
            msg += "Although I tried to make them as accurate to the Classic % strats and mission routing as possible," + "\n";
            msg += "there might be some points where the characters do not have the correct weapons, and I am sorry for that." + "\n";
            msg += "" + "\n";
            msg += "In that case, instead of making a new save file yourself or correcting it yourself, (which you can do as well)," + "\n";
            msg += "I would request that you reach out to me on discord so that I can correct it." + "\n";
            msg += "" + "\n";
            msg += "If there are any other questions/ comments / concerns, please let me know on discord at @AntherXx#5392";

            //PopupWrapper.PopupOk(msg, 16).ShowDialog();
        }

        private void btn_Uninstall_Click(object sender, RoutedEventArgs e)
        {
            Components MyComponent = GetComponentFromButton((Button)sender);

            if (MyComponent == Components.Base)
            {
                HelperClasses.Logger.Log("ComponentMngr - Cant uninstall Base Component");
                PopupWrapper.PopupOk("Cant delete our Base Component:\n'" + Components.Base.GetNiceName() + "'");
            }
            else
            {
                if (RequiredComponentsBasedOnSettings.Contains(MyComponent))
                {
                    if (!ComponentManager.RecommendUpgradedGTA())
                    {
                        HelperClasses.Logger.Log("ComponentMngr - Cant uninstall Component: '" + MyComponent + "' (" + MyComponent.GetInstalledVersion() + "), since its required and user failed the RecommendUpgradedGTA check");
                        PopupWrapper.PopupOk("Cant uninstall this Component, since its required and we are Downgraded.");
                        return;
                    }
                }

                if (MyComponent == Components.DowngradedSC)
                {
                    LaunchAlternative.SocialClubReset();
                }

                MyComponent.Uninstall();
            }
        }

        private void btn_Install_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Components MyComponent = GetComponentFromButton((Button)sender);
            if (MyComponent.IsInstalled())
            {
                MyComponent.Verify();
                PopupWrapper.PopupOk("Done verifying:\n'" + MyComponent.GetNiceName() + "'");
            }
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonMouseOverMagic((Button)sender);
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonMouseOverMagic((Button)sender);
        }

        /// <summary>
        /// Logic behind all MouseOver Stuff. Checkboxes and Refresh Button
        /// </summary>
        /// <param name="myBtn"></param>
        private void ButtonMouseOverMagic(Button myBtn)
        {
            switch (myBtn.Name)
            {
                case "btn_Refresh":
                    if (myBtn.IsMouseOver)
                    {
                        MainWindow.MW.SetControlBackground(btn_Refresh, "Artwork/refresh_mo.png");
                    }
                    else
                    {
                        MainWindow.MW.SetControlBackground(btn_Refresh, "Artwork/refresh.png");
                    }
                    break;
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            MyRefresh(true);
        }

        public static void CheckForUpdates()
        {
            foreach (Components myComponent in InstalledComponents)
            {
                myComponent.UpdateLogic();
            }
        }

        public static void StartupCheck()
        {
            LauncherLogic.HandleUnsureInstallationState();
            LauncherLogic.HandleRockstarFuckingUs();
            CheckForUpdates();
            CheckIfRequiredComponentsAreInstalled(true);
        }


        private void ResetAllStatus()
        {
            btn_lbl_FilesMain_Name.Content = "Required Files (Dragon Emu)";

            List<Button> labels = new List<Button>
            {
                lbl_FilesMain_Status,
                lbl_FilesBase124_Status,
                lbl_FilesBase129_Status,
                lbl_FilesSCLRockstar127_Status,
                lbl_FilesSCLRockstar124_Status,
                lbl_FilesSCLRockstar129_Status,
                lbl_FilesSCLSteam127_Status,
                lbl_FilesSCLSteam124_Status,
                lbl_FilesSCLSteam129_Status,
                lbl_FilesSCLDowngradedSC_Status,
                lbl_FilesAdditionalSF_Status
            };

            for (int i = 0; i <= labels.Count - 1; i++)
            {
                labels[i].Content = "Unknown";
                labels[i].Foreground = MyColors.MyColorWhite;
                labels[i].ToolTip = "Unknown";
            }
        }

        private async void MyRefresh(bool CheckForUpdatesPls = false, bool ReInitDownloadManager = true)
        {
            MainWindow.MW.Dispatcher.Invoke((Action)delegate
            {
                ResetAllStatus();

                Border_Loading.Visibility = Visibility.Visible;
            });

            if (ReInitDownloadManager)
            {
                await Globals.SetUpDownloadManager(false);
            }

            if (CheckForUpdatesPls)
            {
                CheckForUpdates();
            }

            MainWindow.MW.Dispatcher.Invoke((Action)delegate
            {
                Components.Base.UpdateStatus(lbl_FilesMain_Status);
                Components.Base124.UpdateStatus(lbl_FilesBase124_Status);
                Components.Base129.UpdateStatus(lbl_FilesBase129_Status);
                Components.SCLRockstar124.UpdateStatus(lbl_FilesSCLRockstar124_Status);
                Components.SCLRockstar127.UpdateStatus(lbl_FilesSCLRockstar127_Status);
                Components.SCLRockstar129.UpdateStatus(lbl_FilesSCLRockstar129_Status);
                Components.SCLSteam124.UpdateStatus(lbl_FilesSCLSteam124_Status);
                Components.SCLSteam127.UpdateStatus(lbl_FilesSCLSteam127_Status);
                Components.SCLSteam129.UpdateStatus(lbl_FilesSCLSteam129_Status);
                Components.DowngradedSC.UpdateStatus(lbl_FilesSCLDowngradedSC_Status);
                Components.AdditionalSaveFiles.UpdateStatus(lbl_FilesAdditionalSF_Status);

                btn_lbl_FilesMain_Name.ToolTip = Components.Base.GetToolTip();
                lbl_FilesBase124_Name.ToolTip = Components.Base124.GetToolTip();
                lbl_FilesBase129_Name.ToolTip = Components.Base129.GetToolTip();
                lbl_FilesSCLRockstar127_Name.ToolTip = Components.SCLRockstar127.GetToolTip();
                lbl_FilesSCLRockstar124_Name.ToolTip = Components.SCLRockstar124.GetToolTip();
                lbl_FilesSCLRockstar129_Name.ToolTip = Components.SCLRockstar129.GetToolTip();
                lbl_FilesSCLSteam127_Name.ToolTip = Components.SCLSteam127.GetToolTip();
                lbl_FilesSCLSteam124_Name.ToolTip = Components.SCLSteam124.GetToolTip();
                lbl_FilesSCLSteam129_Name.ToolTip = Components.SCLSteam129.GetToolTip();
                lbl_FilesSCLDowngradedSC_Name.ToolTip = Components.DowngradedSC.GetToolTip();
                lbl_FilesAdditionalSF_Name.ToolTip = Components.AdditionalSaveFiles.GetToolTip();



                Version tmp = Components.Base.GetInstalledVersion();
                if (tmp != new Version("0.0.0.0"))
                {
                    btn_lbl_FilesMain_Name.Content = "Required Files (Dragon Emu) (v." + tmp.Minor + ")";
                }
                else
                {
                    btn_lbl_FilesMain_Name.Content = "Required Files (Dragon Emu)";
                }

                Border_Loading.Visibility = Visibility.Hidden;
            });
        }

        private void btn_lbl_Component_Name_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 3)
            {
                Components MyComponent = GetComponentFromButton((Button)sender);
                string MyReturnString = PopupWrapper.PopupTextbox("Enter forced Version for Component:\n'" + MyComponent + "'.\nClick cancel,\nif you dont know what youre doing.", MyComponent.GetInstalledVersion().ToString());
                Version tmpV = new Version("0.0.0.0");
                try
                {
                    tmpV = new Version(MyReturnString);
                }
                catch { }
                if (tmpV != new Version("0.0.0.0"))
                {
                    MyComponent.ForceSetInstalled(tmpV);
                }
            }
        }

        private Components GetComponentFromButton(Button button)
        {
            Components Component = Components.Base;
            try
            {
                string RealTag = (button.Tag.ToString().TrimStart("Files".ToCharArray()));
                Component = (Components)System.Enum.Parse(typeof(Components), RealTag);
            }
            catch
            {
                PopupWrapper.PopupError("Error Code 6");
                HelperClasses.Logger.Log("[AAAAAA] - Getting the enum from the Tag of the UI Stuff from the ComponenetManager failed");
                Globals.ProperExit();
            }

            return Component;
        }

        private void btn_Refresh_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 3)
            {
                bool yesno = PopupWrapper.PopupYesNo("Do you want to reset the Information if Components are installed?");
                if (yesno == true)
                {
                    LaunchAlternative.SocialClubReset();
                    HelperClasses.RegeditHandler.SetValue("DownloadManagerInstalledSubassemblies", "");
                    MyRefresh();
                    StartupCheck();
                }
                MyRefresh();
            }
        }

        private void btn_lbl_Mode_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 3)
            {
                PopupWrapper.PopupMode();
            }
        }


        private void ImportZIPForComponenet(Components MyComponent, string LocalZIPFilePath)
        {
            HelperClasses.Logger.Log("ComponentMngr - Component: '" + MyComponent + "', Importing local ZIP");

            string tmpLocation = MyComponent.GetPathWhereZIPIsExtracted();
            HelperClasses.Logger.Log("ComponentMngr - Component: '" + MyComponent + "', ZIPPath where we extract for that one: '" + tmpLocation + "'.", 1);

            PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.ZIPFile, LocalZIPFilePath, null, tmpLocation);

            HelperClasses.Logger.Log("ComponentMngr - Guess the ZIP Extraction worked since we got here...", 1);

            if (MyComponent == Components.Base)
            {
                tmpLocation = tmpLocation.TrimEnd('\\') + @"\Project_127_Files\";
            }
            tmpLocation = tmpLocation.TrimEnd('\\') + @"\Version.txt";

            HelperClasses.Logger.Log("ComponentMngr - tmpLocation = '" + tmpLocation + "'", 1);

            bool doWeNeedVersion = true;
            Version FI = MyComponent.GetInstalledVersion();
            if (HelperClasses.FileHandling.doesFileExist(tmpLocation))
            {
                string content = HelperClasses.FileHandling.ReadContentOfFile(tmpLocation);

                if (MyComponent == Components.Base)
                {
                    int _ZipVersion = 0;
                    Int32.TryParse(content, out _ZipVersion);

                    if (_ZipVersion > 0)
                    {
                        FI = new Version(1, _ZipVersion, 0, 0);
                        doWeNeedVersion = false;
                    }
                    else
                    {
                        try
                        {
                            FI = new Version(content);
                            doWeNeedVersion = false;
                        }
                        catch { }
                    }
                }
                else
                {
                    try
                    {
                        FI = new Version(content);
                        doWeNeedVersion = false;
                    }
                    catch { }
                }
            }
            HelperClasses.FileHandling.deleteFile(tmpLocation);

            if (doWeNeedVersion)
            {
                if (FI == new Version("0.0.0.0"))
                {
                    FI = new Version("1.0.0.0");
                }

                string tmp2 = PopupWrapper.PopupTextbox("Enter the Version you want to force.", FI.ToString());

                try
                {
                    FI = new Version(tmp2);
                }
                catch { }
            }

            HelperClasses.Logger.Log("ComponentMngr - ForceSetting the Component ('" + MyComponent + "'), to Version: '" + FI.ToString() + "'.", 1);
            MyComponent.ForceSetInstalled(FI);
        }

        private void btn_Uninstall_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 3)
            {
                HelperClasses.Logger.Log("ComponentMngr - User wants to hidden import ZIP for Component.");
                string tmp = PopupWrapper.PopupTextbox("Enter the Link provided by us.", "");
                if (HelperClasses.FileHandling.URLExists(tmp))
                {
                    HelperClasses.Logger.Log("ComponentMngr - Link ('" + tmp + "') exists.", 1);

                    string localFilePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\CustomFile.zip";
                    HelperClasses.FileHandling.deleteFile(localFilePath);
                    PopupWrapper.PopupDownload(tmp, localFilePath, "Downloading Custom Files");
                    if (HelperClasses.FileHandling.doesFileExist(localFilePath))
                    {
                        HelperClasses.Logger.Log("ComponentMngr - File post Download found ('" + localFilePath + "').", 1);
                        ImportZIPForComponenet(GetComponentFromButton((Button)sender), localFilePath);
                        HelperClasses.FileHandling.deleteFile(localFilePath);
                    }
                    else
                    {
                        HelperClasses.Logger.Log("ComponentMngr - File cant be found post DL ('" + localFilePath + "').", 1);
                        PopupWrapper.PopupOk("Cant find the downloaded File on Disk.\nWill abort");
                    }
                }
                else
                {
                    HelperClasses.Logger.Log("ComponentMngr - Link ('" + tmp + "') does NOT exist.", 1);
                    PopupWrapper.PopupOk("Cant find that File online.\nWill abort");
                }
            }
        }

        private void btn_DragMove(object sender, MouseButtonEventArgs e)
        {
            MainWindow.MW.DragMove();
        }

        private void btn_lbl_Status_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 3)
            {
                ImportComponenet(GetComponentFromButton((Button)sender));
            }
        }

        private void ImportComponenet(Components Component)
        {
            string rtrn = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Select the ZIP File you want to import", @"C:\", false, "ZIP Files|*.zip*");

            if (HelperClasses.FileHandling.doesFileExist(rtrn))
            {
                ImportZIPForComponenet(Component, rtrn);
            }
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            PopupWrapper.PopupOk("Select the ZIP file you want to import for this Component.\nIf you dont know what you are doing,\nor clicked this by accident,\njust close the File Picker.");
            ImportComponenet(GetComponentFromButton((Button)sender));
        }
    }


    static class ComponentsExtensions
    {
        public static string GetAssemblyName(this ComponentManager.Components Component)
        {
            string rtrn = "";
            switch (Component)
            {
                case ComponentManager.Components.Base:
                    rtrn = "STANDARD_BASE";
                    break;
                case ComponentManager.Components.Base124:
                    rtrn = "124_DR490N";
                    break;
                case ComponentManager.Components.Base129:
                    rtrn = "129_DR490N";
                    break;
                case ComponentManager.Components.SCLRockstar124:
                    rtrn = "AM_124_ROCKSTAR";
                    break;
                case ComponentManager.Components.SCLRockstar127:
                    rtrn = "AM_127_ROCKSTAR";
                    break;
                case ComponentManager.Components.SCLRockstar129:
                    rtrn = "AM_129_ROCKSTAR";
                    break;
                case ComponentManager.Components.SCLSteam124:
                    rtrn = "AM_124_STEAM";
                    break;
                case ComponentManager.Components.SCLSteam127:
                    rtrn = "AM_127_STEAM";
                    break;
                case ComponentManager.Components.SCLSteam129:
                    rtrn = "AM_129_STEAM";
                    break;
                case ComponentManager.Components.DowngradedSC:
                    rtrn = "SOCIALCLUB_1178";
                    break;
                case ComponentManager.Components.AdditionalSaveFiles:
                    rtrn = "ADDITIONAL_SAVEFILES";
                    break;
            }
            return rtrn;
        }

        public static string GetNiceName(this ComponentManager.Components Component)
        {
            string rtrn = "";
            switch (Component)
            {
                case ComponentManager.Components.Base:
                    rtrn = "Required Files (P127 and Downgraded GTA)";
                    break;
                case ComponentManager.Components.Base124:
                    rtrn = "Files required to launch version 1.24 through dr490n Launcher";
                    break;
                case ComponentManager.Components.Base129:
                    rtrn = "Files required to launch version 1.29 through dr490n Launcher";
                    break;
                case ComponentManager.Components.SCLRockstar124:
                    rtrn = "Files for Launching through Social Club for Rockstar 1.24";
                    break;
                case ComponentManager.Components.SCLRockstar127:
                    rtrn = "Files for Launching through Social Club for Rockstar 1.27";
                    break;
                case ComponentManager.Components.SCLRockstar129:
                    rtrn = "Files for Launching through Social Club for Rockstar 1.29";
                    break;
                case ComponentManager.Components.SCLSteam124:
                    rtrn = "Files for Launching through Social Club for Steam 1.24";
                    break;
                case ComponentManager.Components.SCLSteam127:
                    rtrn = "Files for Launching through Social Club for Steam 1.27";
                    break;
                case ComponentManager.Components.SCLSteam129:
                    rtrn = "Files for Launching through Social Club for Steam 1.29";
                    break;
                case ComponentManager.Components.DowngradedSC:
                    rtrn = "Files for Launching through Social Club (Downgraded Social Club Files)";
                    break;
                case ComponentManager.Components.AdditionalSaveFiles:
                    rtrn = "Additional SaveFiles";
                    break;
            }
            return rtrn;
        }

        public static bool IsInstalled(this ComponentManager.Components Component)
        {
            return Globals.MyDM.isInstalled(Component.GetAssemblyName());
        }

        public static void ForceSetInstalled(this ComponentManager.Components Component, Version myVersion)
        {
            HelperClasses.Logger.Log("ComponentMngr - Forcing SetInstalled Component: '" + Component + "' (Subassemblyname: '" + Component.GetAssemblyName() + "') to version: '" + myVersion.ToString() + "'. Previous Version will be '" + Component.GetInstalledVersion() + "'"); ;
            Globals.MyDM.setVersion(Component.GetAssemblyName(), myVersion);

#if DEBUG
			HelperClasses.Logger.Log("Just forced an Installation SetInstalled. Currerntly this is the State:",4);
			foreach (ComponentManager.Components myComponent in ComponentManager.AllComponents)
				{
					HelperClasses.Logger.Log("    ComponentMngr - Component: '" + myComponent.ToString() + "', Installed: '" + myComponent.IsInstalled() + "'. Version: '" + myComponent.GetInstalledVersion() + "'",5);
				}
#endif

            ComponentManager.MyRefreshStatic();
        }

        public static string GetPathWhereZIPIsExtracted(this ComponentManager.Components Component)
        {
            // ALL ZIP FILES FROM DOWNLOAD MANAGER (GITHUB XML AND FRODOS FTP) ARE UNZIPPED WITH PROPER FOLDER STRUCTURE BAKED INTO THE ZIP
            // ALL ZIP FILES FOR IMPORTING MANUALLY DO NOT HAVE THAT, AND ARE EXTRACTED AT THE EXACT PLACE WHERE WE WANT THE FILES
            switch (Component)
            {
                case ComponentManager.Components.Base:
                    return LauncherLogic.ZIPFilePath;
                case ComponentManager.Components.Base124:
                    return LauncherLogic.DowngradeBase124FilePath;
                case ComponentManager.Components.Base129:
                    return LauncherLogic.DowngradeBase129FilePath;
                case ComponentManager.Components.SCLRockstar124:
                    return LauncherLogic.DowngradeAlternativeFilePathRockstar124;
                case ComponentManager.Components.SCLRockstar127:
                    return LauncherLogic.DowngradeAlternativeFilePathRockstar127;
                case ComponentManager.Components.SCLRockstar129:
                    return LauncherLogic.DowngradeAlternativeFilePathRockstar129;
                case ComponentManager.Components.SCLSteam124:
                    return LauncherLogic.DowngradeAlternativeFilePathSteam124;
                case ComponentManager.Components.SCLSteam127:
                    return LauncherLogic.DowngradeAlternativeFilePathSteam127;
                case ComponentManager.Components.SCLSteam129:
                    return LauncherLogic.DowngradeAlternativeFilePathSteam129;
                case ComponentManager.Components.DowngradedSC:
                    return LaunchAlternative.SCL_SC_DOWNGRADED;
                case ComponentManager.Components.AdditionalSaveFiles:
                    return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\SaveFiles";
                default:
                    return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files";
            }
        }

        public static bool IsOnDisk(this ComponentManager.Components Component)
        {
            switch (Component)
            {
                case ComponentManager.Components.Base:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeEmuFilePath);
                case ComponentManager.Components.Base124:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeBase124FilePath);
                case ComponentManager.Components.Base129:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeBase129FilePath);
                case ComponentManager.Components.DowngradedSC:
                    return (LaunchAlternative.Get_SCL_InstallationState(LaunchAlternative.SCL_SC_DOWNGRADED) == LaunchAlternative.SCL_InstallationStates.Downgraded);
                case ComponentManager.Components.SCLRockstar124:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeAlternativeFilePathRockstar124);
                case ComponentManager.Components.SCLRockstar127:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeAlternativeFilePathRockstar127);
                case ComponentManager.Components.SCLRockstar129:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeAlternativeFilePathRockstar129);
                case ComponentManager.Components.SCLSteam124:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeAlternativeFilePathSteam124);
                case ComponentManager.Components.SCLSteam127:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeAlternativeFilePathSteam127);
                case ComponentManager.Components.SCLSteam129:
                    return LauncherLogic.IsDowngradedGTA(LauncherLogic.DowngradeAlternativeFilePathSteam129);
                case ComponentManager.Components.AdditionalSaveFiles:
                    return true;
                default:
                    return true;
            }
        }

        public static bool UpdateLogic(this ComponentManager.Components Component)
        {
            HelperClasses.Logger.Log("ComponentMngr - Checking if Update for Component: '" + Component + "' (Subassemblyname: '" + Component.GetAssemblyName() + "') is available.");

            if (Globals.MyDM.isUpdateAvalailable(Component.GetAssemblyName(), true))
            {
                HelperClasses.Logger.Log("ComponentMngr - It is");
                bool yesno = PopupWrapper.PopupYesNo("Update for: '" + Component.GetNiceName() + "' available.\nDo you want to Download it?");
                if (yesno == true)
                {
                    if (ComponentManager.RequiredComponentsBasedOnSettings.Contains(Component) && Component != ComponentManager.Components.DowngradedSC)
                    {
                        if (ComponentManager.RecommendUpgradedGTA())
                        {
                            HelperClasses.Logger.Log("ComponentMngr - User wants update. Passed RecommendUpgradedGTA check.");
                            Globals.MyDM.updateSubssembly(Component.GetAssemblyName(), true).GetAwaiter().GetResult();
                            bool PICrtrn = PopupWrapper.PopupInstallComponent(Component, PopupInstallComponent.ComponentActions.Updating);
                            ComponentManager.MyRefreshStatic();
                            return PICrtrn;
                        }
                        else
                        {
                            HelperClasses.Logger.Log("ComponentMngr - User wants update. Failed RecommendUpgradedGTA check. Will abondon update.");
                            PopupWrapper.PopupOk("Abandoning Update of Component:\n'" + Component.GetNiceName() + "'");
                            ComponentManager.MyRefreshStatic();
                            return false;
                        }
                    }
                    else
                    {
                        HelperClasses.Logger.Log("ComponentMngr - User wants update.");
                        if (Component == ComponentManager.Components.AdditionalSaveFiles)
                        {
                            HelperClasses.Logger.Log("ComponentMngr - Updating SaveFiles.");

                            List<MyFileOperation> MFOs = new List<MyFileOperation>();
                            bool yesno2 = PopupWrapper.PopupYesNo("Do you want to back up your old SaveFiles??");
                            if (yesno2 == true)
                            {
                                HelperClasses.Logger.Log("ComponentMngr - User wants to back up old savefiles.");

                                // move everything one folder down
                                string[] FilePaths = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.SaveFilesPath);
                                string[] SubFolders = HelperClasses.FileHandling.GetSubFolders(LauncherLogic.SaveFilesPath);

                                string SaveFilesOldFolder = LauncherLogic.SaveFilesPath.TrimEnd('\\') + @"\Old_Savefiles";
                                MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Create, SaveFilesOldFolder, "", "Creating Old SaveFile Folder (" + SaveFilesOldFolder + ")", 0, MyFileOperation.FileOrFolder.Folder));

                                foreach (string FilePath in FilePaths)
                                {
                                    string NewPath = FilePath.Replace(@"\Project_127_Files\SupportFiles\SaveFiles\", @"\Project_127_Files\SupportFiles\SaveFiles\Old_Savefiles\");
                                    if (HelperClasses.FileHandling.doesFileExist(NewPath))
                                    {
                                        MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, NewPath, "", "Deleting '" + NewPath + "' since it exists and we want to move an old savefile there", 0, MyFileOperation.FileOrFolder.File));
                                    }
                                    MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, FilePath, NewPath, "Moving '" + FilePath + "' to '" + NewPath + "' to save old Savefiles", 0, MyFileOperation.FileOrFolder.File));
                                }
                                foreach (string SubFolder in SubFolders)
                                {
                                    MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SubFolder, "", "Deleting '" + SubFolder + "' (subfolder) since we have moved the files to backup directory", 0, MyFileOperation.FileOrFolder.Folder));
                                }
                            }
                            else
                            {
                                HelperClasses.Logger.Log("ComponentMngr - User wants to nuke old savefiles.");

                                // nuke folder, re-create folder
                                MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, LauncherLogic.SaveFilesPath, "", "Deleting old SaveFile Folder (" + LauncherLogic.SaveFilesPath + ")", 0, MyFileOperation.FileOrFolder.Folder));
                                MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Create, LauncherLogic.SaveFilesPath, "", "Re-creating SaveFile Folder (" + LauncherLogic.SaveFilesPath + ")", 0, MyFileOperation.FileOrFolder.Folder));
                            }
                            if (MFOs.Count > 0)
                            {
                                PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Managing old Savefiles...", MFOs);
                            }
                        }
                        if (Component == ComponentManager.Components.DowngradedSC)
                        {
                            HelperClasses.Logger.Log("ComponentMngr - Updating Downgraded SocialClub.");

                            // if we update social club, delete all 4 folder locations if they are not upgraded, to remove all traces

                            List<string> FilePaths = new List<string>
                                {
                                    LaunchAlternative.SCL_SC_DOWNGRADED,
                                    LaunchAlternative.SCL_SC_DOWNGRADED_CACHE,
                                    LaunchAlternative.SCL_SC_Installation,
                                    LaunchAlternative.SCL_SC_TEMP_BACKUP
                                };

                            List<MyFileOperation> MFOs = new List<MyFileOperation>();

                            foreach (string FilePath in FilePaths)
                            {
                                if (LaunchAlternative.Get_SCL_InstallationState(FilePath) != LaunchAlternative.SCL_InstallationStates.Upgraded)
                                {
                                    MFOs.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, FilePath, "", "Deleting '" + FilePath + "' since we are Upgrading the Social Club Component and want all old downgrades gone.", 0, MyFileOperation.FileOrFolder.Folder));
                                }
                            }

                            if (MFOs.Count > 0)
                            {
                                PopupWrapper.PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Deleting previous downgraded Social Clubs", MFOs);
                            }
                        }
                        Globals.MyDM.updateSubssembly(Component.GetAssemblyName(), true).GetAwaiter().GetResult();
                        if (Component == ComponentManager.Components.DowngradedSC)
                        {
                            // So we just upgraded Social Club Component, after deleting all non-upgraded social club folders.
                            // we now want to upgrade social club,
                            LaunchAlternative.SocialClubUpgrade();
                        }
                        ComponentManager.MyRefreshStatic();
                        return true;
                    }
                }
                HelperClasses.Logger.Log("ComponentMngr - User does NOT want update.");
            }
            HelperClasses.Logger.Log("ComponentMngr - No Update found.");
            //ComponentManager.MyRefreshStatic();
            return false;
        }

        /// <summary>
        /// Installs a Specific Assembly. Returns Success bool.
        /// </summary>
        /// <param name="Component"></param>
        /// <returns></returns>
        public static bool Install(this ComponentManager.Components Component)
        {
            HelperClasses.Logger.Log("ComponentMngr - Installing Component: '" + Component + "' (Subassemblyname: '" + Component.GetAssemblyName() + "')"); ;
            bool PICrtrn = PopupWrapper.PopupInstallComponent(Component, PopupInstallComponent.ComponentActions.Installing);
            if (Component.ToString().StartsWith("SCL"))
            {
                if (!ComponentManager.Components.DowngradedSC.IsInstalled())
                {
                    HelperClasses.Logger.Log("ComponentMngr - Installing Downgraded Social Club as well, since its not installed. Subassemblyname: '" + ComponentManager.Components.DowngradedSC.GetAssemblyName() + "')"); ;
                    bool PIC2rtrn = PopupWrapper.PopupInstallComponent(ComponentManager.Components.DowngradedSC, PopupInstallComponent.ComponentActions.Installing);
                    if (PIC2rtrn == false)
                    {
                        ComponentManager.MyRefreshStatic();
                        return false;
                    }
                }
            }
            ComponentManager.MyRefreshStatic();
            return PICrtrn;
        }

        /// <summary>
        /// Re-Installs a specific Assembly. Returns Success bool.
        /// </summary>
        /// <param name="Component"></param>
        /// <returns></returns>
        public static bool ReInstall(this ComponentManager.Components Component)
        {
            HelperClasses.Logger.Log("ComponentMngr - Re-Installing Component: '" + Component + "' (Subassemblyname: '" + Component.GetAssemblyName() + "'). Currently installed: '" + Component.GetInstalledVersion() + "'"); ;
            bool PICrtrn = PopupWrapper.PopupInstallComponent(Component, PopupInstallComponent.ComponentActions.ReInstalling);
            ComponentManager.MyRefreshStatic();
            return PICrtrn;
        }


        /// <summary>
        /// Uninstall a Specific assembly.
        /// </summary>
        /// <param name="Component"></param>
        public static void Uninstall(this ComponentManager.Components Component)
        {
            HelperClasses.Logger.Log("ComponentMngr - Uninstalling Component: '" + Component + "' (Subassemblyname: '" + Component.GetAssemblyName() + "'). Currently installed: '" + Component.GetInstalledVersion() + "'");
            try
            {
                Globals.MyDM.delSubassembly(Component.GetAssemblyName());
            }
            catch
            {
                PopupWrapper.PopupError("Failed to uninstall Component.\nMost likely cause is no connection to github.");
                HelperClasses.Logger.Log("Failed to uninstall from Component Manager. Most likely github offline or user has no internet");
            }
            ComponentManager.MyRefreshStatic();
        }

        /// <summary>
        /// Checks if a Subassembly is actuall installed (matches Hashes). Doesnt repair. Returns Success bool.
        /// </summary>
        /// <param name="Component"></param>
        /// <returns></returns>
        public static bool Verify(this ComponentManager.Components Component)
        {
            HelperClasses.Logger.Log("ComponentMngr - Verifying Component: '" + Component + "' (Subassemblyname: '" + Component.GetAssemblyName() + "'). Currently installed: '" + Component.GetInstalledVersion() + "'");
            ComponentManager.MyRefreshStatic();
            return Globals.MyDM.verifySubassembly(Component.GetAssemblyName()).GetAwaiter().GetResult();
        }

        /// <summary>
        /// GUI Method to Update the label
        /// </summary>
        /// <param name="Component"></param>
        /// <param name="myLbl"></param>
        public static void UpdateStatus(this ComponentManager.Components Component, Button myLbl)
        {
            if (Component.IsInstalled())
            {
                myLbl.Content = "Installed";
                myLbl.Foreground = MyColors.MyColorGreen;
                myLbl.ToolTip = "Installed Version: " + Component.GetInstalledVersion().ToString();
            }
            else
            {
                myLbl.Content = "Not Installed";
                myLbl.Foreground = MyColors.MyColorOrange;
                myLbl.ToolTip = "Not Installed";
            }
        }

        /// <summary>
        /// GetInstallVersionUIText
        /// </summary>
        /// <param name="Component"></param>
        public static Version GetInstalledVersion(this ComponentManager.Components Component)
        {
            Version rtrn = new Version("0.0.0.0");
            if (Component.IsInstalled())
            {
                rtrn = Globals.MyDM.getVersion(Component.GetAssemblyName());
            }
            return rtrn;
        }

        /// <summary>
        /// GetToolTip
        /// </summary>
        /// <param name="Component"></param>
        /// <param name="myLbl"></param>
        public static string GetToolTip(this ComponentManager.Components Component)
        {
            string toolTip = Component.GetNiceName();
            if (Component.IsInstalled())
            {
                toolTip = toolTip + ". Installed Version: " + Component.GetInstalledVersion().ToString();
            }
            return toolTip;
        }
    }
}
