﻿using System;
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

namespace Project_127.Popups
{
	/// <summary>
	/// Interaction logic for PopupMode.xaml
	/// </summary>
	public partial class PopupMode : Window
	{
		public static bool DirtyNoUpdate = false;

		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			if (MainWindow.MW.IsVisible)
			{
				this.Left = MainWindow.MW.Left + (MainWindow.MW.Width / 2) - (this.Width / 2);
				this.Top = MainWindow.MW.Top + (MainWindow.MW.Height / 2) - (this.Height / 2);
			}
		}


		/// <summary>
		/// Constructor of our Mode Popup
		/// </summary>
		public PopupMode()
		{
			InitializeComponent();

			if (MainWindow.MW.IsVisible)
			{
				this.Owner = MainWindow.MW;
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			this.lbl_CurrMode.Content = MySettings.Settings.P127Mode;
			this.tb_Main.Text = MySettings.Settings.P127Mode;

			this.lbl_CurrMode_DM.Content = MySettings.Settings.DMMode;
			this.tb_Main_DM.Text = MySettings.Settings.DMMode;
		}

		/// <summary>
		/// Changing Mode if wanted
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SetNew_Click(object sender, RoutedEventArgs e)
		{
			SetNew();
		}

		private async void SetNew()
		{
            Border_Loading.Visibility = Visibility.Visible;

            string mode1 = tb_Main.Text;
            string mode2 = tb_Main_DM.Text;
            string URL1 = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/" + mode1.ToLower() + "/Installer/Update.xml";
            string URL2 = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/" + mode2.ToLower() + "/Installer/DownloadManager.xml";
			Task<string> Task1 = HelperClasses.FileHandling.GetStringFromURL(URL1, true);
			Task<string> Task2 = HelperClasses.FileHandling.GetStringFromURL(URL2, true);
			string Github1 = await Task1;
			string Github2 = await Task2;

            if ((mode1 != "default" && String.IsNullOrWhiteSpace(Github1)) ||
                (mode2 != "default" && String.IsNullOrWhiteSpace(Github2)))
            {
                bool yesno = PopupWrapper.PopupYesNo("Cant find that version online. Do you still want to set this Mode?");
                if (yesno == false)
                {
                    Border_Loading.Visibility = Visibility.Hidden;
                    return;
                }
            }

			DirtyNoUpdate = true;
            MySettings.Settings.P127Mode = mode1;
            MySettings.Settings.DMMode = mode2;
			DirtyNoUpdate = false;

			await MySettings.Settings.CheckForUpdateClick(false);

			Border_Loading.Visibility = Visibility.Hidden;
        }

		/// <summary>
		/// Exit Click. Closes Window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Exit_Click(object sender, RoutedEventArgs e)
		{
			ComponentManager.MyRefreshStatic();
			this.Close();
		}

		/// <summary>
		/// Reset back to default Branch / Mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Reset_Click(object sender, RoutedEventArgs e)
		{
			MySettings.Settings.P127Mode = "default";
			MySettings.Settings.DMMode = "default";

			this.lbl_CurrMode.Content = MySettings.Settings.P127Mode;
			this.tb_Main.Text = MySettings.Settings.P127Mode;

			this.lbl_CurrMode_DM.Content = MySettings.Settings.DMMode;
			this.tb_Main_DM.Text = MySettings.Settings.DMMode;

			ComponentManager.MyRefreshStatic();
			this.Close();
		}

		/// <summary>
		/// Move around...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		/// <summary>
		/// Key Down on the Textbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				btn_SetNew_Click(null, null);
			}
		}

		/// <summary>
		/// Text of Textbox changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tb_Main_TextChanged(object sender, TextChangedEventArgs e)
		{
			//tb_Main.Text = tb_Main.Text.ToLower();
		}
	}
}
