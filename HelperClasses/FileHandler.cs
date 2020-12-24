using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Diagnostics;
using System.Net;

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

		public static string SaveFileDialog(string Title, string Filter)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			saveFileDialog.Filter = Filter;
			saveFileDialog.Title = Title;
			saveFileDialog.ShowDialog();
			return saveFileDialog.FileName;
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
		/// Creates the Path of a FilePath
		/// </summary>
		/// <param name="pFilePath"></param>
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

		/// <summary>
		/// Moving a Path. Moving (Cut, Paste a Folder) or Rename a folder
		/// </summary>
		/// <param name="Source"></param>
		/// <param name="Dest"></param>
		public static void movePath(string Source, string Dest)
		{
			try
			{
				if (doesPathExist(Source))
				{
					if (doesPathExist(Dest))
					{
						DeleteFolder(Dest);
					}
					Directory.Move(Source, Dest);
				}
			}
			catch (Exception e)
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Moving Path: '" + Source + "' to '" + Dest + "' failed.\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
				HelperClasses.Logger.Log("Moving Path: '" + Source + "' to '" + Dest + "' failed.", true, 0);
			}
		}

		/// <summary>
		/// Overwriting a string array to a file
		/// </summary>
		/// <param name="pFilePath"></param>
		/// <param name="pContent"></param>
		public static void WriteStringToFileOverwrite(string pFilePath, string[] pContent)
		{

			if (doesFileExist(pFilePath))
			{
				deleteFile(pFilePath);
			}
			File.WriteAllLines(pFilePath, pContent);
		}



		/// <summary>
		/// Uses to generate Random Numbers
		/// </summary>
		static Random random = new Random();

		/// <summary>
		/// Used to indicate which instance we log from.
		/// </summary>
		static int intrandom = random.Next(1000, 9999);


		/// <summary>
		/// Adding to DebugFile.
		/// </summary>
		/// <param name="pLineContent"></param>
		public static void AddToDebug(string pLineContent)
		{

			pLineContent = "[" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "] (" + intrandom + ") - " + pLineContent;

			string pFilePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\AAA - DEBUG.txt";

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
				//System.Windows.Forms.MessageBox.Show("Writing to Log failed. File was probably deleted after start of Program.\n\nLogmessage:'" + pLineContent + "'\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString());
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
				//System.Windows.Forms.MessageBox.Show("Writing to Log failed. File was probably deleted after start of Program.\n\nLogmessage:'" + pLineContent + "'\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString());
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
			pXML = pXML.Replace("\n", "").Replace("\r", "");

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
		/// Does remote URL exist. Timeout in MS
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static bool URLExists(string url, int TimeOutMS = 500)
		{
			bool result = true;

			WebRequest webRequest = WebRequest.Create(url);
			webRequest.Timeout = TimeOutMS;
			webRequest.Method = "HEAD";

			try
			{
				webRequest.GetResponse();
			}
			catch
			{
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Checks if 2 Files are equal.
		/// </summary>
		/// <param name="pFilePathA"></param>
		/// <param name="pFilePathB"></param>
		/// <param name="SlowButStable"></param>
		/// <returns></returns>
		public static bool AreFilesEqual(string pFilePathA, string pFilePathB, bool SlowButStable)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			bool Sth = AreFilesEqualReal(pFilePathA, pFilePathB, SlowButStable);
			sw.Stop();
			HelperClasses.Logger.Log("AAAA - It took '" + sw.ElapsedMilliseconds + "' ms to compare: '" + pFilePathA + "' and '" + pFilePathB + "'. Result is: " + Sth.ToString());
			return Sth;
		}



		private static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
		{
			const int bufferSize = 1024 * sizeof(Int64);
			var buffer1 = new byte[bufferSize];
			var buffer2 = new byte[bufferSize];

			while (true)
			{
				int count1 = stream1.Read(buffer1, 0, bufferSize);
				int count2 = stream2.Read(buffer2, 0, bufferSize);

				if (count1 != count2)
				{
					return false;
				}

				if (count1 == 0)
				{
					return true;
				}

				int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
				for (int i = 0; i < iterations; i++)
				{
					if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
					{
						return false;
					}
				}
			}
		}


		/// <summary>
		/// Actual File Comparison Logic
		/// </summary>
		/// <param name="pFilePathA"></param>
		/// <param name="pFilePathB"></param>
		/// <param name="SlowButStable"></param>
		/// <returns></returns>
		public static bool AreFilesEqualReal(string pFilePathA, string pFilePathB, bool SlowButStable)
		{
			FileInfo fileInfo1 = new FileInfo(pFilePathA);
			FileInfo fileInfo2 = new FileInfo(pFilePathB);

			if (!fileInfo1.Exists || !fileInfo2.Exists)
			{
				return false;
			}
			if (fileInfo1.Length != fileInfo2.Length)
			{
				return false;
			}
			else if (SlowButStable)
			{
				using (var file1 = fileInfo1.OpenRead())
				{
					using (var file2 = fileInfo2.OpenRead())
					{
						return StreamsContentsAreEqual(file1, file2);
					}
				}
			}
			else
			{
				return true;
			}
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
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Could not find some of the Paths we use to keep track of GTA Files.").ShowDialog();
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

		public static string MostLikelyProfileFolder()
		{
			string profilesDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			profilesDir = System.IO.Path.Combine(profilesDir, @"Rockstar Games\GTA V\Profiles");
			var di = new System.IO.DirectoryInfo(profilesDir);
			DateTime lastHigh = new DateTime(1900, 1, 1);
			string highDir = "";
			foreach (var profile in di.GetDirectories())
			{
				if (Regex.IsMatch(profile.Name, "[0-9A-F]{8}"))
				{
					DateTime created = profile.LastWriteTime;

					if (created > lastHigh)
					{
						highDir = profile.FullName;
						lastHigh = created;
					}
				}
			}
			return highDir;
		}

		/// <summary>
		/// Gets String from URL
		/// </summary>
		/// <param name="pURL"></param>
		/// <returns></returns>
		public static string GetStringFromURL(string pURL, bool surpressPopup = false)
		{
			string rtrn = "";

			try
			{
				rtrn = new System.Net.Http.HttpClient().GetStringAsync(pURL).GetAwaiter().GetResult();
			}
			catch (Exception e)
			{
				if (Globals.OfflineErrorThrown == false && surpressPopup == false)
				{
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Project 1.27 can not connect to Github and check for Latest Files or Updates.\nThis might cause some things to not work." + e.ToString()).ShowDialog();
					Globals.OfflineErrorThrown = true;
				}
				HelperClasses.Logger.Log("GetStringFromURL failed. Probably Network related. URL = '" + pURL + "'", true, 0);
			}

			return rtrn;
		}


		/// <summary>
		/// Get all SubFolders from a Path
		/// </summary>
		/// <param name="pPath"></param>
		/// <returns></returns>
		public static string[] GetSubFolders(string pPath)
		{
			if (doesPathExist(pPath))
			{
				try
				{
					return Directory.GetDirectories(pPath);
				}
				catch (Exception e)
				{
					HelperClasses.Logger.Log("Get Sub Folders failed. " + e.ToString());
				}
			}
			return new string[0];
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
		/// Renaming a file. Second param is FileName not FilePath
		/// </summary>
		/// <param name="pFilePathSource"></param>
		/// <param name="pFileNameDest"></param>
		public static void RenameFile(string pFilePathSource, string pFileNameDest)
		{
			if (doesFileExist(pFilePathSource))
			{
				string pFilePathDest = PathSplitUp(pFilePathSource)[0].TrimEnd('\\') + @"\" + pFileNameDest;
				try
				{
					//HelperClasses.FileHandling.AddToDebug("Renaming: '" + pFilePathSource + "' to '" + pFilePathDest + "'.");
					System.IO.File.Move(pFilePathSource, pFilePathDest);
				}
				catch (Exception e)
				{
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Renaming File failed ('" + pFilePathSource + "' to '" + pFilePathDest + "').\nI suggest you restart the Program and contact me if it happens again.\n\nErrorMessage:\n" + e.ToString()).ShowDialog();
					HelperClasses.Logger.Log("Renaming File failed ('" + pFilePathSource + "' to '" + pFilePathDest + "').", true, 0);

				}
			}
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
					if (currentLine != null)
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
		/// Copy File A to file B. DOES overwrite
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
				FileHandling.deleteFile(pDestination);
			}
			try
			{
				FileHandling.createPathOfFile(pDestination);
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

		/// <summary>
		/// Deletes a Folder
		/// </summary>
		/// <param name="pPath"></param>
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

		/// <summary>
		/// Deletes target dir...
		/// </summary>
		/// <param name="sourceDirectory"></param>
		/// <param name="targetDirectory"></param>
		public static void CopyPath(string sourceDirectory, string targetDirectory)
		{
			if (doesPathExist(sourceDirectory))
			{
				if (doesPathExist(targetDirectory))
				{
					DeleteFolder(targetDirectory);
				}
				var diTarget = new DirectoryInfo(targetDirectory);
				var diSource = new DirectoryInfo(sourceDirectory);

				CopyAll(diSource, diTarget);
			}


		}

		private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
		{
			Directory.CreateDirectory(target.FullName);

			// Copy each file into the new directory.
			foreach (FileInfo fi in source.GetFiles())
			{
				fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
			}

			// Copy each subdirectory using recursion.
			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir =
					target.CreateSubdirectory(diSourceSubDir.Name);
				CopyAll(diSourceSubDir, nextTargetSubDir);
			}
		}

		public static void CreateAllZIPPaths(string pZIPFileExtractLocation)
		{
			// TODO, CTRLF FIX THIS MESS. OTHERWISE ZIP EXTRACTING SHIT WILL BREAK BECAUSE ITS A PIECE OF SHIT
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles\update");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles_Backup");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles_Backup\update");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles\update");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\Notes");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\Installer");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\SaveFiles");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\rockstar\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\rockstar\127\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\rockstar\127\update\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\rockstar\124\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\rockstar\124\update\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\steam\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\steam\127\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\steam\127\update\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\steam\124\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\DowngradeFiles_Alternative\steam\124\update\");
			HelperClasses.FileHandling.createPath(pZIPFileExtractLocation.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\DowngradedSocialClub\");



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
