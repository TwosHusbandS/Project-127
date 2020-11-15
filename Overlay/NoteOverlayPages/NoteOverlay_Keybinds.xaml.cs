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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Project_127.HelperClasses;
using Keys = System.Windows.Forms.Keys;
using Project_127.MySettings;

namespace Project_127.Overlay.NoteOverlayPages
{
	/// <summary>
	/// Interaction logic for NoteOverlay_Keybinds.xaml
	/// </summary>
	public partial class NoteOverlay_Keybinds : Page
	{
		public NoteOverlay_Keybinds()
		{
			InitializeComponent();

			btn_OverlayHotkeyToggle.Content = Settings.KeyOverlayToggle;
			btn_OverlayHotkeyScrollUp.Content = Settings.KeyOverlayScrollUp;
			btn_OverlayHotkeyScrollDown.Content = Settings.KeyOverlayScrollDown;
			//btn_OverlayHotkeyScrollLeft.Content = Settings.KeyOverlayScrollLeft;
			//btn_OverlayHotkeyScrollRight.Content = Settings.KeyOverlayScrollRight;
			btn_OverlayHotkeyNotePrev.Content = Settings.KeyOverlayNotePrev;
			btn_OverlayHotkeyNoteNext.Content = Settings.KeyOverlayNoteNext;
		}

		private async void btn_OverlayHotkeyToggle_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key Now]";
			Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != Keys.None && MyNewKey != Keys.Escape)
			{
				Settings.KeyOverlayToggle = MyNewKey;
			}
			((Button)sender).Content = Settings.KeyOverlayToggle;
		}

		private async void btn_OverlayHotkeyScrollUp_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key Now]";
			Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != Keys.None && MyNewKey != Keys.Escape)
			{
				Settings.KeyOverlayScrollUp = MyNewKey;
			}
			((Button)sender).Content = Settings.KeyOverlayScrollUp;
		}

		private async void btn_OverlayHotkeyScrollDown_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key Now]";
			Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != Keys.None && MyNewKey != Keys.Escape)
			{
				Settings.KeyOverlayScrollDown = MyNewKey;
			}
			((Button)sender).Content = Settings.KeyOverlayScrollDown;
		}

		private async void btn_OverlayHotkeyScrollRight_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key Now]";
			Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != Keys.None && MyNewKey != Keys.Escape)
			{
				Settings.KeyOverlayScrollRight = MyNewKey;
			}
			((Button)sender).Content = Settings.KeyOverlayScrollRight;
		}

		private async void btn_OverlayHotkeyScrollLeft_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key Now]";
			Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != Keys.None && MyNewKey != Keys.Escape)
			{
				Settings.KeyOverlayScrollLeft = MyNewKey;
			}
			((Button)sender).Content = Settings.KeyOverlayScrollLeft;
		}

		private async void btn_OverlayHotkeyNotePrev_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key Now]";
			Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != Keys.None && MyNewKey != Keys.Escape)
			{
				Settings.KeyOverlayNotePrev = MyNewKey;
			}
			((Button)sender).Content = Settings.KeyOverlayNotePrev;
		}

		private async void btn_OverlayHotkeyNoteNext_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Content = "[Press new Key Now]";
			Keys MyNewKey = await KeyboardHandler.GetNextKeyPress();
			if (MyNewKey != Keys.None && MyNewKey != Keys.Escape)
			{
				Settings.KeyOverlayNoteNext = MyNewKey;
			}
			((Button)sender).Content = Settings.KeyOverlayNoteNext;
		}
	}
}
