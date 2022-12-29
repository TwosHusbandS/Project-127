using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Project_127.MySettings
{
    /// <summary>
    /// Interaction logic for InitImportantSettings.xaml
    /// </summary>
    public partial class InitImportantSettings : Window
    {
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            if (MainWindow.MW.IsVisible)
            {
                this.Left = MainWindow.MW.Left + (MainWindow.MW.Width / 2) - (this.Width / 2);
                this.Top = MainWindow.MW.Top + (MainWindow.MW.Height / 2) - (this.Height / 2);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove(); // Pre-Defined Method
        }




        public InitImportantSettings()
        {
            if (MainWindow.MW.IsVisible)
            {
                this.Owner = MainWindow.MW;
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            // Initializing all WPF Elements
            InitializeComponent();
            btn_Save.IsEnabled = false;
            pb_sth.Value = 5;
            lbl_Header.Content = "Initiating the most important Settings Project 1.27 needs. (0/4)";

            AddGuesses();
            btn_No.Visibility = Visibility.Hidden;
            btn_Yes.Visibility = Visibility.Hidden;
            btn_BigBtn.Visibility = Visibility.Hidden;
            cb_cb.Visibility = Visibility.Hidden;
            Loop();
        }

        List<string> GTAVPathGuesses = new List<string>();
        int currIndex = -1;

        public string NS_GTAVPath = "";
        public string NS_ZIPPath = "";
        public bool NS_EnableCopyFilesInsteadOfHardlinking = false;
        public string NS_Retailer = "";

        public void AddGuesses()
        {
            // Adding all Guesses
            GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicSteam());
            GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicRockstar());
            GTAVPathGuesses.Add(LauncherLogic.GetGTAVPathMagicEpic());

            try
            {
                RegistryKey myRKTemp1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Rockstar Games\GTAV");
                GTAVPathGuesses.Add(HelperClasses.RegeditHandler.GetValue(myRKTemp1, "InstallFolderSteam"));
            }
            catch { }

            try
            {
                RegistryKey myRKTemp2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Rockstar Games\Grand Theft Auto V");
                GTAVPathGuesses.Add(HelperClasses.RegeditHandler.GetValue(myRKTemp2, "InstallFolderEpic"));
            }
            catch { }

            try
            {
                RegistryKey myRKTemp3 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Rockstar Games\Grand Theft Auto V");
                GTAVPathGuesses.Add(HelperClasses.RegeditHandler.GetValue(myRKTemp3, "InstallFolder"));
            }
            catch { }

            GTAVPathGuesses.Add(Globals.ProjectInstallationPath.TrimEnd('\\').Substring(0, Globals.ProjectInstallationPath.LastIndexOf('\\')));
            GTAVPathGuesses.Add(Globals.ProjectInstallationPath.TrimEnd('\\'));
            GTAVPathGuesses.Add(Settings.ZIPExtractionPath.TrimEnd('\\').Substring(0, Settings.ZIPExtractionPath.LastIndexOf('\\')));
            GTAVPathGuesses.Add(Settings.ZIPExtractionPath);
        }

        public void Loop(int start = 0)
        {
            // Loop for the Guesses
            for (int i = start; i <= GTAVPathGuesses.Count - 1; i++)
            {
                HelperClasses.Logger.Log("GTAV Guess Number " + (i + 1).ToString() + " is: '" + GTAVPathGuesses[i] + "'");
                if (LauncherLogic.IsGTAVInstallationPathCorrect(GTAVPathGuesses[i], false))
                {
                    HelperClasses.Logger.Log("GTAV Guess Number " + (i + 1).ToString() + " is theoretically VALID. Asking user if he wants it");
                    btn_No.Visibility = Visibility.Visible;
                    btn_Yes.Visibility = Visibility.Visible;
                    btn_Yes.Tag = "ConfirmGuess";
                    btn_No.Tag = "ConfirmGuess";
                    btn_BigBtn.Visibility = Visibility.Hidden;
                    lbl_Main.Content = "Is: '" + GTAVPathGuesses[i] + "'\nyour GTA V Install Location?";
                    currIndex = i;
                    return;
                }
                else
                {
                    HelperClasses.Logger.Log("GTAV Guess Number " + (i + 1).ToString() + " is theoretically invalid, moving on");
                }
            }

            HelperClasses.Logger.Log("After " + GTAVPathGuesses.Count + " guesses we still dont have the correct GTAVInstallationPath. User has to do it manually now. Fucking casual");
            btn_No.Visibility = Visibility.Hidden;
            btn_Yes.Visibility = Visibility.Hidden;
            btn_BigBtn.Visibility = Visibility.Visible;
            btn_BigBtn.Tag = "GTAVPath";
            lbl_Main.Content = "Select the Path where your gta5.exe is located.";
        }

        public void CheckIfZIPPathDone()
        {
            if (!String.IsNullOrWhiteSpace(NS_ZIPPath))
            {
                HelperClasses.Logger.Log("ZIP Path set: '" + NS_ZIPPath + "'");

                pb_sth.Value = 55;
                lbl_Header.Content = "Initiating the most important Settings Project 1.27 needs. (2/4)";

                lbl_P127Path_Setting.Content = NS_ZIPPath;
                lbl_P127Path_Setting.ToolTip = NS_ZIPPath;

                lbl_Main.Content = "";
                btn_No.Visibility = Visibility.Hidden;
                btn_Yes.Visibility = Visibility.Hidden;
                btn_BigBtn.Visibility = Visibility.Hidden;

                SetUpDefaultEnableCopyingHardlinking();
            }
        }

        public void CheckIfGTAVPathDone()
        {
            if (!String.IsNullOrWhiteSpace(NS_GTAVPath))
            {
                HelperClasses.Logger.Log("GTAV Path set: '" + NS_GTAVPath + "'");

                pb_sth.Value = 30;
                lbl_Header.Content = "Initiating the most important Settings Project 1.27 needs. (1/4)";

                lbl_GTAVPath_Setting.Content = NS_GTAVPath;
                lbl_GTAVPath_Setting.ToolTip = NS_GTAVPath;

                lbl_Main.Content = "";
                btn_No.Visibility = Visibility.Hidden;
                btn_Yes.Visibility = Visibility.Hidden;
                btn_BigBtn.Visibility = Visibility.Hidden;

                HelperClasses.Logger.Log("Asking about ZIP Path (Project_127_Folder) now");
                lbl_Main.Content = "Project 1.27 needs a Folder where it installs all of its Components and saves Files for Upgrading and Downgrading.\nIt is recommend to do this on the same Drive / Partition as your GTAV Installation Path\nBest Case (and default Location) is your GTAV Path.\nDo you want to use the default recommendation?";
                btn_No.Visibility = Visibility.Visible;
                btn_Yes.Visibility = Visibility.Visible;
                btn_No.Tag = "ZIPDefault";
                btn_Yes.Tag = "ZIPDefault";
                btn_BigBtn.Visibility = Visibility.Hidden;
            }
        }



        public void PickZIPPathLocation()
        {
            HelperClasses.Logger.Log("Asking User for ZIP path");
            string StartUpPath = NS_GTAVPath;
            string UserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder you want to use to save P127 Files", StartUpPath);
            HelperClasses.Logger.Log("Users picked path is: '" + UserChoice + "'");

            if (String.IsNullOrWhiteSpace(UserChoice))
            {
                HelperClasses.Logger.Log("No Folder selected. Canceling User Action of Changing GTAV Installation Path");
            }
            else
            {

                HelperClasses.Logger.Log("Users picked path '" + UserChoice + "'. Asking User if User hes sure.");
                btn_No.Visibility = Visibility.Visible;
                btn_Yes.Visibility = Visibility.Visible;
                btn_Yes.Tag = "ZIPPathConfirmation";
                btn_No.Tag = "ZIPPathConfirmation";
                btn_BigBtn.Visibility = Visibility.Hidden;
                lbl_Main.Content = "Do you want:\n'" + UserChoice + "'/nas your ZIP Path Location?";
                lbl_Main.Tag = UserChoice;
                UserChoice = "";
            }

            NS_ZIPPath = UserChoice;
            CheckIfZIPPathDone();
        }

        public void PickGTAVLocation()
        {
            HelperClasses.Logger.Log("Asking User for GTA V Installation path");
            string StartUpPath = Settings.GTAVInstallationPath;
            if (String.IsNullOrWhiteSpace(StartUpPath))
            {
                StartUpPath = @"C:\";
            }
            else
            {
                StartUpPath = HelperClasses.FileHandling.PathSplitUp(StartUpPath.TrimEnd('\\'))[0];
            }
            string GTAVInstallationPathUserChoice = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTA5.exe", StartUpPath);
            HelperClasses.Logger.Log("Users picked path is: '" + GTAVInstallationPathUserChoice + "'");


            if (String.IsNullOrWhiteSpace(GTAVInstallationPathUserChoice))
            {
                HelperClasses.Logger.Log("No Folder selected. Canceling User Action of Changing GTAV Installation Path");
            }
            else
            {
                if (LauncherLogic.IsGTAVInstallationPathCorrect(GTAVInstallationPathUserChoice, false))
                {
                    HelperClasses.Logger.Log("Picked path '" + GTAVInstallationPathUserChoice + "' is valid and will be set as Settings.GTAVInstallationPath.");
                }
                else
                {
                    HelperClasses.Logger.Log("Users picked path '" + GTAVInstallationPathUserChoice + "' detected to be faulty. Asking User if User wants to force it.");
                    btn_No.Visibility = Visibility.Visible;
                    btn_Yes.Visibility = Visibility.Visible;
                    btn_Yes.Tag = "ForcePath";
                    btn_No.Tag = "ForcePath";
                    btn_BigBtn.Visibility = Visibility.Hidden;
                    lbl_Main.Content = "GTA V Path detected to be not correct. Are you sure?\nForce '" + GTAVInstallationPathUserChoice + "' as your GTAV Installation Location?";
                    lbl_Main.Tag = GTAVInstallationPathUserChoice;
                    GTAVInstallationPathUserChoice = "";
                }
            }

            NS_GTAVPath = GTAVInstallationPathUserChoice;
            CheckIfGTAVPathDone();
        }



        public void SetUpDefaultEnableCopyingHardlinking()
        {
            bool currentSetting = Settings.EnableCopyFilesInsteadOfHardlinking;
            bool recommendSetting = !(NS_ZIPPath[0] == NS_GTAVPath[0]);

            HelperClasses.Logger.Log("Checking to see if Settings.EnableCopyFilesInsteadOfHardlinking is on recommended value");
            HelperClasses.Logger.Log("NS_ZIPPath: '" + NS_ZIPPath + "'");
            HelperClasses.Logger.Log("NS_GTAVPath: '" + NS_GTAVPath + "'");
            HelperClasses.Logger.Log("Setting: EnableCopyFilesInsteadOfHardlinking");
            HelperClasses.Logger.Log("    currentSettingsValue: '" + currentSetting + "'");
            HelperClasses.Logger.Log("    recommendSettingsValue: '" + recommendSetting + "'");

            if (currentSetting == recommendSetting)
            {
                HelperClasses.Logger.Log("Recommend Settings Value is the Current Settings Value");

                pb_sth.Value = 80;
                lbl_Header.Content = "Initiating the most important Settings Project 1.27 needs. (3/4)";



                SetUpRetailer();
            }
            else
            {
                HelperClasses.Logger.Log("Recommend Settings Value is NOT the Current Settings Value. Asking User what he wants to do");

                btn_No.Visibility = Visibility.Visible;
                btn_Yes.Visibility = Visibility.Visible;
                btn_Yes.Tag = "RecommendedHardlinking";
                btn_No.Tag = "RecommendedHardlinking";
                btn_BigBtn.Visibility = Visibility.Hidden;

                if (recommendSetting == true)
                {
                    lbl_Main.Content = "It is recommended to use the slow but stable way for Upgrading / Downgrading.\nDo you want to do that?";
                }
                else
                {
                    lbl_Main.Content = "It is recommended to NOT use the slow but stable way for Upgrading / Downgrading,\nsince its faster.\n\nDo you want to do that?\n\nClick No if you changed this because something didnt work.";
                }

            }
        }


        public void UserAnswer(bool replyvalue, string MyTag)
        {
            if (MyTag == "ConfirmRetailerGuess")
            {
                if (replyvalue)
                {
                    FinishSetup();
                }
                else
                {
                    ActualSetUpRetailer();
                }
            }
            else if (MyTag == "ConfirmRetailer")
            {
                NS_Retailer = cb_cb.SelectedItem.ToString();
                FinishSetup();
            }
            else if (MyTag == "RecommendedHardlinking")
            {
                bool recommendedSettings = !(NS_ZIPPath[0] == NS_GTAVPath[0]);
                if (replyvalue)
                {
                    HelperClasses.Logger.Log("User wants recommended Setting");
                    NS_EnableCopyFilesInsteadOfHardlinking = recommendedSettings;
                }
                else
                {
                    HelperClasses.Logger.Log("User does NOT want recommended Setting");
                    NS_EnableCopyFilesInsteadOfHardlinking = !recommendedSettings;
                }
                lbl_Main.Content = "";
                btn_No.Visibility = Visibility.Hidden;
                btn_Yes.Visibility = Visibility.Hidden;
                btn_BigBtn.Visibility = Visibility.Hidden;
                SetUpRetailer();
            }
            else if (MyTag == "ConfirmGuess")
            {
                if (replyvalue)
                {
                    HelperClasses.Logger.Log("GTAV Guess Number " + (currIndex + 1).ToString() + " was picked by User");

                    switch (currIndex)
                    {
                        case 0:
                            NS_Retailer = "Steam";
                            break;
                        case 1:
                            NS_Retailer = "Rockstar";
                            break;
                        case 2:
                            NS_Retailer = "Epic";
                            break;
                        case 3:
                            NS_Retailer = "Steam";
                            break;
                        case 4:
                            NS_Retailer = "Epic";
                            break;
                        default:
                            break;
                    }

                    NS_GTAVPath = GTAVPathGuesses[currIndex];
                }
                else
                {
                    HelperClasses.Logger.Log("GTAV Guess Number " + (currIndex + 1).ToString() + " was NOT picked by User, moving on");
                    Loop(currIndex + 1);
                }
                CheckIfGTAVPathDone();
            }
            else if (MyTag == "ForcePath")
            {
                if (replyvalue)
                {
                    NS_GTAVPath = lbl_Main.Tag.ToString();
                    HelperClasses.Logger.Log("GTAV Path was forced.");
                    CheckIfGTAVPathDone();
                }
                else
                {
                    HelperClasses.Logger.Log("GTAV Path force was canceled.");
                    btn_No.Visibility = Visibility.Hidden;
                    btn_Yes.Visibility = Visibility.Hidden;
                    btn_BigBtn.Visibility = Visibility.Visible;
                    lbl_Main.Content = "Select the Path where your gta5.exe is located.";
                }
            }
            else if (MyTag == "ZIPPathConfirmation")
            {
                if (replyvalue)
                {
                    NS_ZIPPath = lbl_Main.Tag.ToString();
                    HelperClasses.Logger.Log("ZIP Path was confirmed.");
                    CheckIfZIPPathDone();
                }
                else
                {
                    HelperClasses.Logger.Log("ZIP Path was NOT confirmed. Asking again");
                    btn_No.Visibility = Visibility.Hidden;
                    btn_Yes.Visibility = Visibility.Hidden;
                    btn_BigBtn.Visibility = Visibility.Visible;
                    lbl_Main.Content = "Pick the Folder you want to use to save P127 Files";
                }
            }
            else if (MyTag == "ZIPDefault")
            {
                if (replyvalue)
                {
                    HelperClasses.Logger.Log("User wants default ZIPPath (same as GTAV Path)");
                    NS_ZIPPath = NS_GTAVPath;
                    CheckIfZIPPathDone();
                }
                else
                {
                    HelperClasses.Logger.Log("User does NOT want default ZIPPath. Fucking prick. Asking him manually");
                    SetUpManualZip();
                }
            }
        }


        private void SetUpManualZip()
        {
            btn_No.Visibility = Visibility.Hidden;
            btn_Yes.Visibility = Visibility.Hidden;
            btn_BigBtn.Visibility = Visibility.Visible;
            btn_BigBtn.Tag = "ZIPPath";
            lbl_Main.Content = "Pick the Folder you want to use to save P127 Files";
        }


        private void btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            UserAnswer(true, ((Button)sender).Tag.ToString());
        }

        private void btn_No_Click(object sender, RoutedEventArgs e)
        {
            UserAnswer(false, ((Button)sender).Tag.ToString());
        }

        private void btn_BigBtn_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag.ToString() == "GTAVPath")
            {
                PickGTAVLocation();
            }
            else if (((Button)sender).Tag.ToString() == "ZIPPath")
            {
                PickZIPPathLocation();
            }
        }

        public void FinishSetup()
        {
            lbl_Retailer_Setting.Content = NS_Retailer;
            lbl_Retailer_Setting.ToolTip = NS_Retailer;
            pb_sth.Value = 100;
            lbl_Header.Content = "Initiating the most important Settings Project 1.27 needs. (4/4)";

            btn_No.Visibility = Visibility.Hidden;
            btn_Yes.Visibility = Visibility.Hidden;
            btn_BigBtn.Visibility = Visibility.Hidden;
            cb_cb.Visibility = Visibility.Hidden;
            lbl_Main.Content = "Confirm your settings via the save-button\non the very bottom right.";
            btn_Save.IsEnabled = true;
        }

        public void ActualSetUpRetailer()
        {
            btn_No.Visibility = Visibility.Visible;
            btn_Yes.Visibility = Visibility.Hidden;
            btn_BigBtn.Visibility = Visibility.Hidden;
            btn_No.Content = "Confirm";
            btn_No.Tag = "ConfirmRetailer";
            lbl_Main.Content = "Select your Retailer.";
            cb_cb.Visibility = Visibility.Visible;
            cb_cb.ItemsSource = Enum.GetValues(typeof(Settings.Retailers));
        }

        public void SetUpRetailer()
        {
            lbl_Hardlinking_Setting.Content = NS_EnableCopyFilesInsteadOfHardlinking;
            lbl_Hardlinking_Setting.ToolTip = NS_EnableCopyFilesInsteadOfHardlinking;
            if (!String.IsNullOrWhiteSpace(NS_Retailer))
            {
                // Ask if user actually wants it
                btn_No.Visibility = Visibility.Visible;
                btn_Yes.Visibility = Visibility.Visible;
                btn_BigBtn.Visibility = Visibility.Hidden;
                btn_No.Tag = "ConfirmRetailerGuess";
                btn_Yes.Tag = "ConfirmRetailerGuess";
                lbl_Main.Content = "Detected: '" + NS_Retailer + "' as your retailer.\nIs that correct?";
            }
            else
            {
                ActualSetUpRetailer();
            }

        }


        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            HelperClasses.Logger.Log("LogInfo - GTAVInstallationPath: '" + NS_GTAVPath + "'");
            HelperClasses.Logger.Log("LogInfo - ZIPExtractionPath: '" + NS_ZIPPath + "'");
            HelperClasses.Logger.Log("LogInfo - EnableCopyOverHardlink: '" + NS_EnableCopyFilesInsteadOfHardlinking + "'");
            HelperClasses.Logger.Log("LogInfo - Retailer: '" + NS_Retailer + "'");
            HelperClasses.Logger.Log("End of InitImportantSettings. Setting now and exiting.");

            Settings.GTAVInstallationPath = NS_GTAVPath;
            Settings.ChangeZIPExtractionPath(NS_ZIPPath);
            Settings.EnableCopyFilesInsteadOfHardlinking = NS_EnableCopyFilesInsteadOfHardlinking;
            Settings.Retailer = (Settings.Retailers)System.Enum.Parse(typeof(Settings.Retailers), NS_Retailer);

            this.Close();
        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Popups.Popup yn = new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupYesNo, "Are you sure you want to exit?\nYour new Settings will not save.");
            yn.ShowDialog();
            if (yn.DialogResult == true)
            {
                this.Close();
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            MainWindow.MW.Left = this.Left;
            MainWindow.MW.Top = this.Top;
        }
    }
}

