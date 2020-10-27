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
using System.Windows.Resources;
using System.Windows.Shapes;
using Color = System.Drawing.Color;

namespace Project_127.Overlay.NoteOverlayPages
{
	/// <summary>
	/// Interaction logic for NoteOverlay_Looks.xaml
	/// </summary>
	public partial class NoteOverlay_Look : Page
	{
		public static FontFamily[] AllFonts = Fonts.SystemFontFamilies.ToArray();
		public static string SelectedFont = "Arial";
		public static GTAOverlay.Positions Position = GTAOverlay.Positions.TopLeft;

		public NoteOverlay_Look()
		{
			InitializeComponent();

			ComboBox_Fonts.ItemsSource = AllFonts;

			foreach (FontFamily myFF in AllFonts)
			{
				if (myFF.ToString() == NoteOverlay_Look.SelectedFont)
				{
					ComboBox_Fonts.SelectedItem = myFF;
				}
			}



			List<string> myEnumValues = new List<string>();
			foreach (string myString in Position.GetType().GetEnumNames())
			{
				myEnumValues.Add(myString);
			}

			string enumname = Position.GetType().ToString();
			ComboBox_OverlayLocation.ItemsSource = myEnumValues;
			ComboBox_OverlayLocation.SelectedItem = Position.ToString();

		}

		private void ComboBox_Fonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedFont = ((FontFamily)ComboBox_Fonts.SelectedItem).ToString();
		}

		private void ComboBox_OverlayLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Position = (GTAOverlay.Positions)System.Enum.Parse(typeof(GTAOverlay.Positions), ComboBox_OverlayLocation.SelectedItem.ToString());
		}


		private void btn_Toggle_Preview_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ColorPicker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
		{
			//Color asdf = (Color)new ColorConverter().ConvertFrom(ColorPicker_Background.SelectedColor);

		}

		private void ColorPicker_Foreground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
		{

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