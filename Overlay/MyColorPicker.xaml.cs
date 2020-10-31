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

namespace Project_127
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
		/// Actual Value of the Selected Color
		/// </summary>
		private Color InternalSelectedColor;

		/// <summary>
		/// Internal Property for the SelectedColor Property
		/// </summary>
		private Color _SelectedColor
		{
			get
			{
				return InternalSelectedColor;
			}
			set
			{
				InternalSelectedColor = value;

				// Get a Brush, sets it as background of Button
				Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, InternalSelectedColor.R, InternalSelectedColor.G, InternalSelectedColor.B));
				double tmp = (double)((byte)InternalSelectedColor.A);
				brush.Opacity = tmp / 255;
				btn.Background = brush;

				// raises event which then can get used by the WPF Window / Page
				if (ColorChanged != null)
				{
					ColorChanged();
				}
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
				// Load this color in the cef window.
				// As in update the sliders / textboxes and most importantly the HSL square

				// "value" is of System.Drawing.Color
				// "value" contains 4 properties (A,R,G,B) which are all bytes

				// if doing that raises the "My_Browser_JavascriptMessageReceived" event, the line below can be commented out
				_SelectedColor = value;
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
				this._SelectedColor = MySettings.Settings.GetColorFromString(A + "," + R + "," + G + "," + B);
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

		/// <summary>
		/// Event when Popup is closed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void popup_Closed(object sender, EventArgs e)
		{
			// Raises Event
			if (Closed != null)
			{
				Closed();
			}
		}
	}
}
