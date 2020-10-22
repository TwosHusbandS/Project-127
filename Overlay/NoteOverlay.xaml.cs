using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Project_127.HelperClasses;
using Project_127.SettingsStuff;

namespace Project_127.Overlay
{
	/// <summary>
	/// Interaction logic for NoteOverlay.xaml
	/// </summary>
	public partial class NoteOverlay : Page
	{
		public static NoteOverlay NO;

		public NoteOverlay()
		{
			NoteOverlay.NO = this;
			InitializeComponent();

			btn_OverlayHotkeyToggle.Content = Settings.KeyOverlayToggle;
			btn_OverlayHotkeyScrollUp.Content = Settings.KeyOverlayScrollUp;
			btn_OverlayHotkeyScrollDown.Content = Settings.KeyOverlayScrollDown;
		}

		public static void KeyBoardEvent(Keys pKey)
		{
			if (pKey == Settings.KeyOverlayToggle)
			{
				OverlayToggle();
			}
			else if (pKey == Settings.KeyOverlayScrollUp)
			{
				OverlayScrollUp();
			}
			else if (pKey == Settings.KeyOverlayScrollDown)
			{
				OverlayScrollDown();
			}
		}

		public static void OverlayToggle()
		{

		}

		public static void OverlayScrollUp()
		{

		}

		public static void OverlayScrollDown()
		{

		}

		private void btn_NoteFile_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_OverlayHotkeyToggle_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void btn_OverlayHotkeyScrollUp_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_OverlayHotkeyScrollDown_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
