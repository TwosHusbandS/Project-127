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
			Task.Run(() => KeyboardListener.Start());
		}




		public static void KeyBoardEvent(Keys pKey)
		{
			if (pKey == Keys.Insert)
			{
				KeyboardListener.Stop();
			}
			else
			{
				NoteOverlay.NO.Dispatcher.Invoke((Action)delegate
				{
					NoteOverlay.NO.btn_Tmp.Content = pKey.ToString();
				});
			}
		}

		private void btn_Tmp_Click(object sender, RoutedEventArgs e)
		{
			if (!KeyboardListener.IsRunning)
			{
				Globals.DebugPopup("Not running already, so ill start");
				Task.Run(() => KeyboardListener.Start());
			}
			else
			{
				Globals.DebugPopup("Running already, so ill NOT start");
			}
		}
	}
}
