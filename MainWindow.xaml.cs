/*
 
Main Documentation / Dev Diary here:

Actual code (partially closed source) which authentificates, handles entitlement and launches the game (and Overlay Backend) is done by @dr490n
@Special For, who also did a lot of RE'ing, testing, brainstorming, information gathering and 2nd level support
@JakeMiester did some project management, acted as the communication party between the team and the speedrun mods, "invented" trello
@zCri did a research, RE and coding in the initial information gathering stage
Artwork, Design of GUI, GUI Behaviourehaviour, Colorchoices etc. by "@Hossel"
Project 1.27 Client by "@thS"
A number of other members of the team, including but not limited to @MoMo, @Diamondo25, @S.M.G, @gogsi, @Antibones, @Unemployed, @Aperture, @luky, @CrynesSs, @Daniel Kinau contributed to this project one way or another, and my thanks go out to them.
Version: 1.0.9.4

Build Instructions:
	Press CTRLF + F5, pray that nuget does its magic.
	If this doesnt work, required DLLs and files can be gotten by running the latest installer

Deploy Instructions:
	Change Version Number a few Lines Above.
	Change Version Number in both of the last lines in AssemblyInfo.cs
	Check if BetaMode / InternalMode in Globals.cs is correct
	Check if BuildInfo in Globals.cs is correct
	Make sure app manifest is set TO require admin
	Build this program in release
		Verify this by going to Information -> About and check for Version Number and Build Info
	Build installer via Innosetup (script we use for that is in \Installer\)
		Change Version in Version
		Change Version in OutputName
		Change BuildPath to your local Path of the repo...(make sure it builds in the \Installer Folder
	Delete Project_127_Installer_Latest.exe. Copy Paste the Installer we just compiled and name the copy of it Project_127_Installer_Latest.exe
	Change Version number and Installer Path in "\Installer\Update.xml"
	Push Commit to github branch.
	Merge branch into master

Comments like "TODO", "TO DO", "CTRLF", "CTRL-F", and "CTRL F" are just ways of finding a specific line quickly via searching

Hybrid code can be found in AAA_HybridCode.

	- Changelog past 1.0.0.0 Build 3:
		=> Changed Polling Rate of GTA V Running or Not from 5 to 2.5 seconds
		=> Gave Reloe the new Installer Link
		=> Fixed Spelling Mistakes
		=> Throwing only one nice Network error now, instead of all of them with exception string
		=> Removed the language selection from firstlaunch and Reset Settings
		=> Improved Popup Startup Location Code to make it look nicer
		=> Moved Code around
		=> Improved GTA V Path detection Logging
		=> Added Import GTA V Settings Button back
		=> Moved SourceCodeFiles around to make it easier to find stuff
		=> Fixed Not launching after pressing Launch when non-auth
		=> Dragon Implemented the GameOverlay (GTAOverlay)
		=> KeyboardListener (conntected to Backend)
		=> WindowChangeForegroundEventListener (connected to Backend)
		=> "Look for Updates" Button
		=> Made it generate debugfile and open explorer window of project 1.27 on rightclick of auth button
		=> Fixed Auth Mouse Over
		=> Fixed Consistent Margins and BorderThicknesses for Pages
		=> Made sure Process Priority is set correctly
		=> Implemented GTA Overlay debugmode which means the rest of that stuff is properly connected to backend
		=> Hotkeys only work when Overlay is visible
		=> Made sure GTA Overlay will turn off when not ingame
		=> Running Core stuff when launching through steam. This might not have any effect
		=> Design of UX for NoteOverlay.xaml
		=> Updated Settings with JumpScript and NoteOverlay stuff
		=> Rolling Log (fixed potential off by one very elegantly)
		=> ToolTips on all Icon-Buttons
		=> Annoucement Feature
		=> FailSafe Backup System for Upgrade Files
		=> Auth will no longer crash when not reachable. Well at least we check it on page load...
		=> Overlay for notes and UI for it
		=> Jumpscript
		=> Auto-Start XYZ on Game Launch working dir fix
		=> Downgrade/Upgrade/Repair improvements:
			- Detecting Updates automatically (checking for it on start, upgrade, downgrade), throwing one popup per P127 Launch
			- Throwing Popup with potential Fixes for non-changing InstallationState (upgraded, downgraded, unsure)
			- Not having own files in GTA V Folder when upgraded
		=> FIXED command line args internal once and for all
		=> Implemented Jumpscript via Autohotkey
		=> Integrate Title from Dragon both as in content and as in customizability
		=> Fixed Both Listeners for the hopefully final time
		=> Fixed Update Detection
		=> Commented out new SaveFileHandler Code
		=> Fixed Process Priority setting too often
		=> Added GameLaunched and GameExited methods based on the polling
		=> Fixed ForegroundChangeListener not setting on fullscreen by polling every 2.5 seconds
		=> Fix Command line args crashing it...
		=> Make "DetectUpgrade" more efficent
		=> Integrate Latest working branch. 
		=> Integrate Dragons Fixes for Rockstar Endpoint change
		=> Took care of all Listeners. Using and keeping track of Threads for it as of right now. Seems to work
		=> Split upgrading downgrading into 17 progress popups
		=> Finish Readme (Speedrun text + Reset Button + DL of big zip)
		=> ZIP Hash for big ZIP
		=> Webscraping for DDLs from anonfiles
		=> ALL Styles moved to App.xml
		=> Bring back functionality from which were forgetten in new GUI
		=> Selection after deletion (datagrid) fixxed
		=> Yoshis Info regarding Versions
		=> NoteOverlay Null Reference Fix + CPU Improvements
		=> Cache works, there are other cache files tho...argh. ~~Investigate CEF Cache~~
		=> Added Logging for AutoStart stuff
		=> Added Force Option to Downgrade / Upgrade when GTA V Path is detected to be false on Upgrade / Downgrade
		=> Removed Delay on Downgrade / Upgrade, throwing 3 separate ProgressBar popups for it.
		=> Using Portable AHK now with script written to desk
		=> Released under MIT
		=> Improved UX overall. Lots of small things.

	Release 1.1

		Quick and Dirty notes:
			- SFH Below
			- Clean up Code / Readme / Patchnotes
			- Release new ZIP
			- Make it create Folder and Savefile for new release...for SFH Demo
		
		- Fullscreen mode for overlay
			=> With added front-end and mid-end support for that
				--> add GUI Options
				--> Making a WPF Window for that.
					=> ^ Make WPF Window Resizable, indicate in GUI that margin and location have no effect
					=> ^ Make WPF Window Show up / not show up via hotkeys...make it chagne width and height on look change
					=> ^ Make WPF Window show up on look subpage of noteoverlay (same when preview appears)
					=> ^ Making Hotkeys toggle the Window Visibility instead Overlay itself
					=> ^ Same with the code that makes overlay invisible on game minimize
		- SFH Improvements
			=> [DONE] Add Folder Support
				>> ReWrite of SaveFileHandler class with enum for File or Folder
				>> Folders as clickable items in list at the very top with a "[FolderName]
				>> Top Folder being "[..]" like in WinRar
				>> Connect to Backend in terms of rename, move around, etc.
			=> [DONE] Rename left file doesnt update text in brackets on right side...
			=> [DONE] MouseOver displays full fillename
			=> [DONE] Make selected File act like NoteOverlay_NoteFiles
			=> Search Bar
			=> Better Hotkey support
			=> Add Support for Copy & Move (in Ram) and Paste.
			=> Multiselect
			=> RightClick on File (Copy, Rename, Delete)
			=> RightClick on Files (Copy, Delete, Delete)
			=> RightClick on Background (new Folder, Paste)
			=> RightClick on Folder
			=> Horizontal Scroll bar
			
		
		- Update ReadMe, to reflect that its not being actively developed and read through it in general. 
			=> Add credits to new people (hosting, version info, legends of Community)
		- NameSpace clean up...
		- Code clean up
		- Code documentation
		- Comments clean up
		- Add Logging

Bug Reportings:
	- [RESOLVED][IDC]
			GTA5.exe being "stuck" in taskmanager...hossels virus shit
	- [RESOLVED][IDC][Its fine for now, no idea how to improve]
			Open Twice message (and killing old process) not working for one guy
			Works for me and on some other testers machines.
			Confirmed funky. Also can not kill Process spawned by VS
			Works for me...even in VS...
	- [RESOLVED] Installer Link wrong (gave Reloe new one, it got changed)
	- [RESOLVED] Changed a lot of that, should be all good now
			Reloe and JakeMiester and dr490n had some issues with the GTA V Path Settings
			Changed a lot of the backend for that. Should all be fixed.
	- [RESOLVED][FIXED] Game wasnt launching for most
			I actually hardcoded the Path...its now fixed.
	- [RESOLVED][FIXED] People had to run as Admin manually even tho I have the admin relauncher.
	- [RESOLVED][FIXED] ZIP Extraction failed for 2 people when not manually running as admin
			Added more and better logging, issue can not be reproduced as of right now
	- [RESOLVED][FIXED] Auth Window popup crashed for Hossel in some circumstances. (CredentialManager dll missing)
	- [RESOLVED] Dragon Fixed that on his end, Investigate oneDrive shit (turned out to be crypto char in document path [or path variable??])
	- [RESOLVED] Just this one machine being dumb...Dragons NAS machine was just doing weird stuff with importing zip.
		-> Simple stuff like (If Path doesnt exist, create it) crashed for no reason
		-> Added logging to extract zip didnt work and failed in weird places
		-> Complained about STATHread even its there
		-> "Old" no progressbar zip extracting also failed
		-> Probably due to NAS
	- [RESOLVED] Investigate Specials worry about user getting "Already auth" even when session expired.
		=> Took a quick look at the code, this should not happen, but special said it happened before...
															also Software doing Software things...
		=> Bottom Line: INVESTIGATE
		=> Confirmed NON RELEVANT by brainstorm from dragon and ths

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
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using CefSharp;

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
					foreach (Process p in HelperClasses.ProcessHandler.GetProcessesContains(Process.GetCurrentProcess().ProcessName))
					{
						if (p != Process.GetCurrentProcess())
						{
							HelperClasses.ProcessHandler.Kill(p);
						}
					}
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

			// Some Background Change based on Date
			ChangeBackgroundBasedOnSeason();

			if (Globals.InternalMode)
			{
				string msg = "We are in internal mode. I need testing on:\n\n" +
					"- Upgrading / Downgrading / Repairing" + "\n" +
					"- Automatically detecting Upgrades" + "\n" +
					"- Performance (CPU & Ram)" + "\n" +
					"- Crashes" + "\n" +
					"- NoteOverlay" + "\n" +
					"- Jumpscript" + "\n" +
					"- Bugfixes in general" + "\n" +
					"\nThanks. Appreciated. Have a great day : )";

				new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
			}

			// GUI SHIT
			SetButtonMouseOverMagic(btn_Exit);
			SetButtonMouseOverMagic(btn_Auth);
			SetButtonMouseOverMagic(btn_Hamburger);
			Globals.HamburgerMenuState = Globals.HamburgerMenuStates.Hidden;


			HelperClasses.Logger.Log("Only CEF Init to go...");

			// Moved CEFInitialize(); to Globals.Init() since its not GUI Related or Execution Related
			CEFInitialize();

			HelperClasses.Logger.Log("Startup procedure (Constructor of MainWindow) completed.");
			HelperClasses.Logger.Log("--------------------------------------------------------");

#if DEBUG
   GTAOverlay.DebugMode = true;
#endif


			// Testing Purpose for the overlay shit
			if (GTAOverlay.DebugMode)
			{
				//We currently need this here, normally this will be started by GameState(but this points to GTA V.exe as of right now)
				NoteOverlay.InitGTAOverlay();

				// Same as other two thingies here lolerino
				HelperClasses.WindowChangeListener.Start();
			}
		}


		/// <summary>
		/// Initialzes CEF settings
		/// </summary>
		public static void CEFInitialize()
		{
			HelperClasses.Logger.Log("Initializing CEF...");
			var s = new CefSharp.Wpf.CefSettings();
			s.CachePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\CEF_CacheFiles";
			s.BackgroundColor = 0;//0x13 << 16 | 0x15 << 8 | 0x18;
			s.DisableGpuAcceleration();
			s.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";
#if DEBUG
			s.RemoteDebuggingPort = 8088;
#endif
			Cef.Initialize(s);
		}


		/// <summary>
		/// Changing Background based on current Date
		/// </summary>
		private void ChangeBackgroundBasedOnSeason()
		{
			DateTime Now = DateTime.Now;

			if (Now.Month == 4 && Now.Day == 20)
			{
				Globals.BackgroundImage = Globals.BackgroundImages.FourTwenty;
			}
			else if ((Now.Month == 10 && Now.Day >= 29) ||
					(Now.Month == 11 && Now.Day == 1))
			{
				Globals.BackgroundImage = Globals.BackgroundImages.Spooky;
			}
			else if ((Now.Month == 12 && Now.Day >= 6) ||
					(Now.Month == 1 && Now.Day <= 6))
			{
				Globals.BackgroundImage = Globals.BackgroundImages.XMas;
			}
			else
			{
				Globals.BackgroundImage = Globals.BackgroundImages.Main;
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
				catch (Exception)
				{
					System.Windows.Forms.MessageBox.Show("This program must be run as an administrator!");
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
				lbl_GTA.Foreground = Globals.MW_GTALabelUnsureForeground;
				lbl_GTA.Content = "Unsure";
			}

			lbl_GTA.Content += " (" + Globals.GetGameVersionOfBuildNumber(Globals.GameVersion) + ")";

			if (LauncherLogic.PollGameState() == LauncherLogic.GameStates.Running)
			{
				GTA_Page.btn_GTA_static.BorderBrush = Globals.MW_ButtonGTAGameRunningBorderBrush;
				GTA_Page.btn_GTA_static.Content = "Exit GTA V";
			}
			else
			{
				GTA_Page.btn_GTA_static.BorderBrush = Globals.MW_ButtonGTAGameNotRunningBorderBrush;
				GTA_Page.btn_GTA_static.Content = "Launch GTA V";
			}
			SetButtonMouseOverMagic(btn_Auth);
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
		/// WPF Magic to stop the Frame from doing dumb shit to its pages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Frame_Main_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.Forward)
			{
				e.Cancel = true;
			}
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
		/// <param name="myCtrl"></param>
		/// <param name="pArtpath"></param>
		public void SetControlBackground(Control myCtrl, string pArtpath)
		{
			try
			{
				Uri resourceUri = new Uri(pArtpath, UriKind.Relative);
				StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
				BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
				var brush = new ImageBrush();
				brush.ImageSource = temp;
				myCtrl.Background = brush;
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
		public void SetButtonMouseOverMagic(Button myBtn)
		{
			switch (myBtn.Name)
			{
				case "btn_Hamburger":
					if (myBtn.IsMouseOver)
					{
						SetControlBackground(myBtn, @"Artwork\hamburger_mo.png");
					}
					else
					{
						SetControlBackground(myBtn, @"Artwork\hamburger.png");
					}
					break;
				case "btn_Exit":
					if (myBtn.IsMouseOver)
					{
						SetControlBackground(myBtn, @"Artwork\exit_mo.png");
					}
					else
					{
						SetControlBackground(myBtn, @"Artwork\exit.png");
					}
					break;
				case "btn_Auth":
					string BaseArtworkPath = "";

					if (LauncherLogic.AuthState == LauncherLogic.AuthStates.Auth)
					{
						BaseArtworkPath = @"Artwork\lock_closed";
					}
					else if (LauncherLogic.AuthState == LauncherLogic.AuthStates.NotAuth)
					{
						BaseArtworkPath = @"Artwork\lock_open";
					}

					if (Globals.PageState == Globals.PageStates.Auth)
					{
						// Reverse Mouse Over Effect

						if (myBtn.IsMouseOver)
						{
							SetControlBackground(myBtn, BaseArtworkPath + ".png");
						}
						else
						{
							SetControlBackground(myBtn, BaseArtworkPath + "_mo.png");
						}
					}
					else if (Globals.PageState != Globals.PageStates.Auth)
					{
						// Normal Mouse Over Effect

						if (myBtn.IsMouseOver)
						{
							SetControlBackground(myBtn, BaseArtworkPath + "_mo.png");
						}
						else
						{
							SetControlBackground(myBtn, BaseArtworkPath + ".png");
						}
					}

					break;
			}
		}

		/// <summary>
		/// MouseEnter event for updating background image of buttons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			SetButtonMouseOverMagic((Button)sender);
		}

		/// <summary>
		/// MouseLeave event for updating background image of buttons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			SetButtonMouseOverMagic((Button)sender);
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
			if (Globals.HamburgerMenuState == Globals.HamburgerMenuStates.Visible)
			{
				Globals.HamburgerMenuState = Globals.HamburgerMenuStates.Hidden;
			}
			// If is not visible
			else
			{
				Globals.HamburgerMenuState = Globals.HamburgerMenuStates.Visible;
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
				//HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\System32\notepad.exe", pCommandLineArguments: Globals.Logfile);
			}
			
			new Overlay_MultipleMonitor().Show();
		}



		/// <summary>
		/// Method which gets called when the Auth Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Auth_Click(object sender, RoutedEventArgs e)
		{
			if (Globals.PageState != Globals.PageStates.Auth)
			{
				if (LauncherLogic.AuthState == LauncherLogic.AuthStates.NotAuth)
				{
					Globals.PageState = Globals.PageStates.Auth;
				}
				else
				{
					new Popup(Popup.PopupWindowTypes.PopupOk, "You are already authenticated.").ShowDialog();
				}
			}
			else
			{
				Globals.PageState = Globals.PageStates.GTA;
			}

			// Yes this is correct
			SetButtonMouseOverMagic(btn_Auth);
		}


		/// <summary>
		/// Right click on Auth button. Gives proper Debug Output
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Auth_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
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

			HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: Globals.ProjectInstallationPath);
		}


		/// <summary>
		/// Method which gets called when the exit Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Exit_Click(object sender, RoutedEventArgs e)
		{
			if (Globals.PageState == Globals.PageStates.GTA)
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
				Globals.PageState = Globals.PageStates.GTA;
			}
		}

		/// <summary>
		/// Right Mouse Button Down on Exit forces Close instantly
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Exit_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.Close();
			Environment.Exit(0);
			//Globals.DebugPopup(Globals.CommandLineArgs.ToString());
			//Globals.DebugPopup(Globals.InternalMode.ToString());
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
				}
				else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
				{
					HelperClasses.Logger.Log("This program THINKS you are already Upgraded. Update procedure will be called anyways since it shouldnt break things.", 1);
				}
				else
				{
					HelperClasses.Logger.Log("Installation State Broken.", 1);
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Installation State is broken. I suggest trying to repair.\nWill try to Upgrade anyways.").ShowDialog();
				}

				if (LauncherLogic.IsGTAVInstallationPathCorrect(false))
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "GTA Installation Path detected to be wrong.\nForce this Upgrade?");
					yesno.ShowDialog();
					if (yesno.DialogResult != true)
					{
						HelperClasses.Logger.Log("Will abort Upgrade, since GTA V Installation Path is wrong, and user does not want to force the Upgrade");
						return;
					}
				}
				LauncherLogic.Upgrade();
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
				}
				else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
				{
					HelperClasses.Logger.Log("This program THINKS you are already Downgraded. Downgrade procedure will be called anyways since it shouldnt break things.", 1);
				}
				else
				{
					HelperClasses.Logger.Log("Installation State Broken. Downgrade procedure will be called anyways since it shouldnt break things.", 1);
					new Popup(Popup.PopupWindowTypes.PopupOk, "Installation State is broken. I suggest trying to repair.\nWill try to Downgrade anyways").ShowDialog();
				}

				if (LauncherLogic.IsGTAVInstallationPathCorrect(false))
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "GTA Installation Path detected to be wrong.\nForce this Downgrade?");
					yesno.ShowDialog();
					if (yesno.DialogResult != true)
					{
						HelperClasses.Logger.Log("Will abort Downgrade, since GTA V Installation Path is wrong, and user does not want to force the Downgrade");
						return;
					}
				}
				LauncherLogic.Downgrade();
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
		/// Method which gets called when the SaveFileHandler Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SaveFiles_Click(object sender, RoutedEventArgs e)
		{
			if (Globals.PageState == Globals.PageStates.SaveFileHandler)
			{
				Globals.PageState = Globals.PageStates.GTA;
			}
			else
			{
				Globals.PageState = Globals.PageStates.SaveFileHandler;
			}
		}

		/// <summary>
		/// Method which gets called when the Settings Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Settings_Click(object sender, RoutedEventArgs e)
		{
			if (Globals.PageState == Globals.PageStates.Settings)
			{
				Globals.PageState = Globals.PageStates.GTA;
			}
			else
			{
				Globals.PageState = Globals.PageStates.Settings;
			}
		}

		/// <summary>
		/// Method which gets called when you click on the ReadMe Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_ReadMe_Click(object sender, RoutedEventArgs e)
		{
			if (Globals.PageState == Globals.PageStates.ReadMe)
			{
				Globals.PageState = Globals.PageStates.GTA;
			}
			else
			{
				Globals.PageState = Globals.PageStates.ReadMe;
			}
		}





		#endregion


		public void SetBackground(string pArtworkPath)
		{
			Uri resourceUri = new Uri(pArtworkPath, UriKind.Relative);
			StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
			BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
			var brush = new ImageBrush();
			brush.ImageSource = temp;
			MainWindow.MW.GridMain.Background = brush;
		}

		private void btn_NoteOverlay_Click(object sender, RoutedEventArgs e)
		{
			if (Globals.PageState == Globals.PageStates.NoteOverlay)
			{
				Globals.PageState = Globals.PageStates.GTA;
			}
			else
			{
				Globals.PageState = Globals.PageStates.NoteOverlay;
			}
		}



	} // End of Class
} // End of Namespace
