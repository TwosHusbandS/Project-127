using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127
{
	public class MyFileOperation
	{
		public enum FileOperations
		{
			Copy,
			Move,
			Hardlink,
			Delete
		}

		public FileOperations FileOperation { get; private set; }
		public string OriginalFile { get; private set; }
		public string NewFile { get; private set; }
		public string Log { get; private set; }
		public int LogLevel { get; private set; }

		public MyFileOperation(FileOperations pFileOperation, string pOriginalFile, string pNewFile, string pLog, int pLogLevel)
		{
			FileOperation = pFileOperation;
			OriginalFile = pOriginalFile;
			NewFile = pNewFile;
			Log = pLog;
			LogLevel = pLogLevel;
		}

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
