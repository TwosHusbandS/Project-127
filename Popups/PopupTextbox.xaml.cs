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
	/// Interaction logic for PopupTextbox.xaml
	/// </summary>
	public partial class PopupTextbox : Window
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
		/// Property of String we use to "return" things
		/// </summary>
		public string MyReturnString = "";


		/// <summary>
		/// Constructor of PopupTextbox
		/// </summary>
		/// <param name="Message"></param>
		/// <param name="TextBoxDefault"></param>
		/// <param name="pFontSize"></param>
		public PopupTextbox(string Message, string TextBoxDefault, int pFontSize = 18)
		{
			if (MainWindow.MW.IsVisible)
			{
				this.Owner = MainWindow.MW;
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			InitializeComponent();

			lbl_Main.Content = Message;
			lbl_Main.FontSize = pFontSize;

			tb_Main.Text = TextBoxDefault;

			tb_Main.Focus();
		}


		/// <summary>
		/// Click on "Yes". Sets DialogResult to "Yes" and closes itself.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Yes_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}


		/// <summary>
		/// Click on "No". Sets DialogResult to "No" and closes itself.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_No_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();

		}

		/// <summary>
		/// Event when the Text is changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox tmp = (TextBox)sender;
			if (tmp != null)
			{
				MyReturnString = tmp.Text;
			}
		}

		/// <summary>
		/// Event when a key is pressed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MyTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				btn_Yes_Click(null, null);
			}
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
