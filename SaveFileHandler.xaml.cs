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
		/// Constructor of SaveFileHandler
		/// </summary>
		public SaveFileHandler()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			// Used for DataBinding
			this.DataContext = this;

			// Resetting the Observable Collections
			btn_Refresh_Click();

			// Set the ItemSource of Both Datagrids for the DataBinding
			dg_BackupFiles.ItemsSource = MySaveFile.BackupSaves;
			dg_GTAFiles.ItemsSource = MySaveFile.GTASaves;
		}


		/// <summary>
		/// Button Click on the LeftArrow (From GTA Path to Backup Path)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_LeftArrow_Click(object sender, RoutedEventArgs e)
		{
			if (dg_GTAFiles.SelectedItem != null)
			{
				MySaveFile tmp = (MySaveFile)dg_GTAFiles.SelectedItem;
				tmp.MoveToBackup("NewNameInsideBackup");
			}
		}


		/// <summary>
		/// Click on the Right Arrow (From Backup Path to GTA Path)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_RightArrow_Click(object sender, RoutedEventArgs e)
		{
			if (dg_BackupFiles.SelectedItem != null)
			{
				MySaveFile tmp = (MySaveFile)dg_BackupFiles.SelectedItem;
				tmp.MoveToGTA("NewNameInsideGTA");
			}
		}



		/// <summary>
		/// Rename Button for the Files in the "our" Folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Rename_Click(object sender, RoutedEventArgs e)
		{
			if (dg_BackupFiles.SelectedItem != null)
			{
				MySaveFile tmp = (MySaveFile)dg_BackupFiles.SelectedItem;
				tmp.Rename("newName");
			}
		}

		/// <summary>
		/// Delete Button for the files in the GTAV Location
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Delete_Click(object sender, RoutedEventArgs e)
		{
			if (dg_GTAFiles.SelectedItem != null)
			{
				MySaveFile tmp = (MySaveFile)dg_GTAFiles.SelectedItem;
				tmp.Delete();
			}
		}


		/// <summary>
		/// Click on the Refresh Button. Reads files from disk again.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Refresh_Click(object sender = null, RoutedEventArgs e = null)
		{
			// Resetting the Obvservable Collections:
			MySaveFile.BackupSaves = new ObservableCollection<MySaveFile>();
			MySaveFile.GTASaves = new ObservableCollection<MySaveFile>();

			// Files in BackupSaves (own File Path)
			string[] MyBackupSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(MySaveFile.BackupSavesPath);
			foreach (string MyBackupSaveFile in MyBackupSaveFiles)
			{
				if (!MyBackupSaveFile.Contains(".bak"))
				{
					MySaveFile.BackupSaves.Add(new MySaveFile(MyBackupSaveFile));
				}
			}

			// Files in actual GTAV Save File Locations
			string[] MyGTAVSaveFiles = HelperClasses.FileHandling.GetFilesFromFolder(MySaveFile.GTAVSavesPath);
			foreach (string MyGTAVSaveFile in MyGTAVSaveFiles)
			{
				if (!MyGTAVSaveFile.Contains(".bak") && MyGTAVSaveFile.Contains("SGTA500"))
				{
					MySaveFile.GTASaves.Add(new MySaveFile(MyGTAVSaveFile));
				}
			}
		}


		/// <summary>
		/// Close Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Close_Click(object sender, RoutedEventArgs e)
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
			sv_GTAFiles.ScrollToVerticalOffset(sv_BackupFiles.VerticalOffset - e.Delta / 3);
		}

	} // End of Class
} // End of Namespace
