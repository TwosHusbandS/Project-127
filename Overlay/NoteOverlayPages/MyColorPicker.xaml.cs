using CefSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using Color = System.Drawing.Color;

namespace Project_127.Overlay.NoteOverlayPages
{
	/// <summary>
	/// Interaction logic for MyColorPicker.xaml
	/// </summary>
	public partial class MyColorPicker : UserControl
	{
		/*
		
			Custom Control Colorpicker.
			Uses cef Framework.
			Uses the HTML / JS / CSS Colorpicker by @dr490n / jaredtb

		*/

		/// <summary>
		/// Devegate Method for ColorChanged Event (updated in real time on each click)
		/// </summary>
		public delegate void ColorChangedHandler();

		/// <summary>
		/// Delegate Method for Closed Event (called when Colorpicker is closed)
		/// </summary>
		public delegate void ClosedHandler();

		/// <summary>
		/// Exposing ColorChanged Event publically
		/// </summary>
		public event ColorChangedHandler ColorChanged;

		/// <summary>
		/// Exposing Closed Event publically
		/// </summary>
		public event ClosedHandler Closed;

		/// <summary>
		/// Internal Property for the SelectedColor Property
		/// </summary>
		private Color _SelectedColor;

		/// <summary>
		/// Property exposed which returns the SelectedColor as Brush and can get set from the Window / Page using this CustomControl
		/// </summary>
		private Brush SelectedColorBrush
		{
			get
			{
				Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, SelectedColor.R, SelectedColor.G, SelectedColor.B));
				brush.Opacity = (double)((byte)SelectedColor.A) / 255;
				return brush;
			}
		}


		/// <summary>
		/// Property exposed which returns the SelectedColor and can get set from the Window / Page using this CustomControl
		/// </summary>
		public Color SelectedColor
		{
			get
			{
				return _SelectedColor;
			}
			set
			{
				_SelectedColor = value;

				btn.Background = SelectedColorBrush;

				// raises event which then can get used by the WPF Window / Page
				if (ColorChanged != null)
				{
					ColorChanged();
				}
			}
		}


		/// <summary>
		/// Constructor of Custom Control
		/// </summary>
		public MyColorPicker()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Event when a JavaScript Method is received
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void My_Browser_JavascriptMessageReceived(object sender, CefSharp.JavascriptMessageReceivedEventArgs e)
		{
			this.Dispatcher.Invoke((Action)delegate
			{
				// Calculating RGBA based on the javascript method
				var message = e.Message.ToString();
				message = message.Substring(1, message.Length - 2);
				var vals = message.Split(',');
				int R = int.Parse(vals[0]);
				int G = int.Parse(vals[1]);
				int B = int.Parse(vals[2]);
				int A = (int)(Convert.ToDouble(vals[3], CultureInfo.InvariantCulture) * 255);

				// Sets the Property
				this.SelectedColor = MySettings.Settings.GetColorFromString(A + "," + R + "," + G + "," + B);
			});
		}

		/// <summary>
		/// Click of the actual Button of the UserControl which opens the Popup with the cef window with the colorpicker
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Click(object sender, RoutedEventArgs e)
		{
			popup.IsOpen = true;
		}

		public void setColor(int r, int g, int b, double a)
		{
			string sender = "document.getElementById('RCS').value = {0};document.getElementById('GCS').value = {1};document.getElementById('BCS').value = {2};document.getElementById('ACS').value = {3};document.getElementById('RC').value = {0};document.getElementById('GC').value = {1};document.getElementById('BC').value = {2};document.getElementById('AC').value = {3};rgba2hsla();";
			My_Browser.GetMainFrame().ExecuteJavaScriptAsync(String.Format(sender, r.ToString(), g.ToString(), b.ToString(), a.ToString()));
		}

		private void popup_Closed(object sender, EventArgs e)
		{
			if (Closed != null)
			{
				Closed();
			}
		}

		private async void popup_Opened(object sender, EventArgs e)
		{
			await Task.Delay(200);
			if (My_Browser.IsInitialized)
			{
				setColor(SelectedColor.R, SelectedColor.G, SelectedColor.B, (double)((byte)SelectedColor.A));
			}
		}
	}
}
