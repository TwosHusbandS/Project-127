using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Resources;
using System.Windows.Shapes;
using Project_127.MySettings;
using Color = System.Drawing.Color;

namespace Project_127.Overlay.NoteOverlayPages
{
	/// <summary>
	/// Interaction logic for NoteOverlay_Looks.xaml
	/// </summary>
	public partial class NoteOverlay_Look : Page
	{
		// the DropDowns / ComboBoxes are getting Writte and Read from Settings on getter / setter

		// Setting those 4 getters setters below all the time
		// and writing it to settings on close / mouseleftup events

		private static int _OverlayMarginX = Settings.OverlayMarginX;
		private static int _OverlayMarginY = Settings.OverlayMarginY;
		private static int _OverlayWidth = Settings.OverlayWidth;
		private static int _OverlayHeight = Settings.OverlayHeight;
		private static int _OverlayTextSize = Settings.OverlayTextSize;
		private static Color _OverlayBackground = Settings.OverlayBackground;
		private static Color _OverlayForeground = Settings.OverlayForeground;

		private static NoteOverlay_Look NO_L;

		public static void RefreshIfHideOrNot()
		{
			if (NO_L != null)
			{
				if (Settings.OverlayMultiMonitorMode)
				{
					NO_L.Rect_HideOptions.Visibility = Visibility.Visible;
				}
				else
				{
					NO_L.Rect_HideOptions.Visibility = Visibility.Hidden;
				}
			}
		}

		public static int OverlayMarginX
		{
			get
			{
				return _OverlayMarginX;
			}
			set
			{
				_OverlayMarginX = value;
				Overlay_Preview.OP.SetMarginX(OverlayMarginX);
			}
		}

		public static int OverlayMarginY
		{
			get
			{
				return _OverlayMarginY;
			}
			set
			{
				_OverlayMarginY = value;
				Overlay_Preview.OP.SetMarginY(OverlayMarginY);
			}
		}

		public static int OverlayWidth
		{
			get
			{
				return _OverlayWidth;
			}
			set
			{
				_OverlayWidth = value;
				Overlay_Preview.OP.SetWidth(OverlayWidth);
			}
		}

		public static int OverlayHeight
		{
			get
			{
				return _OverlayHeight;
			}
			set
			{
				_OverlayHeight = value;
				Overlay_Preview.OP.SetHeight(OverlayHeight);
			}
		}

		public static int OverlayTextSize
		{
			get
			{
				return _OverlayTextSize;
			}
			set
			{
				_OverlayTextSize = value;
				Overlay_Preview.OP.SetTextSize(OverlayTextSize);
			}
		}

		public static GTAOverlay.Positions OverlayLocation
		{
			get
			{
				return Settings.OverlayLocation;
			}
			set
			{
				Settings.OverlayLocation = value;
				Overlay_Preview.OP.SetLocation();
				if (NoteOverlay.IsOverlayInit())
				{
					NoteOverlay.MyGTAOverlay.Position = Settings.OverlayLocation;
				}
			}
		}

		public static string OverlayTextFont
		{
			get
			{
				return Settings.OverlayTextFont;
			}
			set
			{
				Settings.OverlayTextFont = value;
				Overlay_Preview.OP.SetFont();
				if (NoteOverlay.IsOverlayInit())
				{
					NoteOverlay.MyGTAOverlay.setFont(Settings.OverlayTextFont, Settings.OverlayTextSize);
				}
			}
		}

		public static Color OverlayBackground
		{
			get
			{
				return _OverlayBackground;
			}
			set
			{
				_OverlayBackground = value;
				Overlay_Preview.OP.SetBackground(OverlayBackground);
			}
		}

		public static Color OverlayForeground
		{
			get
			{
				return _OverlayForeground;
			}
			set
			{
				_OverlayForeground = value;
				Overlay_Preview.OP.SetForeground(OverlayForeground);
			}
		}



		public NoteOverlay_Look()
		{
			InitializeComponent();

			NO_L = this;

			_OverlayHeight = Settings.OverlayHeight;
			_OverlayMarginX = Settings.OverlayMarginX;
			_OverlayMarginY = Settings.OverlayMarginY;
			_OverlayWidth = Settings.OverlayWidth;
			_OverlayTextSize = Settings.OverlayTextSize;

			sl_Width.Value = OverlayWidth;
			sl_MarginX.Value = OverlayMarginX;
			sl_MarginY.Value = OverlayMarginY;
			sl_Height.Value = OverlayHeight;
			sl_TextSize.Value = OverlayTextSize;

			try { lbl_Width.Content = OverlayWidth.ToString() + " px"; } catch { }
			try { lbl_Height.Content = OverlayHeight.ToString() + " px"; } catch { }
			try { lbl_MarginX.Content = OverlayMarginX.ToString() + " px"; } catch { }
			try { lbl_MarginY.Content = OverlayMarginY.ToString() + " px"; } catch { }
			try { lbl_TextSize.Content = OverlayTextSize.ToString() + " pt"; } catch { }

			ComboBox_Fonts.ItemsSource = Fonts.SystemFontFamilies.ToArray();

			foreach (FontFamily myFF in ComboBox_Fonts.ItemsSource)
			{
				if (myFF.ToString() == NoteOverlay_Look.OverlayTextFont)
				{
					ComboBox_Fonts.SelectedItem = myFF;
				}
			}

			List<string> myEnumValues = new List<string>();
			foreach (string myString in OverlayLocation.GetType().GetEnumNames())
			{
				myEnumValues.Add(myString);
			}

			ComboBox_OverlayLocation.ItemsSource = myEnumValues;
			ComboBox_OverlayLocation.SelectedItem = NoteOverlay_Look.OverlayLocation.ToString();

			MyColorPicker_Background.SelectedColor = Settings.OverlayBackground;
			MyColorPicker_Foreground.SelectedColor = Settings.OverlayForeground;

			RefreshIfHideOrNot();
		}

