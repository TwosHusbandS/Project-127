using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Compression;
using Project_127.MySettings;

namespace Project_127.Popups
{
	/// <summary>
	/// Interaction logic for CopyFileProgress.xaml
	/// This is responsible for ProgressBar on File copying (etc.) and Extracting the ZIP File
	/// </summary>
	public partial class PopupProgress : Window
	{
		/// <summary>
		/// Enum of ProgressTypes
		/// </summary>
		public enum ProgressTypes
		{
			ZIPFile,
			FileOperation,
			Upgrade,
			Downgrade,
			DidUpdateHit
		}

		/// <summary>
		/// ProgressType of Instance
		/// </summary>
		ProgressTypes ProgressType;

		/// <summary>
		/// List of File Operations
		/// </summary>
		List<MyFileOperation> MyFileOperations;

		/// <summary>
		/// List of File Operations
		/// </summary>
		public List<MyFileOperation> RtrnMyFileOperations;

		public bool RtrnBool = false;

		/// <summary>
		/// Name of Operation for GUI
		/// </summary>
		string Operation;

		/// <summary>
		/// Path of ZIP File (on disk) we are extracting
		/// </summary>
		string ZipFileWeWannaExtract;

		/// <summary>
		/// Constructor of PopupProgress.
		/// String pParam1 is either the GUI Text for File Operations or the ZIP File physical location
		/// </summary>
		/// <param name="pProgressType"></param>
		/// <param name="pParam1"></param>
		/// <param name="pMyFileOperations"></param>
		public PopupProgress(ProgressTypes pProgressType, string pParam1, List<MyFileOperation> pMyFileOperations = null)
		{
			if (MainWindow.MW.IsVisible)
			{
				this.Owner = MainWindow.MW;
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			InitializeComponent();

			// Setting all Properties needed later
			ProgressType = pProgressType;

			if (ProgressType == ProgressTypes.FileOperation)
			{
				Operation = pParam1;
				MyFileOperations = pMyFileOperations;
				myLBL.Content = "Doing a " + Operation + "...(0%)";
			}
			else if (ProgressType == ProgressTypes.ZIPFile)
			{
				ZipFileWeWannaExtract = pParam1;
				myLBL.Content = "Extracting ZIP...(0%)";
			}
			else if (ProgressType == ProgressTypes.Downgrade)
			{
				myPB.Value = 0;
				myLBL.Content = "Gathering Information";
			}
			else if (ProgressType == ProgressTypes.Upgrade)
			{
				myPB.Value = 0;
				myLBL.Content = "Gathering Information";
			}
			else if (ProgressType == ProgressTypes.DidUpdateHit)
			{
				myPB.Value = 0;
				myLBL.Content = "Checking if Update hit";
			}

			// Lets do some shit
			StartWork();
		}

		[STAThread]
		public async void StartWork()
		{
			// Awaiting the Task of the Actual Work
			await Task.Run(new Action(ActualWork));

			// Close this
			this.Close();
		}

		[STAThread]
		public void ActualWork()
		{
			HelperClasses.Logger.Log("ProgressType: '" + ProgressType + "'");

			if (ProgressType == ProgressTypes.FileOperation)
			{
				double count = MyFileOperations.Count;
				double j = 0;

				HelperClasses.Logger.Log("Lets do some File Operation Stuff");
				for (int i = 0; i <= MyFileOperations.Count - 1; i++)
				{
					MyFileOperation.Execute(MyFileOperations[i]);
					j++;
					this.Dispatcher.Invoke(() =>
					{
						myPB.Value = (int)(j / count * 100);
						myLBL.Content = "Doing a " + Operation + "...(" + myPB.Value + "%)";
					});
				}
				HelperClasses.Logger.Log("Done with File Operation Stuff");
			}
			else if (ProgressType == ProgressTypes.ZIPFile)
			{
				HelperClasses.Logger.Log("ZipFileWeWannaExtract: '" + ZipFileWeWannaExtract + "'");
				HelperClasses.Logger.Log("ZIPExtractPath: '" + LauncherLogic.ZIPFilePath + "'");

				List<System.IO.Compression.ZipArchiveEntry> fileList = new List<System.IO.Compression.ZipArchiveEntry>();
				var totalFiles = 0;
				var filesExtracted = 0;

				if (!HelperClasses.FileHandling.doesFileExist(ZipFileWeWannaExtract))
				{
					HelperClasses.Logger.Log("ERROR. ZIP File we are extracting in Popup window doesnt exist..", true, 0);
					new Popup(Popup.PopupWindowTypes.PopupOk, "ERROR. ZIP File we are extracting in Popup window doesnt exist..").ShowDialog();
					return;
				}

				try
				{
					using (var archive = ZipFile.OpenRead(ZipFileWeWannaExtract))
					{
						totalFiles = archive.Entries.Count();

						// Looping through all Files in the ZIPFile
						foreach (var file in archive.Entries)
						{
							// If the File exists and is not a folder
							if (!string.IsNullOrEmpty(file.Name))
							{
								bool doExtract = true;
								string PathOnDisk = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\" + file.FullName.Replace(@"/", @"\");
								HelperClasses.FileHandling.createPathOfFile(PathOnDisk); // 99% Chance I fixed this with the createZipPaths Method. Lets keep this to make sure...
								if (HelperClasses.FileHandling.doesFileExist(PathOnDisk))
								{
									if (PathOnDisk.Contains("UpgradeFiles") ||
										PathOnDisk.Contains("SupportFiles"))
									//PathOnDisk.Contains(@"DowngradeFiles\GTA5.exe") ||
									//PathOnDisk.Contains(@"DowngradeFiles\x64a.rpf") ||
									//PathOnDisk.Contains(@"DowngradeFiles\update\update.rpf"))
									{
										doExtract = false;
									}
									else
									{
										HelperClasses.FileHandling.deleteFile(PathOnDisk);
									}
								}

								if (doExtract)
								{
									file.ExtractToFile(PathOnDisk);
								}
							}

							// Update GUI
							Application.Current.Dispatcher.Invoke((Action)delegate
							{
								filesExtracted++;
								long progress = (100 * filesExtracted / totalFiles);
								myPB.Value = progress;
								myLBL.Content = "Extracting ZIP...(" + progress + "%)";
							});


							// // Lets hope we never need this but I want to keep this here for now, in case that code snipped becomes useful
							//this.Dispatcher.Invoke(() =>
							//{
							//	myPB.Value = progress;
							//	myLBL.Content = "Extracting ZIP...(" + progress + "%)";
							//});
						}
					}
				}
				catch (Exception e)
				{
					HelperClasses.Logger.Log("TryCatch failed while extracting ZIP with progressbar." + e.ToString());
					new Popup(Popup.PopupWindowTypes.PopupOkError, "trycatch failed while extracting zip with progressbar\n" + e.ToString());
				}
			}
			else if (ProgressType == ProgressTypes.Downgrade)
			{
				// Saving all the File Operations I want to do, executing this at the end of this Method
				List<MyFileOperation> MyFileOperationsTmp = new List<MyFileOperation>();

				// Creates Hardlink Link in GTAV Installation Folder to all the files of Downgrade Folder
				// If they exist in GTAV Installation Folder, and in Upgrade Folder, we delete them from GTAV Installation folder
				// If they exist in GTAV Installation Folder, and NOT in Upgrade Folder, we move them there

				HelperClasses.Logger.Log("Initiating Downgrade", 0);
				HelperClasses.Logger.Log("GTAV Installation Path: " + LauncherLogic.GTAVFilePath, 1);
				HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
				HelperClasses.Logger.Log("ZIP File Location: " + LauncherLogic.ZIPFilePath, 1);
				HelperClasses.Logger.Log("DowngradeFilePath: " + LauncherLogic.DowngradeFilePath, 1);
				HelperClasses.Logger.Log("UpgradeFilePath: " + LauncherLogic.UpgradeFilePath, 1);

				// Those are WITH the "\" at the end
				string[] FilesInDowngradeFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeFilePath);
				string[] CorrespondingFilePathInGTALocation = new string[FilesInDowngradeFiles.Length];
				string[] CorrespondingFilePathInUpgradeFiles = new string[FilesInDowngradeFiles.Length];

				HelperClasses.Logger.Log("Found " + FilesInDowngradeFiles.Length.ToString() + " Files in Downgrade Folder.");

				// Loop through all Files in Downgrade Files Folder
				for (int i = 0; i <= FilesInDowngradeFiles.Length - 1; i++)
				{
					// Update GUI
					Application.Current.Dispatcher.Invoke((Action)delegate
					{
						long progress = (100 * i / FilesInDowngradeFiles.Length - 1);
						myPB.Value = progress;
						myLBL.Content = "Gathering Information(" + i.ToString() + "/" + (FilesInDowngradeFiles.Length - 1).ToString() + ")";
					});


					// Build the Corresponding theoretical Filenames for Upgrade Folder and GTA V Installation Folder
					CorrespondingFilePathInGTALocation[i] = LauncherLogic.GTAVFilePath + FilesInDowngradeFiles[i].Substring(LauncherLogic.DowngradeFilePath.Length);
					CorrespondingFilePathInUpgradeFiles[i] = LauncherLogic.UpgradeFilePath + FilesInDowngradeFiles[i].Substring(LauncherLogic.DowngradeFilePath.Length);

					// If the File exists in GTA V Installation Path
					if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInGTALocation[i]))
					{
						// If the File Exists in Upgrade Folder
						if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInUpgradeFiles[i]))
						{
							// Delete from GTA V Installation Path
							MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and $UpgradeFiles. Will delelte from GTA V Installation", 1));
						}
						else
						{
							// If its not the same file as in DownGradeFiles
							if (!HelperClasses.FileHandling.AreFilesEqual(CorrespondingFilePathInGTALocation[i], FilesInDowngradeFiles[i]))
							{
								// Move File from GTA V Installation Path to Upgrade Folder
								MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, CorrespondingFilePathInGTALocation[i], CorrespondingFilePathInUpgradeFiles[i], "Found '" + CorrespondingFilePathInGTALocation[i] + "' in GTA V Installation Path and NOT in $UpgradeFiles. Will move it from GTA V Installation to $UpgradeFiles", 1));
							}
						}
					}

					// Creates actual Hard Link (this will further down check if we should copy based on settings in MyFileOperation.Execute())
					MyFileOperationsTmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Hardlink, FilesInDowngradeFiles[i], CorrespondingFilePathInGTALocation[i], "Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + FilesInDowngradeFiles[i] + "'", 1));

				}

				RtrnMyFileOperations = MyFileOperationsTmp;
			}
			else if (ProgressType == ProgressTypes.Upgrade)
			{
				// Saving all the File Operations I want to do, executing this at the end of this Method
				List<MyFileOperation> MyFileOperationsTMP = new List<MyFileOperation>();

				// Creates Hardlink Link in GTAV Installation Folder to all the files of Upgrade Folder
				// If they exist in GTAV Installation Folder,  we delete them from GTAV Installation folder

				HelperClasses.Logger.Log("Initiating Upgrade", 0);
				HelperClasses.Logger.Log("GTAV Installation Path: " + LauncherLogic.GTAVFilePath, 1);
				HelperClasses.Logger.Log("InstallationLocation: " + Globals.ProjectInstallationPath, 1);
				HelperClasses.Logger.Log("ZIP File Location: " + LauncherLogic.ZIPFilePath, 1);
				HelperClasses.Logger.Log("DowngradeFilePath: " + LauncherLogic.DowngradeFilePath, 1);
				HelperClasses.Logger.Log("UpgradeFilePath: " + LauncherLogic.UpgradeFilePath, 1);

				// Those are WITH the "\" at the end
				string[] FilesInDowngradeFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeFilePath);
				string[] CorrespondingFilePathInGTALocation = new string[FilesInDowngradeFiles.Length];
				string[] CorrespondingFilePathInUpgradeFiles = new string[FilesInDowngradeFiles.Length];

				// Loop through all Files in Downgrade Files Folder
				// Looping through Downgrade Files here, since that gives info if we need to delete the file afterwards
				for (int i = 0; i <= FilesInDowngradeFiles.Length - 1; i++)
				{
					// Update GUI
					Application.Current.Dispatcher.Invoke((Action)delegate
					{
						long progress = (100 * i / FilesInDowngradeFiles.Length - 1);
						myPB.Value = progress;
						myLBL.Content = "Gathering Information(" + i.ToString() + "/" + (FilesInDowngradeFiles.Length - 1).ToString() + ")";
					});

					// Build the Corresponding theoretical Filenames for Upgrade Folder and GTA V Installation Folder
					CorrespondingFilePathInGTALocation[i] = LauncherLogic.GTAVFilePath + FilesInDowngradeFiles[i].Substring(LauncherLogic.DowngradeFilePath.Length);
					CorrespondingFilePathInUpgradeFiles[i] = LauncherLogic.UpgradeFilePath + FilesInDowngradeFiles[i].Substring(LauncherLogic.DowngradeFilePath.Length);

					// If the File exists in GTA V Installation Path
					if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInGTALocation[i]))
					{

						// if it also exists in upgrade files
						if (HelperClasses.FileHandling.doesFileExist(CorrespondingFilePathInUpgradeFiles[i]))
						{
							// Delete from GTA V Installation Path
							MyFileOperationsTMP.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "File found in GTA V Installation Path and the Upgrade Folder. Game File we overwrite. Will delete '" + CorrespondingFilePathInGTALocation[i] + "'", 1));
						}
						else
						{
							// Exists in GTAV, not in Upgrades

							// But exists in Downgrades
							if (HelperClasses.FileHandling.doesFileExist(FilesInDowngradeFiles[i]))
							{
								MyFileOperationsTMP.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, CorrespondingFilePathInGTALocation[i], "", "File found in GTA V Installation Path, in Downgrade Folder but NOT in Upgrade Folder. Leftover Downgrade File. Will delete '" + CorrespondingFilePathInGTALocation[i] + "'", 1));
							}
						}
					}

					// Creates actual Hard Link (this will further down check if we should copy based on settings in MyFileOperation.Execute())
					MyFileOperationsTMP.Add(new MyFileOperation(MyFileOperation.FileOperations.Hardlink, CorrespondingFilePathInUpgradeFiles[i], CorrespondingFilePathInGTALocation[i], "Will create HardLink in '" + CorrespondingFilePathInGTALocation[i] + "' to the file in '" + CorrespondingFilePathInUpgradeFiles[i] + "'", 1));
				}

				RtrnMyFileOperations = MyFileOperationsTMP;

			}
			else if (ProgressType == ProgressTypes.DidUpdateHit)
			{
				// Check if the file matches the one in downgrade files...
				// Update GUI
				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					long progress = (100 * 0 / 8);
					myPB.Value = progress;
					myLBL.Content = "Checking if Update hit (0/8)";
				});

				string GTA_GTA5 = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\gta5.exe";
				string GTA_PlayGTAV = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\playgtav.exe";
				string GTA_UpdateRPF = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\update\update.rpf";

				string Upgrade_GTA5 = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\gta5.exe";
				string Upgrade_PlayGTAV = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe";
				string Upgrade_UpdateRPF = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf";

				string Downgrade_GTA5 = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\gta5.exe";
				string Downgrade_PlayGTAV = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\playgtav.exe";
				string Downgrade_UpdateRPF = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf";

				bool eq_GTA_Downgrade_GTA5 = HelperClasses.FileHandling.AreFilesEqual(GTA_GTA5, Downgrade_GTA5);

				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					long progress = (100 * 1 / 8);
					myPB.Value = progress;
					myLBL.Content = "Checking if Update hit (1/8)";
				});

				bool eq_GTA_Downgrade_PlayGTAV = HelperClasses.FileHandling.AreFilesEqual(GTA_PlayGTAV, Downgrade_PlayGTAV);

				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					long progress = (100 * 2 / 8);
					myPB.Value = progress;
					myLBL.Content = "Checking if Update hit (2/8)";
				});

				bool eq_GTA_Downgrade_UpdateRPF = HelperClasses.FileHandling.AreFilesEqual(GTA_UpdateRPF, Downgrade_UpdateRPF);

				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					long progress = (100 * 3 / 8);
					myPB.Value = progress;
					myLBL.Content = "Checking if Update hit (3/8)";
				});



				if (eq_GTA_Downgrade_GTA5 &&
					eq_GTA_Downgrade_PlayGTAV &&
					eq_GTA_Downgrade_UpdateRPF)
				{
					RtrnBool = false;
					return;
				}
				else
				{
					Application.Current.Dispatcher.Invoke((Action)delegate
					{
						long progress = (100 * 4 / 8);
						myPB.Value = progress;
						myLBL.Content = "Checking if Update hit (4/8)";
					});


					bool eq_GTA_Upgrade_GTA5 = HelperClasses.FileHandling.AreFilesEqual(GTA_GTA5, Upgrade_GTA5);

					Application.Current.Dispatcher.Invoke((Action)delegate
					{
						long progress = (100 * 5 / 8);
						myPB.Value = progress;
						myLBL.Content = "Checking if Update hit (5/8)";
					});

					bool eq_GTA_Upgrade_PlayGTAV = HelperClasses.FileHandling.AreFilesEqual(GTA_PlayGTAV, Upgrade_PlayGTAV);

					Application.Current.Dispatcher.Invoke((Action)delegate
					{
						long progress = (100 * 6 / 8);
						myPB.Value = progress;
						myLBL.Content = "Checking if Update hit (6/8)";
					});

					bool eq_GTA_Upgrade_UpdateRPF = HelperClasses.FileHandling.AreFilesEqual(GTA_UpdateRPF, Upgrade_UpdateRPF);

					Application.Current.Dispatcher.Invoke((Action)delegate
					{
						long progress = (100 * 7 / 8);
						myPB.Value = progress;
						myLBL.Content = "Checking if Update hit (7/8)";
					});


					if (HelperClasses.FileHandling.doesFileExist(Upgrade_GTA5) &&
						HelperClasses.FileHandling.doesFileExist(Upgrade_PlayGTAV) &&
						HelperClasses.FileHandling.doesFileExist(Upgrade_UpdateRPF))
					{
						Application.Current.Dispatcher.Invoke((Action)delegate
						{
							long progress = (100 * 8 / 8);
							myPB.Value = progress;
							myLBL.Content = "Checking if Update hit (8/8)";
						});


						if (eq_GTA_Upgrade_GTA5 &&
						eq_GTA_Upgrade_PlayGTAV &&
						eq_GTA_Upgrade_UpdateRPF)
						{
							RtrnBool = false;
						}
						else
						{
							RtrnBool = true;
						}
					}
					else
					{
						RtrnBool = false;
					}
				}
			}
		}

		////////////////////////////////////////////////////////////////////
		// Below are Methods we need to make the behaviour of this nice. ///
		////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

	}
}
