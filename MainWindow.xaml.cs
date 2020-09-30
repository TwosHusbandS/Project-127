/*

Main Documentation:
Actual code (partially closed source) which authentificates, handles entitlement and launches the game is done by @dr490n with the help of other members of the core team like @Special For and @zCri
Artwork, Design of GUI, GUI Behaviourehaviour, Colorchoices etc. by "@Hossel"
Client by "@thS"
Version: 0.0.3.0 Closed Beta

Build Instructions:
	Press CTRLF + F5, pray that nuget does its magic.

Deploy Instructions:
	Change Version Number a few Lines Above.
	Change Version Numbner in both of the last lines in AssemblyInfo.cs
	Check if BetaMode in Globals.cs is correct
	Check if BuildInfo in Globals.cs is correct
	Make sure app manifest is set to NOT require admin
	Build this program in release
	Build installer via Innosetup (Script is in \Installer\) [Change Version in Version and OutputName]
	Change Version number and Installer Location in "\Installer\Update.xml"
	Push Commit to github branch.
	Merge branch into master

Getting People into Beta:
	Add their String inside the Regedit Value "MachineGuid" inside the RegeditKey: "Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography"
	to /Installer/AuthUser.txt (on github master branch btw)

Comments like "TODO", "TO DO", "CTRLF", "CTRL-F", and "CTRL F" are just ways of finding a specific line quickly via searching

Hybrid code can be found in AAA_HybridCode.

General Files / Classes:
	Windows:
		MainWindow.xaml.cs
		Settings.xaml.cs
			SettingsPartial.cs
		SaveFileModder.xaml.cs
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

	#####################################################################################################
	##### This needs some some cleanup. Just pushing now so we can get closed beta Auth testing #########
	#####################################################################################################

Main To do:
	- Things changed since last official release (not last commit)
		-> Pretty OpenFolderDialog
		-> ZIP File gets downloaded AFTER checking if user is allowed to run...
		-> Full Cleanup code (auto document everything and also write a few lines in important locations)
		-> Set popup to "No ZIP Installed" if version is 0
		-> Automatically detect Retailer based on Path guess
		-> auto high priority
		-> BugFix when deleting Folder
		-> Fix CommandLineArgs

	-REMEMBER:
		-> Release with admin mode manifest thingy...		
		-> Fix Installer with everything (autolaunch app,include new files)
					
	- TO DO:
		-> Think about making a spawner to spawn processes
		   (Process.Start(@"C:\Users\ingow\source\repos\ProcessSpawner127\bin\x64\Release\ProcessSpawner127.exe", "testA testB");)
		-> Figure out which files I need to distribute
		-> Open Twice shit may be broken...gotta investigate. Works for me.
		-> rollback to old Admin Launch Methods. This should fix Investige zip file unzipping crashing for some.

		// Release

		-> Language Select
		-> auto steam core fix
		-> Custom ZIP File Location User Error Checks:
			=> User might get confused with the Project_127_Files Folder. 
				Maybe we should actually check parent folders and child folders when User is selecting a Path for ZIP File
		-> Regedit Value "LastLaunchedVersion" is there and be used with the next Version.
		-> SaveFileHandler, just manage our own SaveFiles, probably only need one list for datagrid, ask if we need to overwrite
		-> Regedit Cleanup of everything not in default settings
		-> $UpgradeFiles has downgrade files in them. Why? And how to Fix?
			=> Cant figure out how to fix that at the moment
		-> Settings dont update content
			=> Currently it calls the Refresh Method after each click...which works but is ugly
			=> Get data binding to work after everything else is Gucci 
		-> Implement all Other features
			=> Just see Settings.XAML for what I need to implement


    - Low Prio:
		Convert Settings and SaveFileHandler in CustomControls
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

		/// <summary>
		/// Constructor of Main Window
		/// </summary>
		public MainWindow()
		{
			// Initializing all WPF Elements
			InitializeComponent();

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
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Program is open twice. Do you want to force close the old Instance?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.ProcessHandler.KillProcessContains("Project 1.27");
				}
				else
				{
					Environment.Exit(2);
				}
			}

			// Start the Init Process of Logger, Settings, Globals, Regedit here, since we need the Logger in the next Line if it fails...
			Globals.Init(this);

			// Checks if you are allowed to run this Beta
			if (!CheckIfAllowedToRun())
			{
				HelperClasses.Logger.Log("You are not allowed to run this Beta.");
				new Popup(Popup.PopupWindowTypes.PopupOkError, "You are not allowed to run this Beta.").ShowDialog();
				Environment.Exit(3);
			}

			// Deleting all Installer and ZIP Files from own Project Installation Path
			DeleteOldFiles();

			// Make sure Hamburger Menu is invisible when opening window
			this.GridHamburgerOuter.Visibility = Visibility.Hidden;

			// Set Image of Buttons
			SetButtonMouseOverMagic(btn_Exit, false);
			SetButtonMouseOverMagic(btn_Auth, false);
			SetButtonMouseOverMagic(btn_Hamburger, false);

			// Auto Updater
			CheckForUpdate();

			// Check whats the latest Version of the ZIP File in GITHUB
			CheckForZipUpdate();

			HelperClasses.Logger.Log("Startup procedure (Constructor of MainWindow) completed.");
			HelperClasses.Logger.Log("--------------------------------------------------------");
		}


		/// <summary>
		/// Responsible for Re-Launching this App as Admin if it isnt.
		/// </summary>
		private void AdminRelauncher()
		{
			// If not run as Admin
			if (!IsRunAsAdmin())
			{
				// Prepare new ProcessStartInfo with all Arguments and stuff
				ProcessStartInfo proc = new ProcessStartInfo();
				proc.UseShellExecute = true;
				proc.WorkingDirectory = Environment.CurrentDirectory;
				proc.FileName = Assembly.GetEntryAssembly().CodeBase;
				proc.Arguments = Globals.CommandLineArguments.ToString();

				// Make sure its admin
				proc.Verb = "runas";

				try
				{
					Process.Start(proc);
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
		/// Exits when you are not supposed to run this. Its ugly. It works. Meh.
		/// </summary>
		private bool CheckIfAllowedToRun()
		{
			try
			{
				// If we are in BetaMode
				if (Globals.BetaMode)
				{
					// If we skip the login
					foreach (string argument in Environment.GetCommandLineArgs())
					{
						if (argument.ToLower().Contains("skiplogin"))
						{
							return true;
						}
					}

					// Getting own GUID from Server
					RegistryKey MyKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("Microsoft").CreateSubKey("Cryptography");
					string GUID = HelperClasses.RegeditHandler.GetValue(MyKey, "MachineGuid");

					// Getting the URL of the File which has all the AuthInfo in it
					string URL_AuthUser = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "authuser");

					// Downloading the File into string
					string AuthUserOnServer = HelperClasses.FileHandling.GetStringFromURL(URL_AuthUser);

					// Letting Users in if github is down...
					if (String.IsNullOrEmpty(AuthUserOnServer))
					{
						new Popup(Popup.PopupWindowTypes.PopupOk, "Letting you in since Github appears to be down...").ShowDialog();
						return true;
					}

					// If own GUID is in the GUIDs on the server
					if (AuthUserOnServer.Contains(GUID))
					{
						return true;
					}
					else
					{
						return false;
					}
				}

				// If we are not in Betamode
				return true;
			}
			catch
			{
				// If something shit the bed somewhere
				return true;
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
					HelperClasses.Logger.Log("Update found,1");
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

						new PopupDownload(PopupDownloadTypes.Installer, DLPath, LocalFileName).ShowDialog();
						HelperClasses.ProcessHandler.StartProcess(LocalFileName, "", true, false);
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
					string pathOfNewZip = HelperClasses.FileHandling.GetXMLTagContent(HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AutoUpdate), "zip");

					// Downloads ZIP, calls extraction Method after download
					new PopupDownload(PopupDownloadTypes.ZIP, pathOfNewZip, Globals.ZipFileDownloadLocation).ShowDialog();
					LauncherLogic.ImportZip(Globals.ZipFileDownloadLocation, true);
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
			else
			{
				lbl_GTA.Foreground = Globals.MW_GTALabelUpgradedForeground;
				lbl_GTA.Content = "Upgraded";
			}
			if (LauncherLogic.GameState == LauncherLogic.GameStates.Running)
			{
				btn_GTA.BorderBrush = Globals.MW_ButtonGTAGameRunningBorderBrush;
				btn_GTA.Content = "Exit GTA V";
			}
			else
			{
				btn_GTA.BorderBrush = Globals.MW_ButtonGTAGameNotRunningBorderBrush;
				btn_GTA.Content = "Launch GTA V";
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
			}
			// If is not visible
			else
			{
				// Make visible
				this.GridHamburgerOuter.Visibility = Visibility.Visible;
			}
		}

		/// <summary>
		/// Rightclick on Hamburger Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Hamburger_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Globals.BetaMode)
			{
				// Opens the File
				Process.Start("notepad.exe", Globals.Logfile);
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
				ROSIntegration myROSIntegration = new ROSIntegration();
				myROSIntegration.ShowDialog();
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
			if (Globals.BetaMode)
			{
				// Debug Info users can give me easily...
				List<string> DebugMessage = new List<string>();

				DebugMessage.Add("Project 1.27 Version: '" + Globals.ProjectVersion + "'");
				DebugMessage.Add("ZIP Version: '" + Globals.ZipVersion + "'");
				DebugMessage.Add("Project 1.27 Installation Path '" + Globals.ProjectInstallationPath + "'");
				DebugMessage.Add("ZIP Extraction Path '" + LauncherLogic.ZIPFilePath + "'");
				DebugMessage.Add("LauncherLogic.GTAVFilePath: '" + LauncherLogic.GTAVFilePath + "'");
				DebugMessage.Add("LauncherLogic.UpgradeFilePath: '" + LauncherLogic.UpgradeFilePath + "'");
				DebugMessage.Add("LauncherLogic.DowngradeFilePath: '" + LauncherLogic.DowngradeFilePath + "'");
				DebugMessage.Add("LauncherLogic.SupportFilePath: '" + LauncherLogic.SupportFilePath + "'");
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
				Process.Start("notepad.exe", DebugFile);
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
				this.Close();
				Environment.Exit(0);
			}
		}


		/// <summary>
		/// Method which gets called when the Launch GTA Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_Click(object sender, RoutedEventArgs e)
		{
			if (LauncherLogic.GameState == LauncherLogic.GameStates.Running)
			{
				HelperClasses.Logger.Log("Game deteced running.", 1);
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Game is detected as running.\nDo you want to close it\nand run it again?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.Logger.Log("Killing all Processes.", 1);
					LauncherLogic.KillRelevantProcesses();
				}
				else
				{
					HelperClasses.Logger.Log("Not wanting to kill all Processes, im aborting Launch function", 1);
					return;
				}
			}
			else
			{
				string msg = "GTA V Installation is (probably) " + LauncherLogic.InstallationState.ToString() + ". The Game is (probably) " + LauncherLogic.GameState.ToString() + ".";

				Popup conf = new Popup(Popup.PopupWindowTypes.PopupYesNo, msg + "\nDo you want to Launch the Game?");
				conf.ShowDialog();
				if (conf.DialogResult == true)
				{
					HelperClasses.Logger.Log("User wantst to Launch", 1);
					LauncherLogic.Launch();
				}
			}
			UpdateGUIDispatcherTimer();
		}


		/// <summary>
		/// Method which gets called when the Launch GTA Button is RIGHT - clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you want to close GTAV?");
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				LauncherLogic.KillRelevantProcesses();
			}
			UpdateGUIDispatcherTimer();
		}


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
				else
				{
					HelperClasses.Logger.Log("This program THINKS you are already Upgraded. Update procedure will not be called.", 1);
					if (Globals.BetaMode)
					{
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "We are in Beta Mode. Do you want to FORCE the Upgrade?\nThe program thinks youre already Upgraded. Things might go wrong if you force this.");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
						{
							HelperClasses.Logger.Log("Gamestate looks SHIT (Upgraded). Forcing Upgrade since we are in Beta Mode.", 1);
							LauncherLogic.Upgrade();
						}
						else
						{
							HelperClasses.Logger.Log("Gamestate looks SHIT (Upgraded). Upgrade was not forced.", 1);
						}
					}
				}
			}
			else
			{
				HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Error: GTA V Installation Path incorrect or ZIP Version == 0.\nGTAV Installation Path: '" + LauncherLogic.GTAVFilePath + "'\nInstallationState (probably): '" + LauncherLogic.InstallationState.ToString() + "'\n. Force this Upgrade?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					LauncherLogic.Downgrade();
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
			Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "This Method is supposed to be called when there was a Game Update.\nOr you verified Files through Steam.\nIs that the case?\nGTAV Installation Path is: '" + Settings.GTAVInstallationPath + "'");
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
					Popup conf = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Error: GTA V Installation Path incorrect or ZIP Version == 0.\nGTAV Installation Path: '" + LauncherLogic.GTAVFilePath + "'\nForce this Repair?");
					conf.ShowDialog();
					if (conf.DialogResult == true)
					{
						LauncherLogic.Repair();
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

			// Actual Code behind Downgrade Button
			HelperClasses.Logger.Log("Clicked the Downgrade Button");
			if (LauncherLogic.IsGTAVInstallationPathCorrect() && Globals.ZipVersion != 0)
			{
				if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
				{
					HelperClasses.Logger.Log("Gamestate looks OK (Upgraded). Will Proceed to try to Downgrade.", 1);
					LauncherLogic.Downgrade();
				}
				else
				{
					HelperClasses.Logger.Log("This program THINKS you are already Downgraded. Upgrade procedure will not be called.", 1);
					if (Globals.BetaMode)
					{
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "We are in Beta Mode. Do you want to FORCE the Downgrade?\nThe program thinks youre already Downgraded. Things might go wrong if you force this.");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
						{
							HelperClasses.Logger.Log("Gamestate looks SHIT (Downgraded). Forcing Downgrade since we are in Beta Mode.", 1);
							LauncherLogic.Downgrade();
						}
						else
						{
							HelperClasses.Logger.Log("Gamestate looks SHIT (Downgraded). Downgrade was not forced.", 1);
						}
					}
				}
			}
			else
			{
				HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Error:\nGTA V Installation Path incorrect or ZIP Version == 0.\nGTAV Installation Path: '" + LauncherLogic.GTAVFilePath + "'\nInstallationState (probably): '" + LauncherLogic.InstallationState.ToString() + "'\n. Force this Downgrade?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					LauncherLogic.Downgrade();
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
			string ZipFileLocation = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Import ZIP File", Globals.ProjectInstallationPath, "ZIP Files(*.zip *) | *.zip * ");
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
			new Popup(Popup.PopupWindowTypes.PopupOk, "SaveFileHanlder not fully implemented yet").ShowDialog();
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
		/// Method which gets called when the Readme Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Readme_Click(object sender, RoutedEventArgs e)
		{
			new Popup(Popup.PopupWindowTypes.PopupOk,
			"You are running Project 1.27, a tool for the GTAV Speedrunning Community.\n" +
			"This was created for the Patch 1.27 Downgrade Problem which started in August of 2020\n" +
			"This Tool has a few Features (apart from Downgrading, Upgrading, and Launching the Game)\n" +
			"and I suggest you checkout the Github Link of this Project to have an overview of all Features\n" +
			"as well as as detailed Instructions on how to use this Program, Access to the latest Installer,\n" +
			"Uninstaller, Patchnotes, ReadMe etc.\n\n" +
			"Special Shoutouts to @dr490n who was responsible for getting the Downgraded Game\n" +
			"to Launch, added Patches against In Game Triggers, wrote the Authentification Backend,\n" +
			"Decryption and got the PreOrder Entitlement to work. Thanks Mate.\n\n" +
			"Also: whoever reads this. I hope you have a great day.\n\n" +
			"Project 1.27 Version: '" + Globals.ProjectVersion + "', BuildInfo: '" + Globals.BuildInfo + "', ZIP Version: '" + Globals.ZipVersion + "'\n\n" +
			"If you have any troubles with this Program or Ideas for new Features or anything,\n" +
			"feel free to Contact me on Discord: @thS#0305"
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
			"Solving the Patch 1.27 Downgrade Problem has been achieved by a month of hard work\n" +
			"by a number of dedicated individuals. This would not have been possible without the\n" +
			"hard work of a number of very talented individuals from all walks of life,\n" +
			"who have contributed skills in Reverse Engineering, Programming,\n" +
			"Decryption, Project Management, Scripting and Testing.\n" +
			"Below is a list of some of the main contributors to the project,\n" +
			"although our thanks go out to everyone who has helped throughout the process.\n\n" +
			"@dr490n, @Special For, @thS, @zCri\n" +
			"@hossel, @JakeMiester, @MOMO, @Daniel Kinau\n" +
			"@Antibones, @Aperture, @Diamondo25, @wojtekpolska\n\n" +
			"Shoutouts to FiveM and Goldberg, whose Source Code proved to be vital\n" +
			"to understand and reverse engineer the GTAV Launch Process\n\n" +
			"Special Shoutouts to @dr490n who was responsible for getting the Downgraded Game\n" +
			"to Launch, added Patches against In Game Triggers, wrote the Authentification Backend,\n" +
			"Decryption and got the PreOrder Entitlement to work. Thanks Mate."
			).ShowDialog();
		}


		#endregion



	} // End of Class
} // End of Namespace
