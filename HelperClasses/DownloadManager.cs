using Project_127.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
	class DownloadManager
	{
		public static void Demo()
		{
			// Would be cool to have static getter properties of this class to check if they are really downgraded (size of folder or folder exists...)
			// so i can check which of the 6 comonments are downloaded
		
			// There will be a WPF Window / Page / CustomControl using this. So...
			// a) Downloading a componment
			// b) re-downloading a componment
			// c) is a componment downloaded
			// are all needed. More maybe to come.
			
			
			// All 6 componments:
			// -1.27 LaunchThroughSocialClub Rockstar 
			// -1.27 LaunchThroughSocialClub Steam
			// -1.24 LaunchThroughSocialClub Rockstar
			// -1.24 LaunchThroughSocialClub Steam
			// Downgraded Social Club
			// Additional SaveFiles (will provide you ZIP)

			// Point to the paths (probably non existing and empty at this point)
			string tmp = "";
			tmp = LauncherLogic.DowngradeAlternativeFilePathSteam127;
			tmp = LauncherLogic.DowngradeAlternativeFilePathSteam124;
			tmp = LauncherLogic.DowngradeAlternativeFilePathRockstar127;
			tmp = LauncherLogic.DowngradeAlternativeFilePathRockstar124;
			tmp = LauncherLogic.DowngradedSocialClub;
			tmp = LauncherLogic.SaveFilesPath;

			/* Paths inside zip file of those would be:
			
			\Project_127_Files\DowngradeFiles_Alternative\steam\127\
			\Project_127_Files\DowngradeFiles_Alternative\steam\124\
			\Project_127_Files\DowngradeFiles_Alternative\rockstar\127\
			\Project_127_Files\DowngradeFiles_Alternative\rockstar\124\
			\Project_127_Files\SupportFiles\DowngradedSocialClub\
			\Project_127_Files\SupportFiles\SaveFiles\

			*/

			// So. Code to get the text inside a specific XML tag from the update.xml file on github
			tmp = HelperClasses.FileHandling.GetXMLTagContent(Globals.XML_AutoUpdate, "zip");

			// Code to download a file
			new PopupDownload(@"https://some.url.com/myfile.zip", @"C:\Some\Location\myfilename.zip", "ZIP-File").ShowDialog();

			// Code to extract a zip file (zip file needs to have some folder convention as current zips out there...
			// I recommend deleting the local ZIP file after that.
			// this replaces all files (if it exists already) apart from $UpgradeFiles
			new PopupProgress(PopupProgress.ProgressTypes.ZIPFile, @"C:\Some\Location\myfilename.zip").ShowDialog();

			// Code to do a few file operations. 
			// Code here is...historically grown and not the greatest..but it works.
			// this cannot hardlink folders.
			// if called with hardlink, it will check further down if we are inside GTA Installation directory and switch to copying if needed.
			List<MyFileOperation> ListOfFileOperations = new List<MyFileOperation>();
			ListOfFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, @"C:\SourceDirectory", @"C:\DestinationDirectory", "my-log-message", 0, MyFileOperation.FileOrFolder.Folder));
			ListOfFileOperations.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, @"C:\DestinationDirectory\Somefile", "", "my-log-message", 0, MyFileOperation.FileOrFolder.File));

			// Actually executing the File Operations
			new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "GUI - Title", ListOfFileOperations).ShowDialog();

			// it will treat this as 2 operations, thus the progressbar and percentage text is gonna be stuck at 50% for most of the time, since copying a folder takes longer than deleting one file.
			// you might want to add a MyFileOperation object for each file inside a folder, instead of copying the entire folder, so UX is better. 

			// Helper Methods you might find usefull
			HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(@"C:\Some\Path"); // Returns full Filepaths for all files
			HelperClasses.FileHandling.GetSubFolders(@"C:\Some\Path"); // returns full filepath for all folders
			HelperClasses.FileHandling.GetHashFromFile(@"C:\Some\Path\Somefile.txt"); // gets Hash of one file, "" if non existant
			HelperClasses.FileHandling.GetSizeOfFile(@"C:\Some\Path\Somefile.txt"); // gets size of one file, 0 if non existant


			// All path related stuff (does path of new file exists, does file exist etc. should be taken care of inside the executing... Copy / Move / Hardlink will replace target file if it exists
		}
	}
}
