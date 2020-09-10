/*
 
Main Documentation:
Implemented by @thS#0305 and @zCri#5552

Version: 0.0.1 unreleased, not working, not fully implemented.


Probably needs some work and re-implementation. 

We are taking some shortcuts here, like requiring admin acces,so we dont have to deal with file or regedit permissions..(app.manifest lines 20 and 21 btw)

Still needs the actual creative way of Launching and the DRM.

Comments like "TODO", "TO DO", "CTRLF", "CTRL-F", and "CTRL F" are just ways of finding a specific line quickly via searching

Hybrid code can be found in AAA_HybridCode.

Nothing. I repeat: Literally NOTHING. Has been tested.

General Files / Classes:
    MainWindow.xaml.cs
    Settings.xaml.cs
    Popup.xaml.cs
    SaveFileModder.xaml.cs
    
Main To do:
    Finish implementing MainWindow.xaml (mainly WPF Styles)
        Think about look of GTAV Button depending on game state.
    Finish Moving Colors from GUI.cs to MainWindow.xaml.cs
    Use MainWindow.xaml styles as a baseline for other WIndows. (Settings, Popup, SaveFileModder)
    Talk to zCri about behavior of update / downgrade button

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
        public static Brush MyColorWhite { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#ffffff");
        public static Brush MyColorBlack { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#000000");
        public static Brush MyColor1 { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#006ec7");
        public static Brush MyColor2 { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#004177");

        /// <summary>
        /// DebugPopup Method. Just opens Messagebox with pMsg
        /// </summary>
        /// <param name="pMsg"></param>
        public static void DebugPopup(string pMsg)
        {
            System.Windows.Forms.MessageBox.Show(pMsg);
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btn_Hamburger_Click(object sender, RoutedEventArgs e)
        {
            if (this.GridHamburger.Visibility == Visibility.Visible)
            {
                this.GridHamburger.Visibility = Visibility.Hidden;
            }
            else
            {
                this.GridHamburger.Visibility = Visibility.Visible;
            }
        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you really want to exit?");
            yesno.ShowDialog();
            if (yesno.DialogResult == true)
            {
                Environment.Exit(0);
            }
        }

        private void btn_Upgrade_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Downgrade_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_SaveFiles_Click(object sender, RoutedEventArgs e)
        {
            (new SaveFileHandler()).Show();
        }

        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            (new Settings()).Show();
        }

        private void btn_Readme_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Test";
            msg += "\nTestLine2";
            msg += "\nTestLine3";

            new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
        }

        private void btn_GTA_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
