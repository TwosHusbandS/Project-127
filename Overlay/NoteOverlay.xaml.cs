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
		// Open the NoteOverlayShit, Click Top Button, select 1-n TEXT Files. everything past that should be based on KeyInputs


		public static NoteOverlay NO;
		public static string[] NotesLoaded = new string[0];
		public static int NotesLoadedIndex = 0;

		public static GTAOverlay MyGTAOverlay;

		public NoteOverlay()
		{
			NoteOverlay.NO = this;

			// We currently need this here, normally this will be started by GameState (but this points to GTA V.exe as of right now)
			NoteOverlay.InitGTAOverlay();

			// We currently need this here, normally this will be started by WindowEventThingy (but this only starts or stops based on GTA V.exe)
			KeyboardListener.Start();


			InitializeComponent();

			btn_OverlayHotkeyToggle.Content = Settings.KeyOverlayToggle;
			btn_OverlayHotkeyScrollUp.Content = Settings.KeyOverlayScrollUp;
			btn_OverlayHotkeyScrollDown.Content = Settings.KeyOverlayScrollDown;
			btn_OverlayHotkeyScrollLeft.Content = Settings.KeyOverlayScrollLeft;
			btn_OverlayHotkeyScrollRight.Content = Settings.KeyOverlayScrollRight;
		}

		public static void InitGTAOverlay()
		{
			MyGTAOverlay = new GTAOverlay();
			MyGTAOverlay.setText("");
		}

		public static void DisposeGTAOverlay()
		{
			MyGTAOverlay.Dispose();
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
				HelperClasses.Logger.Log("Set Visibility to false");
				MyGTAOverlay.Visible = false;
			}
			else
			{
				HelperClasses.Logger.Log("Set Visibility to true");
				MyGTAOverlay.Visible = true;
			}

		}

		public static void OverlayScrollUp()
		{

		}

		public static void OverlayScrollDown()
		{

		}

		public static void OverlayNoteNext()
		{
			int LengthOfNotesLoaded = NotesLoaded.Length;
			if (NotesLoadedIndex == LengthOfNotesLoaded - 1)
			{
				NotesLoadedIndex = 0;
			}
			else
			{
				NotesLoadedIndex += 1;
			}
			HelperClasses.Logger.Log("NotesLoadedIndex is now: " + NotesLoadedIndex);
			NoteOverlay.MyGTAOverlay.setText(NotesLoaded[NotesLoadedIndex]);
		}

		public static void OverlayNotePrev()
		{
			int LengthOfNotesLoaded = NotesLoaded.Length;
			if (NotesLoadedIndex == 0)
			{
				NotesLoadedIndex = NotesLoaded.Length - 1;
			}
			else
			{
				NotesLoadedIndex -= 1;
			}
			HelperClasses.Logger.Log("NotesLoadedIndex is now: " + NotesLoadedIndex);
			NoteOverlay.MyGTAOverlay.setText(NotesLoaded[NotesLoadedIndex]);
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

		private void btn_OverlayHotkeyToggle_Click(object sender, RoutedEventArgs e)
		{
			// Methods to change the hotkey for this. Will just be here for POC Implementation of that	
		}

		private void btn_OverlayHotkeyScrollUp_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_OverlayHotkeyScrollDown_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_OverlayHotkeyScrollRight_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_OverlayHotkeyScrollLeft_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
