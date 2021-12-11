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
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;

namespace Project_127.Popups
{
	/// <summary>
	/// Interaction logic for PopupCombobox.xaml
	/// </summary>
	public partial class PopupCombobox : Window
	{
		/// <summary>
		/// Property which has the toString of the ComboBox selected Item to "return" it
		/// </summary>
		public string MyReturnString;


		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			if (MainWindow.MW.IsVisible)
			{
				this.Left = MainWindow.MW.Left + (MainWindow.MW.Width / 2) - (this.Width / 2);
				this.Top = MainWindow.MW.Top + (MainWindow.MW.Height / 2) - (this.Height / 2);
			}
		}
			

		/// <summary>
		/// Constructor of Popup Combobox
		/// </summary>
		/// <param name="asdf"></param>
		/// <param name="pEnum"></param>
		public PopupCombobox(string pMsg, Enum pEnum, int pFontSize = 18)
		{
			if (MainWindow.MW.IsVisible)
			{
				this.Owner = MainWindow.MW;
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			// Initializing all WPF Elements
			InitializeComponent();

			// Set the Parameters as Properties of new Popup Window
			lbl_Main.FontSize = pFontSize;
			lbl_Main.Content = pMsg;

			if (pEnum != null)
			{
				List<string> myEnumValues = new List<string>();
				foreach (string myString in pEnum.GetType().GetEnumNames())
				{
					myEnumValues.Add(myString);
				}

				string enumname = pEnum.GetType().ToString();

				cb_Main.ItemsSource = myEnumValues;

				if (enumname.Contains("Retailer"))
				{
					cb_Main.SelectedItem = Settings.Retailer.ToString();
				}
				else if (enumname.Contains("Language"))
				{
					cb_Main.SelectedItem = Settings.LanguageSelected.ToString();
				}

				MyReturnString = cb_Main.SelectedItem.ToString();
			}

		}

		/// <summary>
		/// Selection Changed Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox tmp = (ComboBox)sender;
			if (tmp != null)
			{
				MyReturnString = tmp.SelectedItem.ToString();
			}
		}



		/// <summary>
		/// Click on "OK". Closes itself.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Ok_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true; // probably not needed...
			this.Close();
		}


		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}
	}
}
