// Huge shoutouts to hossel and hoxi_ for making this mod possible !


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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Project_127;
using Project_127.Popups;
using Project_127.HelperClasses;
using Project_127.MySettings;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace Project_127
{
    /// <summary>
    /// Logique d'interaction pour BloatDebloat.xaml
    /// </summary>
    public partial class BloatDebloat : Page
    {
        public BloatDebloat()
        {
            InitializeComponent();
            if (LauncherLogic.GameState == LauncherLogic.GameStates.Running)
            {
                Popup GTARunning = new Popup(Popup.PopupWindowTypes.PopupOk, "The game is running. Please close the game in order to Bloat or Debloat it.");
                GTARunning.ShowDialog();
                btn_Bloat.IsEnabled = false;
                btn_DeBloat.IsEnabled = false;
                return;
            }

            if (LauncherLogic.InstallationState != LauncherLogic.InstallationStates.Downgraded)
            {
                Popup notinstalled = new Popup(Popup.PopupWindowTypes.PopupOk, "The game is not downgraded. You must downgrade the game before debloating it.");
                notinstalled.ShowDialog();
                btn_Bloat.IsEnabled = false;
                btn_DeBloat.IsEnabled = false;
                lbl_BloatState.Content = "Game is not downgraded.";
                return;
            }
            var hashResult = hashTest();

            if (hashResult != DeBloatStates.DeBloated)
            {
                bool is1 = File.Exists(LauncherLogic.SupportFilePath + @"\DeBloatV\RequiredFiles\x64a.rpf");
                bool is2 = File.Exists(LauncherLogic.SupportFilePath + @"\DeBloatV\RequiredFiles\x64b.rpf");
                bool is3 = File.Exists(LauncherLogic.SupportFilePath + @"\DeBloatV\RequiredFiles\PlayGTAV.exe");
                if(is1 && is2 && is3)
                {
                }
                else
                {
                    Popup notInst = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Files required to debloat GTA V are not installed. Click Yes to download the ZIP then go into settings to import the ZIP file.");
                    notInst.ShowDialog();
                    if(notInst.DialogResult == true)
                    {
                        System.Diagnostics.Process.Start("https://drive.google.com/file/d/1t3TmPLeu7VSeUs5Vziry5_84JG7Jcdn9/view?usp=sharing");
                    }
                    btn_DeBloat.IsEnabled = false;
                    btn_Bloat.IsEnabled=false;
                }
            }
        }

        public DeBloatStates hashTest()
        {
            DeBloatStates DeBloatStatea;
            if (!File.Exists(LauncherLogic.GTAVFilePath + @"\x64b.rpf"))
            {
                DeBloatStatea = DeBloatStates.Unsure;
                lbl_BloatState.Content = "Unsure";
                lbl_BloatState.Foreground = MyColors.MW_GTALabelUnsureForeground;
                btn_Bloat.IsEnabled = true;
                btn_DeBloat.IsEnabled = true;
                return DeBloatStatea;
            }
            string hash = CheckMD5(LauncherLogic.GTAVFilePath + @"\x64b.rpf");
            
            if (hash == "cK8kzU/iyO5Y7bkC8BilWA==")
            {
                DeBloatStatea = DeBloatStates.Bloated;
                lbl_BloatState.Content = "Bloated";
                lbl_BloatState.Foreground = MyColors.MW_GTALabelUpgradedForeground;
                btn_Bloat.IsEnabled = false;
                btn_DeBloat.IsEnabled = true;
                return DeBloatStatea;
            }
            else if (hash == "vvNonlOmvrhDCyerJQl9MQ==")
            {
                DeBloatStatea = DeBloatStates.DeBloated;
                lbl_BloatState.Content = "DeBloated";
                lbl_BloatState.Foreground = MyColors.MW_GTALabelDowngradedForeground;
                btn_Bloat.IsEnabled = true;
                btn_DeBloat.IsEnabled = false;
                return DeBloatStatea;
            }
            else
            {
                DeBloatStatea = DeBloatStates.Unsure;
                lbl_BloatState.Content = "Unsure";
                lbl_BloatState.Foreground = MyColors.MW_GTALabelUnsureForeground;
                btn_Bloat.IsEnabled = false;
                btn_DeBloat.IsEnabled = false;
                return DeBloatStatea;
            }
        }
        public enum DeBloatStates
        {
            DeBloated,
            Bloated,
            Unsure
        }

        /// <summary>
        /// Checks if the game is debloated or not
        /// </summary>
        
        public static DeBloatStates DeBloatState
        {

            get
            {
                string hash = BloatDebloat.CheckMD5(LauncherLogic.GTAVFilePath + @"\x64b.rpf");
                DeBloatStates DeBloatStatea;
                if (hash == "cK8kzU/iyO5Y7bkC8BilWA==")
                {
                    DeBloatStatea = DeBloatStates.Bloated;
                    return DeBloatStatea;
                }
                else if (hash == "vvNonlOmvrhDCyerJQl9MQ==")
                {
                    DeBloatStatea = DeBloatStates.DeBloated;
                    return DeBloatStatea;
                }
                else
                {
                    DeBloatStatea = DeBloatStates.Unsure;
                    return DeBloatStatea;
                }
            }
        }
        
        // files integrity stuff
        public static string CheckMD5(string FileNameAndPath)
        {
            using (FileStream fs = new FileStream(FileNameAndPath,
            FileMode.Open))
            {
                return Convert.ToBase64String(new
                MD5CryptoServiceProvider().ComputeHash(fs));
            }
        }

        private void btn_DeBloat_Click(object sender, EventArgs e)
        {
            try
            {
                if(LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
                {
                    Popup notinstalled = new Popup(Popup.PopupWindowTypes.PopupOk, "The game is not downgraded. You must downgrade the game before debloating it.");
                    notinstalled.ShowDialog();
                    return;
                }
                var GTAVPath = LauncherLogic.GTAVFilePath;
                var DeBloatPath = LauncherLogic.SupportFilePath + @"\DeBloatV";
                Directory.CreateDirectory(DeBloatPath + @"\BackupFiles");
               // Moves current files to Backup folder
                if(File.Exists(GTAVPath + @"\GTAVLauncher.exe"))
                {
                    File.Move(GTAVPath + @"\GTAVLauncher.exe", DeBloatPath + @"\BackupFiles\GTAVLauncher.exe");
                }
                if (File.Exists(GTAVPath + @"\x64a.rpf"))
                {
                    File.Move(GTAVPath + @"\x64a.rpf", DeBloatPath + @"\BackupFiles\x64a.rpf");
                }
                if (File.Exists(GTAVPath + @"\x64b.rpf"))
                {
                    File.Move(GTAVPath + @"\x64b.rpf", DeBloatPath + @"\BackupFiles\x64b.rpf");
                }
                if (File.Exists(GTAVPath + @"\launc.dll"))
                {
                    File.Move(GTAVPath + @"\launc.dll", DeBloatPath + @"\BackupFiles\launc.dll");
                }
                if(Directory.Exists(GTAVPath + @"\update"))
                {
                    Directory.Move(GTAVPath + @"\update", DeBloatPath + @"\BackupFiles\update");
                }
                if (File.Exists(GTAVPath + @"\PlayGTAV.exe"))
                {
                    File.Move(GTAVPath + @"\PlayGTAV.exe", DeBloatPath + @"\BackupFiles\PlayGTAV.exe");
                }

                // Moves Required files to GTA folder
                File.Move(DeBloatPath + @"\RequiredFiles\playgtav.exe", GTAVPath + @"\playgtav.exe");
                File.Move(DeBloatPath + @"\RequiredFiles\x64a.rpf", GTAVPath + @"\x64a.rpf");
                File.Move(DeBloatPath + @"\RequiredFiles\x64b.rpf", GTAVPath + @"\x64b.rpf");
                if(File.Exists(DeBloatPath + @"\RequiredFiles\GTAVLauncher.exe")){
                    File.Move(DeBloatPath + @"\RequiredFiles\GTAVLauncher.exe", GTAVPath + @"\GTAVLauncher.exe");
                }
                File.Move(DeBloatPath + @"\RequiredFiles\launc.dll", GTAVPath + @"\launc.dll");
                Directory.Move(DeBloatPath + @"\RequiredFiles\update", GTAVPath + @"\update");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\mpchristmas2\");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\mpheist\");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\mpluxe\");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\mppatchesng\");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\patchday1ng\");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\patchday2bng\");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\patchday2ng\");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\patchday3ng\");
                Directory.CreateDirectory(GTAVPath + @"\update\x64\dlcpacks\patchday4ng\");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\mpchristmas2\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\mpchristmas2\dlc.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\mpheist\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\mpheist\dlc.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\mpluxe\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\mpluxe\dlc.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\mppatchesng\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\mppatchesng\dlc.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday1ng\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\patchday1ng\dlc.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday2bng\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\patchday2bng\dlc.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday2ng\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\patchday2ng\dlc.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday3ng\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\patchday3ng\dlc.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday4ng\dlc.rpf", GTAVPath + @"\update\x64\dlcpacks\patchday4ng\dlc.rpf");


            } catch (Exception ex)
            {
                Popup DeBloatError = new Popup(Popup.PopupWindowTypes.PopupOk, "The Debloating process failed : " + ex.Message + " ( " + ex.ToString() + " )");
                DeBloatError.ShowDialog();
                return;
            }
            btn_DeBloat.IsEnabled = false;
            btn_Bloat.IsEnabled = true;
            Popup success = new Popup(Popup.PopupWindowTypes.PopupOk, "GTA was succesfully debloated.");
            success.ShowDialog();
            lbl_BloatState.Content = "DeBloated";
            lbl_BloatState.Foreground = MyColors.MW_GTALabelDowngradedForeground;
        }

        private void btn_Bloat_Click(object sender, EventArgs e)
        {
            try
            {
                var GTAVPath = LauncherLogic.GTAVFilePath;
                var DeBloatPath = LauncherLogic.SupportFilePath + @"\DeBloatV";
                // Moves current files to Required File Folder
                if (File.Exists(GTAVPath + @"\GTAVLauncher.exe"))
                {
                    File.Move(GTAVPath + @"\GTAVLauncher.exe", DeBloatPath + @"\RequiredFiles\GTAVLauncher.exe");
                }
                if (File.Exists(GTAVPath + @"\x64a.rpf"))
                {
                    File.Move(GTAVPath + @"\x64a.rpf", DeBloatPath + @"\RequiredFiles\x64a.rpf");
                }
                if (File.Exists(GTAVPath + @"\x64b.rpf"))
                {
                    File.Move(GTAVPath + @"\x64b.rpf", DeBloatPath + @"\RequiredFiles\x64b.rpf");
                }
                if (File.Exists(GTAVPath + @"\launc.dll"))
                {
                    File.Move(GTAVPath + @"\launc.dll", DeBloatPath + @"\RequiredFiles\launc.dll");
                }
                File.Move(GTAVPath + @"\update\x64\dlcpacks\mpchristmas2\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\mpchristmas2\dlc.rpf");
                File.Move(GTAVPath + @"\update\x64\dlcpacks\mpheist\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\mpheist\dlc.rpf"); 
                File.Move(GTAVPath + @"\update\x64\dlcpacks\mpluxe\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\mpluxe\dlc.rpf"); 
                File.Move(GTAVPath + @"\update\x64\dlcpacks\mppatchesng\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\mppatchesng\dlc.rpf"); 
                File.Move(GTAVPath + @"\update\x64\dlcpacks\patchday1ng\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday1ng\dlc.rpf"); 
                File.Move(GTAVPath + @"\update\x64\dlcpacks\patchday2bng\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday2bng\dlc.rpf"); 
                File.Move(GTAVPath + @"\update\x64\dlcpacks\patchday2ng\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday2ng\dlc.rpf"); 
                File.Move(GTAVPath + @"\update\x64\dlcpacks\patchday3ng\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday3ng\dlc.rpf"); 
                File.Move(GTAVPath + @"\update\x64\dlcpacks\patchday4ng\dlc.rpf", DeBloatPath + @"\BackupFiles\update\x64\dlcpacks\patchday4ng\dlc.rpf"); 

                if (Directory.Exists(GTAVPath + @"\update"))
                {
                    Directory.Move(GTAVPath + @"\update", DeBloatPath + @"\RequiredFiles\update");
                }
                if (File.Exists(GTAVPath + @"\PlayGTAV.exe"))
                {
                    File.Move(GTAVPath + @"\PlayGTAV.exe", DeBloatPath + @"\RequiredFiles\PlayGTAV.exe");
                }

                // Moves Required files to GTA folder
                
                File.Move(DeBloatPath + @"\BackupFiles\playgtav.exe", GTAVPath + @"\playgtav.exe");
                File.Move(DeBloatPath + @"\BackupFiles\x64a.rpf", GTAVPath + @"\x64a.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\x64b.rpf", GTAVPath + @"\x64b.rpf");
                File.Move(DeBloatPath + @"\BackupFiles\GTAVLauncher.exe", GTAVPath + @"\GTAVLauncher.exe");
                File.Move(DeBloatPath + @"\BackupFiles\launc.dll", GTAVPath + @"\launc.dll");
                Directory.Move(DeBloatPath + @"\BackupFiles\update", GTAVPath + @"\update");


            }
            catch (Exception ex)
            {
                Popup DeBloatError = new Popup(Popup.PopupWindowTypes.PopupOk, "The bloating process failed : " + ex.Message + " ( " + ex.ToString() + " )");
                DeBloatError.ShowDialog();
                return;
            }
            btn_DeBloat.IsEnabled = true;
            btn_Bloat.IsEnabled = false;
            Popup success = new Popup(Popup.PopupWindowTypes.PopupOk, "GTA was succesfully bloated.");
            success.ShowDialog();
            lbl_BloatState.Content = "Bloated";
            lbl_BloatState.Foreground = MyColors.MW_GTALabelUpgradedForeground;

        }

        private void btn_ModsAct_Click(object sender, EventArgs e)
        {

        }
        private void btn_ModsDeact_Click(object sender, EventArgs e)
        {

        }
    }
}
