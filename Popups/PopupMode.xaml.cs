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

namespace Project_127.Popups
{
	/// <summary>
	/// Interaction logic for PopupMode.xaml
	/// </summary>
	public partial class PopupMode : Window
	{


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
			string mode1 = tb_Main.Text.ToLower();
			string mode2 = tb_Main_DM.Text.ToLower();
			string URL1 = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/" + mode1 + "/Installer/Update.xml";
			string URL2 = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/" + mode2 + "/Installer/DownloadManager.xml";
			if ((mode1 != "default" && String.IsNullOrWhiteSpace(HelperClasses.FileHandling.GetStringFromURL(URL1, true))) ||
				(mode2 != "default" && String.IsNullOrWhiteSpace(HelperClasses.FileHandling.GetStringFromURL(URL2, true))))
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Cant find that version online. Do you still want to set this Mode?");
				yesno.ShowDialog();
				if (yesno.DialogResult == false)
				{
					return;
				}
			}
			MySettings.Settings.P127Mode = mode1;
			MySettings.Settings.DMMode = mode2;

			Globals.CheckForUpdate();
			ComponentManager.MyRefreshStatic();
			this.Close();
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
			tb_Main.Text = tb_Main.Text.ToLower();
		}
	}
}
