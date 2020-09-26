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

		string ZipFileWeWannaExtract;


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
				ZipFileWeWannaExtract = pParam1;
				myLBL.Content = "Extracting ZIP...(0%)";
			}

			StartWork();
		}

		public async void StartWork()
		{
			await Task.Run(new Action(ActualWork));

			this.Close();
		}

		public void ActualWork()
		{
			HelperClasses.Logger.Log("lets see if we get here");
			HelperClasses.Logger.Log("ProgressType: '" + ProgressType + "'");
			HelperClasses.Logger.Log("ZipFileWeWannaExtract: '" + ZipFileWeWannaExtract + "'");
			HelperClasses.Logger.Log("ZIPExtractPath: '" + LauncherLogic.ZIPFilePath + "'");


			if (ProgressType == ProgressTypes.FileOperation)
			{
				double count = MyFileOperations.Count;
				double j = 0;
				for (int i = 0; i <= MyFileOperations.Count - 1; i++)
				{
					MyFileOperation.Execute(MyFileOperations[i]);
					j++;
					this.Dispatcher.Invoke(() =>
					{
						HelperClasses.Logger.Log("j = '" + j.ToString() + "', count = '" + count.ToString() + "'");
						myPB.Value = j / count * 100;
						myLBL.Content = "Doing a " + Operation + "...(" + myPB.Value + "%)";
					});
				}
			}
			else
			{
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
						foreach (var file in archive.Entries)
						{
							fileList.Add(file);
							totalFiles++;
						}
						foreach (var file in fileList)
						{
							if (!string.IsNullOrEmpty(file.Name))
							{
								file.ExtractToFile(LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\" + file.FullName);
							}

							filesExtracted++;
							long progress = (100 * filesExtracted / totalFiles);
							this.Dispatcher.Invoke(() =>
							{
								HelperClasses.Logger.Log("filesExtracted = '" + filesExtracted + "', totalFiles = '" + totalFiles + "'");
								myPB.Value = progress;
								myLBL.Content = "Extracting ZIP...(" + progress + "%)";
							});
						}
					}
				}
				catch (Exception e)
				{
					HelperClasses.Logger.Log("TryCatch failed while extracting ZIP with progressbar." + e.Message);
					new Popup(Popup.PopupWindowTypes.PopupOkError, "trycatch failed while extracting zip with progressbar\n" + e.Message);
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
