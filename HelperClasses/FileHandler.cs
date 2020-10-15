using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
		public static string OpenDialogExplorer(PathDialogType pPathDialogType, string pTitle, string pStartLocation, bool pMultiSelect = false, string pFilter = null)
		{
			if (pPathDialogType == PathDialogType.File)
			{
				OpenFileDialog myFileDialog = new OpenFileDialog();
				myFileDialog.Filter = pFilter;
				myFileDialog.InitialDirectory = pStartLocation;
				myFileDialog.Title = pTitle;
				myFileDialog.Multiselect = pMultiSelect;

				myFileDialog.ShowDialog();

				return string.Join(",", myFileDialog.FileNames);
			}
			else if (pPathDialogType == PathDialogType.Folder)
			{
				var fsd = new OwnOpenFolderDialog.FolderSelectDialog(pTitle, pStartLocation);
				fsd.ShowDialog();
				return fsd.FileName;
			}
			return "";
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



		public static void createPathOfFile(string pFilePath)
		{
			string path = pFilePath.Substring(0, pFilePath.LastIndexOf('\\'));
			createPath(path);
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
		/// Gets the Creation Date of one file in "yyyy-MM-ddTHH:mm:ss" Format
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static string GetCreationDate(string pFilePath)
		{
			string rtrn = "";
			if (doesFileExist(pFilePath))
			{
				try
				{
					DateTime creation = File.GetLastWriteTime(pFilePath);
					rtrn = creation.ToString("yyyy-MM-ddTHH:mm:ss");
				}
				catch
				{
					HelperClasses.Logger.Log("Getting Creation Date of File: '" + pFilePath + "' failed.");
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Getting Creation Date of File: '" + pFilePath + "' failed.").ShowDialog();
				}
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



		public static void WriteStringToFileOverwrite(string pFilePath, string[] pContent)
		{

			if (doesFileExist(pFilePath))
			{
				deleteFile(pFilePath);
			}
			File.WriteAllLines(pFilePath, pContent);
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
				return new string[0];
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
		/// Gets the MD5 Hash of one file
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <returns></returns>
		public static string GetHashFromFile(string pFilePath)
		{
			string rtrn = "";
			if (doesFileExist(pFilePath))
			{
				try
				{
					using (var md5 = MD5.Create())
					{
						// hash contents
						byte[] contentBytes = File.ReadAllBytes(pFilePath);
						md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
						md5.TransformFinalBlock(new byte[0], 0, 0);
						return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
					}
				}
				catch (Exception e)
				{
					HelperClasses.Logger.Log("Hashing of a single file failed." + e.ToString());
				}


			}
			return rtrn;
		}


		/// <summary>
		/// Method to get Hash from a Folder
		/// </summary>
		/// <param name="srcPath"></param>
		/// <returns></returns>
		public static string CreateDirectoryMd5(string srcPath)
		{
			if (!doesPathExist(srcPath))
			{
				Logger.Log("ZIP Extraction Path is not a valid Filepath...wut");
				Globals.DebugPopup("Yeah shit broke...You gotta uninstall\nvia Uninstaller from Github, bro...");
				Environment.Exit(5);
			}

			var myFiles = Directory.GetFiles(srcPath, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();

			using (var md5 = MD5.Create())
			{
				foreach (var myFile in myFiles)
				{
					// hash path
					byte[] pathBytes = Encoding.UTF8.GetBytes(myFile);
					md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

					// hash contents
					byte[] contentBytes = File.ReadAllBytes(myFile);

					md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
				}

				//Handles empty filePaths case
				md5.TransformFinalBlock(new byte[0], 0, 0);

				return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
			}
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
		try
		{

			if (doesFileExist(pFilePath))
			{
				File.Delete(pFilePath);

			}
		}
		catch (Exception e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOkError, "Deleting File failed ('" + pFilePath + "').\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
			HelperClasses.Logger.Log("Deleting File failed ('" + pFilePath + "').", true, 0);
		}
	}


	/// <summary>
	/// Copy File A to file B. Does not overwrite
	/// </summary>
	/// <param name="pSource"></param>
	/// <param name="pDestination"></param>
	public static void copyFile(string pSource, string pDestination)
	{
		if (!File.Exists(pSource))
		{
			HelperClasses.Logger.Log("Copying File ['" + pSource + "' to '" + pDestination + "'] failed since SourceFile ('" + pSource + "') does NOT exist.", true, 0);
			return;
		}
		if (File.Exists(pDestination))
		{
			HelperClasses.Logger.Log("Copying File ['" + pSource + "' to '" + pDestination + "'] failed since DestinationFile ('" + pDestination + "') DOES exist.", true, 0);
			return;
		}
		try
		{
			File.Copy(pSource, pDestination);
		}
		catch (Exception e)
		{
			HelperClasses.Logger.Log("Copying File ['" + pSource + "' to '" + pDestination + "'] failed since trycatch failed." + e.Message.ToString(), true, 0);
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


	public static void DeleteFolder(string pPath)
	{
		try
		{
			if (doesPathExist(pPath))
			{
				Directory.Delete(pPath, true);
			}
		}
		catch (Exception e)
		{
			HelperClasses.Logger.Log("Failed to delete Folder for some reason. " + e.ToString());
		}
	}

	/// <summary>
	/// Creates a Path. Works for SubSubPaths.
	/// </summary>
	/// <param name="pFolderPath"></param>
	public static void createPath(string pFolderPath)
	{
		try
		{
			if (!doesPathExist(pFolderPath))
			{
				Directory.CreateDirectory(pFolderPath);
			}
		}
		catch (Exception e)
		{
			HelperClasses.Logger.Log("The code looked good to me. #SadFace. Crashing while creating Path ('" + pFolderPath + "'): " + e.ToString());
		}
	}


	public static void CreateAllZIPPaths(string pZIPFileExtractLocation)
	{
		// TODO, CTRLF FIX THIS MESS. OTHERWISE ZIP EXTRACTING SHIT WILL BREAK BECAUSE ITS A PIECE OF SHIT
		HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files");
		HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles");
		HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles\update");
		HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles");
		HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles\update");
		HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\");
		HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\Installer");
		HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\SaveFiles");
	}

	/// <summary>
	/// Deletes a File
	/// </summary>
	/// <param name="pFilePath"></param>
	public static void deletePath(string pFilePath)
	{
		if (doesPathExist(pFilePath))
		{
			Directory.Delete(pFilePath, true);
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
