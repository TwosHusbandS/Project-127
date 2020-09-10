/*
 
Main Documentation:
Implemented by @thS#0305. 2020-09-08-19:20 CET

Version: 0.0.1 unreleased, not working, not fully implemented.

Proof of Concept Implementation of potential own Launcher.

Probably needs some work and re-implementation. 

We are taking some shortcuts here, like requiring admin acces,so we dont have to deal with file or regedit permissions..(app.manifest lines 20 and 21 btw)

Still needs the actual creative way of Launching and the DRM.

Comments like "TODO", "TO DO", "CTRLF", "CTRL-F", and "CTRL F" are just ways of finding a specific line quickly via searching

Hybrid code can be found in AAA_HybridCode.

Nothing. I repeat: Literally NOTHING. Has been tested.

General Files / Classes:
    MainWindow.xaml.cs
    Settings.xaml.cs
    Globals.cs
    HelperClasses\FilerHandling.cs
    HelperClasses\RegeditHandler.cs
    HelperClasses\Logging.cs

*/


using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode)]
        public static extern int CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        /// <summary>
        /// Constructor of main Window
        /// </summary>
        public MainWindow()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File($"{LauncherLogic.Settings.Get("FilesFolder")}\\Logs\\RollSun-Launcher.log", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                Log.Information("Starting up.");

                if (Environment.GetCommandLineArgs().Length > 2 && Environment.GetCommandLineArgs()[1].Equals("mcdonalds"))
                {
                    Log.Information("Ordering mcdonalds.");
                    MessageBox.Show("using card number 1, cvv 274, expiration 06/23 to order", "ordering mcdonalds");
                }

                LauncherLogic.Init();

                InitializeComponent();
                Log.Information("Initialized UI.");
            } catch (Exception e)
            {
                MessageBox.Show(e.StackTrace, e.Message);
            }
            
        }



        #region UI functionalities
        /// <summary>
        /// Yeah...will not comment these things below yet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings
            {
                Owner = this
            };

            settingsWindow.Show();
        }

        private void btn_Downgrade_Click(object sender, RoutedEventArgs e)
        {
            btn_Settings.IsEnabled = false;
            btn_Downgrade.IsEnabled = false;
            btn_Upgrade.IsEnabled = false;
            btn_LaunchGame.IsEnabled = false;

            LauncherLogic.Downgrade();

            btn_Settings.IsEnabled = true;
            btn_Downgrade.IsEnabled = true;
            btn_Upgrade.IsEnabled = true;
            btn_LaunchGame.IsEnabled = true;
        }

        private void btn_Upgrade_Click(object sender, RoutedEventArgs e)
        {
            btn_Settings.IsEnabled = false;
            btn_Downgrade.IsEnabled = false;
            btn_Upgrade.IsEnabled = false;
            btn_LaunchGame.IsEnabled = false;

            LauncherLogic.Upgrade();

            btn_Settings.IsEnabled = true;
            btn_Downgrade.IsEnabled = true;
            btn_Upgrade.IsEnabled = true;
            btn_LaunchGame.IsEnabled = true;
        }

        private void btn_LaunchGame_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
