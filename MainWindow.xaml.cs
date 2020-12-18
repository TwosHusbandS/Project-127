/*
 
Main Documentation / Dev Diary here:

Actual code (partially closed source) which authentificates, handles entitlement and launches the game (and Overlay Backend) is done by @dr490n
@Special For, who also did a lot of RE'ing, testing, brainstorming, information gathering and 2nd level support
@JakeMiester did some project management, acted as the communication party between the team and the speedrun mods, "invented" trello
@zCri did a research, RE and coding in the initial information gathering stage
Artwork, Design of GUI, GUI Behaviourehaviour, Colorchoices etc. by "@Hossel"
Project 1.27 Client by "@thS"
A number of other members of the team, including but not limited to @MoMo, @Diamondo25, @S.M.G, @gogsi, @Antibones, @Unemployed, @Aperture, @luky, @CrynesSs, @Daniel Kinau contributed to this project one way or another, and my thanks go out to them.
Version: 1.0.9.6

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
		=> Lots of SaveFileHandler Improvments. Really shitty code, really shitty performance, but UX is great.
		=> Overlay (Borderless + MultiMonitor) done.
		=> Jumpscrip done
		=> Fixed Starting other programs with P127

	Release 1.1

		- Internal Testing Reports Bugs:
			
			=> [DONE] Automatic Update of Files detected broken (when update.rpf missing. Maybe check other file attributes instead of size? Mhm. Or different faster method to detect if files are the same
			=> [DONE] More efficent isEqual method for checking if gta update hit
			=> [DONE] popup that path is wrong and you have to force downgrade
			=> [DONE] long freeze on check if update hit...actually as efficent as can be
			=> [DONE] Using Backup broken (folder locked...Fixed when explorer closed. Kinda weird-ish)
			=> [DONE] No "new files blabla popup when upgrade_files is empty
			=> [DONE] Make settings not write enums to settins on startup. Maybe check on Settings property if its the same as current before setting?
			=> [DONE] Change Popup Text from "if Update hit" to something better
			=> [DONE] Change Popup Text from "AutostartBelow" to something better
			=> [DONE] Create Backup method
			=> [DONE] Re-Downmload ZIP Popup on Check for updates
			=> [DONE] Do actual Modes (internal, beta, master etc.) on some hidden UI shit, "default", textbox, "set new", cancel
			=> [DONE] Add "internal mode" and "buildinfo" and "buildtime" to debug info
			=> [DONE] DebugFile async task,  check if what we are overwriting isnt larger than our message, popup then
			=> [DONE] Ugly startup
			=> [DONE] Release installer for a few people to test update on 2020-12-15
			=> [DONE] Hide options when launching through socialclub (GTA V ingame name, pre order bonus, hide from steam)
			=> [DONE] Hide options (launch through social club and shit) when on epic retailer.
			=> [DONE] Add blue face guy to credits. (AntherXx)
			=> [DONE] Scroll faster
			=> [DONE] Launching retailer steam, hide from steam enabled, when upgraded, launches into rockstar launcher...argh
			=> [DONE] Reset settings is wonky UX
			=> [DONE] (Download Manager popup gonna replace that Check for update button) Button to "reset" and get $DowngradeFiles new, since Rockstar fucks us..
			=> [DONE] Better ProgressBar on CreatingBackup...
			=> [DONE] Fix grammar from dragons screen + Other Text Improvements.
			=> [DONE] Rightclick on create and use backup to give options to name it in a specific way. For mods and shit
				>> Create: Custom Control Popup (not new Window)
						Header
						Textbox name popup,
						2 buttons "Create", "Cancel"
				>> Use: Custom Control Popup (not new Window)
						Header
						Select available Backuos from Dropdown / Combobox, delete empty back ups when reading in the info
						Think of rename and exit functions...
						Buttons "Use, "Exit".
			=> [DONE] Check if rolling log works.
			=> [DONE] Less accurate but faster Method for detecting upgrades
			=> [DONE] New SafeFile Export / Import
			=> [Working for me] Launching dragon Emu through steam broken?
			=> [APPARENTLY DONE???] May not need DidUpdateHit Method...Its not called anywhere...
			=> [DONE] Save WPF MM Overlay startup location somehow...
			=> [DIRTFIXED] Overlay cant be toggled when multi monitor mode set before GTA started.
			=> [DONE] Hide WPF MM Overlay from Alt + Tab
			=> [DONE] Reset Window Location of OverlayMonitor
			=> [DONE] Hide Settings when not on Steam
			=> [DONE] Hide Settings when overlay Mode not enabled

			=> More efficent compare of files
			=> Check if Backup Methods (use / create) need to call Upgrade or Downgrade after or before
			=> Back up DowngradeFiles because of rockstar update
			=> On OverlaySettingsChange call when Game started due to weird edge case, make overlay be in same state (visible or not as it was before we had to call the method)
			=> Investigate Jumpscript with Logs for crapideot
			=> Deployment system with modes / branches like above
				--> XML Tag for link / name to specific build.
				--> Download the build, then call Launcher.exe with command line args to swap the files out correctly, so we have the new build.




			=> [NOT CONNECTED TO ANY FILE RELATED LOGIC] Dragons stuff. Both paths, all settings
			=> Remember to not only check if alternative launch, but also check out if epic...
			=> Download Manager keeping track of componments
			=> Support for 1.24
			=> Safe File Handler path switch because of social club switch
			=> Think about integrating new lauch version
					- what files we need, how we get them, with Optional stuff
					- where do we keep social club files? How are we messing with them.
					- what do we need to do if user checks the checkmark and wants new way of launching. Etc.

		Quick and Dirty notes:
			- Clean up Code / Readme / Patchnotes
			- [DONE] Release new ZIP
			- [DONE] Binary Folder and stuff
			- [DONE] Make Launcher Built on Main Built
			- [DONE] Copy (Build event) License File to Proper directory
			- [DONE] Copy (Build event) Jumpscript Exe
			- [DONE] Make it create Folder and Savefile for new release...for SFH Demo
			- [DONE] Translate Keys... 
			- [DONE] Delete Internal File for everyone.
			- [DONE] Clean up "big three" method. Make users click no, check for size > 0 instead of file exists...
			- [DONE] Add Fullscreen mode to settings. Added other stuff to settings. Fixed settings bugs.
			- [DONE] Split settings into 3 subpages
			- [DONE] Command Line args...pass from Launcher to main executable. Check code on main executable.
			- [DONE] Add Jumpscript and Overlay to "only when downgraded". Just check in start methods of those things, check on Setting to true OfSettings what should be done based on settings
			- [DONE] Cant do Overlay "only when downgraded" since we dont tie it to game running or game window when multi monitor mode...
		
	- [DONE] Fullscreen mode for overlay
				--> [DONE] Window with just fixed height bar. Fixed Color. Offblack and white boarder and text.
				--> [DONE] Fullscreen / Multi monitor mode Checkmark on top (under enable) with tooltip
				--> [DONE] implement settings backend
				--> [DONE] Margin and Location greyed out and disabled. With popup. On Enable / Disable overlaw. Method to refresh if those are greyed out or not. Or hook on top UI..
				--> [DONE] Implement overlay stuff...thinking of if check inside constructor where to draw on top on, and call it a day.
						- Enum param on Overlay object which gets checked on changing stuff
				--> [DONE] Y and X Margin sepperate settings. 
				--> [DONE] Options scrollable there...

				--> [DONE] Debug tests POC of showing / stopping showing Overlay hooked to our WPF Window
				--> [DONE] check if we need to add Y Margin to it because of WPF Overlay "Titlebar"
				--> [DONE] Check if it works with our hidden WPF Window...
				--> [DONE] Semi-Connected to backend. With all settings correct on P127 launch, shit works.
				--> [DONE] ReWrite Looks stuff...it should update the actual overlay, but just write to settings on mouseLeftUp
				--> [DONE] make WPF WIndow size width accordingly
				--> [DONE] When we click into the monitor to the side of our WPF Window, it will get back to background, but overlay will stay
				--> [DONE] Theres this thing where you force stuff to be in foreground...that could help. (WPF.Window.Instance.TopMost)
				--> [DONE] WPF Window + Overlay gets init with correct target windows on P127 launch.
				--> [DONE] WPF Window Closes on Hotkey globally
				--> [DONE] WPF Window Close on P127 close
				--> [DONE] WPF Window opens on Hotkey
				--> [DONE] WPF Window Size changes with settings change...
				--> [DONE] Changing Settings of OverlayMode and OverlayEnable work while P127 is running and while GTA is running and non running
				--> [DONE] Display Logic on Look tab. 
				--> [DONE] Bevor messing with stuff below, check how we hide / show currently...and how to untangle that logic
						=> Maybe use OverlayMode for that?
						=> we may be referencing DebugMode of GTAOverlay for that...how can we use that.
						=> Rethink KeyboardListener. Might have to run it 24/7. Maybe already tied to debugmode? 
				--> [DONE] WPF Window Closes on Settings Disable
				--> [DONE] WPF Window openes on Settings Enable
				--> [DONE] WPF Window opens on Look Tab
				--> [DONE] WPF Window closes on Look Tab (unless it was shown before open)
				--> [DONE] Game Overlay doesnt disappear when alt tabbing and in tabbing
				--> [DONE] Make sure shit works when changing settings with GTA Running...
		- [DONE] SFH Improvements
			=> [DONE] Add Folder Support
				>> ReWrite of SaveFileHandler class with enum for File or Folder
				>> Folders as clickable items in list at the very top with a "[FolderName]
				>> Top Folder being "[..]" like in WinRar
				>> Connect to Backend in terms of rename, move around, etc.
			=> [DONE] Add Support for Copy & Move (in Ram) and Paste.
				>> Backend Properites
				>> Taking care of when we show the contextmenus..
				>> Copy / Cut Methods
				>> Pasting Methods
			=> [DONE] Make it load async...with loading gif
			=> [SCRATCHED] Search Bar
			=> [DONE] Rename left file doesnt update text in brackets on right side...
			=> [DONE] MouseOver displays full fillename
			=> [DONE] Make selected File act like NoteOverlay_NoteFiles
			=> [DONE] Better Hotkey support
			=> [SCRATCHED] Multiselect
			=> [DONE] RightClick on File (Copy, Rename, Delete)
			=> [DONE] RightClick on Files (Copy, Delete, Delete)
			=> [DONE] RightClick on Background (new Folder, Paste)
			=> [DONE] RightClick on Folder
			=> [SCRATCHED] Horizontal Scroll bar
			
		
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
using System.Drawing;
using System.Threading;

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

		public static Overlay_MultipleMonitor OL_MM = null;

		private System.Windows.Forms.NotifyIcon notifyIcon = null;

		public static Mutex myMutex;

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

			this.Width = 900;

			// Checking if Mutex is already running
			Mutex m = new Mutex(false, "P127_Mutex");
			if (m.WaitOne(100))
			{
				// When it isnt

				//HelperClasses.FileHandling.AddToDebug("This is our first instance");
				// Globals.DebugPopup("This is our first instance");
			}
			else
			{
				// It is already running

				AlreadyRunning();
			}

			// Starting Mutex
			myMutex = new Mutex(false, "P127_Mutex");
			myMutex.WaitOne();


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

			// Intepreting all Command Line shit
			Globals.CommandLineArgumentIntepretation();

			if (Globals.Mode == "internal")
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
			if (Settings.Mode.ToLower() != "default")
			{
				MainWindow.MW.btn_lbl_Mode.Content = "Curr Mode: '" + MySettings.Settings.Mode.ToLower() + "'";
				MainWindow.MW.btn_lbl_Mode.Visibility = Visibility.Visible;
			}
			else
			{
				MainWindow.MW.btn_lbl_Mode.Content = "";
				MainWindow.MW.btn_lbl_Mode.Visibility = Visibility.Hidden;
			}
			MainWindow.MW.btn_lbl_Mode.ToolTip = MainWindow.MW.btn_lbl_Mode.Content;

			// Init NotifyIcon does not need to be called, its called on Loaded()
			// InitNotifyIcon();

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

		public async void AlreadyRunning()
		{
			//HelperClasses.FileHandling.AddToDebug("In AlreadyRunning(), renaming file now");

			string myPath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\dirtyprogramming";
			string myPathNew = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\pleaseshow";
			// create file. 

			HelperClasses.FileHandling.RenameFile(myPath, "pleaseshow");

			//HelperClasses.FileHandling.AddToDebug("In AlreadyRunning(), File created, before Sleeps");

			await Task.Delay(2000);

			//HelperClasses.FileHandling.AddToDebug("In AlreadyRunning(), After Sleeps");

			if (HelperClasses.FileHandling.doesFileExist(myPathNew))
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Program is open twice.\nAttempt to talk to already running instance failed.\nForce Close Everything?");
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


				//HelperClasses.FileHandling.AddToDebug("In AlreadyRunning(), renamed File exists");
			}
			else
			{
				//HelperClasses.FileHandling.AddToDebug("In AlreadyRunning(), renamed File doesnt exist. Closing this instance");
			}

			Globals.ProperExit();

		}

		/// <summary>
		/// Initialzes CEF settings
		/// </summary>R
		public static void CEFInitialize()
		{
			HelperClasses.Logger.Log("Initializing CEF...");
			var s = new CefSharp.Wpf.CefSettings();
			s.CachePath = Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\CEF_CacheFiles";
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
					string[] args = Environment.GetCommandLineArgs();
					string arg = string.Join(" ", args.Skip(1).ToArray());
					HelperClasses.ProcessHandler.StartProcess(Assembly.GetEntryAssembly().CodeBase, Environment.CurrentDirectory, arg, true, true, false);
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



		private void btn_lbl_Mode_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Settings.Mode.ToLower() != "default")
			{
				new PopupMode().ShowDialog();
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
			e.Cancel = true;
			if (Settings.ExitWay == Settings.ExitWays.HideInTray)
			{
				menuItem_Hide_Click(null, null);
			}
			else
			{
				Globals.ProperExit();
			}
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
			if (Globals.Mode == "internal" || Globals.Mode == "beta")
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
			GenerateDebug();
		}

		private async void GenerateDebug()
		{
			await Task.Run(() =>
			{
				string MyCreationDate = HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName);

				// Debug Info users can give me easily...
				List<string> DebugMessage = new List<string>();

				DebugMessage.Add("Project 1.27 Version: '" + Globals.ProjectVersion + "'");
				DebugMessage.Add("BuildInfo: '" + Globals.BuildInfo + "'");
				DebugMessage.Add("BuildTime: '" + MyCreationDate + "'");
				DebugMessage.Add("Time Now: '" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "'");
				DebugMessage.Add("ZIP Version: '" + Globals.ZipVersion + "'");
				DebugMessage.Add("Mode (Branch): '" + Globals.Mode + "'");
				DebugMessage.Add("InternalMode (Overwites, mode / branch): '" + Globals.InternalMode + "'");
				DebugMessage.Add("Project 1.27 Installation Path '" + Globals.ProjectInstallationPath + "'");
				DebugMessage.Add("Project 1.27 Installation Path Binary '" + Globals.ProjectInstallationPathBinary + "'");
				DebugMessage.Add("ZIP Extraction Path '" + LauncherLogic.ZIPFilePath + "'");
				DebugMessage.Add("LauncherLogic.GTAVFilePath: '" + LauncherLogic.GTAVFilePath + "'");
				DebugMessage.Add("LauncherLogic.UpgradeFilePath: '" + LauncherLogic.UpgradeFilePath + "'");
				DebugMessage.Add("LauncherLogic.DowngradeFilePath: '" + LauncherLogic.DowngradeFilePath + "'");
				DebugMessage.Add("LauncherLogic.SupportFilePath: '" + LauncherLogic.SupportFilePath + "'");
				DebugMessage.Add("Detected AuthState: '" + LauncherLogic.AuthState + "'");
				DebugMessage.Add("Detected GameState: '" + LauncherLogic.GameState + "'");
				DebugMessage.Add("Detected InstallationState: '" + LauncherLogic.InstallationState + "'");
				DebugMessage.Add("    Size of GTA5.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in GTAV Installation Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in DowngradeFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in DowngradeFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in DowngradeFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("    ------------------------------------------------");
				DebugMessage.Add("    Size of GTA5.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\GTA5.exe") + Globals.GetGameInfoForDebug(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\GTA5.exe"));
				DebugMessage.Add("    Size of update.rpf in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\update\update.rpf"));
				DebugMessage.Add("    Size of playgtav.exe in BACKUP UpdateFiles Path: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.UpgradeFilePathBackup.TrimEnd('\\') + @"\playgtav.exe"));
				DebugMessage.Add("Settings: ");
				foreach (KeyValuePair<string, string> KVP in Globals.MySettings)
				{
					DebugMessage.Add("    " + KVP.Key + ": '" + KVP.Value + "'");
				}

				// Building DebugPath
				string DebugFile = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\AAA - DEBUG.txt";

				// Deletes File, Creates File, Adds to it

				string[] currContents = HelperClasses.FileHandling.ReadFileEachLine(DebugFile);

				if (currContents.Length > DebugMessage.Count + 1)
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "The file we are trying to overwrite contains more Lines than we want to write it it.\nBy overwriting it, we might lose information in the debugfile.\nDo you want to overwrite?");
					yesno.ShowDialog();
					if (yesno.DialogResult == false)
					{
						return;
					}
				}

				HelperClasses.FileHandling.WriteStringToFileOverwrite(DebugFile, DebugMessage.ToArray());

				HelperClasses.ProcessHandler.StartProcess(@"C:\Windows\explorer.exe", pCommandLineArguments: Globals.ProjectInstallationPath);
			});
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
				if (Settings.ExitWay == Settings.ExitWays.Close)
				{
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you really want to quit?");
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						Globals.ProperExit();
					}
				}
				else if (Settings.ExitWay == Settings.ExitWays.HideInTray)
				{
					MI_ExitToTray_Click(null, null);
				}
				else if (Settings.ExitWay == Settings.ExitWays.Minimize)
				{
					MI_Minimize_Click(null, null);
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
			ContextMenu cm = new ContextMenu();

			MenuItem mi = new MenuItem();
			mi.Header = "Minimize";
			mi.Click += MI_Minimize_Click;
			cm.Items.Add(mi);

			MenuItem mi2 = new MenuItem();
			mi2.Header = "Hide in Tray";
			mi2.Click += MI_ExitToTray_Click;
			cm.Items.Add(mi2);

			MenuItem mi3 = new MenuItem();
			mi3.Header = "Close P127";
			mi3.Click += MI_Close_Click;
			cm.Items.Add(mi3);

			//MenuItem mi4 = new MenuItem();
			//mi4.Header = "Compare 2 Files";
			//mi4.Click += MI_Debug_Click;
			//cm.Items.Add(mi4);

			//MenuItem mi5 = new MenuItem();
			//mi5.Header = "Did Update Hit";
			//mi5.Click += MI_Debug2_Click;
			//cm.Items.Add(mi5);

			//MenuItem mi6 = new MenuItem();
			//mi6.Header = "ResetBtn";
			//mi6.Click += MI_Debug3_Click;
			//cm.Items.Add(mi6);

			cm.IsOpen = true;

			//Globals.DebugPopup(Globals.CommandLineArgs.ToString());
			//Globals.DebugPopup(Globals.InternalMode.ToString());
		}

		private void MI_Debug3_Click(object sender, RoutedEventArgs e)
		{
			//Globals.DebugPopup(Globals.XML_AutoUpdate);

			btn_Downgrade.Content = "Downgrade";
			HelperClasses.FileHandling.HardLinkFiles(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\LICENSE_LINK", Globals.ProjectInstallationPath.TrimEnd('\\') + @"\LICENSE");
		}

		private void MI_Debug2_Click(object sender, RoutedEventArgs e)
		{
			bool areTheyEqual = LauncherLogic.DidUpdateHit();
			btn_Downgrade.Content = "Downgrade: Did Update Hit: '" + areTheyEqual.ToString() + "'";
		}

		private void MI_Debug_Click(object sender, RoutedEventArgs e)
		{
			btn_Downgrade.Content = "Checking...";

			string GTA_GTA5 = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\gta5.exe";
			string GTA_PlayGTAV = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\playgtav.exe";
			string GTA_UpdateRPF = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\update\update.rpf";

			string Upgrade_GTA5 = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\gta5.exe";
			string Upgrade_PlayGTAV = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\playgtav.exe";
			string Upgrade_UpdateRPF = LauncherLogic.UpgradeFilePath.TrimEnd('\\') + @"\update\update.rpf";

			string Downgrade_GTA5 = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\gta5.exe";
			string Downgrade_PlayGTAV = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\playgtav.exe";
			string Downgrade_UpdateRPF = LauncherLogic.DowngradeFilePath.TrimEnd('\\') + @"\update\update.rpf";

			//bool areTheyEqual = HelperClasses.FileHandling.AreFilesEqual(GTA_UpdateRPF, Upgrade_UpdateRPF, );
			//btn_Downgrade.Content = "Update Files are equal: '" + areTheyEqual.ToString() + "'";

			//if (FileHandling.GetSizeOfFile(GTA_UpdateRPF) == FileHandling.GetSizeOfFile(Downgrade_UpdateRPF))
			//{
			//	areTheyEqual = true;
			//}
			//else
			//{
			//	areTheyEqual = false;
			//}


		}


		private void MI_Minimize_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void MI_ExitToTray_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}

		private void MI_Close_Click(object sender, RoutedEventArgs e)
		{
			Globals.ProperExit();
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

				if (!LauncherLogic.IsGTAVInstallationPathCorrect(false))
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

				if (Globals.Mode == "internal" || Globals.Mode == "beta")
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

				if (!LauncherLogic.IsGTAVInstallationPathCorrect(false))
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

				if (Globals.Mode == "internal" || Globals.Mode == "beta")
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

		private void btn_Upgrade_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{

			if (e.ClickCount == 3)
			{
				new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOk, "Shoutouts to @crapideot for being awesome and a\ngreat friend and Helper during Project 1.27 :)\nHope you have a great day buddy").ShowDialog();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitNotifyIcon();

			NoteOverlay.OverlaySettingsChanged();
		}


		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
		}

		private void notifyIcon_Click(object sender, EventArgs e)
		{
			if (this.Visibility == Visibility.Visible)
			{
				menuItem_Hide_Click(null, null);
			}
			else
			{
				menuItem_Show_Click(null, null);
			}
		}


		private void InitNotifyIcon()
		{
			notifyIcon = new System.Windows.Forms.NotifyIcon();
			notifyIcon.Click += new EventHandler(notifyIcon_Click);
			notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
			notifyIcon.Visible = true;


			Uri resourceUri = new Uri(@"Artwork\icon.ico", UriKind.Relative);
			System.Windows.Forms.NotifyIcon icon = new System.Windows.Forms.NotifyIcon();
			using (Stream iconStream = Application.GetResourceStream(resourceUri).Stream)
			{
				icon.Icon = new System.Drawing.Icon(iconStream);
				notifyIcon.Icon = icon.Icon;
				iconStream.Dispose();
			}

			System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();

			System.Windows.Forms.MenuItem mi1 = new System.Windows.Forms.MenuItem();
			mi1.Text = "Show P127";
			mi1.Click += new System.EventHandler(this.menuItem_Show_Click);
			cm.MenuItems.Add(mi1);

			System.Windows.Forms.MenuItem mi2 = new System.Windows.Forms.MenuItem();
			mi2.Text = "Hide P127";
			mi2.Click += new System.EventHandler(this.menuItem_Hide_Click);
			cm.MenuItems.Add(mi2);

			cm.MenuItems.Add("-");

			System.Windows.Forms.MenuItem mi3 = new System.Windows.Forms.MenuItem();
			mi3.Text = "Upgrade";
			mi3.Click += new System.EventHandler(this.menuItem_Upgrade_Click);
			cm.MenuItems.Add(mi3);

			System.Windows.Forms.MenuItem mi4 = new System.Windows.Forms.MenuItem();
			mi4.Text = "Downgrade";
			mi4.Click += new System.EventHandler(this.menuItem_Downgrade_Click);
			cm.MenuItems.Add(mi4);

			System.Windows.Forms.MenuItem mi5 = new System.Windows.Forms.MenuItem();
			mi5.Text = "Launch Game";
			mi5.Click += new System.EventHandler(this.menuItem_LaunchGame_Click);
			cm.MenuItems.Add(mi5);

			cm.MenuItems.Add("-");

			System.Windows.Forms.MenuItem mi6 = new System.Windows.Forms.MenuItem();
			mi6.Text = "SaveFileHandler";
			mi6.Click += new System.EventHandler(this.menuItem_SaveFileHandler_Click);
			cm.MenuItems.Add(mi6);

			System.Windows.Forms.MenuItem mi7 = new System.Windows.Forms.MenuItem();
			mi7.Text = "NoteOverlay";
			mi7.Click += new System.EventHandler(this.menuItem_NoteOverlay_Click);
			cm.MenuItems.Add(mi7);

			System.Windows.Forms.MenuItem mi8 = new System.Windows.Forms.MenuItem();
			mi8.Text = "Settings";
			mi8.Click += new System.EventHandler(this.menuItem_Settings_Click);
			cm.MenuItems.Add(mi8);

			System.Windows.Forms.MenuItem mi9 = new System.Windows.Forms.MenuItem();
			mi9.Text = "Information";
			mi9.Click += new System.EventHandler(this.menuItem_Information_Click);
			cm.MenuItems.Add(mi9);

			cm.MenuItems.Add("-");

			System.Windows.Forms.MenuItem mi10 = new System.Windows.Forms.MenuItem();
			mi10.Text = "Close P127";
			mi10.Click += new System.EventHandler(this.menuItem_Close_Click);
			cm.MenuItems.Add(mi10);

			notifyIcon.ContextMenu = cm;

			if (Settings.StartWay == Settings.StartWays.Maximized)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
		}

		public void menuItem_Show_Click(object Sender, EventArgs e)
		{
			this.WindowState = WindowState.Normal;
			this.Show();
			this.Activate();
		}

		private void menuItem_Hide_Click(object Sender, EventArgs e)
		{
			try
			{
				this.Hide();
			}
			catch (Exception ex)
			{
				//Globals.DebugPopup(ex.ToString());
			}
		}

		private void menuItem_Upgrade_Click(object Sender, EventArgs e)
		{
			this.btn_Upgrade_Click(null, null);
		}

		private void menuItem_Downgrade_Click(object Sender, EventArgs e)
		{
			this.btn_Downgrade_Click(null, null);
		}

		private void menuItem_LaunchGame_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.GTA;
			GTA_Page.btn_GTA_Click_Static();

			//string oldPath = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles";
			//string newPath = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\UpgradeFiles_Backup";
			//Globals.DebugPopup("yep");
			//HelperClasses.FileHandling.movePath(newPath, oldPath);
			//Globals.DebugPopup("nay");
		}

		private void menuItem_SaveFileHandler_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.SaveFileHandler;
		}

		private void menuItem_NoteOverlay_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.NoteOverlay;
		}

		private void menuItem_Settings_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.Settings;
		}

		private void menuItem_Information_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.ReadMe;
		}

		private void menuItem_Close_Click(object Sender, EventArgs e)
		{
			Globals.ProperExit();
		}

	} // End of Class
} // End of Namespace
