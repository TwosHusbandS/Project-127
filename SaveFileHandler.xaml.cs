using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
	public partial class SaveFileHandler : Page
	{
		/// <summary>
		/// Constructor of SaveFileHandler
		/// </summary>
		public SaveFileHandler()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			// Used for DataBinding
			this.DataContext = this;

			btn_Refresh_Click(null, null);
			MouseOverMagic(btn_LeftArrow);
			MouseOverMagic(btn_RightArrow);
			MouseOverMagic(btn_Refresh);
		}


		/// <summary>
		/// Mouse Over Magic
		/// </summary>
		/// <param name="myBtn"></param>
		public static void MouseOverMagic(Button myBtn)
		{
			switch (myBtn.Name)
			{
				case "btn_LeftArrow":
					if (myBtn.IsMouseOver)
					{
						MainWindow.MW.SetControlBackground(myBtn,@"Artwork/arrowleft_mo.png");
					}
					else
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork/arrowleft.png");
					}
					break;
				case "btn_RightArrow":
					if (myBtn.IsMouseOver)
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork/arrowright_mo.png");
					}
					else
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork/arrowright.png");
					}
					break;
				case "btn_Refresh":
					if (myBtn.IsMouseOver)
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork/refresh_mo.png");
					}
					else
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork/refresh.png");
					}
					break;
			}
		}


		/// <summary>
		/// Sorts a DataGrid
		/// </summary>
		/// <param name="pDataGrid"></param>
		private void Sort(DataGrid pDataGrid)
		{
			// What can I say...
			// https://stackoverflow.com/a/40395019

			// Since we always add one more MySaveFile to the collection we could 
			// Loop through the childitems and then move it to the correct index
			// Pretty much implement our own Sorting Method which uses MySaveFile.BackupSaves.Move(a,b);

			if (pDataGrid.ItemsSource == null)
				pDataGrid.ItemsSource = MySaveFile.BackupSaves;
			CollectionViewSource.GetDefaultView(pDataGrid.ItemsSource).Refresh();
			pDataGrid.Items.SortDescriptions.Clear();
			pDataGrid.Items.SortDescriptions.Add(new SortDescription(pDataGrid.Columns[0].SortMemberPath, ListSortDirection.Ascending));
			foreach (var col in pDataGrid.Columns)
			{
				col.SortDirection = null;
			}
			pDataGrid.Columns[0].SortDirection = ListSortDirection.Ascending;
			pDataGrid.Items.Refresh();
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
			// Copying a File from the GTA V Saves to the Backup Saves
			// We ask User for Name

			// Null Check
			if (dg_GTAFiles.SelectedItem != null)
			{
				// Get MySaveFile from the selected Item
				MySaveFile tmp = (MySaveFile)dg_GTAFiles.SelectedItem;

				// Get the Name for it
				string newName = GetNewFileName(tmp.FileName, MySaveFile.BackupSavesPath);
				if (!string.IsNullOrWhiteSpace(newName))
				{
					// Only do if it the name is not "" or null
					tmp.CopyToBackup(newName);
					Sort(dg_BackupFiles);
				}
			}
		}




		/// <summary>
		/// Click on the Right Arrow (From Backup Path to GTA Path)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_RightArrow_Click(object sender, RoutedEventArgs e)
		{
			// Copying a File from the Backup Saves to the GTA V Saves
			// Build name which GTA Uses

			// Null checker
			if (dg_BackupFiles.SelectedItem != null)
			{
				// Get MySaveFile from the selected Item
				MySaveFile tmp = (MySaveFile)dg_BackupFiles.SelectedItem;

				// Building the first theoretical FileName
				int i = 0;
				string NewFileName = MySaveFile.ProperSaveNameBase + i.ToString("00");
				string FilePathInsideGTA = MySaveFile.GTAVSavesPath.TrimEnd('\\') + @"\" + NewFileName;

				// While Loop through all 16 names, breaking out when File does NOT exist or when we reached the manimum
				while (HelperClasses.FileHandling.doesFileExist(FilePathInsideGTA))
				{
					if (i >= 15)
					{
						// Save Files with a Number larger than that will not be read by game
						new Popup(Popup.PopupWindowTypes.PopupOk, "No free SaveSlot (00-15) inside the GTA Save File Path.\nDelete one and try again").ShowDialog();
						return;
					}

					// Build new theoretical FileName
					i++;
					NewFileName = MySaveFile.ProperSaveNameBase + i.ToString("00");
					FilePathInsideGTA = MySaveFile.GTAVSavesPath.TrimEnd('\\') + @"\" + NewFileName;
				}
				tmp.CopyToGTA(NewFileName);
				Sort(dg_GTAFiles);
				new Popup(Popup.PopupWindowTypes.PopupOk, "Copied '" + tmp.FileName + "' to the GTA V Saves Location under the Name '" + NewFileName + "' !").ShowDialog();
			}
		}



		/// <summary>
		/// Rename Button for the Files in the "our" Folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Rename_Click(object sender, RoutedEventArgs e)
		{
			// Getting the Selecting SaveFile, checking if it aint null
			MySaveFile tmp = GetSelectedSaveFile();
			if (tmp != null)
			{
				// If its inside GTA Save directory
				if (tmp.SaveFileKind == MySaveFile.SaveFileKinds.GTAV)
				{
					// Asking user if he really wants to rename since its fucked...
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Re-Naming something inside the GTA V Saves Location makes no sense,\nsince it wont get recognized by the game.\nStill want to continue?");
					yesno.ShowDialog();
					if (yesno.DialogResult == false)
					{
						// If user decides against renaming, return and end function
						return;
					}
				}

				// Popup for new name of File
				string newName = GetNewFileName(tmp.FileName, tmp.Path);
				if (!string.IsNullOrWhiteSpace(newName))
				{
					// Rename File, Sort Datagrid
					tmp.Rename(newName);
					if (tmp.SaveFileKind == MySaveFile.SaveFileKinds.GTAV)
					{
						Sort(dg_GTAFiles);
					}
					else
					{
						Sort(dg_BackupFiles);
					}
				}
			}
		}



		/// <summary>
		/// Delete Button for the files in the GTAV Location
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Delete_Click(object sender, RoutedEventArgs e)
		{
			// Gets selected File, checks if its not null
			MySaveFile tmp = GetSelectedSaveFile();
			if (tmp != null)
			{
				// Ask user if he wants to remove it
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Are you sure you want to delete this SaveFile?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					// Deleting it and sorting them
					tmp.Delete();
					if (tmp.SaveFileKind == MySaveFile.SaveFileKinds.GTAV)
					{
						Sort(dg_GTAFiles);
					}
					else
					{
						Sort(dg_BackupFiles);
					}
				}
			}
		}

		/// <summary>
		/// Gets the single selected File from Both Grids
		/// </summary>
		/// <returns></returns>
		private MySaveFile GetSelectedSaveFile()
		{
			// Returns the Selected SaveFile (since it is only one), else returns null
			if (dg_BackupFiles.SelectedItem != null)
			{
				return (MySaveFile)dg_BackupFiles.SelectedItem;
			}
			else if (dg_GTAFiles.SelectedItem != null)
			{
				return (MySaveFile)dg_GTAFiles.SelectedItem;
			}
			return null;
		}


		/// <summary>
		/// Mouse Enter Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			MouseOverMagic((Button)sender);
		}

		/// <summary>
		/// Mouse Leave Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			MouseOverMagic((Button)sender);
		}

		/// <summary>
		/// New Name Popup Logic
		/// </summary>
		/// <param name="pMySaveFile"></param>
		/// <param name="pDestination"></param>
		/// <returns></returns>
		private string GetNewFileName(string pMySaveFileName, string pPathToCheck)
		{
			string newName = "";

			// Asking for Name 
			PopupTextbox newNamePU = new PopupTextbox("Enter new Name for the SaveFile:\n'" + pMySaveFileName + "'", pMySaveFileName);
			newNamePU.ShowDialog();
			if (newNamePU.DialogResult == true)
			{
				// Getting the Name chosen
				newName = newNamePU.MyReturnString;

				// While name was give OR fikle exists
				while (String.IsNullOrWhiteSpace(newName) || HelperClasses.FileHandling.doesFileExist(pPathToCheck.TrimEnd('\\') + @"\" + newName))
				{
					// Not a Valid FilePath
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "File does already exists or is not a valid FileName.\n" +
																					"Click yes if you want to try again.");
					yesno.ShowDialog();
					if (yesno.DialogResult == false)
					{
						// When you wanna exit
						return "";
					}
					else
					{
						// When you wanna stay in while loop
						PopupTextbox newNamePU2 = new PopupTextbox("Enter new Name for the SaveFile:\n'" + pMySaveFileName + "'", pMySaveFileName);
						newNamePU2.ShowDialog();
						if (newNamePU2.DialogResult == true)
						{
							newName = newNamePU2.MyReturnString;
						}
					}
				}
			}
			return newName;
		}

		/// <summary>
		/// Enables the scrolling behaviour of the DataGrid for Backup Save-Files
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_BackupFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_BackupFiles.ScrollToVerticalOffset(sv_BackupFiles.VerticalOffset - e.Delta / 3);
		}

		/// <summary>
		/// Enables the scrolling behaviour of the DataGrid for GTA Save-Files
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_GTAFiles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			sv_GTAFiles.ScrollToVerticalOffset(sv_GTAFiles.VerticalOffset - e.Delta / 3);
		}

		/// <summary>
		/// Reset Selection when the other datagrid was clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_GTAFiles_GotFocus(object sender, RoutedEventArgs e)
		{
			// if GTA Datagrid got Focus, we are removing selection from Backup Datagrid
			dg_BackupFiles.SelectedItem = null;
		}

		/// <summary>
		/// Reset Selection when the other datagrid was clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_BackupFiles_GotFocus(object sender, RoutedEventArgs e)
		{
			// if got Backup Datagrid Focus, we are removing selection from GTA Datagrid 
			dg_GTAFiles.SelectedItem = null;
		}

		/// <summary>
		/// KeyDown Method
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_KeyDown(object sender, KeyEventArgs e)
		{
			// This should now just be easy and useable with keyboard only.
			// KeyDown and KeyUp are already implemented basic DataGrid behaviour
			if (e.Key == Key.Delete)
			{
				btn_Delete_Click(null, null);
			}
			else if (e.Key == Key.F2)
			{
				btn_Rename_Click(null, null);
			}
			else if (e.Key == Key.Right)
			{
				btn_RightArrow_Click(null, null);
			}
			else if (e.Key == Key.Left)
			{
				btn_LeftArrow_Click(null, null);
			}
			else if (e.Key == Key.F5)
			{
				btn_Refresh_Click(null, null);
			}
		}


		/// <summary>
		/// Importing multiple savefiles
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Import_Click(object sender, RoutedEventArgs e)
		{
			string MySelectedFilesRtrn = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Pick the SaveFiles you want to import", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
											@"\Rockstar Games\GTA V\Profiles", true);

			// Close this if return is empty or ""
			if (String.IsNullOrEmpty(MySelectedFilesRtrn))
			{
				return;
			}

			// List of all Selected Files
			List<string> MySelectedFiles = MySelectedFilesRtrn.Split(',').ToList();
			List<string> MySelectedFilesUnique = new List<string>();

			// Looping through all Selected Files
			for (int i = 0; i <= MySelectedFiles.Count - 1; i++)
			{
				// if is .bak
				if (MySelectedFiles[i].Substring(MySelectedFiles[i].Length - 4) == ".bak")
				{
					string correspondingNonBakFile = MySelectedFiles[i].Substring(0, MySelectedFiles[i].Length - 4);
					if (!HelperClasses.FileHandling.doesFileExist(correspondingNonBakFile))
					{
						// Create correspondingNonBakFile if it doesnt exist
						HelperClasses.FileHandling.copyFile(MySelectedFiles[i], correspondingNonBakFile);
					}
				}
				// Non .bak
				else
				{
					string correspondingBakFile = MySelectedFiles[i] + ".bak";
					if (!HelperClasses.FileHandling.doesFileExist(correspondingBakFile))
					{
						// Create correspondingBakFile if it doesnt exist
						HelperClasses.FileHandling.copyFile(MySelectedFiles[i], correspondingBakFile);
					}
				}

				// Making the FileName non Bak
				MySelectedFiles[i] = HelperClasses.FileHandling.TrimEnd(MySelectedFiles[i], ".bak");

				// Adding it to new list if it doesnt contain it yet
				if (!MySelectedFilesUnique.Contains(MySelectedFiles[i]))
				{
					string FileName = MySelectedFiles[i].Substring(MySelectedFiles[i].LastIndexOf('\\') + 1);
					if (HelperClasses.FileHandling.doesFileExist(MySaveFile.GTAVSavesPath.TrimEnd('\\') + @"\" + FileName))
					{
						FileName = GetNewFileName(FileName, MySaveFile.BackupSavesPath);
					}
					MySaveFile.Import(MySelectedFiles[i], FileName);

					MySelectedFilesUnique.Add(MySelectedFiles[i]);
				}
			}

			Sort(dg_BackupFiles);
		}


	} // End of Class
} // End of Namespace
