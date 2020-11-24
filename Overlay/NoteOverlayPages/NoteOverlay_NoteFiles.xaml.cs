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
		ObservableCollection<MyNoteFile> _MyNoteFiles;


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

		public static string NotePath
		{
			get
			{
				return LauncherLogic.SupportFilePath.TrimEnd('\\') + @"\Notes";
			}
		}

		public NoteOverlay_NoteFiles()
		{
			InitializeComponent();

			this.DataContext = this;

			MyNoteFiles = new ObservableCollection<MyNoteFile>();

			dg_Files.ItemsSource = MyNoteFiles;

			LoadMainNotes();
		}

		public void LoadMainNotes()
		{
			while (MyNoteFiles.Count > 0)
			{
				MyNoteFiles.MyRemove(0);
			}

			foreach (string mystring in Settings.OverlayNotesMain)
			{
				MyNoteFiles.MyAdd(new MyNoteFile(mystring));
			}

			SelectFirst();
		}


		private void dg_Files_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_Files.ScrollToVerticalOffset(sv_Files.VerticalOffset - e.Delta / 3);
		}


		private void btn_Add_Click(object sender, RoutedEventArgs e)
		{
			string rtrn = HelperClasses.FileHandling.OpenDialogExplorer(FileHandling.PathDialogType.File, "Select the Note Files you want to add to your selection:", NotePath, true, "TXT Files|*.txt*");
			if (rtrn == "")
			{
				return;
			}
			List<string> files = rtrn.Split(',').ToList();
			for (int i = 0; i <= files.Count - 1; i++)
			{
				string filename = HelperClasses.FileHandling.PathSplitUp(files[i])[1];
				string filepath = NotePath + @"\" + filename;
				if (!files[i].Contains(NotePath))
				{
					if (HelperClasses.FileHandling.doesFileExist(filepath))
					{
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "The file: '" + filename + "' already exists in the note Folder.\nDo you want to overwrite it with '" + files[i] + "'");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
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
				if (!MyNoteFiles.Contains(tmp))
				{
					MyNoteFiles.MyAdd(tmp);
				}
			}
		}


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



		private void btn_Delete_Click(object sender, RoutedEventArgs e)
		{
			foreach (MyNoteFile MNF in GetSelectedNoteFiles())
			{
				MyNoteFiles.MyRemove(MNF);
			}
			SelectFirst();
		}

		private void SelectFirst()
		{
			if (dg_Files.Items.Count > 0)
			{
				//dg_Files.SelectedItems.Add(dg_Files.Items[0]);

				//dg_Files.CurrentCell = new DataGridCellInfo(dg_Files.Items[0], dg_Files.Columns[0]);
				//dg_Files.SelectedItems.Clear();
				//dg_Files.SelectedCells.Add(dg_Files.CurrentCell);

				SelectRowByIndex(dg_Files, 0);
			}

		}

		public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
		{
			if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
				//throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

				if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
					//throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

					dataGrid.SelectedItems.Clear();
			/* set the SelectedItem property */
			object item = dataGrid.Items[rowIndex]; // = Product X
			dataGrid.SelectedItem = item;

			DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
			if (row == null)
			{
				/* bring the data item (Product object) into view
				 * in case it has been virtualized away */
				dataGrid.ScrollIntoView(item);
				row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
			}
			if (row != null)
			{
				DataGridCell cell = GetCell(dataGrid, row, 0);
				if (cell != null)
					cell.Focus();
			}
		}

		public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
					return (T)child;
				else
				{
					T childOfChild = FindVisualChild<T>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
		{
			if (rowContainer != null)
			{
				DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				if (presenter == null)
				{
					/* if the row has been virtualized away, call its ApplyTemplate() method 
					 * to build its visual tree in order for the DataGridCellsPresenter
					 * and the DataGridCells to be created */
					rowContainer.ApplyTemplate();
					presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				}
				if (presenter != null)
				{
					DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					if (cell == null)
					{
						/* bring the column into view
						 * in case it has been virtualized away */
						dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
						cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					}
					return cell;
				}
			}
			return null;
		}

		private void btn_PresetLoad_Click(object sender, RoutedEventArgs e)
		{
			// TODO TO DO CTRLF CTRL F
			// Currently calling LoadMainNotes() twice instead of one...idk why this is neccesary...
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

		private void dg_Files_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				btn_Delete_Click(null, null);
				e.Handled = true;
			}
		}

	}


	public static class ExtensionMethods
	{
		public static void Refresh(ObservableCollection<MyNoteFile> OC)
		{
			List<string> tmp = new List<string>();
			foreach (MyNoteFile temp in OC)
			{
				tmp.Add(temp.FileName);
			}
			Settings.OverlayNotesMain = tmp;

			NoteOverlay.LoadTexts();
		}


		public static void MyAdd(this ObservableCollection<MyNoteFile> OC, MyNoteFile MNF)
		{
			OC.Add(MNF);

			Refresh(OC);
		}

		public static void MyRemove(this ObservableCollection<MyNoteFile> OC, MyNoteFile MNF)
		{
			OC.Remove(MNF);

			Refresh(OC);
		}

		public static void MyRemove(this ObservableCollection<MyNoteFile> OC, int Index)
		{
			if (0 <= Index && Index <= OC.Count - 1)
			{
				OC.RemoveAt(Index);
			}

			Refresh(OC);
		}
	}

	/// <summary>
	/// Class for "MyFile" Objects. Used in the DataGrids for the SaveFileManagement
	/// </summary>
	public class MyNoteFile
	{
		private string _FileName;

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

