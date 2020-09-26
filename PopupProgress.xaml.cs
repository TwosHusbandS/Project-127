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
//using Ionic.Zip;
using System.IO.Compression;


namespace Project_127
{
	/// <summary>
	/// Interaction logic for CopyFileProgress.xaml
	/// </summary>
	public partial class PopupProgress : Window
	{
		public enum ProgressTypes
		{
			ZIPFile,
			FileOperation
		}

		ProgressTypes ProgressType;

		List<MyFileOperation> MyFileOperations;
		string Operation;

		string ZipFileLocation;


		public PopupProgress(ProgressTypes pProgressType, string pParam1, List<MyFileOperation> pMyFileOperations = null)
		{
			InitializeComponent();

			ProgressType = pProgressType;

			if (ProgressType == ProgressTypes.FileOperation)
			{
				Operation = pParam1;
				MyFileOperations = pMyFileOperations;
				myLBL.Content = "Doing a " + Operation + "...(0%)";
			}
			else
			{
				ZipFileLocation = pParam1;
				myLBL.Content = "Extracting ZIP...(0%)";
			}

			StartWork();
		}

		public async void StartWork()
		{
			await Task.Run(new Action(ActualWork));

			this.Close();
		}

		public async void ActualWork()
		{
			HelperClasses.Logger.Log("lets see if we get here");
			HelperClasses.Logger.Log("ProgressType: '" + ProgressType + "'");
			HelperClasses.Logger.Log("ZipFileLocation: '" + ZipFileLocation + "'");

			double count = MyFileOperations.Count;
			double j = 0;
			//if (ProgressType == ProgressTypes.FileOperation)
			//{
			//	for (int i = 0; i <= MyFileOperations.Count - 1; i++)
			//	{
			//		MyFileOperation.Execute(MyFileOperations[i]);
			//		j++;
			//		this.Dispatcher.Invoke(() =>
			//		{
			//			HelperClasses.Logger.Log("j = '" + j.ToString() + "', count = '" + count.ToString() + "'");
			//			myPB.Value = j / count * 100;
			//			myLBL.Content = "Doing a " + Operation + "...(" + myPB.Value + "%)";
			//		});
			//	}
			//}
			//else
			//{
				HelperClasses.Logger.Log("A");
				List<System.IO.Compression.ZipArchiveEntry> fileList = new List<System.IO.Compression.ZipArchiveEntry>();
				var totalFiles = 0;
				var filesExtracted = 0;

				using (var archive = await Task.Run(() => ZipFile.OpenRead(ZipFileLocation)))
				{
					HelperClasses.Logger.Log("B");
					foreach (var file in archive.Entries)
					{
						HelperClasses.Logger.Log("C");
						fileList.Add(file);
						totalFiles++;
					}
					HelperClasses.Logger.Log("D");
					foreach (var file in fileList)
					{
						HelperClasses.Logger.Log("E");

						HelperClasses.Logger.Log("F");
						file.ExtractToFile($"{Globals.ProjectInstallationPath}{file.Name}");
						filesExtracted++;
						//long progress = (100 * filesExtracted / totalFiles);
						//this.Dispatcher.Invoke(() =>
						//{
						//	HelperClasses.Logger.Log("filesExtracted = '" + filesExtracted + "', totalFiles = '" + totalFiles + "'");
						//	myPB.Value = progress;
						//	myLBL.Content = "Extracting ZIP...(" + progress + "%)";
						//});
						HelperClasses.Logger.Log("G");
					}
					HelperClasses.Logger.Log("H");
				}
				HelperClasses.Logger.Log("I");



				//var directoryInfo = new DirectoryInfo(Globals.ProjectInstallationPath);

				//List<System.IO.Compression.ZipArchiveEntry> fileList = new List<System.IO.Compression.ZipArchiveEntry>();
				//long totalFiles = 0;
				//long filesExtracted = 0;

				//using (var archive = await Task.Run(() => ZipFile.OpenRead(ZipFileLocation)))
				//{
				//	foreach (var file in archive.Entries)
				//	{
				//		fileList.Add(file);
				//		totalFiles++;
				//	}

				//	foreach (var file in fileList)
				//	{
				//		await Task.Run(() =>
				//		{
				//			this.Dispatcher.Invoke(() =>
				//			{
				//				file.ExtractToFile($"{Globals.ProjectInstallationPath}{file.Name}");
				//				filesExtracted++;
				//				HelperClasses.Logger.Log("filesExtracted = '" + filesExtracted.ToString() + "', totalFiles = '" + totalFiles.ToString() + "'");
				//				myPB.Value = filesExtracted / totalFiles * 100;
				//				myLBL.Content = "Extracting ZIP...(" + myPB.Value + "%)";
				//			});
				//		});
				//	}
				//}
			//}
			HelperClasses.Logger.Log("Huh");

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
