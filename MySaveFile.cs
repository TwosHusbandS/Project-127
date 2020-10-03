using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		/// Path for the SaveFiles we provide
		/// </summary>
		public static string BackupSavesPath = LauncherLogic.SaveFilesPath;

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
				return this.FilePath.Substring(this.FilePath.LastIndexOf('\\') + 1) + "";
			}
		}

		/// <summary>
		/// Filename of the same with the .bak addon
		/// </summary>
		public string FilePathBak { get { return FilePath + ".bak"; } }

		/// <summary>
		/// PathName Property
		/// </summary>
		public string Path { get { return this.FilePath.Substring(0,this.FilePath.LastIndexOf('\\')); } }

		/// <summary>
		/// Standart Constructor. Dont need to forbid default Constructor since it wont be generated.
		/// </summary>
		/// <param name="pFilePath"></param>
		public MySaveFile(string pFilePath)
		{
			this.FilePath = pFilePath;
		}


		public void CopyToBackup(string pNewName)
		{
			HelperClasses.Logger.Log("CopyToBackup():", 2);
			string newFilePath = MySaveFile.BackupSavesPath.TrimEnd('\\') + @"\" + pNewName;
			HelperClasses.Logger.Log("newFilePath: '" + newFilePath + "'", 3);
			HelperClasses.Logger.Log("Copying '" + this.FilePath + "' to '" + newFilePath + "'", 3);
			HelperClasses.FileHandling.copyFile(this.FilePath, newFilePath);
			HelperClasses.Logger.Log("Copying '" + this.FilePathBak + "' to '" + newFilePath + ".bak" + "'", 3);
			HelperClasses.FileHandling.copyFile(this.FilePathBak, newFilePath + ".bak");
			HelperClasses.Logger.Log("Adding it to BackUpSaves Collection", 3);
			BackupSaves.Add(new MySaveFile(newFilePath));
		}


		public void CopyToGTA(string pNewName)
		{
			HelperClasses.Logger.Log("CopyToBackup():", 2);
			string newFilePath = MySaveFile.GTAVSavesPath.TrimEnd('\\') + @"\" + pNewName;
			HelperClasses.Logger.Log("newFilePath: '" + newFilePath + "'", 3);
			HelperClasses.Logger.Log("Copying '" + this.FilePath + "' to '" + newFilePath + "'", 3);
			HelperClasses.FileHandling.copyFile(this.FilePath, newFilePath);
			HelperClasses.Logger.Log("Copying '" + this.FilePathBak + "' to '" + newFilePath + ".bak" + "'", 3);
			HelperClasses.FileHandling.copyFile(this.FilePathBak, newFilePath + ".bak");
			HelperClasses.Logger.Log("Adding it to GTASaves Collection", 3);
			GTASaves.Add(new MySaveFile(newFilePath));
		}



		public void Rename(string pNewName)
		{
			HelperClasses.Logger.Log("Rename():", 2);
			string oldFileName = this.FileName;
			HelperClasses.Logger.Log("oldFileName: '" + oldFileName + "'", 3);
			string newFilePath = this.FilePath.Replace(oldFileName, pNewName);
			HelperClasses.Logger.Log("newFilePath: '" + newFilePath + "'", 3);
			HelperClasses.Logger.Log("Moving '" + this.FilePath + "' to '" + newFilePath + "'", 3);
			HelperClasses.FileHandling.moveFile(this.FilePath, newFilePath);
			HelperClasses.Logger.Log("Moving '" + this.FilePathBak + "' to '" + newFilePath + ".bak" + "'", 3);
			HelperClasses.FileHandling.moveFile(this.FilePathBak, newFilePath + ".bak");
			HelperClasses.Logger.Log("Removing ourselfes from our Own collection", 3);
			this.MyCollection.Remove(this);
			HelperClasses.Logger.Log("Adding new MySaveFile('" + newFilePath +"') to our Collection", 3);
			this.MyCollection.Add(new MySaveFile(newFilePath));
		}


		public void Delete()
		{
			HelperClasses.Logger.Log("Delete():", 2);
			HelperClasses.Logger.Log("Deleting '" + this.FilePath + "'", 3);
			HelperClasses.FileHandling.deleteFile(this.FilePath);
			HelperClasses.Logger.Log("Deleting '" + this.FilePathBak + "'", 3);
			HelperClasses.FileHandling.deleteFile(this.FilePathBak);
			HelperClasses.Logger.Log("Removing ourselfes from our Own collection", 3);
			this.MyCollection.Remove(this);
		}

	} // End of Class
} // End of Namespace
