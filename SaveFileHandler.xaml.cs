using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Project_127
{
	/// <summary>
	/// Class SaveFileHandler.xaml
	/// </summary>
	public partial class SaveFileHandler : Window
	{
		// THIS CLASS IS NOT FULLY IMPLEMENTED OR WORKING AT ALL

		/// <summary>
		/// Collection of "MyFile" which are used for the Save-Files in the Backup Folder.
		/// </summary>
		public static ObservableCollection<MySaveFile> BackupSaves = new ObservableCollection<MySaveFile>();

		/// <summary>
		/// Collection of "MyFile" which are used for the Save-Files in the GTA Folder.
		/// </summary>
		public static ObservableCollection<MySaveFile> GTASaves = new ObservableCollection<MySaveFile>();

		/// <summary>
		/// Constructor of SaveFileHandler
		/// </summary>
		public SaveFileHandler()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			// Used for DataBinding
			this.DataContext = this;

			// Set the ItemSource of Both Datagrids for the DataBinding
			dg_BackupFiles.ItemsSource = BackupSaves;
			dg_GTAFiles.ItemsSource = GTASaves;

			string[] MyBackupSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(LauncherLogic.SaveFilesPath);
			foreach (string MyBackupSaveFile in MyBackupSaveFiles)
			{
				if (!MyBackupSaveFile.Contains(".bak"))
				{
					BackupSaves.Add(new MySaveFile(MyBackupSaveFile));
				}
			}

			string[] MyGTAVSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(LauncherLogic.SaveFilesPath);
			foreach (string MyGTAVSaveFile in MyGTAVSaveFiles)
			{
				if (!MyGTAVSaveFile.Contains(".bak"))
				{
					GTASaves.Add(new MySaveFile(MyGTAVSaveFile));
				}
			}
		}


		/// <summary>
		/// Button Click on the LeftArrow (From GTA Path to Backup Path)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_LeftArrow_Click(object sender, RoutedEventArgs e)
		{

		}


		/// <summary>
		/// Click on the Right Arrow (From Backup Path to GTA Path)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_RightArrow_Click(object sender, RoutedEventArgs e)
		{
			// Not Fully Implemented
			//MyFile tmp = (MyFile)dg_BackupFiles.SelectedItem; // we need to null check this XD
			//BackupSaves.Remove(tmp);
			//GTASaves.Add(tmp);
		}


		/// <summary>
		/// Click on the Refresh Button. Reads files from disk again.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Refresh_Click(object sender, RoutedEventArgs e)
		{

		}


		/// <summary>
		/// Close Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_close_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}


		// Below are Methods we need to make the behaviour of this nice.


		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

		/// <summary>
		/// Enables the scrolling behaviour of the DataGrid for Backup Save-Files
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Dg_BackupFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_BackupFiles.ScrollToVerticalOffset(sv_BackupFiles.VerticalOffset - e.Delta / 3);
		}

		/// <summary>
		/// Enables the scrolling behaviour of the DataGrid for GTA Save-Files
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Dg_GTAFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_BackupFiles.ScrollToVerticalOffset(sv_BackupFiles.VerticalOffset - e.Delta / 3);
		}

	} // End of Class
} // End of Namespace
