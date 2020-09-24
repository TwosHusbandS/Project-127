using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Class for FileHandling
	/// </summary>
	static class FileHandling
	{

		/// <summary>
		/// Reference to the Method which creates the hardlinks
		/// </summary>
		/// <param name="lpFileName"></param>
		/// <param name="lpExistingFileName"></param>
		/// <param name="lpSecurityAttributes"></param>
		/// <returns></returns>
		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

		/// <summary>
		/// Enum of PathDialogType to open a FileExplorer or a FolderExplorer depending on which we need.
		/// </summary>
		public enum PathDialogType
		{
			File,
			Folder
		}

		/// <summary>
		/// Opens Dialog for File or Folder picker. Returns "" if nothing is picked.
		/// </summary>
		/// <param name="pPathDialogType"></param>
		/// <param name="pTitle"></param>
		/// <param name="pFilter"></param>
		/// <param name="pStartLocation"></param>
		/// <returns></returns>
		public static string OpenDialogExplorer(PathDialogType pPathDialogType, string pTitle, string pFilter, string pStartLocation)
		{
			if (pPathDialogType == PathDialogType.File)
			{
				OpenFileDialog myFileDialog = new OpenFileDialog();
				myFileDialog.Filter = pFilter;
				myFileDialog.InitialDirectory = pStartLocation;
				myFileDialog.Title = pTitle;
				myFileDialog.Multiselect = false;

				myFileDialog.ShowDialog();

				return myFileDialog.FileName;
			}
			else if (pPathDialogType == PathDialogType.Folder)
			{
				// TODO CTRLF

				// WIP, Will be the pretty version with better UX in the feature
				FolderBrowserDialog myFolderDialog = new FolderBrowserDialog();
				myFolderDialog.Description = pTitle;
				myFolderDialog.ShowNewFolderButton = true;
				// myFolderDialog.Something = pStartLocation;

				myFolderDialog.ShowDialog();

				return myFolderDialog.SelectedPath;
			}
			return "";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pPathDialogType"></param>
		/// <param name="pTitle"></param>
		/// <param name="pStartLocation"></param>
		/// <returns></returns>
		public static string OpenDialogExplorer(PathDialogType pPathDialogType, string pTitle, string pStartLocation)
		{
			return OpenDialogExplorer(pPathDialogType, pTitle, "Executable files(*.exe *) | *.exe *", pStartLocation);
		}

		/// <summary>
		/// Gets all the Files in one Folder (and its Subfolders)
		/// </summary>
		/// <param name="pPath"></param>
		/// <returns></returns>
		public static string[] GetFilesFromFolderAndSubFolder(string pPath)
		{
			if (doesPathExist(pPath))
			{
				return (Directory.GetFiles(pPath, "*", SearchOption.AllDirectories));
			}
			else
			{
				return (new string[0]);
			}
		}

		/// <summary>
		/// Gets all the Files in one Folder (NOT Subfolders)
		/// </summary>
		/// <param name="pPath"></param>
		/// <returns></returns>
		public static string[] GetFilesFromFolder(string pPath)
		{
			if (doesPathExist(pPath))
			{
				return (Directory.GetFiles(pPath, "*", SearchOption.TopDirectoryOnly));
			}
			else
			{
				return (new string[0]);
			}
		}

		/// <summary>
		/// Reads the content of a File. In one string (not string array for line)
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static string ReadContentOfFile(string pFilePath)
		{
			string rtrn = "";

			if (doesFileExist(pFilePath))
			{
				rtrn = File.ReadAllText(pFilePath);
			}

			return rtrn;
		}

		/// <summary>
		/// Creates Hardlink A for File B
		/// </summary>
		/// <param name="pLinkFilePath"></param>
		/// <param name="pRealFilePath"></param>
		public static void HardLinkFiles(string pLinkFilePath, string pRealFilePath)
		{
			// If the file already exists, we delete it
			if (doesFileExist(pLinkFilePath))
			{
				HelperClasses.Logger.Log("Creating Hardlink in: '" + pLinkFilePath + "' for existing file '" + pRealFilePath + "'. Target File already exists. Im deleting it. YOLO.", true, 0);
				deleteFile(pLinkFilePath);
			}

			// Try to Hardlink
			try
			{
				// String of Folder of Hardlink
				string[] sth = PathSplitUp(pLinkFilePath);

				// Create the folder (if it doesnt exist...
				createPath(sth[0]);

				//Creating the actual Hardlink
				CreateHardLink(pLinkFilePath, pRealFilePath, IntPtr.Zero);
			}
			catch (Exception e)
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Creating Hardlink in: '" + pLinkFilePath + "' for existing file '" + pRealFilePath + "' failed.\nI suggest you restart the Program (maybe Repair) and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
				HelperClasses.Logger.Log("Creating Hardlink in: '" + pLinkFilePath + "' for existing file '" + pRealFilePath + "' failed", true, 0);
			}
		}

		/// <summary>
		/// Moves a File from A to B
		/// </summary>
		/// <param name="pMoveFromFilePath"></param>
		/// <param name="pMoveToFilePath"></param>
		public static void moveFile(string pMoveFromFilePath, string pMoveToFilePath)
		{
			if (doesFileExist(pMoveToFilePath))
			{
				HelperClasses.Logger.Log("Moving File: '" + pMoveFromFilePath + "' to '" + pMoveToFilePath + "'. TargetFile exists, so im deleting it. YOLO", true, 0);
				deleteFile(pMoveToFilePath);
			}
			try
			{
				string[] sth = PathSplitUp(pMoveToFilePath);
				createPath(sth[0]);
				File.Move(pMoveFromFilePath, pMoveToFilePath);
			}
			catch (Exception e)
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Moving File: '" + pMoveFromFilePath + "' to '" + pMoveToFilePath + "' failed.\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
				HelperClasses.Logger.Log("Moving File: '" + pMoveFromFilePath + "' to '" + pMoveToFilePath + "' failed.", true, 0);
			}
		}

		/// <summary>
		/// Method we use to add one line of text as a new line to a text file
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <param name="pLineContent"></param>
		public static void AddToLog(string pFilePath, string pLineContent)
		{
			// Should be quicker than checking if File exists, and also checks for more erros
			try
			{
				StreamWriter sw;
				sw = File.AppendText(pFilePath);
				sw.Write(pLineContent + Environment.NewLine);
				sw.Close();
			}
			catch (Exception e)
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Writing to Log failed. File was probably deleted after start of Program.\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
				System.Windows.Forms.MessageBox.Show("Writing to Log failed. File was probably deleted after start of Program.\n\nLogmessage:'" + pLineContent + "'\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString());
			}
		}

		// A lot of self written functions for File stuff below.
		// Lots of overloaded stuff. Nothing checks for errors, like file permissions or sth.

		/// <summary>
		/// ReadFileEachLine
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static string[] ReadFileEachLine(string pFilePath)
		{
			if (doesFileExist(pFilePath))
			{
				return File.ReadAllLines(pFilePath);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// GetXMLTagContent
		/// </summary>
		/// <param name="pXML"></param>
		/// <param name="pTag"></param>
		/// <returns></returns>
		public static string GetXMLTagContent(string pXML, string pTag)
		{
			string rtrn = "";

			Regex regex = new Regex(@"<" + pTag + ">.+</" + pTag + ">");
			Match match = regex.Match(pXML);

			if (match.Success)
			{
				string tmp = match.Value;

				rtrn = tmp.Substring(tmp.IndexOf('>') + 1, tmp.LastIndexOf('<') - 2 - pTag.Length);
			}

			return rtrn;
		}


		/// <summary>
		/// Gets String from URL
		/// </summary>
		/// <param name="pURL"></param>
		/// <returns></returns>
		public static string GetStringFromURL(string pURL)
		{
			string rtrn = "";

			try
			{
				rtrn = new System.Net.Http.HttpClient().GetStringAsync(pURL).GetAwaiter().GetResult();
			}
			catch (Exception e)
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "GetStringFromURL failed. Probably Network related. URL = '" + pURL + "'.\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
				HelperClasses.Logger.Log("GetStringFromURL failed. Probably Network related. URL = '" + pURL + "'", true, 0);
			}

			return rtrn;
		}



		/// <summary>
		/// Makes "C:\Some\Path(\)" and "Somefile.txt" into "C:\Some\Path\Somefile.txt". Doesnt check for errors.
		/// </summary>
		/// <param name="pPath"></param>
		/// <param name="pFile"></param>
		/// <returns></returns>
		public static string PathCombine(string pPath, string pFile)
		{
			return pPath.TrimEnd('\\') + @"\" + pFile;
		}

		/// <summary>
		/// Makes "C:\Some\Path\Somefile.txt" into "C:\Some\Path" and "Somefile.txt". The last "\" disappears. Doesnt check for errors.
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static string[] PathSplitUp(string pFilePath)
		{
			string[] rtrn = new string[2];
			rtrn[0] = pFilePath.Substring(0, pFilePath.LastIndexOf('\\'));
			rtrn[1] = pFilePath.Substring(pFilePath.LastIndexOf('\\') + 1);
			return rtrn;
		}

		/// <summary>
		/// Gets the Size of one File in Bytes as Long
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static long GetSizeOfFile(string pFilePath)
		{
			long mySize = 0;
			if (doesFileExist(pFilePath))
			{
				FileInfo myFileInfo = new FileInfo(pFilePath);
				mySize = myFileInfo.Length;
			}
			return mySize;
		}

		/// <summary>
		/// read File
		/// </summary>
		/// <param name="pPath"></param>
		/// <param name="pFile"></param>
		/// <returns></returns>
		public static List<string> ReadFile(string pPath, string pFile)
		{
			return ReadFile(PathCombine(pPath, pFile));
		}

		/// <summary>
		/// readFile
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static List<string> ReadFile(string pFilePath)
		{
			List<string> rtrnList = new List<string>();
			StreamReader sr;
			string currentLine;
			bool doesLineExist = true;
			if (doesFileExist(pFilePath))
			{
				sr = new StreamReader(pFilePath);
				while (doesLineExist)
				{
					doesLineExist = false;
					currentLine = sr.ReadLine();
					if (currentLine != null && currentLine != "")
					{
						rtrnList.Add(currentLine);
						doesLineExist = true;
					}
				}
				sr.Close();
			}
			return rtrnList;
		}

		/// <summary>
		/// deleteFile
		/// </summary>
		/// <param name="pPath"></param>
		/// <param name="pFile"></param>
		public static void deleteFile(string pPath, string pFile)
		{
			deleteFile(PathCombine(pPath, pFile));
		}

		/// <summary>
		/// deleteFile
		/// </summary>
		/// <param name="pFilePath"></param>
		public static void deleteFile(string pFilePath)
		{
			if (doesFileExist(pFilePath))
			{
				try
				{
					File.Delete(pFilePath);
				}
				catch (Exception e)
				{
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Deleting File failed ('" + pFilePath + "').\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
					HelperClasses.Logger.Log("Deleting File failed ('" + pFilePath + "').", true, 0);
				}
			}
		}

		/// <summary>
		/// Creates File (and Folder(s). Overrides existing file.
		/// </summary>
		/// <param name="pPath"></param>
		/// <param name="pFile"></param>
		public static void createFile(string pPath, string pFile)
		{
			if (!doesPathExist(pPath))
			{
				createPath(pPath);
			}
			else if (doesFileExist(pPath, pFile))
			{
				deleteFile(pPath, pFile);
			}

			try
			{
				File.CreateText(PathCombine(pPath, pFile)).Close();
			}
			catch (Exception e)
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Create File failed ('" + PathCombine(pPath, pFile) + "').\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
				HelperClasses.Logger.Log("Create File failed ('" + PathCombine(pPath, pFile) + "').", true, 0);
			}
		}

		/// <summary>
		/// Creates File (and Folder(s)). Overrides existing file.
		/// </summary>
		/// <param name="pFilePath"></param>
		public static void createFile(string pFilePath)
		{
			string[] paths = PathSplitUp(pFilePath);
			createFile(paths[0], paths[1]);
		}

		/// <summary>
		/// Checks if a File exists.
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static bool doesFileExist(string pFilePath)
		{
			return File.Exists(pFilePath);
		}

		/// <summary>
		/// Checks if a File exists.
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static bool doesFileExist(string pPath, string pFile)
		{
			return doesFileExist(PathCombine(pPath, pFile));
		}

		/// <summary>
		/// Checks if a path exists.
		/// </summary>
		/// <param name="pFolderPath"></param>
		/// <returns></returns>
		public static bool doesPathExist(string pFolderPath)
		{
			return Directory.Exists(pFolderPath);
		}

		/// <summary>
		/// Creates a Path. Works for SubSubPaths.
		/// </summary>
		/// <param name="pFolderPath"></param>
		public static void createPath(string pFolderPath)
		{
			Directory.CreateDirectory(pFolderPath);
		}

		/// <summary>
		/// Deletes a File
		/// </summary>
		/// <param name="pFilePath"></param>
		public static void deletePath(string pFilePath)
		{
			if (doesFileExist(pFilePath))
			{
				File.Delete(pFilePath);
			}
		}

		// ALL SORTS OF RANDOM METHODS

		/// <summary>
		/// String.TrimEnd() now works with a string as well with chars
		/// </summary>
		/// <param name="input"></param>
		/// <param name="suffixToRemove"></param>
		/// <param name="comparisonType"></param>
		/// <returns></returns>
		public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			if (suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
			{
				return input.Substring(0, input.Length - suffixToRemove.Length);
			}

			return input;
		}

	} // End of Class
} // End of NameSpace
