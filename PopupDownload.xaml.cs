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
	/// Enum for Download Types
	/// </summary>
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
		/// <summary>
		/// PopupDownloadType Propertie of this object (PopupDownload Window)
		/// </summary>
		public PopupDownloadTypes PopupDownloadType;

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
		/// <param name="pPopupDownloadType"></param>
		/// <param name="pDownloadURL"></param>
		/// <param name="pDownloadLocation"></param>
		public PopupDownload(PopupDownloadTypes pPopupDownloadType, string pDownloadURL, string pDownloadLocation)
		{
			InitializeComponent();

			// Setting Properties of our Object (Popup Window)
			PopupDownloadType = pPopupDownloadType;
			DownloadURL = pDownloadURL;
			DownloadLocation = pDownloadLocation;

			// Logging
			HelperClasses.Logger.Log("Download Popup: DownloadType: '" + pPopupDownloadType.ToString() + "'");
			HelperClasses.Logger.Log("Download Popup: DownloadURL: '" + pDownloadURL.ToString() + "'");
			HelperClasses.Logger.Log("Download Popup: DownloadLocation: '" + pDownloadLocation.ToString() + "'");

			// If we have a ZIP
			if (this.PopupDownloadType == PopupDownloadTypes.ZIP)
			{
				lbl_Main.Content = "Downloading ZIP File...";
			}
			// If we have an Installer
			else if (this.PopupDownloadType == PopupDownloadTypes.Installer)
			{
				lbl_Main.Content = "Downloading Installer...";
			}

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
				HelperClasses.Logger.Log("Update of '" + PopupDownloadType.ToString() + "' failed for some reason." + e.Message);
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Update of '" + PopupDownloadType.ToString() + "' failed for some reason.\nI suggest restarting the program and opting out of update");
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
			if (this.PopupDownloadType == PopupDownloadTypes.ZIP)
			{
				lbl_Main.Content = "Downloading ZIP File...(" + pb_Main.Value + "%)";
			}
			else if (this.PopupDownloadType == PopupDownloadTypes.Installer)
			{
				lbl_Main.Content = "Downloading Installer...(" + pb_Main.Value + "%)";
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

	} // End of Class
} // End of Namespace