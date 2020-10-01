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
		/// FileName Property.
		/// </summary>
		public string FilePath { private set; get; }

		/// <summary>
		/// FileName Property.
		/// </summary>
		public string FileName { get { return FilePath.Substring(FilePath.LastIndexOf('\\') + 1) + ""; } }

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
	
    } // End of Class
} // End of Namespace
