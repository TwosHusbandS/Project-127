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

		private static NoteOverlayPages _NoteOverlayPage = NoteOverlayPages.NoteFiles;

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
						NoteOverlay.DisposePreview();

						// Set actual Frame_Main Content to the correct Page
						Frame_Notes.Content = new Project_127.Overlay.NoteOverlayPages.NoteOverlay_NoteFiles();
						btn_Notes.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						// Call Mouse_Over false on other Buttons where a page is behind
						btn_Looks.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						btn_Keybindings.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case NoteOverlayPages.Keybinds:
						NoteOverlay.DisposePreview();

						// Set actual Frame_Main Content to the correct Page
						Frame_Notes.Content = new Project_127.Overlay.NoteOverlayPages.NoteOverlay_Keybinds();
						btn_Keybindings.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

						// Call Mouse_Over false on other Buttons where a page is behind
						btn_Looks.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						btn_Notes.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
						break;
					case NoteOverlayPages.Look:
						LoadPreview();

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

		public static void LoadPreview()
		{
			MainWindow.MW.Frame_Game.Content = new Overlay_Preview();
			MainWindow.MW.Width = 1600;
			MainWindow.MW.Grid_Preview.Visibility = Visibility.Visible;

			MM_WasOpen = false;

			if (IsOverlayInit())
			{
				if (GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.MultiMonitor)
				{
					bool tmp = IsOverlayVisible();
					OverlaySetVisible();
					MM_WasOpen = tmp;
				}
			}

			Overlay_Preview.StartDispatcherTimer();
		}

		private static bool MM_WasOpen = true;

		public static void DisposePreview()
		{
			MainWindow.MW.Width = 900;
			MainWindow.MW.Grid_Preview.Visibility = Visibility.Hidden;
			MainWindow.MW.Frame_Game.Content = new EmptyPage();

			if (IsOverlayInit())
			{
				if (GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.MultiMonitor)
				{
					if (!MM_WasOpen)
					{
						OverlaySetInvisible();
					}
				}
			}

			Overlay_Preview.StopDispatcherTimer();

			MM_WasOpen = true;
		}


		public NoteOverlay()
		{
			InitializeComponent();

			NoteOverlayPage = LoadNoteOverlayWithCustomPage;
			LoadNoteOverlayWithCustomPage = NoteOverlayPages.NoteFiles;

			ButtonMouseOverMagic(btn_cb_Set_EnableOverlay);
			ButtonMouseOverMagic(btn_cb_Set_OverlayMultiMonitorMode);
		}

		public static void InitGTAOverlay()
		{
			//HelperClasses.Logger.Log("Trying to Init GTA Overlay");
			if (MyGTAOverlay == null)
			{
				MyGTAOverlay = new GTAOverlay();
				MyGTAOverlay.setTextColors(Settings.OverlayForeground, Color.Transparent);
				MyGTAOverlay.setBackgroundColor(Settings.OverlayBackground);
				MyGTAOverlay.setFont(Settings.OverlayTextFont, Settings.OverlayTextSize);
				MyGTAOverlay.Position = Settings.OverlayLocation;
				MyGTAOverlay.XMargin = Settings.OverlayMarginX;
				MyGTAOverlay.YMargin = Settings.OverlayMarginY;
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


		public static void OverlaySetVisible()
		{
			if (IsOverlayInit())
			{
				if (GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.MultiMonitor)
				{
					if (MainWindow.OL_MM != null)
					{
						MainWindow.OL_MM.MyShow();
						if (_NoteOverlayPage == NoteOverlayPages.Look)
						{
							MM_WasOpen = true;
						}
					}
				}
				HelperClasses.Logger.Log("Setting Visibility to true");
				MyGTAOverlay.Visible = true;
			}
		}

		public static void OverlaySetInvisible()
		{
			if (IsOverlayInit())
			{
				if (GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.MultiMonitor)
				{
					if (MainWindow.OL_MM != null)
					{
						MainWindow.OL_MM.MyHide();
						if (_NoteOverlayPage == NoteOverlayPages.Look)
						{
							MM_WasOpen = true;
						}
					}
				}

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


		public static void DisposeAllOverlayStuff()
		{
			NoteOverlay.DisposeGTAOverlay();
			if (MainWindow.OL_MM != null)
			{
				MainWindow.OL_MM.Close();
				MainWindow.OL_MM = null;
			}
			HelperClasses.Keyboard.KeyboardListener.Stop();
			HelperClasses.WindowChangeListener.Stop();
		}

		public static void OverlaySettingsChanged()
		{
			if (!GTAOverlay.DebugMode)
			{
				if (Settings.EnableOverlay == false)
				{
					DisposeAllOverlayStuff();
				}
				else if (Settings.EnableOverlay == true)
				{
					DisposeAllOverlayStuff();

					if (Settings.OverlayMultiMonitorMode)
					{
						if (MainWindow.OL_MM != null)
						{
							MainWindow.OL_MM.Close();
						}
						MainWindow.OL_MM = new Overlay_MultipleMonitor();
						MainWindow.OL_MM.Show();
						MainWindow.MW.Show();
						MainWindow.MW.Focus();
						NoteOverlay.InitGTAOverlay();
						MainWindow.OL_MM.MyHide();
						HelperClasses.Keyboard.KeyboardListener.Start();
					}
					else
					{
						if (LauncherLogic.GameState == LauncherLogic.GameStates.Running)
						{
							// Only Start Stop shit here when the overlay is not in debugmode
							if (!GTAOverlay.DebugMode && GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.Fullscreen)
							{
								NoteOverlay.InitGTAOverlay();
								HelperClasses.WindowChangeListener.Start();
							}
						}
					}

					if (_NoteOverlayPage == NoteOverlayPages.Look)
					{
						if (GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.MultiMonitor)
						{
							OverlaySetVisible();
							MM_WasOpen = false;
						}
					}
				}
				Overlay.NoteOverlayPages.NoteOverlay_Look.RefreshIfHideOrNot();
			}
		}


		public static void OverlayToggle()
		{
			if (IsOverlayInit())
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
			if (IsOverlayInit())
			{
				if (pNotesLoadedNewIndex >= 0 && pNotesLoadedNewIndex <= NotesLoaded.Length - 1)
				{
					HelperClasses.Logger.Log("NotesLoadedIndex is now: " + pNotesLoadedNewIndex);
					NotesLoadedIndex = pNotesLoadedNewIndex;
					MyGTAOverlay.setText(NotesLoaded[pNotesLoadedNewIndex]);
					MyGTAOverlay.title.text = NotesLoadedTitle[pNotesLoadedNewIndex];
					double tmp = (Overlay.NoteOverlayPages.NoteOverlay_Look.OverlayTextSize + 4) * 1.5 + 20;
					MyGTAOverlay.SetInitialScrollPosition((int)tmp);
				}
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
			Button myBtn = (Button)sender;
			switch (myBtn.Name)
			{
				case "btn_cb_Set_EnableOverlay":
					Settings.EnableOverlay = !Settings.EnableOverlay;
					break;
				case "btn_cb_Set_OverlayMultiMonitorMode":
					Settings.OverlayMultiMonitorMode = !Settings.OverlayMultiMonitorMode;
					break;
			}
			ButtonMouseOverMagic(myBtn);
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
			switch (myBtn.Name)
			{
				case "btn_cb_Set_EnableOverlay":
					SetCheckBoxBackground(myBtn, Settings.EnableOverlay);
					break;
				case "btn_cb_Set_OverlayMultiMonitorMode":
					SetCheckBoxBackground(myBtn, Settings.OverlayMultiMonitorMode);
					break;
			}

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
