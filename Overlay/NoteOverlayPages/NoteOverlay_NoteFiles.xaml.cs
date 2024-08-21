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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Windows.Controls.Primitives;
using Popup = Project_127.Popups.Popup;

namespace Project_127.Overlay.NoteOverlayPages
{
	/// <summary>
	/// Interaction logic for NoteOverlay_NoteFiles.xaml
	/// </summary>
	public partial class NoteOverlay_NoteFiles : Page
	{
		/// <summary>
		/// Internal Property of all NoteFiles
		/// </summary>
		ObservableCollection<MyNoteFile> _MyNoteFiles;

		/// <summary>
		/// Observable Collection of my Note Files
		/// </summary>
		ObservableCollection<MyNoteFile> MyNoteFiles
		{
			get
			{
				return _MyNoteFiles;
			}
			set
			{
				_MyNoteFiles = value;
			}
		}

		/// <summary>
		/// Path where those are located
		/// </summary>
		public static string NotePath
		{
			get
			{
				return LauncherLogic.SupportFilePath.TrimEnd('\\') + @"\Notes";
			}
		}

		/// <summary>
		/// Constructor of NoteFiles Subpage
		/// </summary>
		public NoteOverlay_NoteFiles()
		{
			InitializeComponent();

			this.DataContext = this;

			MyNoteFiles = new ObservableCollection<MyNoteFile>();

			dg_Files.ItemsSource = MyNoteFiles;

			LoadMainNotes(true);
		}

		/// <summary>
		/// Loading the "Main" Notes
		/// </summary>
		public void LoadMainNotes(bool DontLoadTexts = false)
		{
			while (MyNoteFiles.Count > 0)
			{
				MyNoteFiles.MyRemove(0);
			}


			List<MyNoteFile> MyLNF = new List<MyNoteFile>();

			foreach (string mystring in Settings.OverlayNotesMain)
			{
				MyLNF.Add(new MyNoteFile(mystring));
			}

			MyNoteFiles.MyAdd(MyLNF, DontLoadTexts);

			HelperClasses.DataGridHelper.SelectFirst(dg_Files);
		}

		/// <summary>
		/// Good scrolling
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_Files_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_Files.ScrollToVerticalOffset(sv_Files.VerticalOffset - e.Delta / 3);
		}

		/// <summary>
		/// Add File Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Add_Click(object sender, RoutedEventArgs e)
		{
			string rtrn = HelperClasses.FileHandling.OpenDialogExplorer(FileHandling.PathDialogType.File, "Select the Note Files you want to add to your selection:", NotePath, true, "TXT Files|*.txt*");
			if (rtrn == "")
			{
				return;
			}
			List<string> files = rtrn.Split(',').ToList();

			List<MyNoteFile> MNFL = new List<MyNoteFile>();
			for (int i = 0; i <= files.Count - 1; i++)
			{
				string filename = HelperClasses.FileHandling.PathSplitUp(files[i])[1];
				string filepath = NotePath + @"\" + filename;
				if (!files[i].Contains(NotePath))
				{
					if (HelperClasses.FileHandling.doesFileExist(filepath))
					{
						bool yesno = PopupWrapper.PopupYesNo("The file: '" + filename + "' already exists in the note Folder.\nDo you want to overwrite it with '" + files[i] + "'");
						if (yesno == true)
						{
							HelperClasses.FileHandling.deleteFile(filepath);
							HelperClasses.FileHandling.copyFile(files[i], filepath);
						}
					}
					else
					{
						HelperClasses.FileHandling.copyFile(files[i], filepath);
					}
				}
				MyNoteFile tmp = new MyNoteFile(filename);
				MNFL.Add(tmp);
			}
			MyNoteFiles.MyAdd(MNFL);
		}

		/// <summary>
		/// Returns the Selected Note Files
		/// </summary>
		/// <returns></returns>
		private List<MyNoteFile> GetSelectedNoteFiles()
		{
			List<MyNoteFile> tmp = new List<MyNoteFile>();

			if (dg_Files.SelectedItems != null)
			{
				foreach (object wtf in dg_Files.SelectedItems)
				{
					MyNoteFile temp = (MyNoteFile)wtf;
					tmp.Add(temp);
				}
			}
			return tmp;
		}


		/// <summary>
		/// Deletes the selected Notefile (from the Collection, not from Disk)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Delete_Click(object sender, RoutedEventArgs e)
		{

			MyNoteFiles.MyRemove(GetSelectedNoteFiles());
			HelperClasses.DataGridHelper.SelectFirst(dg_Files);
		}

		/// <summary>
		/// Loading a Preset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_PresetLoad_Click(object sender, RoutedEventArgs e)
		{
			// TODO TO DO CTRLF CTRL F
			// Currently calling LoadMainNotes() twice instead of oncee...idk why this is neccesary...
			// so this is kinda a dirtfix, but it works. Once again, no idea why we have to do it this way

			//	-Overlay Presets Fix(according to burhac, shits broke)
			//		=> Confirmed Broken.Regedit settings dont update quick enough.
			//			Using newly assigned regedit variable(Settings.OverlayNotesMain = Settings.OverlayNotesPresetA;)
			//			is not updated by the time we are refering to it(in LoadMainNotes();)

			LoadMainNotes();
			switch (((Button)sender).Tag.ToString())
			{
				case "A":
					Settings.OverlayNotesMain = Settings.OverlayNotesPresetA;
					break;
				case "B":
					Settings.OverlayNotesMain = Settings.OverlayNotesPresetB;
					break;
				case "C":
					Settings.OverlayNotesMain = Settings.OverlayNotesPresetC;
					break;
				case "D":
					Settings.OverlayNotesMain = Settings.OverlayNotesPresetD;
					break;
				case "E":
					Settings.OverlayNotesMain = Settings.OverlayNotesPresetE;
					break;
				case "F":
					Settings.OverlayNotesMain = Settings.OverlayNotesPresetF;
					break;
			}
			LoadMainNotes();
		}

