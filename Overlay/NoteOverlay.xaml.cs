using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Keys = System.Windows.Forms.Keys;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Project_127.HelperClasses;
using Project_127.SettingsStuff;
using System.Diagnostics;

namespace Project_127.Overlay
{
	/// <summary>
	/// Interaction logic for NoteOverlay.xaml
	/// </summary>
	public partial class NoteOverlay : Page
	{
		// Open the NoteOverlayShit, Click Top Button, select 1-n TEXT Files. everything past that should be based on KeyInputs

		// https://github.com/crclayton/WPF-DataBinding-Example/tree/master/WpfApplication2

		// https://stackoverflow.com/a/47582420

		public static string[] NotesLoaded = { "Test-Note to show what it can do\nThis is in the next line\n\nThis is in the next Paragraph" };
		public static int NotesLoadedIndex = 0;

		public static GTAOverlay MyGTAOverlay;

		public NoteOverlay()
		{
			InitializeComponent();

			btn_OverlayHotkeyToggle.Content = Settings.KeyOverlayToggle;
			btn_OverlayHotkeyScrollUp.Content = Settings.KeyOverlayScrollUp;
			btn_OverlayHotkeyScrollDown.Content = Settings.KeyOverlayScrollDown;
			btn_OverlayHotkeyScrollLeft.Content = Settings.KeyOverlayScrollLeft;
			btn_OverlayHotkeyScrollRight.Content = Settings.KeyOverlayScrollRight;
		}

		public static void InitGTAOverlay()
		{
			//HelperClasses.Logger.Log("Trying to Init GTA Overlay");
			if (MyGTAOverlay == null)
			{
				MyGTAOverlay = new GTAOverlay();
				MyGTAOverlay.setTextColors(Color.FromArgb(255, 0, 255, 0), Color.Transparent);
				MyGTAOverlay.setBackgroundColor(Color.FromArgb(102, Color.Black));
				//MyGTAOverlay.setFont("consolas", 24, false, false, false);
				MyGTAOverlay.setText(NotesLoaded[0]);
				NotesLoadedIndex = 0;
				HelperClasses.Logger.Log("GTA Overlay initiated", 1);
			}
			else
			{
				//HelperClasses.Logger.Log("GTA Overlay already initiated", 1);
			}
		}

		public static void DisposeGTAOverlay()
		{
			//HelperClasses.Logger.Log("Trying to Dispose GTA Overlay");
			if (MyGTAOverlay != null)
			{
				MyGTAOverlay.Dispose();
				MyGTAOverlay = null;
				HelperClasses.Logger.Log("GTA Overlay disposed", 1);
			}
			else
			{
				//HelperClasses.Logger.Log("GTA Overlay already disposed", 1);
			}
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
			else if (pKey == Settings.KeyOverlayScrollRight)
			{
				OverlayNoteNext();
			}
			else if (pKey == Settings.KeyOverlayScrollLeft)
			{
				OverlayNotePrev();
			}
		}

		public static void OverlaySetVisible()
		{
			if (IsOverlayInit())
			{
				MyGTAOverlay.Visible = true;
			}
		}

		public static void OverlaySetInvisible()
		{
			if (IsOverlayInit())
			{
				MyGTAOverlay.Visible = false;
			}
		}

		public static bool IsOverlayInit()
		{
			if (MyGTAOverlay == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool IsOverlayVisible()
		{
			if (IsOverlayInit())
			{
				return MyGTAOverlay.Visible;
			}
			else
			{
				return false;
			}
		}

		public static void OverlayToggle()
		{
			if (IsOverlayVisible())
			{
				OverlaySetInvisible();
			}
			else
			{
				OverlaySetVisible();
			}
		}

		public static void OverlayScrollUp()
		{
			MyGTAOverlay.scroll(-5);
			//HelperClasses.Logger.Log("About to scroll Up a bit...JK, this aint implemented yet");
		}

		public static void OverlayScrollDown()
		{
			MyGTAOverlay.scroll(5);
			//HelperClasses.Logger.Log("About to scroll Down a bit...JK, this aint implemented yet");
		}

		public static void OverlayNoteNext()
		{
			int NotesLoadedNewIndex = NotesLoadedIndex;
			if (NotesLoadedNewIndex == NotesLoaded.Length - 1)
			{
				NotesLoadedNewIndex = 0;
			}
			else
			{
				NotesLoadedNewIndex += 1;
			}
			ChangeNoteIndex(NotesLoadedNewIndex);
		}

		public static void ChangeNoteIndex(int pNotesLoadedNewIndex)
		{
			if (pNotesLoadedNewIndex >= 0 && pNotesLoadedNewIndex <= NotesLoaded.Length - 1)
			{
				HelperClasses.Logger.Log("NotesLoadedIndex is now: " + pNotesLoadedNewIndex);
				NotesLoadedIndex = pNotesLoadedNewIndex;
				NoteOverlay.MyGTAOverlay.setText(NotesLoaded[pNotesLoadedNewIndex]);
			}
		}

		public static void OverlayNotePrev()
		{
			int NotesLoadedNewIndex = NotesLoadedIndex;
			if (NotesLoadedNewIndex == 0)
			{
				NotesLoadedNewIndex = NotesLoaded.Length - 1;
			}
			else
			{
				NotesLoadedNewIndex -= 1;
			}
			ChangeNoteIndex(NotesLoadedNewIndex);
		}

		public static void OverlayNoteChapterNext()
		{

		}

		public static void OverlayNoteChapterPrev()
		{

		}

		private void btn_NoteFile_Click(object sender, RoutedEventArgs e)
		{
			string SelectedFiles = HelperClasses.FileHandling.OpenDialogExplorer(FileHandling.PathDialogType.File, "Select the Notes Files you want to load.", LauncherLogic.ZIPFilePath, true, "TEXT Files|*.txt*");
			string[] AllFiles = SelectedFiles.Split(',');
			string[] NotesLoading = new string[AllFiles.Length];

			for (int i = 0; i <= AllFiles.Length - 1; i++)
			{
				string[] MyFileContents = HelperClasses.FileHandling.ReadFile(AllFiles[i]).ToArray();
				string tempFileContent = "";
				for (int j = 0; j <= MyFileContents.Count() - 1; j++)
				{
					// This leaves a \n at the end of the file...this is wanted behaviour tho
					tempFileContent += MyFileContents[j] + "\n";
				}
				NotesLoading[i] = tempFileContent;
			}

			NoteOverlay.NotesLoaded = NotesLoading;
			NoteOverlay.NotesLoadedIndex = 0;
			NoteOverlay.MyGTAOverlay.setText(NotesLoaded[0]);
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
	}
}
