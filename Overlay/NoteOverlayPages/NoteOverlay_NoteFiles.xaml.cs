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

namespace Project_127.Overlay.NoteOverlayPages
{
	/// <summary>
	/// Interaction logic for NoteOverlay_NoteFiles.xaml
	/// </summary>
	public partial class NoteOverlay_NoteFiles : Page
	{
		public NoteOverlay_NoteFiles()
		{
			InitializeComponent();
		}

		private void btn_NoteFile_Click(object sender, RoutedEventArgs e)
		{
			string SelectedFiles = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Select the Notes Files you want to load.", LauncherLogic.ZIPFilePath, true, "TEXT Files|*.txt*");
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
			NoteOverlay.MyGTAOverlay.setText(NoteOverlay.NotesLoaded[0]);
		}

		private void dg_BackupFiles_GotFocus(object sender, RoutedEventArgs e)
		{

		}

		private void dg_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void dg_BackupFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{

		}

		private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void btn_Add_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_Delete_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_PresetLoad_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_PresetSave_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_Help_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
