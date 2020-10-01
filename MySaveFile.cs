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
		/// Collection of "MyFile" which are used for the Save-Files in the GTA Folder.
		/// </summary>
		public static ObservableCollection<MySaveFile> GTASaves = new ObservableCollection<MySaveFile>();

		/// <summary>
		/// Path for the SaveFiles inside GTAV Installation Location
		/// </summary>
		public static string GTAVSavesPath = @"C:\Users\ingow\Documents\Rockstar Games\GTA V\Profiles\Project127\GTA V\0F74F4C4";

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
			//set
			//{
			//	FileName = this.FilePath.Substring(this.FilePath.LastIndexOf('\\') + 1) + "";
			//}
		}

		/// <summary>
		/// Filename of the same with the .bak addon
		/// </summary>
		public string FilePathBak { get { return FilePath + ".bak"; } }

		/// <summary>
		/// PathName Property
		/// </summary>
		public string Pathname { private set; get; }

		/// <summary>
		/// Standart Constructor. Dont need to forbid default Constructor since it wont be generated.
		/// </summary>
		/// <param name="pFilePath"></param>
		public MySaveFile(string pFilePath)
		{
			this.FilePath = pFilePath;
		}


		public void MoveToBackup(string pNewName)
		{
			GTASaves.Remove(this);
			string newFilePath = MySaveFile.BackupSavesPath.TrimEnd('\\') + @"\" + pNewName;
			HelperClasses.FileHandling.moveFile(this.FilePath, newFilePath);
			HelperClasses.FileHandling.moveFile(this.FilePathBak, newFilePath + ".bak");
			BackupSaves.Add(new MySaveFile(newFilePath));
		}


		public void MoveToGTA(string pNewName)
		{
			BackupSaves.Remove(this);
			string newFilePath = MySaveFile.GTAVSavesPath.TrimEnd('\\') + @"\" + pNewName;
			HelperClasses.FileHandling.moveFile(this.FilePath, newFilePath);
			HelperClasses.FileHandling.moveFile(this.FilePathBak, newFilePath + ".bak");
			GTASaves.Add(new MySaveFile(newFilePath));
		}

		public void Rename(string pNewName)
		{
			BackupSaves.Remove(this);
			string oldFileName = this.FileName;
			string newFilePath = this.FilePath.Replace(oldFileName, pNewName);
			HelperClasses.FileHandling.moveFile(this.FilePath, newFilePath);
			HelperClasses.FileHandling.moveFile(this.FilePathBak, newFilePath + ".bak");
			BackupSaves.Add(new MySaveFile(newFilePath));
		}


		public void Delete()
		{
			HelperClasses.FileHandling.deleteFile(this.FilePath);
			HelperClasses.FileHandling.deleteFile(this.FilePathBak);
			GTASaves.Remove(this);
		}

	} // End of Class
} // End of Namespace
