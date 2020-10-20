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

namespace Project_127.Overlay
{
	/// <summary>
	/// Interaction logic for NoteOverlay.xaml
	/// </summary>
	public partial class NoteOverlay : Page
	{
		public static NoteOverlay NO;

		public bool RunKeyboardListener = false;
		public bool RunKeyboardListenerSurpress = false;
		public bool RunWindowChangeListener = false;

		public NoteOverlay()
		{
			NoteOverlay.NO = this;
			InitializeComponent();
			//KeyboardListener.Start();
			//WindowChangeListener.Start();
		}




		public static void KeyBoardEvent(Keys pKey)
		{
			//if (pKey == Keys.Insert)
			//{
			//	KeyboardListener.Stop();
			//}
			//else
			//{
				try
				{
					NoteOverlay.NO.Dispatcher.Invoke((Action)delegate
					{
						NoteOverlay.NO.lbl_Latest_Keypress.Content = pKey.ToString();
						//NoteOverlay.NO.btn_Tmp.Content = pKey.ToString();
					});
				}
				catch (Exception e)
				{
					Globals.DebugPopup(e.ToString());
				}
			//}
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

		private void cb_EnableKeyboardListener_Click(object sender, RoutedEventArgs e)
		{
			if (RunKeyboardListener == false)
			{
				RunKeyboardListener = true;
				KeyboardListener.Start();
			}
			else
			{
				RunKeyboardListener = false;
				KeyboardListener.Stop();
			}
			((System.Windows.Controls.CheckBox)sender).IsChecked = RunKeyboardListener;
		}

		private void cb_EnableKeyboardListenerSurpress_Click(object sender, RoutedEventArgs e)
		{
			if (RunKeyboardListenerSurpress == false)
			{
				RunKeyboardListenerSurpress = true;
			}
			else
			{
				RunKeyboardListenerSurpress = false;
			}
			((System.Windows.Controls.CheckBox)sender).IsChecked = RunKeyboardListenerSurpress;
		}

		private void cb_EnableWindowChangeListener_Click(object sender, RoutedEventArgs e)
		{
			if (RunWindowChangeListener == false)
			{
				RunWindowChangeListener = true;
				WindowChangeListener.Start();
			}
			else
			{
				RunWindowChangeListener = false;
				WindowChangeListener.Stop();
			}
			((System.Windows.Controls.CheckBox)sender).IsChecked = RunWindowChangeListener;
		}
	}
}
