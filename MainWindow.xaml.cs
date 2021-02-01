/*
 
Main Documentation / Dev Diary here:

Actual code (partially closed source) which authentificates, handles entitlement and launches the game (and Overlay Backend) is done by @dr490n
@Special For, who also did a lot of RE'ing, testing, brainstorming, information gathering and 2nd level support
@JakeMiester did some project management, acted as the communication party between the team and the speedrun mods, "invented" trello
@zCri did a research, RE and coding in the initial information gathering stage
Artwork, Design of GUI, GUI Behaviourehaviour, Colorchoices etc. by "@Hossel"
Project 1.27 Client by "@thS"
A number of other members of the team, including but not limited to @MoMo, @Diamondo25, @S.M.G, @gogsi, @Antibones, @Unemployed, @Aperture, @luky, @CrynesSs, @Daniel Kinau contributed to this project one way or another, and my thanks go out to them.
Version: 1.1.9.1

Build Instructions:
	Press CTRLF + F5, pray that nuget does its magic.
	If this doesnt work, required DLLs and files can be gotten by running the latest installer

Deploy Instructions:
	
	Change Version Number a few Lines Above.
	Change Version Number in both of the last lines in AssemblyInfo.cs
	ALSO CHANGE VERSION NUMBER IN ASSEMBLYINFO.CS FROM P127 LAUNCHER
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

Stuff to fix post 1.1:
- Installer:
	>> BUILD WITHOUT EASTER EGG COMMENTED OUT
	>> Build Installer
	>> Test if uninstall delete jumpscript license works.
	>> remove that uninstall delete before pushing
- unit test:
	>> one simulated game update
	>> rockstar fucking us
	>> custom backup stuff
	>> rgl launcher being killed / closed during MTL auth
- UX on Initial stuff as well as resetting as well as moving GTA location or moving ZIP Location
- Auto MTL Auth and close (instead of minimize) on new setting and mtl
- Write Patchnotes

[DONE] Fix Popup Starting Location with parent Window
[DONE] New Import ZIP Magic
[DONE] ReadMe PowerUser instructions and link to it in help + Update existing Help Section - Include Trevor% File
[DONE] Popup general improvements
[DONE] Installing Component UI Freeze Fix
[DONE] Installing Component, automatically installing social club when installing SCL stuff
[DONE] Other UI /UX for Launch and Auth Methods. Have them as enums in settings and as radio-buttons-functionality buttons
[DONE] Potentially fixed remember me issue...
[DONE] i am throwing the "pls disable auto popup" when retailer is rockstar and when legacy auth is disabled. I should also check that "launch through social club" is disabled
[DONE] im error checking something twice, making UX a bit wonky. If you change retailer (and have SCL enabled) it will prompt you to download the new things. If you dont want to, it wont change retailer. Even tho it will also check before game launch and before downgrade
[DONE | Will be Fixed with New UI / UX for launch options] Hiding options on GTA Setting sometimes hides twice...Nothing bad, but just different color-strength on hidden stuff.
[DONE] MTL Error + P127 Foreground stuff is still being doen in loop https://discord.com/channels/501661012844347392/748877687019536384/802128277631533076
[DONE] Log GetStringFromURL with actual exception
[DONE] Reset Social Club folders outside of $P217_Files Post SCL_Social Club install / reinstall / verify
[DONE] Tooltips on Paths inside Settings
[DONE] think about moving ResetSettings inside general P127 Settings
[DONE] add UTC now to Logger on startup
[DONE] improved Debug : )
[DONE] Change this popup text https://discord.com/channels/501661012844347392/748877687019536384/803720895205867524
[DONE] Fix this bug: https://discord.com/channels/501661012844347392/748877687019536384/803748189299539978
[DONE] Might still load files and subfiles on startup of savefilemanager
[DONE] Rightclick loaded note file to edit with notepad
[DONE] Note Overlay Title thingy
[DONE] kill processes from guy https://discord.com/channels/501661012844347392/748877687019536384/803593329652269086
[DONE] Check if RollingLog actually works

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
using System.Windows.Threading;
using System.DirectoryServices.AccountManagement;

namespace Project_127
{
	/// <summary>
	/// CLass of Main Window which gets displayed when you start the application.
	/// </summary>
	public partial class MainWindow : Window
	{

		// Properties and Constructor below

		#region Properties and Constructor

		/// <summary>
		/// Static Property to access Children (mainly Controls) of MainWindow Instance
		/// </summary>
		public static MainWindow MW;

		/// <summary>
		/// Static Reference to our WPF Window for Multi Monitor Overlay
		/// </summary>
		public static Overlay_MultipleMonitor OL_MM = null;

		/// <summary>
		/// Static Property to our NotifyIcon (Tray icon)
		/// </summary>
		public System.Windows.Forms.NotifyIcon notifyIcon = null;

		/// <summary>
		/// Static Property of the Mutex we use to determine if P127 is already running in another instance
		/// </summary>
		public static Mutex myMutex;

		/// <summary>
		/// Property of the Dispatcher Timer we use to keep track of GameState
		/// </summary>
		public static DispatcherTimer MyDispatcherTimer;

		/// <summary>
		/// Property of the Dispatcher Timer we use to control automatic MTL session retrieval
		/// </summary>
		public static DispatcherTimer MTLAuthTimer;

		private Stopwatch StartUpStopwatch;

		/// <summary>
		/// Constructor of Main Window
		/// </summary>
		public MainWindow()
		{
			StartUpStopwatch = new Stopwatch();
			StartUpStopwatch.Start();

			// Initializing all WPF Elements
			InitializeComponent();

			// Setting this for use in other classes later
			MainWindow.MW = this;

			//Dont run anything when we are on 32 bit...
			//If this ever gets changed, take a second look at regedit class and path(different for 32 and 64 bit OS)
			if (Environment.Is64BitOperatingSystem == false)
			{
				(new Popup(Popup.PopupWindowTypes.PopupOkError, "32 Bit Operating System detected.\nGTA (afaik) does not run on 32 Bit at all.")).ShowDialog();
				Environment.Exit(1);
			}

			// Admin Relauncher
			AdminRelauncher();

			// Some shit due to do with the multi monitor preview
			this.Width = 900;

			// Checking if Mutex is already running
			Mutex m = new Mutex(false, "P127_Mutex");
			if (m.WaitOne(100))
			{
				// When it isnt
			}
			else
			{
				// It is already running, calls Globals.ProperExit
				AlreadyRunning();
			}


			// Starting our own Mutex since its not already running
			myMutex = new Mutex(false, "P127_Mutex");
			myMutex.WaitOne();

			// Some Background Change based on Date
			ChangeBackgroundBasedOnSeason();

			// Intepreting all Command Line shit
			Globals.CommandLineArgumentIntepretation();

			// Start the Init Process of Logger, Settings, Globals, Regedit here, since we need the Logger in the next Line if it fails...
			Globals.Init();

			if (Globals.P127Branch == "internal")
			{
				string msg = "We are in internal mode. We need testing on:\n\n" +
					"- NoteOverlay" + "\n" +
					"- Jumpscript" + "\n" +
					"- DISABLED Legacy Auth" + "\n" +
					"- ENABLED Launch through Social Club" + "\n" +
					"\nI do expect everything to work, so no extensive Testing needed." + "\n" +
					"\nThanks. Appreciated. Have a great day : )";

				new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
			}

			// GUI SHIT
			SetButtonMouseOverMagic(btn_Exit);
			SetButtonMouseOverMagic(btn_Auth);
			SetButtonMouseOverMagic(btn_Hamburger);
			Globals.HamburgerMenuState = Globals.HamburgerMenuStates.Hidden;
			if (Settings.P127Mode.ToLower() != "default")
			{
				MainWindow.MW.btn_lbl_Mode.Content = "Curr P127 Mode: '" + MySettings.Settings.P127Mode.ToLower() + "'";
				MainWindow.MW.btn_lbl_Mode.Visibility = Visibility.Visible;
			}
			else
			{
				MainWindow.MW.btn_lbl_Mode.Content = "";
				MainWindow.MW.btn_lbl_Mode.Visibility = Visibility.Hidden;
			}
			MainWindow.MW.btn_lbl_Mode.ToolTip = MainWindow.MW.btn_lbl_Mode.Content;

			StartDispatcherTimer();

			StartMTLDispatcherTimer();

			HelperClasses.Logger.Log("Only CEF Init to go...");

			Auth.ROSIntegration.CEFInitialize();

			// Making this show on WindowLoaded
			//HelperClasses.Logger.Log("Startup procedure (Constructor of MainWindow) completed.");
			//HelperClasses.Logger.Log("--------------------------------------------------------");

			// Testing Purpose for the overlay shit
			if (GTAOverlay.DebugMode)
			{
				//We currently need this here, normally this will be started by GameState(but this points to GTA V.exe as of right now)
				NoteOverlay.InitGTAOverlay();

				// Same as other two thingies here lolerino
				HelperClasses.WindowChangeListener.Start();
			}
		}

		#endregion

		// Properties and Constructor above

		// Windowevents below

		#region windowevents

		/// <summary>
		/// Event when Window finished Loading
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitNotifyIcon();

			NoteOverlay.OverlaySettingsChanged();

			StartUpStopwatch.Stop();

			HelperClasses.Logger.Log("Startup procedure (Constructor of MainWindow) completed. It took " + StartUpStopwatch.ElapsedMilliseconds + " ms.");
			HelperClasses.Logger.Log("------------------------------------------------------------------------------------");

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

		#endregion

		// Window Events above

		// AlreadyRunning, AdminRelauncher and DIspatcher Timer below

		#region AlreadyRunning, Admin Relauncher, DispatcherTimer

		/// <summary>
		/// Gets called when another P127 instance is already running. 
		/// </summary>
		public void AlreadyRunning()
		{

			try
			{
				var pc = new IPCPipeClient("Project127Launcher");

				byte[] rtrn = pc.call("pleaseshow", Encoding.UTF8.GetBytes("ThanksDragonForImplementing"));

				System.Threading.Thread.Sleep(500);

				if (rtrn[0] == Convert.ToByte(true))
				{
					this.Close();
					Environment.Exit(0);
					return;
				}
			}
			catch
			{

			}

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
				Globals.ProperExit();
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



		/// <summary>
		/// Starting the Dispatcher Timer. 2,5 seconds. Used for Polling Gamestate and stuff
		/// </summary>
		private void StartDispatcherTimer()
		{
			// Starting the Dispatcher Timer for the automatic updates of the GTA V Button
			MyDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			MyDispatcherTimer.Tick += new EventHandler(MainWindow.MW.UpdateGUIDispatcherTimer);
			MyDispatcherTimer.Interval = TimeSpan.FromMilliseconds(2500);
			MyDispatcherTimer.Start();
			MainWindow.MW.UpdateGUIDispatcherTimer();
		}



		/// <summary>
		/// Updates the GUI with relevant stuff. Gets called every 2.5 Seconds
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void UpdateGUIDispatcherTimer(object sender = null, EventArgs e = null) // Gets called every DispatcherTimer_Tick. Just starts the read function.
		{
			if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
			{
				lbl_GTA.Foreground = MyColors.MW_GTALabelDowngradedForeground;
				lbl_GTA.Content = "Downgraded";
			}
			else if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Upgraded)
			{
				lbl_GTA.Foreground = MyColors.MW_GTALabelUpgradedForeground;
				lbl_GTA.Content = "Upgraded";
			}
			else
			{
				lbl_GTA.Foreground = MyColors.MW_GTALabelUnsureForeground;
				lbl_GTA.Content = "Unsure";
			}

			lbl_GTA.Content += BuildVersionTable.GetNiceGameVersionString(Globals.GTABuild);

			if (LauncherLogic.PollGameState() == LauncherLogic.GameStates.Running)
			{
				GTA_Page.btn_GTA_static.BorderBrush = MyColors.MW_ButtonGTAGameRunningBorderBrush;
				GTA_Page.btn_GTA_static.Content = "Exit GTA V";
			}
			else
			{
				GTA_Page.btn_GTA_static.BorderBrush = MyColors.MW_ButtonGTAGameNotRunningBorderBrush;
				GTA_Page.btn_GTA_static.Content = "Launch GTA V";
			}

			SetButtonMouseOverMagic(btn_Auth);
		}

		/// <summary>
		/// Starting the Dispatcher Timer. 30 seconds. Used to control automatic MTL session retrieval
		/// </summary>
		private void StartMTLDispatcherTimer()
		{
			// Starting the Dispatcher Timer for the automatic updates of the GTA V Button
			MTLAuthTimer = new System.Windows.Threading.DispatcherTimer();
			MTLAuthTimer.Tick += new EventHandler(MainWindow.MW.AutoAuthMTLTimer);
			MTLAuthTimer.Interval = TimeSpan.FromMilliseconds(2000);
			MTLAuthTimer.Start();
			MainWindow.MW.AutoAuthMTLTimer();
		}

		/// <summary>
		/// Attempts to update auth session using MTL session data. Runs every 30 seconds
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AutoAuthMTLTimer(object sender = null, EventArgs e = null)
		{

			if (LauncherLogic.AuthState == LauncherLogic.AuthStates.Auth)
			{
				MTLAuthTimer.Stop();
			}
			else if (LauncherLogic.LaunchWay == LauncherLogic.LaunchWays.SocialClubLaunch)
			{
				return;
			}
			else
			{
				Auth.ROSCommunicationBackend.LoginMTL();
			}
		}

		#endregion

		// AlreadyRunning, AdminRelauncher and DIspatcher Timer anove

		// PureUI Logic and Helper Classes below

		#region PureUI Logic and helper Classes

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
		/// Changing Background based on current Date
		/// </summary>
		private void ChangeBackgroundBasedOnSeason()
		{
			DateTime Now = DateTime.Now;

			if ((Now.Month == 4 && Now.Day == 20) || (Now.Month == 4 && Now.Day == 22))
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
		/// Set the Background to our WPF Window
		/// </summary>
		/// <param name="pArtworkPath"></param>
		public void SetWindowBackground(string pArtworkPath)
		{
			Uri resourceUri = new Uri(pArtworkPath, UriKind.Relative);
			StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
			BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
			var brush = new ImageBrush();
			brush.ImageSource = temp;
			MainWindow.MW.GridMain.Background = brush;
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

		// PureUI Logic and Helper Classes above


	} // End of Class
} // End of Namespace
