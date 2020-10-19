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
		/// DownloadURL Propertie of this object (PopupDownload Window)
		/// </summary>
		public string DownloadURL;

		/// <summary>
		/// DownloadLocation Propertie of this object (PopupDownload Window)
		/// </summary>
		public string DownloadLocation;

		/// <summary>
		/// Constructor of our Popup download
		/// </summary>
		/// <param name="pDownloadURL"></param>
		/// <param name="pDownloadLocation"></param>
		/// <param name="pMessage"></param>
		public PopupDownload(string pDownloadURL, string pDownloadLocation, string pMessage)
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

			lbl_Main.Content = "Downloading " + DownloadName + "...";

			// Setting up some Webclient stuff. 
			WebClient webClient = new WebClient();
			webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressed);
			webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
			HelperClasses.Logger.Log("Download Popup: Starting Download");

			// TryCatch of the actual Download
			try
			{
				webClient.DownloadFileAsync(new Uri(DownloadURL), DownloadLocation);
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
		/// Method gets Sets the Download Progress Bar, gets called if the Download Progress Changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DownloadProgressed(object sender, DownloadProgressChangedEventArgs e)
		{
			pb_Main.Value = (double)e.ProgressPercentage;
			lbl_Main.Content = "Downloading " + DownloadName + "...(" + pb_Main.Value + "%)";
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

	} // End of Class
} // End of Namespace