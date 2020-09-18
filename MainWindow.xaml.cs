/*
 
Main Documentation:
Main Client Implementation by "@thS#0305"
The actual hard lifting of the launching (with fixes) and authentification stuff is achieved by the hardwork of "@dr490n", "@zCri" and "@Special For"
Artwork, GUI Design, GUI Behaviour, Colorchoice etc. by "@Hossel"

Version: 0.0.1.1 unreleased, not working, not fully implemented.
From now on, versions will be changed here
as well as in the assembly info of this file (Project 127 -> Properties -> AssemblyInfo.cs -> Very bottom of the file

We are taking some shortcuts here, like requiring admin acces,so we dont have to deal with file or regedit permissions..(app.manifest lines 20 and 21 btw)
 
Still needs the actual creative way of Launching and the DRM.

Comments like "TODO", "TO DO", "CTRLF", "CTRL-F", and "CTRL F" are just ways of finding a specific line quickly via searching

Hybrid code can be found in AAA_HybridCode.

Not much. I repeat: Literally Not much. Has been tested.

General Files / Classes:
    MainWindow.xaml.cs
    Popup.xaml.cs
    Settings.xaml.cs + SettingsPartial.cs
    SaveFileModder.xaml.cs
    ROSIntegration.xaml.cs (Auth stuff from @dr490n)
    
    Globals.cs 
	MyFile.cs (Custom Class for Objects for the SaveFileHandler, Very much subject to change)
	LauncherLogic.cs
    HelperClasses\Logger.cs
    HelperClasses\RegeditHandler.cs
	HelperClasses\FileHandler.cs


General Comments and things one should be aware of (still finishing this list)
	Window Icons are set in the Window part of the XAML. Can use resources and relative Paths this way
		This doesnt change the icon when right clicking task bar btw.
	My Other ideas of creating Settings (CustomControl or Programtically) didnt work because
		DataBinding the ButtonContext is hard for some reason. Works which the checkbox tho, which is kinda weird

Main To do:
	Programming ToDo:
		Test Auto Updater
		Check Installer for Custom options etc.
	
	After that:
	Fix Code signing so we dont get anti virus error
	Implemt other features (GTA V Button Behaviour, all Settings, Theming)
    Thoroughly test everything

	Keep in Mind:
		GameStates (no settings) with On_Closing Event code are in place
			if we go with copying files instead of hardlinking
			we need to change that

    Low Prio:
		Convert Settings and SaveFileHandler in CustomControls
		Theming
        Implement Hamburger Animation
		Get DataBinding working on Button Context for Settings
		Own FPS Limiter
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
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
using AutoUpdaterDotNET;
using System.IO.Compression;
using Microsoft.Win32;

namespace Project_127
{
	/// <summary>
	/// CLass of Main Window which gets displayed when you start the application.
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

			// Dont run anything when we are on 32 bit...
			// If this ever gets changed, take a second look at regedit class and path(different for 32 and 64 bit OS)
			if (Environment.Is64BitOperatingSystem == false)
			{
				(new Popup(Popup.PopupWindowTypes.PopupOkError, "32 Bit Operating System detected.\nGTA (afaik) does not run on 32 Bit at all.")).ShowDialog();
				Environment.Exit(1);
			}

			if (!CheckIfAllowedToRun())
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "You are not allowed to run this beta.").ShowDialog();
				Environment.Exit(2);
			}

			// Start the Init Process
			Globals.Init();

			// Make sure Hamburger Menu is invisible when opening window
			this.GridHamburgerOuter.Visibility = Visibility.Hidden;
			this.btn_Auth.Visibility = Visibility.Hidden;

			// Set Image of Auth Button in top right corner.
			SetAuthButtonBackground(Authstatus.NotAuth);

			// Auto Updater
			AutoUpdater.Start(Globals.URL_AutoUpdate);
		}



		#region GUI-Clicks


		/// <summary>
		/// Method which gets called when Hamburger Menu Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Hamburger_Click(object sender, RoutedEventArgs e)
		{
			// If is visible
			if (this.GridHamburgerOuter.Visibility == Visibility.Visible)
			{
				// Make invisible
				this.GridHamburgerOuter.Visibility = Visibility.Hidden;
				this.btn_Auth.Visibility = Visibility.Hidden;
			}
			// If is not visible
			else
			{
				// Make visible
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
			new Popup(Popup.PopupWindowTypes.PopupOk, "Auth not fully implemented yet.\nImage you authed through the\nstuff from dr490n and this\nis why the lock is changing").ShowDialog();
			SetAuthButtonBackground(Authstatus.Auth);
		}

		/// <summary>
		/// Method which gets called when the Launch GTA Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_Click(object sender, RoutedEventArgs e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOk, "Launching Behaviour not fully implemented yet.\nImage you started the game and\n this is why the border changes color...").ShowDialog();
			btn_GTA.BorderBrush = Globals.MW_ButtonGTAGameRunningBorderBrush;
		}

		/// <summary>
		/// Method which gets called when the Launch GTA Button is RIGHT - clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOk, "Right Click Behaviour not fully implemented yet.\nImage this closed the game for you and\n this is why the border changes color...").ShowDialog();
			btn_GTA.BorderBrush = Globals.MW_ButtonGTAGameNotRunningBorderBrush;
		}


		// Hamburger Button Items Below:


		/// <summary>
		/// Method which gets called when the Update Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Upgrade_Click(object sender, RoutedEventArgs e)
		{
			HelperClasses.Logger.Log("Clicked the Upgrade Button");
			if (LauncherLogic.MyGameState == LauncherLogic.GameState.Downgraded)
			{
				HelperClasses.Logger.Log("Gamestate looks OK. Will Proceed to try to downgrade.",1);
				LauncherLogic.Upgrade();
				LauncherLogic.MyGameState = LauncherLogic.GameState.Upgraded;
			}
			else
			{
				HelperClasses.Logger.Log("This program THINKS you are already downgraded. Downgrad procedure will not be called.", 1);
			}
		}

		/// <summary>
		/// Method which gets called when the Update Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Repair_Click(object sender, RoutedEventArgs e)
		{
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "This Method is supposed to be called when there was a Game Update.\nOr you verified Files through Steam.\nIs that the case?");
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				LauncherLogic.Repair();
			}
			LauncherLogic.MyGameState = LauncherLogic.GameState.Upgraded;
		}

		/// <summary>
		/// Method which gets called when the Downgrade Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Downgrade_Click(object sender, RoutedEventArgs e)
		{
			if (LauncherLogic.MyGameState == LauncherLogic.GameState.Upgraded)
			{
				LauncherLogic.Downgrade();
				LauncherLogic.MyGameState = LauncherLogic.GameState.Downgraded;
			}
		}

		/// <summary>
		/// Method which gets called when the "Import ZIP" Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_ImportZip_Click(object sender, RoutedEventArgs e)
		{
			string[] myFiles = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(LauncherLogic.DowngradeFilePath);
			foreach (string myFile in myFiles)
			{
				HelperClasses.FileHandling.deleteFile(myFile);
			}

			string ZipFileLocation = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Title", "", Settings.InstallationPath);
			ZipFile.ExtractToDirectory(ZipFileLocation, Settings.InstallationPath);
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
		/// Method which gets called when you click on the SpeedRun Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Speedrun_Click(object sender, RoutedEventArgs e)
		{

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
			msg += "\n\n" + Globals.ProjectVersion.ToString();

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


		#region GUI Helper Methods


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
				artwork = @"Artwork\lockclosed.png";
			}
			else if (pAuthStatus == Authstatus.NotAuth)
			{
				artwork = @"Artwork\lockopen.png";
			}


			// Im not checking if the file exists, since its compiled in the .exe as of this moment...
			// May have to think about that for theming...

			// Actual Code which changes the image of the auth button
			try
			{
				Uri resourceUri = new Uri(artwork, UriKind.Relative);
				StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
				BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
				var brush = new ImageBrush();
				brush.ImageSource = temp;
				btn_Auth.Background = brush;
			}
			catch
			{
				HelperClasses.Logger.Log("Failed to set Background Image for Auth Button");
			}
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

		/// <summary>
		/// Gets called when MainWindow is being closed by user, task manager (not kill process), ALT+F4, or taskbar
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Globals.ProperExit();
		}

		/// <summary>
		/// Exits when you are not supposed to run this. Its ugly. It works. Meh.
		/// </summary>
		private bool CheckIfAllowedToRun()
		{
			try
			{
				if (Globals.BetaMode)
				{
					if (Globals.CommandLineArguments.Contains("skiplogin"))
					{
						return true; 
					}

					RegistryKey MyKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("Microsoft").CreateSubKey("Cryptography");
					string GUID = HelperClasses.RegeditHandler.GetValue(MyKey, "MachineGuid");


					System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
					string Reply = client.GetStringAsync(Globals.URL_AuthUser).GetAwaiter().GetResult();

					if (Reply.Contains(GUID))
					{
						return true;
					}
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		#endregion


	} // End of Class
} // End of Namespace
