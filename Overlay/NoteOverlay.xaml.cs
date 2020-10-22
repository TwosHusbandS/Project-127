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


		public static string[] NotesLoaded = new string[0];
		public static int NotesLoadedIndex = 0;

		public static GTAOverlay MyGTAOverlay;

		public NoteOverlay()
		{
			// We currently need this here, normally this will be started by GameState (but this points to GTA V.exe as of right now)
			NoteOverlay.InitGTAOverlay();

			// We currently need this here, normally this will be started by WindowEventThingy (but this only starts or stops based on GTA V.exe)
			KeyboardListener.Start();

			// To get Back to real things, we gotta uncomment a few things in Globals.GameState Getter, 
			//	as well as WindowChangeListener.WinEventproc
			// as well as change target window to "Grand Theft Auto V" in GTAOverlay.cs
			// as well as actually processing the keyboard inputs in KeyboardHandler.cs

			InitializeComponent();

			btn_OverlayHotkeyToggle.Content = Settings.KeyOverlayToggle;
			btn_OverlayHotkeyScrollUp.Content = Settings.KeyOverlayScrollUp;
			btn_OverlayHotkeyScrollDown.Content = Settings.KeyOverlayScrollDown;
			btn_OverlayHotkeyScrollLeft.Content = Settings.KeyOverlayScrollLeft;
			btn_OverlayHotkeyScrollRight.Content = Settings.KeyOverlayScrollRight;
		}

		public static void InitGTAOverlay()
		{
			if (MyGTAOverlay == null)
			{
				MyGTAOverlay = new GTAOverlay();
				MyGTAOverlay.setTextColors(Color.FromArgb(255, 0, 255, 0), Color.Transparent);
				MyGTAOverlay.setBackgroundColor(Color.FromArgb(102, Color.Black));
				//MyGTAOverlay.setFont("consolas", 24, false, false, false);
				MyGTAOverlay.setText("TestingSlashN\nTesting2SlashNSlashN\n\nTesting3SlashNSlashNslashN\n\n\nTesting4");
			}
		}

		public static void DisposeGTAOverlay()
		{
			if (MyGTAOverlay != null)
			{
				MyGTAOverlay.Dispose();
				MyGTAOverlay = null;
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

		public static void OverlayToggle()
		{
			if (MyGTAOverlay.Visible)
			{
				MyGTAOverlay.Visible = false;
			}
			else
			{
				MyGTAOverlay.Visible = true;
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
			if (NotesLoadedIndex == NotesLoaded.Length - 1)
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
			if (pNotesLoadedNewIndex > 0 && pNotesLoadedNewIndex <= NotesLoaded.Length - 1)
			{
				HelperClasses.Logger.Log("NotesLoadedIndex is now: " + NotesLoadedIndex);
				NoteOverlay.MyGTAOverlay.setText(NotesLoaded[NotesLoadedIndex]);
			}
		}

		public static void OverlayNotePrev()
		{
			int NotesLoadedNewIndex = NotesLoadedIndex;
			if (NotesLoadedIndex == 0)
			{
				NotesLoadedNewIndex = NotesLoaded.Length - 1;
			}
			else
			{
				NotesLoadedNewIndex -= 1;
			}
			ChangeNoteIndex(NotesLoadedNewIndex);
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