		/// <summary>
		/// Saving a Preset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_PresetSave_Click(object sender, RoutedEventArgs e)
		{
			switch (((Button)sender).Tag.ToString())
			{
				case "A":
					Settings.OverlayNotesPresetA = Settings.OverlayNotesMain;
					break;
				case "B":
					Settings.OverlayNotesPresetB = Settings.OverlayNotesMain;
					break;
				case "C":
					Settings.OverlayNotesPresetC = Settings.OverlayNotesMain;
					break;
				case "D":
					Settings.OverlayNotesPresetD = Settings.OverlayNotesMain;
					break;
				case "E":
					Settings.OverlayNotesPresetE = Settings.OverlayNotesMain;
					break;
				case "F":
					Settings.OverlayNotesPresetF = Settings.OverlayNotesMain;
					break;
			}
		}

		/// <summary>
		/// Key Event on Datagrid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_Files_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				btn_Delete_Click(null, null);
				e.Handled = true;
			}
		}

		private async void dg_Files_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			DataGrid myDataGrid = (DataGrid)sender;

			if (myDataGrid != null)
			{
				e.Handled = false;
				// making the selection 0
				myDataGrid.SelectedItem = null;

				// then simulating leftclick where the mouse is
				HelperClasses.MouseSender.DoMouseClick();

				await Task.Delay(50);

				MyNoteFile MNF = (MyNoteFile)myDataGrid.SelectedItem;

				if (MNF != null)
				{
					if (HelperClasses.FileHandling.doesFileExist(MNF.FilePath))
					{
						ProcessHandler.StartProcess(@"C:\Windows\System32\notepad.exe", pCommandLineArguments: MNF.FilePath);
					}
				}

				myDataGrid.Focus();
			}
		}
	}


	public static class ExtensionMethods
	{
		/// <summary>
		/// Refreshing an Observable Collection
		/// </summary>
		/// <param name="OC"></param>
		public static void Refresh(ObservableCollection<MyNoteFile> OC, bool DontLoadTexts = false)
		{
			List<string> tmp = new List<string>();
			foreach (MyNoteFile temp in OC)
			{
				tmp.Add(temp.FileName);
			}
			Settings.OverlayNotesMain = tmp;

			if (!DontLoadTexts)
			{
				NoteOverlay.LoadTexts();
			}
		}


		/// <summary>
		/// Adding to our Observable Collection
		/// </summary>
		/// <param name="OC"></param>
		/// <param name="MNFL"></param>
		public static void MyAdd(this ObservableCollection<MyNoteFile> OC, List<MyNoteFile> MNFL, bool DontLoadTexts = false)
		{
			foreach (MyNoteFile MNF in MNFL)
			{
				if (!OC.Contains(MNF))
				{
					OC.Add(MNF);
				}
			}

			Refresh(OC, DontLoadTexts);
		}

		/// <summary>
		/// Removing from our Observable Collection
		/// </summary>
		/// <param name="OC"></param>
		/// <param name="MNFL"></param>
		public static void MyRemove(this ObservableCollection<MyNoteFile> OC, List<MyNoteFile> MNFL)
		{
			foreach (MyNoteFile MNF in MNFL)
			{
				OC.Remove(MNF);
			}

			Refresh(OC);
		}

		/// <summary>
		/// Removing from out Observable Collection from a specific Index
		/// </summary>
		/// <param name="OC"></param>
		/// <param name="Index"></param>
		public static void MyRemove(this ObservableCollection<MyNoteFile> OC, int Index)
		{
			if (0 <= Index && Index <= OC.Count - 1)
			{
				OC.RemoveAt(Index);
			}

			// THIS DOESNT REFRESH
		}
	}

	/// <summary>
	/// Class for "MyFile" Objects. Used in the DataGrids for the SaveFileManagement
	/// </summary>
	public class MyNoteFile
	{
		private string _FileName;

		/// <summary>
		/// FileName for one MyNoteFile
		/// </summary>
		public string FileName
		{
			get
			{
				return _FileName;
			}
			set
			{
				_FileName = value;
			}
		}

		private string _FileNiceName;

		/// <summary>
		/// Nice FileName 
		/// </summary>
		public string FileNiceName
		{
			get
			{
				return _FileNiceName;
			}
			set
			{
				_FileNiceName = value;
			}
		}

		private string _FilePath;

		/// <summary>
		/// FilePath to that one File
		/// </summary>
		public string FilePath
		{
			get
			{
				return _FilePath;
			}
			set
			{
				_FilePath = value;
			}
		}

		/// <summary>
		/// Constructor of "MyNoteFile"
		/// </summary>
		/// <param name="pFileName"></param>
		public MyNoteFile(string pFileName)
		{
			this.FileName = pFileName;
			int LastIndexOfDot = pFileName.LastIndexOf('.');
			if (LastIndexOfDot == -1)
			{
				this.FileNiceName = this.FileName;
			}
			else
			{
				this.FileNiceName = pFileName.Substring(0, LastIndexOfDot);
			}
			this.FilePath = NoteOverlay_NoteFiles.NotePath + @"\" + this.FileName;
		}

	} // End of Class
}

