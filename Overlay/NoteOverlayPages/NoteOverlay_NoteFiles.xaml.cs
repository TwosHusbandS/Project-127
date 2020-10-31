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

namespace Project_127.Overlay.NoteOverlayPages
{
	/// <summary>
	/// Interaction logic for NoteOverlay_NoteFiles.xaml
	/// </summary>
	public partial class NoteOverlay_NoteFiles : Page
	{
		ObservableCollection<MyNoteFile> MyNoteFiles;

		public NoteOverlay_NoteFiles()
		{
			InitializeComponent();

			this.DataContext = this;

			MyNoteFiles = new ObservableCollection<MyNoteFile>();

			dg_Files.ItemsSource = MyNoteFiles;

			MyNoteFiles.Add(new MyNoteFile("Test1"));
			MyNoteFiles.Add(new MyNoteFile("Test2"));
			MyNoteFiles.Add(new MyNoteFile("Test3"));

		}


		private void Shit()
		{
			// Resetting the Obvservable Collections:
			MyNoteFiles = new ObservableCollection<MyNoteFile>();

			// Files in BackupSaves (own File Path)
			string[] MyBackupSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(MySaveFile.BackupSavesPath);
			foreach (string MyBackupSaveFile in MyBackupSaveFiles)
			{
				if (!MyBackupSaveFile.Contains(".bak"))
				{
					MyNoteFiles.Add(new MyNoteFile(MyBackupSaveFile));
				}
			}

		}

		private void dg_Files_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_Files.ScrollToVerticalOffset(sv_Files.VerticalOffset - e.Delta / 3);
		}


		private void btn_Add_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_Delete_Click(object sender, RoutedEventArgs e)
		{
			
		}


		private void btn_Import_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_PresetLoad_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_PresetSave_Click(object sender, RoutedEventArgs e)
		{

		}

		private void dg_Files_PreviewKeyDown(object sender, KeyEventArgs e)
		{

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

	
		public MyNoteFile(string pFileName)
		{
			this.FileName = pFileName;
		}

	} // End of Class
}
