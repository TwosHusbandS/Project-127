using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127
{
	/// <summary>
	/// Class for "MyFile" Objects. Used in the DataGrids for the SaveFileManagement
	/// </summary>
	public class MySaveFile
	{
		/// <summary>
		/// Collection of "MyFile" which are used for the Save-Files in the Backup Folder.
		/// </summary>
		public static ObservableCollection<MySaveFile> BackupSaves = new ObservableCollection<MySaveFile>();

		/// <summary>
		/// Root Path for the SaveFiles we provide
		/// </summary>
		public static string BackupSavesPath { get { return LauncherLogic.SaveFilesPath; } }

		/// <summary>
		/// Path of the folder we are currently looking it
		/// </summary>
		public static string CurrentBackupSavesPath = BackupSavesPath;

		/// <summary>
		/// Property. The collection it belongs to.
		/// </summary>
		public ObservableCollection<MySaveFile> MyCollection
		{
			get
			{
				if (this.SaveFileKind == SaveFileKinds.Backup)
				{
					return BackupSaves;
				}
				else
				{
					return GTASaves;
				}
			}
		}

		/// <summary>
		/// Property. The collection it does NOT belong to.
		/// </summary>
		public ObservableCollection<MySaveFile> MyOppositeCollection
		{
			get
			{
				if (this.SaveFileKind == SaveFileKinds.Backup)
				{
					return GTASaves;
				}
				else
				{
					return BackupSaves;
				}
			}
		}

		/// <summary>
		/// Collection of "MyFile" which are used for the Save-Files in the GTA Folder.
		/// </summary>
		public static ObservableCollection<MySaveFile> GTASaves = new ObservableCollection<MySaveFile>();


		/// <summary>
		/// Path for the SaveFiles inside GTAV Installation Location
		/// </summary>
		/// https://stackoverflow.com/a/3492996
		public static string GTAVSavesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
											@"\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4";

		/// <summary>
		/// BaseName for a proper Savefile. Needs to have 2 more digits at the end. (00-15)
		/// </summary>
		public static string ProperSaveNameBase = "SGTA500";

		/// <summary>
		/// Enum we use to difference between Files and Folders
		/// </summary>
		public enum FileOrFolders
		{
			File,
			Folder
		}

		/// <summary>
		/// Enum Property to know if its a Backup - SaveFile or a GTAV - SaveFile
		/// </summary>
		public FileOrFolders FileOrFolder;

		/// <summary>
		/// Enum we use to diffe
		/// </summary>
		public enum SaveFileKinds
		{
			Backup,
			GTAV
		}

		/// <summary>
		/// Enum Property to know if its a Backup - SaveFile or a GTAV - SaveFile
		/// </summary>
		public SaveFileKinds SaveFileKind
		{
			get
			{
				if (this.FilePath.Contains(LauncherLogic.SupportFilePath))
				{
					return SaveFileKinds.Backup;
				}
				else
				{
					return SaveFileKinds.GTAV;
				}
			}
		}

		/// <summary>
		/// FileName Property.
		/// </summary>
		public string FilePath { private set; get; }

		/// <summary>
		/// FileName Property.
		/// </summary>
		public string FileName
		{
			get
			{
				string _FileName = "";
				if (this.FileOrFolder == FileOrFolders.File)
				{
					_FileName = this.FilePath.Substring(this.FilePath.LastIndexOf('\\') + 1);
					if (!String.IsNullOrEmpty(this.FileNameAddition))
					{
						return _FileName + " (" + FileNameAddition + ")";
					}
					else
					{
						return _FileName;
					}
				}
				else
				{
					// we get full path of folder here. We want to display foldername if its an layer down (from CurrentBackupPath)
					// or ".." if its not...
					if (CurrentBackupSavesPath.StartsWith(FilePath))
					{
						_FileName = "..";
					}
					else
					{
						_FileName = this.FilePath.Substring(this.FilePath.LastIndexOf('\\') + 1);

					}
					return "  [  " + _FileName + "  ]  ";
				}
			}
		}

		/// <summary>
		/// FileNameAddition Property
		/// </summary>
		public string FileNameAddition = "";

		/// <summary>
		/// Filename of the same with the .bak addon
		/// </summary>
		public string FilePathBak { get { return FilePath + ".bak"; } }

		/// <summary>
		/// PathName Property
		/// </summary>
		public string Path { get { return this.FilePath.Substring(0, this.FilePath.LastIndexOf('\\')); } }

		/// <summary>
		/// Standart Constructor. Dont need to forbid default Constructor since it wont be generated.
		/// </summary>
		/// <param name="pFilePath"></param>
		public MySaveFile(string pFilePath, bool isFolder = false)
		{
			// Setting the FilePath
			this.FilePath = pFilePath;

			// If this is a GTA SaveFileKind
			if (this.SaveFileKind == SaveFileKinds.GTAV)
			{
				foreach (string MySaveFile in HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(BackupSavesPath))
				{
					if (HelperClasses.FileHandling.GetHashFromFile(MySaveFile) == HelperClasses.FileHandling.GetHashFromFile(pFilePath))
					{
						this.FileNameAddition = HelperClasses.FileHandling.PathSplitUp(MySaveFile)[1];
						break;
					}
				}
			}

			if (isFolder)
			{
				this.FileOrFolder = FileOrFolders.Folder;
			}
			else
			{
				this.FileOrFolder = FileOrFolders.File;
			}
		}

		/// <summary>
		/// Copying one SaveFile to Backup Save File Folder, all User error checks already done before
		/// </summary>
		/// <param name="pNewName"></param>
		public void CopyToBackup(string pNewName)
		{
			HelperClasses.Logger.Log("Copying SaveFiles '" + this.FileName + "' to Backup Folder under Name '" + pNewName + "'");

			string newFilePath = MySaveFile.CurrentBackupSavesPath.TrimEnd('\\') + @"\" + pNewName;
			HelperClasses.FileHandling.copyFile(this.FilePath, newFilePath);
			HelperClasses.FileHandling.copyFile(this.FilePathBak, newFilePath + ".bak");
			BackupSaves.Add(new MySaveFile(newFilePath));
		}

		public void CopyTo(string pPath)
		{
			string newFilePath = pPath.TrimEnd('\\') + @"\" + HelperClasses.FileHandling.PathSplitUp(this.FilePath)[1];

			if (HelperClasses.FileHandling.doesFileExist(newFilePath) || HelperClasses.FileHandling.doesFileExist(newFilePath + ".bak"))
			{
				Popups.Popup yesno = new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupYesNo, "File already exists.\nDo you wish to overwrite it?");
				yesno.ShowDialog();
				if (yesno.DialogResult == false)
				{
					return;
				}
			}

			HelperClasses.FileHandling.copyFile(this.FilePath, newFilePath);
			HelperClasses.FileHandling.copyFile(this.FilePathBak, newFilePath + ".bak");
		}

		public void MoveTo(string pPath)
		{
			string newFilePath = pPath.TrimEnd('\\') + @"\" + HelperClasses.FileHandling.PathSplitUp(this.FilePath)[1];

			if (HelperClasses.FileHandling.doesFileExist(newFilePath) || HelperClasses.FileHandling.doesFileExist(newFilePath + ".bak"))
			{
				Popups.Popup yesno = new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupYesNo, "File already exists.\nDo you wish to overwrite it?");
				yesno.ShowDialog();
				if (yesno.DialogResult == false)
				{
					return;
				}
			}

			HelperClasses.FileHandling.moveFile(this.FilePath, newFilePath);
			HelperClasses.FileHandling.moveFile(this.FilePathBak, newFilePath + ".bak");
		}


		/// <summary>
		/// Copying one SaveFile to GTA Save File Folder, all User error checks already done before
		/// </summary>
		/// <param name="pNewName"></param>
		public void CopyToGTA(string pNewName)
		{
			HelperClasses.Logger.Log("Copying SaveFiles '" + this.FileName + "' to GTA Folder under Name '" + pNewName + "'");

			string newFilePath = MySaveFile.GTAVSavesPath.TrimEnd('\\') + @"\" + pNewName;
			HelperClasses.FileHandling.copyFile(this.FilePath, newFilePath);
			HelperClasses.FileHandling.copyFile(this.FilePathBak, newFilePath + ".bak");
			GTASaves.Add(new MySaveFile(newFilePath));
		}


		/// <summary>
		/// Renaming one MySaveFile, all User error checks already done before
		/// </summary>
		/// <param name="pNewName"></param>
		public void Rename(string pNewName)
		{
			HelperClasses.Logger.Log("Renaming SaveFiles '" + this.FileName + "' to Name '" + pNewName + "'");

			string oldFileName = HelperClasses.FileHandling.PathSplitUp(this.FilePath)[1];
			string newFilePath = this.FilePath.Replace(oldFileName, pNewName);
			HelperClasses.FileHandling.moveFile(this.FilePath, newFilePath);
			HelperClasses.FileHandling.moveFile(this.FilePathBak, newFilePath + ".bak");
			this.MyCollection.Remove(this);
			this.MyCollection.Add(new MySaveFile(newFilePath));
		}


		/// <summary>
		/// Method to delete one MySaveFile, all its physical files and removing it from its own collection
		/// </summary>
		public void Delete()
		{
			HelperClasses.Logger.Log("Deleting SaveFiles '" + this.FileName + "'");

			HelperClasses.FileHandling.deleteFile(this.FilePath);
			HelperClasses.FileHandling.deleteFile(this.FilePathBak);
			this.MyCollection.Remove(this);
		}

		/// <summary>
		/// Importing a pair of SaveFiles
		/// </summary>
		/// <param name="pFilePath"></param>
		public static void Import(string pOriginalFilePath, string pFileName)
		{
			HelperClasses.Logger.Log("Importing SaveFiles '" + pOriginalFilePath + "'");

			string newFilePath = MySaveFile.CurrentBackupSavesPath.TrimEnd('\\') + @"\" + pFileName;
			HelperClasses.FileHandling.copyFile(pOriginalFilePath, newFilePath);
			HelperClasses.FileHandling.copyFile(pOriginalFilePath + ".bak", newFilePath + ".bak");
			MySaveFile.BackupSaves.Add(new MySaveFile(newFilePath));
		}

	} // End of Class
} // End of Namespace
