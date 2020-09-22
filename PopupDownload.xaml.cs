using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
	public enum PopupDownloadTypes
	{
		ZIP,
		Installer
	}


	/// <summary>
	/// Interaction logic for PopupDownload.xaml
	/// </summary>
	public partial class PopupDownload : Window
	{
		public PopupDownloadTypes PopupDownloadType;
		public string DownloadURL;
		public string DownloadLocation;

		public PopupDownload(PopupDownloadTypes pPopupDownloadType, string pDownloadURL, string pDownloadLocation)
		{
			InitializeComponent();

			PopupDownloadType = pPopupDownloadType;
			DownloadURL = pDownloadURL;
			DownloadLocation = pDownloadLocation;

			HelperClasses.Logger.Log("Download Popup: DownloadType: '" + pPopupDownloadType.ToString() + "'");
			HelperClasses.Logger.Log("Download Popup: DownloadURL: '" + pDownloadURL.ToString() + "'");
			HelperClasses.Logger.Log("Download Popup: DownloadLocation: '" + pDownloadLocation.ToString() + "'");

			if (this.PopupDownloadType == PopupDownloadTypes.ZIP)
			{
				lbl_Main.Content = "Downloading ZIP File...";
			}
			else if (this.PopupDownloadType == PopupDownloadTypes.Installer)
			{
				lbl_Main.Content = "Downloading Installer...";
			}

			WebClient webClient = new WebClient();
			webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressed);
			webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
			HelperClasses.Logger.Log("Download Popup: Starting Download");
			webClient.DownloadFileAsync(new Uri(DownloadURL), DownloadLocation);
		}

		private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			HelperClasses.Logger.Log("Download Popup: Download Done");

			if (this.PopupDownloadType == PopupDownloadTypes.ZIP)
			{
				HelperClasses.Logger.Log("Download Popup: Download Done, starting Globals.ImportZip()");
				lbl_Main.Content = "Download Complete\nUnpacking ZIP File...";
				Globals.ImportZip(DownloadLocation, true);
				pb_Main.Value = 100;
				this.Close();
			}
			else if (this.PopupDownloadType == PopupDownloadTypes.Installer)
			{
				HelperClasses.Logger.Log("Download Popup: Download Done, starting new Installer");
				lbl_Main.Content = "Downgrade Complete\nStarting Installer...";
				HelperClasses.ProcessHandler.StartProcess(DownloadLocation, "", true, false);
				Environment.Exit(0);
			}
		}

		private void DownloadProgressed(object sender, DownloadProgressChangedEventArgs e)
		{
			if (this.PopupDownloadType == PopupDownloadTypes.ZIP)
			{
				pb_Main.Value = (double)e.ProgressPercentage;
			}
			else if (this.PopupDownloadType == PopupDownloadTypes.Installer)
			{
				pb_Main.Value = (double)e.ProgressPercentage;
			}
		}


		// Below are Methods we need to make the behaviour of this nice.


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