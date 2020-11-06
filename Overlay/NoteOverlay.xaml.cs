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
using Project_127.MySettings;
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

		public static string[] NotesLoaded = { "" };
		public static string[] NotesLoadedTitle = { "" };
		public static int NotesLoadedIndex = 0;

		public static NoteOverlayPages LoadNoteOverlayWithCustomPage = NoteOverlayPages.NoteFiles;

		public static bool OverlayWasVisible = false;

		public enum NoteOverlayPages
		{
			NoteFiles,
			Keybinds,
			Look
		}

		private NoteOverlayPages _NoteOverlayPage = NoteOverlayPages.NoteFiles;

		public NoteOverlayPages NoteOverlayPage
		{
			get
			{
				return _NoteOverlayPage;
			}
			set
			{
				// Setting actual Enum to the correct Value
				_NoteOverlayPage = value;

				// Switch Value
				switch (value)
				{
					// In Case: Settings
					case NoteOverlayPages.NoteFiles:
						MainWindow.MW.Width = 900;
						MainWindow.MW.Grid_Preview.Visibility = Visibility.Hidden;
						Overlay_Preview.StopDispatcherTimer();

						// Set actual Frame_Main Content to the correct Page
						Frame_Notes.Content = new Project_127.Overlay.NoteOverlayPages.NoteOverlay_NoteFiles();
						btn_Notes.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						// Call Mouse_Over false on other Buttons where a page is behind
						btn_Looks.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						btn_Keybindings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case NoteOverlayPages.Keybinds:
						MainWindow.MW.Width = 900;
						MainWindow.MW.Grid_Preview.Visibility = Visibility.Hidden;
						Overlay_Preview.StopDispatcherTimer();

						// Set actual Frame_Main Content to the correct Page
						Frame_Notes.Content = new Project_127.Overlay.NoteOverlayPages.NoteOverlay_Keybinds();
						btn_Keybindings.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						// Call Mouse_Over false on other Buttons where a page is behind
						btn_Looks.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						btn_Notes.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case NoteOverlayPages.Look:
						MainWindow.MW.Width = 1600;
						MainWindow.MW.Grid_Preview.Visibility = Visibility.Visible;
						Overlay_Preview.StartDispatcherTimer();

						// Set actual Frame_Main Content to the correct Page
						Frame_Notes.Content = new Project_127.Overlay.NoteOverlayPages.NoteOverlay_Look();
						btn_Looks.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						// Call Mouse_Over false on other Buttons where a page is behind
						btn_Notes.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						btn_Keybindings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
				}
			}
		}

		public static GTAOverlay MyGTAOverlay;

		public NoteOverlay()
		{
			InitializeComponent();

			NoteOverlayPage = LoadNoteOverlayWithCustomPage;
			LoadNoteOverlayWithCustomPage = NoteOverlayPages.NoteFiles;

			ButtonMouseOverMagic(btn_cb_Set_EnableOverlay);

		}

		public static void InitGTAOverlay()
		{
			//HelperClasses.Logger.Log("Trying to Init GTA Overlay");
			if (MyGTAOverlay == null)
			{
				// Set NotesLoaded here based on the
				MyGTAOverlay = new GTAOverlay();
				MyGTAOverlay.setTextColors(Settings.OverlayForeground, Color.Transparent);
				MyGTAOverlay.setBackgroundColor(Settings.OverlayBackground);
				MyGTAOverlay.setFont(Settings.OverlayTextFont, Settings.OverlayTextSize, false, false, false);
				MyGTAOverlay.Position = Settings.OverlayLocation;
				MyGTAOverlay.XMargin = Settings.OverlayMargin;
				MyGTAOverlay.YMargin = Settings.OverlayMargin;
				MyGTAOverlay.width = Settings.OverlayWidth;
				MyGTAOverlay.height = Settings.OverlayHeight;
				LoadTexts();
				NotesLoadedIndex = 0;
				HelperClasses.Logger.Log("GTA Overlay initiated", 1);
			}
			else
			{
				//HelperClasses.Logger.Log("GTA Overlay already initiated", 1);
			}
		}


		public static void LoadTexts()
		{
			List<string> NotesTexts = new List<string>();
			List<string> NotesTextsTitles = new List<string>();

			for (int i = 0; i <= Settings.OverlayNotesMain.Count - 1; i++)
			{
				Overlay.NoteOverlayPages.MyNoteFile tmp = new Overlay.NoteOverlayPages.MyNoteFile(Settings.OverlayNotesMain[i]);

				string[] contenta = HelperClasses.FileHandling.ReadFileEachLine(tmp.FilePath);
				string contents = "";


				for (int j = 0; j <= contenta.Length - 1; j++)
				{
					contents += contenta[j];
					if (j != contenta.Length - 1)
					{
						contents += "\n";
					}
				}

				if (String.IsNullOrWhiteSpace(contents))
				{
					contents = "Note - File could not be read. File doesnt exist or is empty";
				}

				NotesTexts.Add(contents);
				NotesTextsTitles.Add("Project 1.27 - Overlay - " + tmp.FileNiceName);
			}

			NotesLoaded = NotesTexts.ToArray();
			NotesLoadedTitle = NotesTextsTitles.ToArray();

			ChangeNoteIndex(0);
		}

		public static void DisposeGTAOverlay()
		{
			//HelperClasses.Logger.Log("Trying to Dispose GTA Overlay");
			if (MyGTAOverlay != null)
			{
				OverlayWasVisible = false;
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
				HelperClasses.Logger.Log("Setting Visibility to true");
				MyGTAOverlay.Visible = true;
			}
		}

		public static void OverlaySetInvisible()
		{
			if (IsOverlayInit())
			{
				HelperClasses.Logger.Log("Setting Visibility to false");
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
			HelperClasses.Logger.Log("A");
			if (IsOverlayVisible())
			{
				HelperClasses.Logger.Log("B");
				OverlaySetInvisible();
			}
			else
			{
				HelperClasses.Logger.Log("C");
				OverlaySetVisible();
			}
		}

		public static void OverlayScrollUp()
		{
			MyGTAOverlay.scroll(5);
		}

		public static void OverlayScrollDown()
		{
			MyGTAOverlay.scroll(-5);
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
				NoteOverlay.MyGTAOverlay.setTitle(NotesLoadedTitle[pNotesLoadedNewIndex]);
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




		private void btn_cb_Click(object sender, RoutedEventArgs e)
		{
			Settings.EnableOverlay = !Settings.EnableOverlay;
			ButtonMouseOverMagic((Button)sender);
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			Button myBtn = (Button)sender;
			ButtonMouseOverMagic(myBtn);
		}

		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			Button myBtn = (Button)sender;
			ButtonMouseOverMagic(myBtn);
		}



		/// <summary>
		/// Sets the Background for one specific CheckboxButton. Needs the second property to know if it should be checked or not
		/// </summary>
		/// <param name="myBtn"></param>
		/// <param name="pChecked"></param>
		private void SetCheckBoxBackground(Button myBtn, bool pChecked)
		{
			string BaseURL = @"Artwork/checkbox";
			if (pChecked)
			{
				BaseURL += "_true";
			}
			else
			{
				BaseURL += "_false";
			}
			if (myBtn.IsMouseOver)
			{
				BaseURL += "_mo.png";
			}
			else
			{
				BaseURL += ".png";
			}
			MainWindow.MW.SetControlBackground(myBtn, BaseURL);
		}

		/// <summary>
		/// Logic behind all MouseOver Stuff. Checkboxes and Refresh Button
		/// </summary>
		/// <param name="myBtn"></param>
		private void ButtonMouseOverMagic(Button myBtn)
		{
			SetCheckBoxBackground(myBtn, Settings.EnableOverlay);
		}

		private void btn_Notes_Click(object sender, RoutedEventArgs e)
		{
			NoteOverlayPage = NoteOverlayPages.NoteFiles;
		}

		private void btn_Looks_Click(object sender, RoutedEventArgs e)
		{
			NoteOverlayPage = NoteOverlayPages.Look;
		}

		private void btn_Keybindings_Click(object sender, RoutedEventArgs e)
		{
			NoteOverlayPage = NoteOverlayPages.Keybinds;
		}
	}
}
