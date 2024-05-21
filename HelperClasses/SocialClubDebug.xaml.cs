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
using System.Windows.Threading;

namespace Project_127.HelperClasses
{
    /// <summary>
    /// Interaction logic for SocialClubDebug.xaml
    /// </summary>
    public partial class SocialClubDebug : Window
    {
        public DispatcherTimer dp = new DispatcherTimer();

        /*

        AAAAAAAAAAAAAAAAAAAAAAA
        AAAAAAAAAAAAAAAAAAAAAAA
        AAAAAAAAAAAAAAAAAAAAAAA

        READ ME

        Added this window,
        added 25ms sleep on KillAllSocialClubProcesses, to actually close the file handle...
        changed steamSCL back to GTAVLauncher.exe in CommandLineArgs (REVERT !!!!!!)
        removed SocialClubUpgrade in GTAClosed(). Just commented out, if comment in remove popup

        added a lot of AddToDebug
        added rightclick upgrade/downgrade buttons to upgrade/downgrade social club
        added rightclick savefilehandler to debugprint addtodebug and new window

        */


        public SocialClubDebug()
        {
            InitializeComponent();
            dp = new System.Windows.Threading.DispatcherTimer();
            dp.Tick += new EventHandler(UpdateGUIDispatcherTimer);
            dp.Interval = TimeSpan.FromMilliseconds(500);
            dp.Start();
            UpdateGUIDispatcherTimer();
        }

        public void UpdateGUIDispatcherTimer(object sender = null, EventArgs e = null)
        {
            List<Button> buttons = new List<Button>
            {
                btn_IS_Downgraded,
                btn_IS_DowngradedCache,
                btn_IS_TempBackup,
                btn_IS_Installation
            };

            List<string> locations = new List<string>
            {
                LaunchAlternative.SCL_SC_DOWNGRADED,
                LaunchAlternative.SCL_SC_DOWNGRADED_CACHE,
                LaunchAlternative.SCL_SC_TEMP_BACKUP,
                LaunchAlternative.SCL_SC_Installation
            };


            for (int i  = 0; i <= buttons.Count -1; i++)
            {
                buttons[i].Content = LaunchAlternative.Get_SCL_InstallationState(locations[i]).ToString().ToUpper() + " | " + locations[i];
                if (buttons[i].Content.ToString().ToLower().StartsWith("upgraded")) { buttons[i].Foreground = new SolidColorBrush(Colors.BlueViolet); }
                else if (buttons[i].Content.ToString().ToLower().StartsWith("downgraded")) { buttons[i].Foreground = new SolidColorBrush(Colors.Green); }
                else if (buttons[i].Content.ToString().ToLower().StartsWith("trash")) { buttons[i].Foreground = new SolidColorBrush(Colors.Brown); }
                else { buttons[i].Foreground = new SolidColorBrush(Colors.DarkGray); }

            }

        }

        private void OpenFolder(string folder)
        {
            try 
            {
                if (!HelperClasses.FileHandling.doesPathExist(folder))
                {
                    folder = HelperClasses.FileHandling.GetParentFolder(folder);
                }
                HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: folder);
            }
            catch (Exception ex)
            {
                HelperClasses.FileHandling.AddToDebug("failed to open folder " + ex.ToString());
            }
        }

        private void btn_IS_Downgraded_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder(LaunchAlternative.SCL_SC_DOWNGRADED);
        }

        private void btn_IS_DowngradedCache_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder(LaunchAlternative.SCL_SC_DOWNGRADED_CACHE);
        }

        private void btn_IS_TempBackup_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder(LaunchAlternative.SCL_SC_TEMP_BACKUP);
        }

        private void btn_IS_Installation_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder(LaunchAlternative.SCL_SC_Installation);
        }

        private void btn_DowngradeSC_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                LaunchAlternative.SocialClubDowngrade(0,"Manually clicked from debug window");
            });
            UpdateGUIDispatcherTimer();
        }

        private void btn_UpgradeSC_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MW.Dispatcher.Invoke(() =>
            {
                LaunchAlternative.SocialClubUpgrade(0, "Manually clicked from debug window");

            });
            UpdateGUIDispatcherTimer();
        }
    }
}
