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

namespace Project_127
{
	/// <summary>
	/// Interaction logic for CopyFileProgress.xaml
	/// </summary>
	public partial class CopyFileProgress : Window
	{
		BackgroundWorker Worker = new BackgroundWorker();

		List<string> SourceFiles = new List<string>();
		List<string> DestinationFiles = new List<string>();

		// Setting those
		public static ulong BytesToCopy = 0;
		public static ulong BytesCopied = 0;

		public CopyFileProgress(List<string> pSourceFiles, List<string> pDestinationFiles)
		{
			InitializeComponent();

			SourceFiles = pSourceFiles;
			DestinationFiles = pDestinationFiles;

			Worker.WorkerSupportsCancellation = true;
			Worker.WorkerReportsProgress = true;

			Worker.ProgressChanged += Worker_ProgressChanged;
			Worker.DoWork += Worker_DoWork;
			Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

			//Worker.RunWorkerAsync();
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Globals.DebugPopup("Done");
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			// Getting BytesToCopy
			//foreach (string myFile in SourceFiles)
			//{
			//	BytesToCopy += (ulong) HelperClasses.FileHandling.GetSizeOfFile(myFile);
			//}

			for (int i = 0; i <= SourceFiles.Count() - 1; i++)
			{
				//myLBL.Content = SourceFiles[i];
				CopyFile(SourceFiles[i], DestinationFiles[i]);
				//BytesCopied += (ulong)HelperClasses.FileHandling.GetSizeOfFile(SourceFiles[i]);
			}
		}

		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			myPB.Value = e.ProgressPercentage;
			//if (myPB.Value >= 100)
			//{
			//	this.Close();
			//}
		}


		private void CopyFile(string pSourcePath, string pDestinationPath)
		{
			FileInfo _source = new FileInfo(pSourcePath);
			FileInfo _destination = new FileInfo(pDestinationPath);

			if (_destination.Exists)
				_destination.Delete();

			Task.Run(() =>
			{
				_source.CopyTo(_destination, x => Dispatcher.Invoke(() => myPB.Value = x));
			});
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Worker.RunWorkerAsync();
		}
	}


	public static class FileInfoExtensions
	{
		public static void CopyTo(this FileInfo file, FileInfo destination, Action<int> progressCallback)
		{
			const int bufferSize = 1024 * 1024;  //1MB
			byte[] buffer = new byte[bufferSize], buffer2 = new byte[bufferSize];
			bool swap = false;
			int progress = 0, reportedProgress = 0, read = 0;
			long len = file.Length;
			float flen = len;
			Task writer = null;

			using (var source = file.OpenRead())
			using (var dest = destination.OpenWrite())
			{
				dest.SetLength(source.Length);
				for (long size = 0; size < len; size += read)
				{
					if ((progress = ((int)((size / flen) * 100))) != reportedProgress)
						progressCallback(reportedProgress = progress);
					read = source.Read(swap ? buffer : buffer2, 0, bufferSize);
					writer?.Wait();  
					writer = dest.WriteAsync(swap ? buffer : buffer2, 0, read);
					swap = !swap;
					CopyFileProgress.BytesCopied += (ulong)size;
					//if ((progress = (int)(((CopyFileProgress.BytesCopied) * 100) / CopyFileProgress.BytesToCopy )) != reportedProgress)
					//	progressCallback(reportedProgress = progress);
				}
				writer?.Wait(); 
			}
		}
	}
}
