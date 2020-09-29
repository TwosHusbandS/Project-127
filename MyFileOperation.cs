using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127
{
	/// <summary>
	/// Class
	/// </summary>
	public class MyFileOperation
	{
		/// <summary>
		/// Enum FileOperations
		/// </summary>
		public enum FileOperations
		{
			Copy,
			Move,
			Hardlink,
			Delete
		}

		/// <summary>
		/// Property of Class Instance, File Operation Type
		/// </summary>
		public FileOperations FileOperation { get; private set; }

		/// <summary>
		/// Property of Class Instance. Name of File
		/// </summary>
		public string OriginalFile { get; private set; }

		/// <summary>
		/// Property of Class Instance. New File Location (for Moving, Copying, Hardlinking)
		/// </summary>
		public string NewFile { get; private set; }

		/// <summary>
		/// Property of Class Instance. The Log message which should appear
		/// </summary>
		public string Log { get; private set; }

		/// <summary>
		/// The loglevel for the LogMessage
		/// </summary>
		public int LogLevel { get; private set; }

		/// <summary>
		/// Constructor, creates one MyFileOperation Object
		/// </summary>
		/// <param name="pFileOperation"></param>
		/// <param name="pOriginalFile"></param>
		/// <param name="pNewFile"></param>
		/// <param name="pLog"></param>
		/// <param name="pLogLevel"></param>
		public MyFileOperation(FileOperations pFileOperation, string pOriginalFile, string pNewFile, string pLog, int pLogLevel)
		{
			FileOperation = pFileOperation;
			OriginalFile = pOriginalFile;
			NewFile = pNewFile;
			Log = pLog;
			LogLevel = pLogLevel;
		}

		/// <summary>
		/// Executes one MyFileOperationObject
		/// </summary>
		/// <param name="pMyFileOperation"></param>
		public static void Execute(MyFileOperation pMyFileOperation)
		{
			switch (pMyFileOperation.FileOperation)
			{
				case FileOperations.Copy:
					{
						HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
						HelperClasses.FileHandling.copyFile(pMyFileOperation.OriginalFile, pMyFileOperation.NewFile);
						break;
					}
				case FileOperations.Move:
					{
						HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
						HelperClasses.FileHandling.moveFile(pMyFileOperation.OriginalFile, pMyFileOperation.NewFile);
						break;
					}
				case FileOperations.Hardlink:
					{
						if (Settings.EnableCopyFilesInsteadOfHardlinking)
						{
							HelperClasses.Logger.Log(Globals.ReplaceCaseInsensitive(pMyFileOperation.Log, "hardlink", "Copy"), pMyFileOperation.LogLevel);
							HelperClasses.FileHandling.copyFile(pMyFileOperation.OriginalFile, pMyFileOperation.NewFile);
						}
						else
						{
							HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
							HelperClasses.FileHandling.HardLinkFiles(pMyFileOperation.NewFile, pMyFileOperation.OriginalFile);
						}
						break;
					}
				case FileOperations.Delete:
					{
						HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
						HelperClasses.FileHandling.deleteFile(pMyFileOperation.OriginalFile);
						break;
					}
				default: break;
			}
		}

	}
}