		private void ComboBox_Fonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NoteOverlay_Look.OverlayTextFont = ((FontFamily)ComboBox_Fonts.SelectedItem).ToString();
		}

		private void ComboBox_OverlayLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NoteOverlay_Look.OverlayLocation = (GTAOverlay.Positions)System.Enum.Parse(typeof(GTAOverlay.Positions), ComboBox_OverlayLocation.SelectedItem.ToString());
		}

		private void sl_MarginX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			OverlayMarginX = (int)(((Slider)sender).Value);
			string myNewContent = OverlayMarginX.ToString() + " px";
			if (lbl_MarginX != null)
			{
				lbl_MarginX.Content = myNewContent;
			}
		}

		private void sl_MarginY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			OverlayMarginY = (int)(((Slider)sender).Value);
			string myNewContent = OverlayMarginY.ToString() + " px";
			if (lbl_MarginY != null)
			{
				lbl_MarginY.Content = myNewContent;
			}
		}

		private void sl_MarginX_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Settings.OverlayMarginX = OverlayMarginX;
			if (NoteOverlay.IsOverlayInit())
			{
				NoteOverlay.MyGTAOverlay.XMargin = Settings.OverlayMarginX;
			}
		}

		private void sl_MarginY_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Settings.OverlayMarginY = OverlayMarginY;
			if (NoteOverlay.IsOverlayInit())
			{
				NoteOverlay.MyGTAOverlay.YMargin = Settings.OverlayMarginY;
			}
		}

		private void sl_Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			OverlayWidth = (int)(((Slider)sender).Value);
			string myNewContent = OverlayWidth.ToString() + " px";
			if (lbl_Width != null)
			{
				lbl_Width.Content = myNewContent;
			}
		}

		private void sl_Width_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Settings.OverlayWidth = OverlayWidth;
			if (NoteOverlay.IsOverlayInit())
			{
				NoteOverlay.MyGTAOverlay.width = Settings.OverlayWidth;
			}
		}

		private void sl_Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			OverlayHeight = (int)(((Slider)sender).Value);
			string myNewContent = OverlayHeight.ToString() + " px";
			if (lbl_Height != null)
			{
				lbl_Height.Content = myNewContent;
			}
		}

		private void sl_Height_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Settings.OverlayHeight = OverlayHeight;
			if (NoteOverlay.IsOverlayInit())
			{
				NoteOverlay.MyGTAOverlay.height = Settings.OverlayHeight;
			}
		}

		private void sl_TextSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			OverlayTextSize = (int)(((Slider)sender).Value);
			string myNewContent = OverlayTextSize.ToString() + " pt";
			if (lbl_TextSize != null)
			{
				lbl_TextSize.Content = myNewContent;
			}
		}

		private void sl_TextSize_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Settings.OverlayTextSize = OverlayTextSize;
			if (NoteOverlay.IsOverlayInit())
			{
				NoteOverlay.MyGTAOverlay.setFont(Settings.OverlayTextFont, Settings.OverlayTextSize);
			}
		}

		private void MyColorPicker_Foreground_ColorChanged()
		{
			OverlayForeground = MyColorPicker_Foreground.SelectedColor;
		}

		private void MyColorPicker_Foreground_Closed()
		{
			Settings.OverlayForeground = OverlayForeground;
			if (NoteOverlay.IsOverlayInit())
			{
				NoteOverlay.MyGTAOverlay.setTextColors(Settings.OverlayForeground, Color.Transparent);
			}
		}

		private void MyColorPicker_Background_ColorChanged()
		{
			OverlayBackground = MyColorPicker_Background.SelectedColor;
		}

		private void MyColorPicker_Background_Closed()
		{
			Settings.OverlayBackground = OverlayBackground;
			if (NoteOverlay.IsOverlayInit())
			{
				NoteOverlay.MyGTAOverlay.setBackgroundColor(Settings.OverlayBackground);
			}
		}
	}


	public class PrintableFontFamilyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var fontFamily = value as FontFamily;

			if (fontFamily != null)
			{
				foreach (var typeface in fontFamily.GetTypefaces())
				{
					if (typeface.TryGetGlyphTypeface(out var glyphTypeface))
					{
						if (glyphTypeface.Symbol)
						{
							return null;
						}
					}
				}
			}

			return fontFamily;
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}
}