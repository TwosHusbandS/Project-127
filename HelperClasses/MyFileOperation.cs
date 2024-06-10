using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Class of MyFileOperation
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
			Delete,
			Create
		}

		/// <summary>
		/// Enum FileOrFolder
		/// </summary>
		public enum FileOrFolder
		{
			File,
			Folder
		}

		/// <summary>
		/// Property of Class Instance,  FileOrFolder Type
		/// </summary>
		public FileOrFolder MyFileOrFolder { get; private set; }


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

		public int RetryAttempts { get; private set; }

        public int MSDelayBetweenAttempts { get; private set; }


        /// <summary>
        /// Constructor, creates one MyFileOperation Object
        /// </summary>
        /// <param name="pFileOperation"></param>
        /// <param name="pOriginalFile"></param>
        /// <param name="pNewFile"></param>
        /// <param name="pLog"></param>
        /// <param name="pLogLevel"></param>
        public MyFileOperation(FileOperations pFileOperation, string pOriginalFile, string pNewFile, string pLog, int pLogLevel, FileOrFolder pFileOrFolder = FileOrFolder.File, int pRetryAttempts = 10, int pMSDelayBetweenAttempts = 100)
		{
			FileOperation = pFileOperation;
			OriginalFile = pOriginalFile;
			NewFile = pNewFile;
			Log = pLog;
			LogLevel = pLogLevel;
			MyFileOrFolder = pFileOrFolder;
			RetryAttempts = pRetryAttempts;
			MSDelayBetweenAttempts = pMSDelayBetweenAttempts;
		}


		public static void ExecuteWrapper(MyFileOperation pMyFileOperation)
		{
			HelperClasses.FileHandling.AddToDebug("    ");
			HelperClasses.FileHandling.AddToDebug(pMyFileOperation.FileOperation.ToString() + " | " + pMyFileOperation.OriginalFile);
			HelperClasses.FileHandling.AddToDebug(pMyFileOperation.Log);
			while (pMyFileOperation.RetryAttempts > 0)
			{
                try
                {
                    HelperClasses.FileHandling.AddToDebug("RetryAttempts: " + pMyFileOperation.RetryAttempts.ToString());
                    Execute(pMyFileOperation, true);
					return;
                }
                catch (Exception ex)
                { 
                    HelperClasses.FileHandling.AddToDebug("Failed");
                    pMyFileOperation.RetryAttempts -= 1;

					Task.Delay(pMyFileOperation.MSDelayBetweenAttempts).GetAwaiter().GetResult();
					HelperClasses.Logger.Log("Retrying file operation...");
                }
            }
            HelperClasses.Logger.Log("Hail Mary Execute");

            Execute(pMyFileOperation);
        }

        /// <summary>
        /// Executes one MyFileOperationObject
        /// </summary>
        /// <param name="pMyFileOperation"></param>
        public static void Execute(MyFileOperation pMyFileOperation, bool pRaiseException = false)
		{
			switch (pMyFileOperation.FileOperation)
			{
				case FileOperations.Copy:
					{
						HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
						if (pMyFileOperation.MyFileOrFolder == FileOrFolder.File)
						{
							HelperClasses.FileHandling.copyFile(pMyFileOperation.OriginalFile, pMyFileOperation.NewFile, RaiseException: pRaiseException);
						}
						else
						{
							HelperClasses.FileHandling.CopyPath(pMyFileOperation.OriginalFile, pMyFileOperation.NewFile, RaiseException: pRaiseException);
						}
						break;
					}
				case FileOperations.Create:
					{
						HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
						if (pMyFileOperation.MyFileOrFolder == FileOrFolder.File)
						{
							HelperClasses.FileHandling.createFile(pMyFileOperation.OriginalFile, RaiseException: pRaiseException);
						}
						else
						{
							HelperClasses.FileHandling.createPath(pMyFileOperation.OriginalFile, RaiseException: pRaiseException);
						}
						break;
					}
				case FileOperations.Move:
					{
						HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
						if (pMyFileOperation.MyFileOrFolder == FileOrFolder.File)
						{
							HelperClasses.FileHandling.moveFile(pMyFileOperation.OriginalFile, pMyFileOperation.NewFile, RaiseException: pRaiseException);
						}
						else
						{
							HelperClasses.FileHandling.movePath(pMyFileOperation.OriginalFile, pMyFileOperation.NewFile, RaiseException: pRaiseException);
						}
						break;
					}
				case FileOperations.Hardlink:
					{
						if (pMyFileOperation.MyFileOrFolder == FileOrFolder.File)
						{
							if (pMyFileOperation.NewFile.ToLower().Contains(Settings.GTAVInstallationPath.ToLower().TrimEnd('\\')))
							{
								if (Settings.EnableCopyFilesInsteadOfHardlinking)
								{
									HelperClasses.Logger.Log(Globals.ReplaceCaseInsensitive(pMyFileOperation.Log, "hardlink", "Copy"), pMyFileOperation.LogLevel);
									HelperClasses.FileHandling.copyFile(pMyFileOperation.OriginalFile, pMyFileOperation.NewFile, RaiseException: pRaiseException);
								}
								else
								{
									HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
									HelperClasses.FileHandling.HardLinkFiles(pMyFileOperation.NewFile, pMyFileOperation.OriginalFile, RaiseException: pRaiseException);
								}
							}
							else
							{
								HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
								HelperClasses.FileHandling.HardLinkFiles(pMyFileOperation.NewFile, pMyFileOperation.OriginalFile, RaiseException: pRaiseException);
							}
						}
						else
						{
                            Globals.PopupError("No idea what happened here...MyFileOperation Execute Hardlink Folder");
						}
						break;
					}
				case FileOperations.Delete:
					{
						HelperClasses.Logger.Log(pMyFileOperation.Log, pMyFileOperation.LogLevel);
						if (pMyFileOperation.MyFileOrFolder == FileOrFolder.File)
						{
							HelperClasses.FileHandling.deleteFile(pMyFileOperation.OriginalFile, RaiseException: pRaiseException);
						}
						else
						{
							HelperClasses.FileHandling.DeleteFolder(pMyFileOperation.OriginalFile, RaiseException: pRaiseException);
						}
						break;
					}
				default: break;
			}
		}

	}
}
