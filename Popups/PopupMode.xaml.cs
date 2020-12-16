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
		public PopupMode()
		{
			InitializeComponent();

			this.lbl_CurrMode.Content = MySettings.Settings.Mode;
			this.tb_Main.Text = MySettings.Settings.Mode;
		}

		private void btn_SetNew_Click(object sender, RoutedEventArgs e)
		{
			string mode = tb_Main.Text.ToLower();
			string URL = "https://raw.githubusercontent.com/TwosHusbandS/Project-127/" + mode + "/Installer/Update.xml";
			if (String.IsNullOrWhiteSpace(HelperClasses.FileHandling.GetStringFromURL(URL, true)) && mode != "default")
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Cant find that version online. Do you still want to set this Mode?");
				yesno.ShowDialog();
				if (yesno.DialogResult == false)
				{
					return;
				}
			}
			MySettings.Settings.Mode = mode;
			new Popup(Popup.PopupWindowTypes.PopupOk, "Set Mode to: '" + mode + "'").ShowDialog();

			this.Close();
		}

		private void btn_Exit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void btn_Reset_Click(object sender, RoutedEventArgs e)
		{
			MySettings.Settings.Mode = "default";

			this.lbl_CurrMode.Content = MySettings.Settings.Mode;
			this.tb_Main.Text = MySettings.Settings.Mode;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void MyTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				btn_SetNew_Click(null, null);
			}
		}

		private void tb_Main_TextChanged(object sender, TextChangedEventArgs e)
		{
			tb_Main.Text = tb_Main.Text.ToLower();
		}
	}
}
