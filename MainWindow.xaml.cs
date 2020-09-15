/*
 
Main Documentation:
Implemented by @thS#0305

Version: 0.0.1.0 unreleased, not working, not fully implemented.

We are taking some shortcuts here, like requiring admin acces,so we dont have to deal with file or regedit permissions..(app.manifest lines 20 and 21 btw)
 
Still needs the actual creative way of Launching and the DRM.

Comments like "TODO", "TO DO", "CTRLF", "CTRL-F", and "CTRL F" are just ways of finding a specific line quickly via searching

Hybrid code can be found in AAA_HybridCode.

Nothing. I repeat: Literally NOTHING. Has been tested.

General Files / Classes:
    MainWindow.xaml.cs
    Settings.xaml.cs
    Popup.xaml.cs
    SaveFileModder.xaml.cs
    ROSIntegration.xaml.cs (Auth stuff from @dr490n)
    
    CustomColors.cs 
    Launcherlogic.cs
    RegeditHandler.cs


General Comments and things one should be aware of (still finishing this list)
Window Icons are set in the Window part of the XAML. Can use resources and relative Paths this way


Main To do:
	There is still some implementation of the classes behind the GUIs to do.
		(GTA V Button Behaviour, Copying Files in SaveFileHandler, Settings read on init and hide rows and stuff)    
	Look at and Comment zCri Backend
    Connect GUI to zCri Backend
	Log everything
    Thoroughly test everything
    Implement Installer + AutoUpdater + Compile .dll's into exe + Sign .exe

    Low Prio:
        Think of making other winodws like settings and save file handler just a part of the main window after clicking the button
        Implement Hamburger Animation
		Theming
		FPS Limiter
*/


using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Project_127
{

	/// <summary>
	/// Main Window which gets displayed when you start the application.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Enum we use to change the Auth Button Image (lock)
		/// </summary>
		enum Authstatus
		{
			NotAuth = 0,
			Auth = 1
		}


		/// <summary>
		/// Constructor of Main Window
		/// </summary>
		public MainWindow()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			// Make sure Hamburger Menu is not opened
			this.GridHamburgerOuter.Visibility = Visibility.Hidden;
			this.btn_Auth.Visibility = Visibility.Hidden;

			// Set Image of Auth Button in top right corner.
			SetAuthButtonBackground(Authstatus.NotAuth);
		}


		/// <summary>
		/// Method which sets the Background Image of the Auth Button
		/// </summary>
		/// <param name="pAuthStatus"></param>
		private void SetAuthButtonBackground(Authstatus pAuthStatus)
		{
			string artwork = "";

			// Set Artwork string depending on which state we have
			if (pAuthStatus == Authstatus.Auth)
			{
				artwork = "Artwork/lockclosed.png";
			}
			else if (pAuthStatus == Authstatus.NotAuth)
			{
				artwork = "Artwork/lockopen.png";
			}

			// Actual Code which changes the image of the auth button
			Uri resourceUri = new Uri(artwork, UriKind.Relative);
			StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
			BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
			var brush = new ImageBrush();
			brush.ImageSource = temp;
			btn_Auth.Background = brush;
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


		/// Below are just the Methods of Button-Clicks

		#region ButtonClicks


		/// <summary>
		/// Method which gets called when Hamburger Menu Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Hamburger_Click(object sender, RoutedEventArgs e)
		{
			if (this.GridHamburgerOuter.Visibility == Visibility.Visible)
			{
				this.GridHamburgerOuter.Visibility = Visibility.Hidden;
				this.btn_Auth.Visibility = Visibility.Hidden;
			}
			else
			{
				this.GridHamburgerOuter.Visibility = Visibility.Visible;
				this.btn_Auth.Visibility = Visibility.Visible;
			}
		}


		/// <summary>
		/// Method which gets called when the exit Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Exit_Click(object sender, RoutedEventArgs e)
		{
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you really want to quit?");
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				Environment.Exit(0);
			}
		}


		/// <summary>
		/// Method which gets called when the Auth Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Auth_Click(object sender, RoutedEventArgs e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOk, "Image you authed through the\nstuff from dr490n and this\nis why the lock is changing").ShowDialog();

			SetAuthButtonBackground(Authstatus.Auth);
		}


		/// <summary>
		/// Method which gets called when the Launch GTA Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_Click(object sender, RoutedEventArgs e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOk, "Image you started the game and\n this is why the border changes color...").ShowDialog();
			btn_GTA.BorderBrush = CustomColors.MW_ButtonGTAGameRunningBorderBrush;
		}


		/// <summary>
		/// Method which gets called when the Launch GTA Button is RIGHT - clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{

		}


		// Hamburger Button Items Below:


		/// <summary>
		/// Method which gets called when the Update Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Upgrade_Click(object sender, RoutedEventArgs e)
		{

		}


		/// <summary>
		/// Method which gets called when the Downgrade Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Downgrade_Click(object sender, RoutedEventArgs e)
		{

		}


		/// <summary>
		/// Method which gets called when the SaveFileHandler Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SaveFiles_Click(object sender, RoutedEventArgs e)
		{
			(new SaveFileHandler()).Show();
		}


		/// <summary>
		/// Method which gets called when the Settings Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Settings_Click(object sender, RoutedEventArgs e)
		{
			(new Settings()).Show();
		}


		/// <summary>
		/// Method which gets called when the Readme Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Readme_Click(object sender, RoutedEventArgs e)
		{
			string msg = "Test";
			msg += "\nTestLine2";
			msg += "\nTestLine3";

			new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
		}


		/// <summary>
		/// Method which gets called when the Readme Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Credits_Click(object sender, RoutedEventArgs e)
		{
			string msg = "DarkViperAU";
			msg += "\nCan fix his Samsung Phone";
			msg += "\nWith a custom Rom";

			new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
		}


		#endregion

	} // End of Class
} // End of Namespace
