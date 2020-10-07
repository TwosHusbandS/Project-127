/*

Main Documentation:
Actual code (partially closed source) which authentificates, handles entitlement and launches the game is done by @dr490n with the help of other members of the core team like @Special For and @zCri
Artwork, Design of GUI, GUI Behaviourehaviour, Colorchoices etc. by "@Hossel"
Client by "@thS"
Version: 0.0.4.0

Build Instructions:
	Press CTRLF + F5, pray that nuget does its magic.

Deploy Instructions:
	Change Version Number a few Lines Above.
	Change Version Number in both of the last lines in AssemblyInfo.cs
	Check if BetaMode in Globals.cs is correct
	Check if BuildInfo in Globals.cs is correct
	Make sure app manifest is set TO require admin
	Build this program in release
	Build installer via Innosetup (Script is in \Installer\) [Change Version in Version and OutputName]
	Change Version number and Installer Location in "\Installer\Update.xml"
	Push Commit to github branch.
	Merge branch into master

Comments like "TODO", "TO DO", "CTRLF", "CTRL-F", and "CTRL F" are just ways of finding a specific line quickly via searching

Hybrid code can be found in AAA_HybridCode.

General Files / Classes:
	Windows:
		MainWindow.xaml.cs
		Settings.xaml.cs
			SettingsPartial.cs
		SaveFileHandler.xaml.cs
		Popup.xaml.cs // Normal Popup (OK & OKERROR & YES/NO)
		PopupDownload.xaml.cs // Popup for Downloading Files
		PopupProgress.xaml.cs // Popup for large file operation with semi-optinal loading bar
		ROSIntegration.xaml.cs // Auth window fo @dr490n

	Classes:
		Globals.cs  // Global Variables and Central Place
		LauncherLogic.cs // Most of the downgrade, upgrade, repair, launch things
		MySaveFile.cs // Custom Class for Objects for the SaveFileHandler, Very much subject to change
		MyFileOperation.cs // One file operation (move/delete/copy/hardlink), used for bigger file operations 
		HelperClasses\Logger.cs // Own logger
		HelperClasses\RegeditHandler.cs // Wrappers for used Regedit things
		HelperClasses\ProcessHandler.cs // Wrappers for process stuff, Subject to change
		HelperClasses\FileHandler.cs // Wrapers for file stuff
		EntitlementBlock.cs // Backend by @d490n
		EntitlementBlockCipher.cs // Backend by @d490n
		RC4.cs // Backend by @d490n
		ROSCommunicationBackend.cs // Backend by @d490n

General Comments and things one should be aware of (still finishing this list)
	- Window Icons are set in the Window part of the XAML. Can use resources and relative Paths this way
		This doesnt change the icon when right clicking task bar btw.
	- My Other ideas of creating Settings (CustomControl or Programtically) didnt work because
		DataBinding the ButtonContext is hard for some reason. Works which the checkbox tho, which is kinda weird
	- BetaMode is hardcoded in Globals.cs
	- Importing ZIP currently overwrites all files (including version.txt) apart from "UpgradeFiles" Folder
	- We are doing some funky AdminRelauncher() stuff. This means debugger wont work tho.
		To actually debug this we need to change the requestedExecutionLevel in App Manifest.
		Never built with requestedExecutionLevel administrator tho. Will fail to launch from installer
	- Installation Path gets written to Registry on every Launch to make sure its always up to date.

Main To do:
	- Things changed since last official release (not last commit)

	-REMEMBER:
		-> Release with admin mode manifest thingy...		
		-> Fix Installer with everything (autolaunch app,include new files)
		-> This requires admin the "proper" way of telling windows. Should fix zip file issues
					
	- TO DO:

	// NEXT PUBLIC RELEASE

		-> Think about checking file hashes in UpgradeFolder as well as GTAV Installation Path
			=> We could solve the need for the RepairFunction, but this would mean CPU Intensive task while Downgrading
		-> $UpgradeFiles has downgrade files in them. Why? And how to Fix?
		-> Core Affinity Shit
		-> Figure out which files I need to distribute
		-> Custom ZIP File Location User Error Checks:
			=> User might get confused with the Project_127_Files Folder. 
				Maybe we should actually check parent folders and child folders when User is selecting a Path for ZIP File
				>> The thing is. This shouldnt be needed since we delete folders on moving ZIP files and stuff
		-> Regedit Value "LastLaunchedVersion" is there and be used with the next Version.
		-> Think about making a spawner to spawn processes
		   (Process.Start(@"C:\Users\ingow\source\repos\ProcessSpawner127\bin\x64\Release\ProcessSpawner127.exe", "testA testB");)
		-> Regedit Cleanup of everything not in default settings
		-> Settings dont update content
			=> Currently it calls the Refresh Method after each click...which works but is ugly
			=> Get data binding to work after everything else is Gucci 
		-> Implement all Other features
			=> Just see Settings.XAML for what I need to implement
		-> Implement note thingy from reloes suggestion (https://discordapp.com/channels/758296338222940211/758296338806341684/762023004183461888)

    - Low Prio:
		Convert Settings and SaveFileHandler and ROSIntegration in CustomControls
		Add Audio Effects
		Popup start in middle of window, instead of CenterScreen
		Fix Code signing so we dont get anti virus error
		Theming

Weird Beta Reportings:
	- Reloe and JakeMiester and dr490n had some issues with the GTA V Path Settings
			Changed a lot of the backend for that. Should all be fixed.
			If given Path is detected to be wrong, but user has to confirm this three times every startup...
	- Game wasnt launching for most
			I actually hardcoded the Path...its now fixed.
	- People had to run as Admin manually even tho I have the admin relauncher.
	- ZIP Extraction failed for 2 people when not manually running as admin
			Added more and better logging, issue can not be reproduced as of right now
	- Open Twice message (and killing old process) not working for one guy
			Works for me and on some other testers machines.
	- Auth Window popup crashed for Hossel in some circumstances. (CredentialManager dll missing)
	- Investigate oneDrive shit (turned out to be crypto char in document path [or path variable??])
	- Dragons NAS machine was just doing weird stuff with importing zip.
		-> Simple stuff like (If Path doesnt exist, create it) crashed for no reason
		-> Added logging to extract zip didnt work and failed in weird places
		-> Complained about STATHread even its there
		-> "Old" no progressbar zip extracting also failed
		-> Probably due to NAS
	- Investigate Specials worry about user getting "Already auth" even when session expired.
		=> Took a quick look at the code, this should not happen, but special said it happened before...
															also Software doing Software things...
		=> Bottom Line: INVESTIGATE

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
using System.IO.Compression;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.ComponentModel;

namespace Project_127
{
	/// <summary>
	/// CLass of Main Window which gets displayed when you start the application.
	/// </summary>
	public partial class MainWindow : Window
	{
		/*
		MainWindow:
		- Contains all Main GUI stuff
		- a few Methods we need on Starting like AutoUpdating or Re-Launching with Admin
		*/

		// This is a static property pointing to the only instance of MainWindow...
		// This ugly like yo mama, but shit happerino, you know?

		/// <summary>
		/// Static Property to access Children (mainly Controls) of MainWindow Instance
		/// </summary>
		public static MainWindow MW;

		/// <summary>
		/// Constructor of Main Window
		/// </summary>
		public MainWindow()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			// Setting this for use in other classes later
			MainWindow.MW = this;

			// Admin Relauncher
			AdminRelauncher();

			//Dont run anything when we are on 32 bit...
			//If this ever gets changed, take a second look at regedit class and path(different for 32 and 64 bit OS)
			if (Environment.Is64BitOperatingSystem == false)
			{
				(new Popup(Popup.PopupWindowTypes.PopupOkError, "32 Bit Operating System detected.\nGTA (afaik) does not run on 32 Bit at all.")).ShowDialog();
				Environment.Exit(1);
			}


			// Checks if a Process with the same ProcessName is already running
			if (HelperClasses.ProcessHandler.GetProcesses(Process.GetCurrentProcess().ProcessName).Length > 1)
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Program is open twice. Do you want to force close the old Instance?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.ProcessHandler.KillProcessesContains(Process.GetCurrentProcess().ProcessName);
				}
				else
				{
					Environment.Exit(2);
				}
			}

			// Start the Init Process of Logger, Settings, Globals, Regedit here, since we need the Logger in the next Line if it fails...
			Globals.Init(this);

			// Checks if you are allowed to run this Beta
			//if (!CheckIfAllowedToRun())
			//{
			//	HelperClasses.Logger.Log("You are not allowed to run this Beta.");
			//	new Popup(Popup.PopupWindowTypes.PopupOkError, "You are not allowed to run this Beta.").ShowDialog();
			//	Environment.Exit(3);
			//}

			// Deleting all Installer and ZIP Files from own Project Installation Path
			DeleteOldFiles();

			// Make sure Hamburger Menu is invisible when opening window
			this.GridHamburgerOuter.Visibility = Visibility.Hidden;
			this.GridHamburgerOuterSeperator.Visibility = Visibility.Hidden;
			PageState = PageStates.GTA;

			// Set Image of Buttons
			SetButtonMouseOverMagic(btn_Exit, false);
			SetButtonMouseOverMagic(btn_Auth, false);
			SetButtonMouseOverMagic(btn_Hamburger, false);

			// Auto Updater
			CheckForUpdate();

			// Downloads the "big 3" gamefiles from github release
			CheckForBigThree();

			// Check whats the latest Version of the ZIP File in GITHUB
			CheckForZipUpdate();

			if (Globals.InternalMode)
			{
				string msg = "We are in internal mode. I need testing on:\n" +
					"NEW DEPLOYMENT CONCEPT" + "\n" +
					"INTERNAL RELEASE SHIT" + "\n" +
					"NAME CHANGER, WITH NEW DRAGON BACKEND" + "\n" +
					"REMEMBER ME FUNCTION FROM DRAGON" + "\n" +
					"LANGUAGE SELECT" + "\n" +
					"AUTO HIGH PRIORITY" + "\n" +
					"SAVEFILEHANDLER" + "\n" +
					"IMPORTING SAVEFILES AND IMPORTING GTAV SETTINGS" + "\n" +
					"NEW LAUNCH METHOD FOR ALL 6 COMBINATIONS (3 RETAILRS. UPGRADED / DOWNGRADED)" + "\n\n" +
					"\nThanks. Appreciated. Have a great day : )";

				new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
			}


			HelperClasses.Logger.Log("Startup procedure (Constructor of MainWindow) completed.");
			HelperClasses.Logger.Log("--------------------------------------------------------");
		}


		public enum PageStates
		{
			Settings,
			SaveFileHandler,
			Auth,
			GTA
		}


		private PageStates _PageState;


		public PageStates PageState
		{
			get
			{
				return _PageState;
			}
			set
			{
				_PageState = value;
				if (value == PageStates.Settings)
				{
					Frame_Main.Content = new Settings();
					btn_Settings.Background = Globals.MW_ButtonMOBackground;
					btn_Settings.Foreground = Globals.MW_ButtonMOForeground;

					btn_SaveFiles.Background = Globals.MW_ButtonBackground;
					btn_SaveFiles.Foreground = Globals.MW_ButtonForeground;
				}
				else if (value == PageStates.SaveFileHandler)
				{
					Frame_Main.Content = new SaveFileHandler();
					btn_SaveFiles.Background = Globals.MW_ButtonMOBackground;
					btn_SaveFiles.Foreground = Globals.MW_ButtonMOForeground;

					btn_Settings.Background = Globals.MW_ButtonBackground;
					btn_Settings.Foreground = Globals.MW_ButtonForeground;
				}
				else if (value == PageStates.Auth)
				{
					Frame_Main.Content = new ROSIntegration();

					btn_SaveFiles.Background = Globals.MW_ButtonBackground;
					btn_SaveFiles.Foreground = Globals.MW_ButtonForeground;

					btn_Settings.Background = Globals.MW_ButtonBackground;
					btn_Settings.Foreground = Globals.MW_ButtonForeground;
				}
				else if (value == PageStates.GTA)
				{
					Frame_Main.Content = new GTA_Page();

					btn_SaveFiles.Background = Globals.MW_ButtonBackground;
					btn_SaveFiles.Foreground = Globals.MW_ButtonForeground;

					btn_Settings.Background = Globals.MW_ButtonBackground;
					btn_Settings.Foreground = Globals.MW_ButtonForeground;
				}
				else
				{
					// This should never happen
				}
			}
		}



		/// <summary>
		/// Responsible for Re-Launching this App as Admin if it isnt.
		/// </summary>
		private void AdminRelauncher()
		{
			// If not run as Admin
			if (!IsRunAsAdmin())
			{
				try
				{
					// CTRLF TODO // THIS MIGHT BE BROKEN WITH COMMAND LINE ARGS THAT CONTAIN SPACES
					HelperClasses.ProcessHandler.StartProcess(Assembly.GetEntryAssembly().CodeBase, Environment.CurrentDirectory, string.Join(" ", Globals.CommandLineArgs.ToString()), true, true, false);
					Application.Current.Shutdown();
				}
				catch (Exception e)
				{
					Globals.DebugPopup("This program must be run as an administrator!");
				}
			}
		}


		/// <summary>
		/// Method which checks if this program is run as admin. Returns one bool
		/// </summary>
		/// <returns></returns>
		private bool IsRunAsAdmin()
		{
			try
			{
				WindowsIdentity id = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(id);
				return principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch (Exception)
			{
				return false;
			}
		}


		/// <summary>
		/// Deleting all Old Files (Installer and ZIP Files) from the Installation Folder
		/// </summary>
		private void DeleteOldFiles()
		{
			HelperClasses.Logger.Log("Checking if there is an old Installer or ZIP Files in the Project InstallationPath during startup procedure.");

			// Looping through all Files in the Installation Path
			foreach (string myFile in HelperClasses.FileHandling.GetFilesFromFolder(Globals.ProjectInstallationPath))
			{
				// If it contains the word installer, delete it
				if (myFile.ToLower().Contains("installer"))
				{
					HelperClasses.Logger.Log("Found old installer File ('" + HelperClasses.FileHandling.PathSplitUp(myFile)[1] + "') in the Directory. Will delete it.");
					HelperClasses.FileHandling.deleteFile(myFile);
				}
				// If it is the Name of the ZIP File we download, we delete it
				if (myFile == Globals.ZipFileDownloadLocation)
				{
					HelperClasses.Logger.Log("Found old ZIP File ('" + HelperClasses.FileHandling.PathSplitUp(myFile)[1] + "') in the Directory. Will delete it.");
					HelperClasses.FileHandling.deleteFile(myFile);
				}
			}
		}


		/// <summary>
		/// Method which does the UpdateCheck on Startup
		/// </summary>
		private void CheckForUpdate()
		{
			// Check online File for Version.
			string MyVersionOnlineString = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "version");

			// If this is empty,  github returned ""
			if (!(String.IsNullOrEmpty(MyVersionOnlineString)))
			{
				// Building a Version out of the String
				Version MyVersionOnline = new Version(MyVersionOnlineString);

				// Logging some stuff
				HelperClasses.Logger.Log("Checking for Project 1.27 Update during start up procedure");
				HelperClasses.Logger.Log("MyVersionOnline = '" + MyVersionOnline.ToString() + "', Globals.ProjectVersion = '" + Globals.ProjectVersion + "'", 1);

				// If Online Version is "bigger" than our own local Version
				if (MyVersionOnline > Globals.ProjectVersion)
				{
					// Update Found.
					HelperClasses.Logger.Log("Update found", 1);
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Version: '" + MyVersionOnline.ToString() + "' found on the Server.\nVersion: '" + Globals.ProjectVersion.ToString() + "' found installed.\nDo you want to upgrade?");
					yesno.ShowDialog();
					// Asking User if he wants update.
					if (yesno.DialogResult == true)
					{
						// User wants Update
						HelperClasses.Logger.Log("Update found. User wants it", 1);
						string DLPath = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "url");
						string DLFilename = DLPath.Substring(DLPath.LastIndexOf('/') + 1);
						string LocalFileName = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\" + DLFilename;

						new PopupDownload(DLPath, LocalFileName, "Installer").ShowDialog();
						HelperClasses.ProcessHandler.StartProcess(LocalFileName);
						Environment.Exit(0);
					}
					else
					{
						// User doesnt want update
						HelperClasses.Logger.Log("Update found. User does not wants it", 1);
					}
				}
				else
				{
					// No update found
					HelperClasses.Logger.Log("No Update Found");
				}
			}
			else
			{
				// String return is fucked
				HelperClasses.Logger.Log("Did not get most up to date Project 1.27 Version from Github. Github offline or your PC offline. Probably. Lets hope so.");
			}
		}


		/// <summary>
		/// Checks Github for the big 3 files we need
		/// </summary>
		public static void CheckForBigThree()
		{
			HelperClasses.Logger.Log("Downloading the 'big three' files");

			string DLLinkG = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkG");
			string DLLinkU = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkU");
			string DLLinkX = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "DLLinkX");

			HelperClasses.Logger.Log("Checking if gta5.exe exists locally", 1);
			if (HelperClasses.FileHandling.doesFileExist(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe"))
			{
				HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			}
			else
			{
				HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);
				new PopupDownload(DLLinkG, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe", "Needed Files (gta5.exe 1/3)").ShowDialog();
			}

			HelperClasses.Logger.Log("Checking if x64a.rpf exists locally", 1);
			if (HelperClasses.FileHandling.doesFileExist(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf"))
			{
				HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			}
			else
			{
				HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);
				new PopupDownload(DLLinkX, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\x64a.rpf", "Needed Files (x64a.rpf, 2/3)").ShowDialog();
			}

			HelperClasses.Logger.Log(@"Checking if update\update.rpf exists locally", 1);
			if (HelperClasses.FileHandling.doesFileExist(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf"))
			{
				HelperClasses.Logger.Log("It does and we dont need to download anything", 2);
			}
			else
			{
				HelperClasses.Logger.Log("It does NOT and we DO need to download something", 2);
				new PopupDownload(DLLinkU, LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf", "Needed Files (Update.rpf, 3/3)").ShowDialog();
			}
		}


		/// <summary>
		/// Checks for Update of the ZIPFile and extracts it
		/// </summary>
		public static void CheckForZipUpdate()
		{
			// Check whats the latest Version of the ZIP File in GITHUB
			int ZipOnlineVersion = 0;
			Int32.TryParse(HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zipversion"), out ZipOnlineVersion);

			HelperClasses.Logger.Log("Checking for ZIP - Update");
			HelperClasses.Logger.Log("ZipVersion = '" + Globals.ZipVersion + "', ZipOnlineVersion = '" + ZipOnlineVersion + "'");

			// If Zip file from Server is newer
			if (ZipOnlineVersion > Globals.ZipVersion)
			{
				HelperClasses.Logger.Log("Update for ZIP found");
				Popup yesno;
				if (Globals.ZipVersion > 0)
				{
					yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "ZIP Version: '" + ZipOnlineVersion.ToString() + "' found on the Server.\nZIP Version: '" + Globals.ZipVersion.ToString() + "' found installed.\nDo you want to upgrade?");
				}
				else
				{
					yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "ZIP Version: '" + ZipOnlineVersion.ToString() + "' found on the Server.\nNo ZIP Version found installed.\nDo you want to install the ZIP?");
				}
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("User wants update for ZIP");

					// Getting the Hash of the new ZIPFile
					string hashNeeded = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zipmd5");
					HelperClasses.Logger.Log("HashNeeded: " + hashNeeded);

					// Looping 0 through 5
					for (int i = 0; i <= 5; i++)
					{
						// Getting DL Link of zip + i
						string pathOfNewZip = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zip" + i.ToString());
						HelperClasses.Logger.Log("Zip-Try: 'zip" + i.ToString() + "'");
						HelperClasses.Logger.Log("DL Link: '" + pathOfNewZip + "'");

						// Deleting old ZIPFile
						HelperClasses.FileHandling.deleteFile(Globals.ZipFileDownloadLocation);

						// Downloading the ZIP File
						new PopupDownload(pathOfNewZip, Globals.ZipFileDownloadLocation, "ZIP-File").ShowDialog();

						// Checking the hash of the Download
						string HashOfDownload = HelperClasses.FileHandling.GetHashFromFile(Globals.ZipFileDownloadLocation);
						HelperClasses.Logger.Log("Download Done, Hash of Downloaded File: '" + HashOfDownload + "'");

						// If Hash looks good, we import it
						if (HashOfDownload == hashNeeded)
						{
							HelperClasses.Logger.Log("Hashes Match, will Import");
							LauncherLogic.ImportZip(Globals.ZipFileDownloadLocation, true);
							return;
						}
						HelperClasses.Logger.Log("Hashes dont match, will move on");
					}
					HelperClasses.Logger.Log("Error. Could not find a suitable ZIP File from a FileHoster. Program cannot download new ZIP at the moment.");
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Update of ZIP File failed (No Suitable ZIP Files Found).\nI suggest restarting the program and opting out of update.");
				}
				else
				{
					HelperClasses.Logger.Log("User does not want update for ZIP");
				}
			}
			else
			{
				HelperClasses.Logger.Log("NO Update for ZIP found");
			}
		}


		#region GUI Helper Methods

		/// <summary>
		/// Updates the GUI with relevant stuff. Gets called every 5 Seconds
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void UpdateGUIDispatcherTimer(object sender = null, EventArgs e = null) // Gets called every DispatcherTimer_Tick. Just starts the read function.
		{
			if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
			{
				lbl_GTA.Foreground = Globals.MW_GTALabelDowngradedForeground;
				lbl_GTA.Content = "Downgraded";
			}
			else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
			{
				lbl_GTA.Foreground = Globals.MW_GTALabelUpgradedForeground;
				lbl_GTA.Content = "Upgraded";
			}
			else
			{
				lbl_GTA.Foreground = Globals.MW_GTALabelBrokenForeground;
				lbl_GTA.Content = "Broken";
			}
			if (LauncherLogic.GameState == LauncherLogic.GameStates.Running)
			{
				GTA_Page.btn_GTA_static.BorderBrush = Globals.MW_ButtonGTAGameRunningBorderBrush;
				GTA_Page.btn_GTA_static.Content = "Exit GTA V";
			}
			else
			{
				GTA_Page.btn_GTA_static.BorderBrush = Globals.MW_ButtonGTAGameNotRunningBorderBrush;
				GTA_Page.btn_GTA_static.Content = "Launch GTA V";
			}
			SetButtonMouseOverMagic(btn_Auth, false);
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
		/// Sets the Backgrund of a specific Button
		/// </summary>
		/// <param name="myBtn"></param>
		/// <param name="pArtpath"></param>
		private void SetButtonBackground(Button myBtn, string pArtpath)
		{
			try
			{
				Uri resourceUri = new Uri(pArtpath, UriKind.Relative);
				StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
				BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
				var brush = new ImageBrush();
				brush.ImageSource = temp;
				myBtn.Background = brush;
			}
			catch
			{
				HelperClasses.Logger.Log("Failed to set Background Image for Button");
			}
		}

		/// <summary>
		/// Method we use to call SetButtonBackground with the right parameters to Update GUI
		/// </summary>
		/// <param name="myBtn"></param>
		/// <param name="pMouseOver"></param>
		private void SetButtonMouseOverMagic(Button myBtn, bool pMouseOver)
		{
			switch (myBtn.Name)
			{
				case "btn_Hamburger":
					if (pMouseOver)
					{
						SetButtonBackground(myBtn, @"Artwork\hamburger_mo.png");
					}
					else
					{
						SetButtonBackground(myBtn, @"Artwork\hamburger.png");
					}
					break;
				case "btn_Exit":
					if (pMouseOver)
					{
						SetButtonBackground(myBtn, @"Artwork\exit_mo.png");
					}
					else
					{
						SetButtonBackground(myBtn, @"Artwork\exit.png");
					}
					break;
				case "btn_Auth":
					if (pMouseOver)
					{
						if (LauncherLogic.AuthState == LauncherLogic.AuthStates.Auth)
						{
							SetButtonBackground(myBtn, @"Artwork\lock_closed_mo.png");
						}
						else if (LauncherLogic.AuthState == LauncherLogic.AuthStates.NotAuth)
						{
							SetButtonBackground(myBtn, @"Artwork\lock_open_mo.png");
						}
					}
					else
					{
						if (LauncherLogic.AuthState == LauncherLogic.AuthStates.Auth)
						{
							SetButtonBackground(myBtn, @"Artwork\lock_closed.png");
						}
						else if (LauncherLogic.AuthState == LauncherLogic.AuthStates.NotAuth)
						{
							SetButtonBackground(myBtn, @"Artwork\lock_open.png");
						}
					}
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// MouseEnter event for updating background image of buttons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Small_MouseEnter(object sender, MouseEventArgs e)
		{
			SetButtonMouseOverMagic((Button)sender, true);
		}

		/// <summary>
		/// MouseLeave event for updating background image of buttons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Small_MouseLeave(object sender, MouseEventArgs e)
		{
			SetButtonMouseOverMagic((Button)sender, false);
		}


		#endregion


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
				this.GridHamburgerOuterSeperator.Visibility = Visibility.Hidden;
				PageState = PageStates.GTA;
			}
			// If is not visible
			else
			{
				// Make visible
				this.GridHamburgerOuter.Visibility = Visibility.Visible;
				this.GridHamburgerOuterSeperator.Visibility = Visibility.Visible;
			}
		}

		/// <summary>
		/// Rightclick on Hamburger Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Hamburger_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Globals.BetaMode || Globals.InternalMode)
			{
				// Opens the File
				HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\System32\notepad.exe", pCommandLineArguments: Globals.Logfile);
			}
		}



		/// <summary>
		/// Method which gets called when the Auth Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Auth_Click(object sender, RoutedEventArgs e)
		{
			if (LauncherLogic.AuthState == LauncherLogic.AuthStates.NotAuth)
			{
				PageState = PageStates.Auth;
			}
			else
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "You are already authenticated.").ShowDialog();
			}

			// Yes this is correct
			SetButtonMouseOverMagic(btn_Auth, false);
		}


		/// <summary>
		/// Right click on Auth button. Gives proper Debug Output
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Auth_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Globals.BetaMode || Globals.InternalMode)
			{
				// Debug Info users can give me easily...
				List<string> DebugMessage = new List<string>();

				DebugMessage.Add("Project 1.27 Version: '" + Globals.ProjectVersion + "'");
				DebugMessage.Add("ZIP Version: '" + Globals.ZipVersion + "'");
				DebugMessage.Add("BetaMode: '" + Globals.BetaMode + "'");
				DebugMessage.Add("InternalMode: '" + Globals.InternalMode + "'");
				DebugMessage.Add("Project 1.27 Installation Path '" + Globals.ProjectInstallationPath + "'");
				DebugMessage.Add("ZIP Extraction Path '" + LauncherLogic.ZIPFilePath + "'");
				DebugMessage.Add("LauncherLogic.GTAVFilePath: '" + LauncherLogic.GTAVFilePath + "'");
				DebugMessage.Add("LauncherLogic.UpgradeFilePath: '" + LauncherLogic.UpgradeFilePath + "'");
				DebugMessage.Add("LauncherLogic.DowngradeFilePath: '" + LauncherLogic.DowngradeFilePath + "'");
				DebugMessage.Add("LauncherLogic.SupportFilePath: '" + LauncherLogic.SupportFilePath + "'");
				DebugMessage.Add("Detected AuthState: '" + LauncherLogic.AuthState + "'");
				DebugMessage.Add("Detected GameState: '" + LauncherLogic.GameState + "'");
				DebugMessage.Add("Detected InstallationState: '" + LauncherLogic.InstallationState + "'");
				DebugMessage.Add("    Size of GTA5.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of GTA5.exe in Downgrade Files Folder: " + LauncherLogic.SizeOfDowngradedGTAV);
				DebugMessage.Add("    Size of update.rpf in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of update.rpf in Downgrade Files Folder: " + LauncherLogic.SizeOfDowngradedUPDATE);
				DebugMessage.Add("Settings: ");
				foreach (KeyValuePair<string, string> KVP in Globals.MySettings)
				{
					DebugMessage.Add("    " + KVP.Key + ": '" + KVP.Value + "'");
				}

				// Building DebugPath
				string DebugFile = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\AAA - DEBUG.txt";

				// Deletes File, Creates File, Adds to it
				HelperClasses.FileHandling.WriteStringToFileOverwrite(DebugFile, DebugMessage.ToArray());

				// Opens the File
				HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\System32\notepad.exe", pCommandLineArguments: DebugFile);
			}
		}


		/// <summary>
		/// Method which gets called when the exit Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Exit_Click(object sender, RoutedEventArgs e)
		{
			if (PageState == PageStates.GTA)
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you really want to quit?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					this.Close();
					Environment.Exit(0);
				}
			}
			else
			{
				PageState = PageStates.GTA;
			}
		}


		// Methods of the GTA Clicks are in GTA_Page



		// Hamburger Button Items Below:


		/// <summary>
		/// Method which gets called when the Update Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Upgrade_Click(object sender, RoutedEventArgs e)
		{
			// Confirmation Popup
			Popup conf = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you want to Upgrade?");
			conf.ShowDialog();
			if (conf.DialogResult == false)
			{
				return;
			}

			// Actual Upgrade Button Code
			HelperClasses.Logger.Log("Clicked the Upgrade Button");
			if (LauncherLogic.IsGTAVInstallationPathCorrect() && Globals.ZipVersion != 0)
			{
				if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
				{
					HelperClasses.Logger.Log("Gamestate looks OK (Downgraded). Will Proceed to try to Upgrade.", 1);
					LauncherLogic.Upgrade();
				}
				else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
				{
					HelperClasses.Logger.Log("This program THINKS you are already Upgraded. Update procedure will be called anyways since it shouldnt break things.", 1);
					LauncherLogic.Upgrade();
				}
				else
				{
					HelperClasses.Logger.Log("Installation State Broken.", 1);
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Installation State is broken. I suggest trying to repair.").ShowDialog();
				}
			}
			else
			{
				HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");

				string msg = "Error: GTA V Installation Path incorrect or ZIP Version == 0.\nGTAV Installation Path: '" + LauncherLogic.GTAVFilePath + "'\nInstallationState (probably): '" + LauncherLogic.InstallationState.ToString() + "'\nZip Version: " + Globals.ZipVersion + ".";

				if (Globals.BetaMode || Globals.InternalMode)
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, msg + "\n. Force this Upgrade?");
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						LauncherLogic.Upgrade();
					}
				}
				else
				{
					new Popup(Popup.PopupWindowTypes.PopupOkError, msg).ShowDialog();
				}
			}

			// Call Update GUI Method
			UpdateGUIDispatcherTimer();
		}

		/// <summary>
		/// Method which gets called when the Update Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Repair_Click(object sender, RoutedEventArgs e)
		{
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "This Method is supposed to be called when there was a Game Update.\nOr you verified Files through Steam.\nIs that the case?\nGTAV Installation ath is: '" + Settings.GTAVInstallationPath + "'");
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				if (LauncherLogic.IsGTAVInstallationPathCorrect() && Globals.ZipVersion != 0)
				{
					LauncherLogic.Repair();
				}
				else
				{

					HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");

					string msg = "Error: GTA V Installation Path incorrect or ZIP Version == 0.\nGTAV Installation Path: '" + LauncherLogic.GTAVFilePath + "'\nInstallationState (probably): '" + LauncherLogic.InstallationState.ToString() + "'\nZip Version: " + Globals.ZipVersion + ".";

					if (Globals.BetaMode || Globals.InternalMode)
					{
						Popup yesno2 = new Popup(Popup.PopupWindowTypes.PopupYesNo, msg + "\n. Force this Repair?");
						yesno2.ShowDialog();
						if (yesno2.DialogResult == true)
						{
							LauncherLogic.Repair();
						}
					}
					else
					{
						new Popup(Popup.PopupWindowTypes.PopupOkError, msg).ShowDialog();
					}
				}
			}

			UpdateGUIDispatcherTimer();
		}

		/// <summary>
		/// Method which gets called when the Downgrade Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Downgrade_Click(object sender, RoutedEventArgs e)
		{
			// Confirmation Popup
			Popup conf = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you want to Downgrade?");
			conf.ShowDialog();
			if (conf.DialogResult == false)
			{
				return;
			}

			// Actual Upgrade Button Code
			HelperClasses.Logger.Log("Clicked the Downgrade Button");
			if (LauncherLogic.IsGTAVInstallationPathCorrect() && Globals.ZipVersion != 0)
			{
				if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
				{
					HelperClasses.Logger.Log("Gamestate looks OK (Upgraded). Will Proceed to try to Downgrade.", 1);
					LauncherLogic.Downgrade();
				}
				else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
				{
					HelperClasses.Logger.Log("This program THINKS you are already Downgraded. Downgrade procedure will be called anyways since it shouldnt break things.", 1);
					LauncherLogic.Downgrade();
				}
				else
				{
					HelperClasses.Logger.Log("Installation State Broken.", 1);
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Installation State is broken. I suggest trying to repair.").ShowDialog();
				}
			}
			else
			{
				HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");

				string msg = "Error: GTA V Installation Path incorrect or ZIP Version == 0.\nGTAV Installation Path: '" + LauncherLogic.GTAVFilePath + "'\nInstallationState (probably): '" + LauncherLogic.InstallationState.ToString() + "'\nZip Version: " + Globals.ZipVersion + ".";

				if (Globals.BetaMode || Globals.InternalMode)
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, msg + "\n. Force this Downgrade?");
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						LauncherLogic.Downgrade();
					}
				}
				else
				{
					new Popup(Popup.PopupWindowTypes.PopupOkError, msg).ShowDialog();
				}
			}
			UpdateGUIDispatcherTimer();
		}

		/// <summary>
		/// Method which gets called when the "Import ZIP" Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_ImportZip_Click(object sender, RoutedEventArgs e)
		{
			string ZipFileLocation = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Import ZIP File", Globals.ProjectInstallationPath, pFilter: "ZIP Files|*.zip*");
			if (HelperClasses.FileHandling.doesFileExist(ZipFileLocation))
			{
				LauncherLogic.ImportZip(ZipFileLocation);
			}
			else
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "No ZIP File selected").ShowDialog();
			}
		}

		/// <summary>
		/// Method which gets called when the SaveFileHandler Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SaveFiles_Click(object sender, RoutedEventArgs e)
		{
			PageState = PageStates.SaveFileHandler;
		}

		/// <summary>
		/// Method which gets called when the Settings Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Settings_Click(object sender, RoutedEventArgs e)
		{
			PageState = PageStates.Settings;
		}

		/// <summary>
		/// Method which gets called when you click on the SpeedRun Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Speedrun_Click(object sender, RoutedEventArgs e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOk,
			"This Popup will contain Information about GTAV Speedrunning.\n" +
			"Paragraphs explaining the basics, rules, categories etc.\n" +
			"And some link to resources like the Leaderboard, Guides\n" +
			"Useful Programs, Maps, and whatever else is useful\n\n" +
			"I am not a speedrunner or very involved with the GTA V Community,\n" +
			"if you read this, and could shoot me a PM on Discord with stuff\n" +
			"you want to Read here, that would be great."
			).ShowDialog();
		}

		/// <summary>
		/// Method which gets called when the About Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_About_Click(object sender, RoutedEventArgs e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOk,
			"You are running Project 1.27, a tool for the GTA V Speedrunning Community.\n" +
			"This was created for the patch 1.27 downgrade problem, which started in August of 2020\n" +
			"This tool has a number of features, including Downgrading, Upgrading and launching the game,\n" +
			"\nSpecial shoutouts to @dr490n who was responsible for getting the downgraded game\n" +
			"to launch, added patches against in-game triggers, wrote the authentication backend,\n" +
			"decryption and got the preorder entitlement to work.\n\n" +
			"If you have any issues with this program or ideas for new features,\n" +
			"feel free to contact me on Discord: @thS#0305\n\n" +
			"Project 1.27 Version: '" + Globals.ProjectVersion + "', BuildInfo: '" + Globals.BuildInfo + "', ZIP Version: '" + Globals.ZipVersion + "'"
			).ShowDialog();
		}

		/// <summary>
		/// Method which gets called when the Readme Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Credits_Click(object sender, RoutedEventArgs e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOk,
			"Solving the patch 1.27 Downgrade problem has been achieved by a month of hard work by a\n" +
			"number of dedicated individuals. This would not have been possible without the time and\n" +
			"effort of a number of very talented individuals from all walks of life, who have\n" +
			"contributed skills in Reverse Engineering, Programming, Decryption, Project Management,\n" +
			"Scripting and Testing. Below is a list of some of the main contributors to the project,\n" +
			"although our thanks go out to EVERYONE who has helped throughout the process.\n\n" +
			"Reverse Engineering:\n" +
			"@dr490n, @Special For, @zCri\n\n" +
			"Launcher / Client Programming, Documentating:\n" +
			"@thS\n\n" +
			"Launcher GUI Design & Artwork:\n" +
			"@hossel\n\n" +
			"Special thanks to:\n" +
			"@JakeMiester, @Antibones, @Aperture, @Diamondo25, @MOMO\n\n" +
			"Shoutout to FiveM and Goldberg, whose Source Code proved to be vital\n" +
			"to understand and reverse engineer the GTA V Launch Process"
			).ShowDialog();
		}


		#endregion



	} // End of Class
} // End of Namespace
