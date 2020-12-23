using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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

namespace Project_127.Popups
{
	/// <summary>
	/// Interaction logic for PopupDownload.xaml
	/// </summary>
	public partial class PopupDownload : Window
	{
		/// <summary>
		/// DownloadName Property of this Object
		/// </summary>
		public string DownloadName;

		/// <summary>
		/// DownloadURL Property of this object (PopupDownload Window)
		/// </summary>
		public string DownloadURL;

		/// <summary>
		/// DownloadLocation Property of this object (PopupDownload Window)
		/// </summary>
		public string DownloadLocation;

		/// <summary>
		/// Hash Property (non null if hashing is enabled)
		/// </summary>
		public string HashString = null;

		/// <summary>
		/// Constructor of our Popup download
		/// </summary>
		/// <param name="pDownloadURL"></param>
		/// <param name="pDownloadLocation"></param>
		/// <param name="pMessage"></param>
		public PopupDownload(string pDownloadURL, string pDownloadLocation, string pMessage, bool autoHash = false)
		{
			if (MainWindow.MW.IsVisible)
			{
				this.Owner = MainWindow.MW;
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			InitializeComponent();

			// Setting Properties of our Object (Popup Window)
			DownloadURL = pDownloadURL;
			DownloadLocation = pDownloadLocation;
			DownloadName = pMessage;

			// Logging
			HelperClasses.Logger.Log("Download Popup: DownloadURL: '" + pDownloadURL.ToString() + "'");
			HelperClasses.Logger.Log("Download Popup: DownloadLocation: '" + pDownloadLocation.ToString() + "'");
			HelperClasses.Logger.Log("Download Popup: pMessage: '" + pMessage.ToString() + "'");

			HelperClasses.FileHandling.createPathOfFile(pDownloadLocation);
			HelperClasses.FileHandling.deleteFile(pDownloadLocation);

			lbl_Main.Content = "Downloading " + DownloadName + "...";

			// Setting up some Webclient stuff. 
			WebClient webClient = new WebClient();
			webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressed);
			if (!autoHash)
            {
				webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
            }
            else
            {
				webClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadCompletedAutoHash);

			}
			HelperClasses.Logger.Log("Download Popup: Starting Download");

			// TryCatch of the actual Download
			try
			{
				if (!autoHash)
                {
					webClient.DownloadFileAsync(new Uri(DownloadURL), DownloadLocation);
                }
                else
                {
					webClient.DownloadDataAsync(new Uri(DownloadURL));
                }
			}
			catch (Exception e)
			{
				HelperClasses.Logger.Log("Download of '" + pMessage.ToString() + "' failed for some reason." + e.Message);
				// No Popup here, Popup will appear in Method which called this
			}
		}


		/// <summary>
		/// Method gets called when Download is done
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			HelperClasses.Logger.Log("Download Popup: Download Done");
			lbl_Main.Content = "Download Complete.";
			pb_Main.Value = 100;
			this.Close();
		}

		/// <summary>
		/// Method gets called when Download is done (autoHashingMode)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DownloadCompletedAutoHash(object sender, DownloadDataCompletedEventArgs e)
		{
			HelperClasses.Logger.Log("Download Popup: Download Done");
			lbl_Main.Content = "Download Complete.";
			byte[] file = e.Result;
			HelperClasses.Logger.Log("Download Popup: Computing Hash");
			using (var md5 = MD5.Create())
			{
				var hash = md5.ComputeHash(file);
				HashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
			}
			pb_Main.Value = 100;
			using (var b = new BinaryWriter(File.Open(DownloadLocation, FileMode.Create)))
			{
				b.Write(file);
			}
			this.Close();
		}

		/// <summary>
		/// Method gets Sets the Download Progress Bar, gets called if the Download Progress Changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DownloadProgressed(object sender, DownloadProgressChangedEventArgs e)
		{
			pb_Main.Value = (double)e.ProgressPercentage;
			lbl_Main.Content = "Downloading " + DownloadName + "...\n(" + pb_Main.Value + "%)";
		}

		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

	} // End of Class
} // End of Namespace