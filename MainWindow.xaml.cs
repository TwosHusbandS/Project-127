/*

##########################

	- Windows 10 Checks all Files for Virus if they are run for the first time. If you open a file (Installer or Program)
				and the file doesnt appear to be running, just wait 20 seconds. Should open within that timeframe.
		
	- Install Program (preferably on the same Drive where your GTA is installed.
						Optimally inside the GTAV Installation Folder)
	- Click the hamburger Icon in the top left. Then click "Import ZIP" and select the ZIP File downloaded above (wait until you get the popup confirming its done)
	- Game automatically detects if the installation is upgraded or downgraded 
	- If upgrading / downgrading doesnt yield good results...validate files via Steam, then click "Repair". Should fix most issues.
	- If something is still not working, please uninstall the program via control panel, run ".bat" AS ADMINISTRATOR, re-install again.
	- If something is still not working, please include the file "AAA - Logfile.log" when reporting the bug

##########################


Main Documentation:
Main Client Implementation by "@thS#0305"
The actual hard lifting of the launching (with fixes) and authentification stuff is achieved by the hardwork of "@dr490n", "@Special For" and "@zCri"
Artwork, GUI Design, GUI Behaviour, Colorchoice etc. by "@Hossel"

Version: 0.0.2.1 Closed Beta

Build Instructions:
	Press CTRLF + F5

Deploy Instructions:
	Change Version Number a few Lines Above.
	Change Version Numbner in both of the last lines in AssemblyInfo.cs
	Build this program in release
	Build installer via Innosetup (Script is in \Installer\) [Change Version in Version and OutputName]
	Change Version number and Installer Location in "\Installer\Update.xml"
	Push Commit to github branch.
	Merge branch into master

Getting People into Beta:
	Add their String inside the Regedit Value "MachineGuid" inside the RegeditKey: "Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography"
	to /Installer/AuthUser.txt (on github master branch btw)

We need Admin Acces to access registry, and possibly some file permission stuff
 
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
	LauncherLogic.cs
	MyFile.cs (Custom Class for Objects for the SaveFileHandler, Very much subject to change)
    HelperClasses\Logger.cs
    HelperClasses\RegeditHandler.cs
    HelperClasses\ProcessHandler.cs
	HelperClasses\FileHandler.cs

General Comments and things one should be aware of (still finishing this list)
	Window Icons are set in the Window part of the XAML. Can use resources and relative Paths this way
		This doesnt change the icon when right clicking task bar btw.
	My Other ideas of creating Settings (CustomControl or Programtically) didnt work because
		DataBinding the ButtonContext is hard for some reason. Works which the checkbox tho, which is kinda weird
	BetaMode is hardcoded in Globals.cs
	Importing ZIP needs some work. Currently overwrites all files (including version.txt) apart from "UpgradeFiles" Folder

Main To do:
	- Giving the option to copy / paste instead of Hardlinks
	- Implemet other features (SaveFileHandler,all Settings, auto high priority, auto darkviperau steam core fix)

    - Low Prio:
		Convert Settings and SaveFileHandler in CustomControls
		Popup start in middle of window
		Fix Code signing so we dont get anti virus error
        Implement Hamburger Animation
		Theming
		Get DataBinding working on Button Context for Settings
		Own FPS Limiter

	- AAAA
		- Was in the middle of making this shit not crash when github is unreachable. Will need to work on that. Not done.
		 Just pushing "hotfix" to make this work with new zip file.

		- Work on Loop crashing for some people. Maybe max limit 3 or something

		


Weird Beta Reportings:
	- Reloe double GTAV Location check on firstlaunch 
		// No idea why this happened...cant explain
		// Changed in what order functions are called
		// and am doing more detailed logging.
		// Shouldnt happen again, or if it does we will know why.

	- Multiple People reporting looping asking for GTAV Path. 
		// Probably has to do with the Method which checks if GTAV Patch is valid
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

			// Start the Init Process
			Globals.Init(this);

			if (!CheckIfAllowedToRun())
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "You are not allowed to run this beta.").ShowDialog();
				Environment.Exit(2);
			}

			// Deleting all Installer Files from own Project Installation Path
			HelperClasses.Logger.Log("Checking if there is an old Installer in the Project InstallationPath during startup procedure.");
			foreach (string myFile in HelperClasses.FileHandling.GetFilesFromFolder(Globals.ProjectInstallationPath))
			{
				if (myFile.ToLower().Contains("installer"))
				{
					HelperClasses.Logger.Log("Found old installer File ('" + HelperClasses.FileHandling.PathSplitUp(myFile)[1] + "') in the Directory. Will delete it.");
					HelperClasses.FileHandling.deleteFile(myFile);
				}
			}

			// Make sure Hamburger Menu is invisible when opening window
			this.GridHamburgerOuter.Visibility = Visibility.Hidden;
			this.btn_Auth.Visibility = Visibility.Hidden;

			// Set Image of Buttons
			SetButtonMouseOverMagic(btn_Auth, false);
			SetButtonMouseOverMagic(btn_Exit, false);
			SetButtonMouseOverMagic(btn_Hamburger, false);

			// Auto Updater
			CheckForUpdate();

			HelperClasses.Logger.Log("Startup procedure (Constructor of MainWindow) completed.");
		}

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

						new PopupProgress(PopupDownloadTypes.Installer, DLPath, LocalFileName).ShowDialog();
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
				this.Close();
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
			if (LauncherLogic.AuthState == LauncherLogic.Authstates.Auth)
			{
				LauncherLogic.AuthState = LauncherLogic.Authstates.NotAuth;
			}
			else
			{
				LauncherLogic.AuthState = LauncherLogic.Authstates.Auth;
			}
			SetButtonMouseOverMagic(btn_Auth, true);
			//SetAuthButtonBackground(Authstatus.Auth);

			//string c1 = Settings.GTAVInstallationPath.Substring(0, 2);
			//string c2 = "cd " + Settings.GTAVInstallationPath;
			//string c3 = "start Test9.bat";

			//Process.Start("cmd.exe", "/c " + c1 + " && " + c2 + " && " + c3);













			//string ZZZGTAVPATH = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\PlayGTAV.exe";

			//Process.Start(@"C:\Program Files\TeamSpeak 3 Client\ts3client_win64.exe");

			//Process p = new Process();
			//p.StartInfo.UseShellExecute = true;
			//p.StartInfo.FileName = Settings.GTAVInstallationPath.TrimEnd('\\') + @"\PlayGTAV.exe"; ;
			//p.StartInfo.CreateNoWindow = true;
			//p.StartInfo.WorkingDirectory = Settings.GTAVInstallationPath.TrimEnd('\\');
			//p.Start();

			//Process.Start(Settings.GTAVInstallationPath.TrimEnd('\\') + @"\Test9.bat");
			//Globals.DebugPopup(Settings.GTAVInstallationPath.TrimEnd('\\') + @"\Test9.bat");
			//Process.Start(Settings.GTAVInstallationPath.TrimEnd('\\') + @"\Test9.bat");
			//HelperClasses.ProcessHandler.StartProcess(Settings.GTAVInstallationPath.TrimEnd('\\') + @"\PlayGTAV.exe", "",false,false);
			//HelperClasses.ProcessHandler.StartProcess(Settings.GTAVInstallationPath.TrimEnd('\\') + @"\Test9.bat", "",false,false);






			//string msg = "Size of GTAV in Folder: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\GTA5.exe");
			//msg += "\nSize of Downgraded GTAV: " + LauncherLogic.SizeOfDowngradedGTAV;
			//msg += "\nSize of update.rpf in Folder: " + HelperClasses.FileHandling.GetSizeOfFile(LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\update\update.rpf");
			//msg += "\nSize of Downgraded update.rpf: " + LauncherLogic.SizeOfDowngradedUPDATE;
			//msg += "\nInstallationState: " + LauncherLogic.InstallationState.ToString();
			//Globals.DebugPopup(msg);

			//string[] MyLines = HelperClasses.FileHandling.ReadFileEachLine(Globals.SteamInstallPath.TrimEnd('\\') + @"\steamapps\libraryfolders.vdf");
			//for (int i = 0; i <= MyLines.Length - 1; i++)
			//{
			//	MyLines[i] = MyLines[i].Replace("\t", "").Replace(" ", "");
			//
			//	// String from Regex: #"\d{1,4}""[a-zA-Z\\:]*"# (yes we are matching ", I used # as semicolons for string beginnign and end
			//	Regex MyRegex = new Regex("\"\\d{1,4}\"\"[a-zA-Z\\\\:]*\"");
			//	Match MyMatch = MyRegex.Match(MyLines[i]);
			//	if (MyMatch.Success)
			//	{
			//		MyLines[i] = MyLines[i].TrimEnd('"');
			//		MyLines[i] = MyLines[i].Substring(MyLines[i].LastIndexOf('"') + 1);
			//		MyLines[i] = MyLines[i].Replace(@"\\", @"\");
			//		if (HelperClasses.FileHandling.doesFileExist(MyLines[i].TrimEnd('\\') + @"\steamapps\appmanifest_271590.acf"))
			//		{
			//			Globals.DebugPopup(MyLines[i].TrimEnd('\\') + @"\steamapps\common\Grand Theft Auto V\");
			//		}
			//	}
			//}

			//string pathOfNewZip = HelperClasses.FileHandling.GetXMLTagContent(new System.Net.Http.HttpClient().GetStringAsync(Globals.URL_AutoUpdate).GetAwaiter().GetResult(), "zip");
			//new PopupDownload(PopupDownloadTypes.ZIP, @"https://github.com/TwosHusbandS/Project-127/releases/download/V0/Project_127_Files_V3.zip", Globals.ZipFileDownloadLocation).ShowDialog();
		}

		/// <summary>
		/// Method which gets called when the Launch GTA Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_Click(object sender, RoutedEventArgs e)
		{
			string msg = "GTA V Installation is (probably) " + LauncherLogic.InstallationState.ToString() + ". The Game is (probably) " + LauncherLogic.GameState.ToString() + ".";

			Popup conf = new Popup(Popup.PopupWindowTypes.PopupYesNo, msg + "\nDo you want to Launch the Game?");
			conf.ShowDialog();
			if (conf.DialogResult == true)
			{
				HelperClasses.Logger.Log("Clicked Launch Button", 1);
				LauncherLogic.Launch();
			}

			SetGTAVButtonBasedOnGameAndInstallationState(null, null);
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
			SetGTAVButtonBasedOnGameAndInstallationState(null, null);
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
			Popup conf = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Are you sure you want to Upgrade?");
			conf.ShowDialog();
			if (conf.DialogResult == false)
			{
				return;
			}

			// Actual Upgrade Button Code
			HelperClasses.Logger.Log("Clicked the Upgrade Button");
			if (LauncherLogic.IsGTAVInstallationPathCorrect())
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
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Error: GTA V Installation Path incorrect.");
			}


			SetGTAVButtonBasedOnGameAndInstallationState(null, null);
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
				if (LauncherLogic.IsGTAVInstallationPathCorrect())
				{
					LauncherLogic.Repair();
				}
				else
				{
					HelperClasses.Logger.Log("GTA V Installation Path not found or incorrect. User will get Popup");
					new Popup(Popup.PopupWindowTypes.PopupOkError, "Error: GTA V Installation Path incorrect.");
				}
			}

			SetGTAVButtonBasedOnGameAndInstallationState(null, null);
		}

		/// <summary>
		/// Method which gets called when the Downgrade Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Downgrade_Click(object sender, RoutedEventArgs e)
		{
			// Confirmation Popup
			Popup conf = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Are you sure you want to Downgrade?");
			conf.ShowDialog();
			if (conf.DialogResult == false)
			{
				return;
			}

			// Actual Code behind Downgrade Button
			HelperClasses.Logger.Log("Clicked the Downgrade Button");
			if (LauncherLogic.IsGTAVInstallationPathCorrect())
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
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Error: GTA V Installation Path incorrect.");
			}
			SetGTAVButtonBasedOnGameAndInstallationState(null, null);
		}

		/// <summary>
		/// Method which gets called when the "Import ZIP" Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_ImportZip_Click(object sender, RoutedEventArgs e)
		{
			string ZipFileLocation = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Title", "", Globals.ProjectInstallationPath);
			if (HelperClasses.FileHandling.doesFileExist(ZipFileLocation))
			{
				Globals.ImportZip(ZipFileLocation);
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
			// (new SaveFileHandler()).Show();
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
			string msg = "DarkViperAU";
			msg += "\nCan fix his shit Samsung Phone";
			msg += "\nWith a custom Rom";
			msg += "\n\n- Haiku by some Discord Member";
			msg += "\n\n(Placeholder Text. Will Include some Speedrun Infos\n and Link to some other Resources)";

			new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
		}

		/// <summary>
		/// Method which gets called when the Readme Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Readme_Click(object sender, RoutedEventArgs e)
		{
			// Check our version of the ZIP File
			int ZipVersion = 0;
			Int32.TryParse(HelperClasses.FileHandling.ReadContentOfFile(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\Project_127_Files\Version.txt"), out ZipVersion);


			string msg = "You are running Project 1.27";
			msg += "\nProgram which helps Speedrunners";
			msg += "\nAnd has a few features";
			msg += "\nPlaceholder Text";
			msg += "\n\nZipFileVersion: '" + ZipVersion.ToString() + "'\nVersion: '" + Globals.ProjectVersion.ToString() + "'";

			new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
		}

		/// <summary>
		/// Method which gets called when the Readme Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Credits_Click(object sender, RoutedEventArgs e)
		{
			string msg = "Special thanks to (no particular order):\n";
			msg += "\n@dr490n";
			msg += "\n@Special For";
			msg += "\n@JakeMiester";
			msg += "\n@MOMO";
			msg += "\n@wojtekpolska";
			msg += "\n@Diamondo25";
			msg += "\n@S.M.G";
			msg += "\n@gogsi";
			msg += "\n@Antibones";
			msg += "\n@zCri";
			msg += "\n@Unemployed";
			msg += "\n@Aperture";
			msg += "\n@luky";
			msg += "\n@CrynesSs";
			msg += "\n@hossel";
			msg += "\n@Daniel Kinau";
			msg += "\n@thS";

			new Popup(Popup.PopupWindowTypes.PopupOk, msg).ShowDialog();
		}


		#endregion


		#region GUI Helper Methods

		public void SetGTAVButtonBasedOnGameAndInstallationState(object sender, EventArgs e) // Gets called every DispatcherTimer_Tick. Just starts the read function.
		{
			if (LauncherLogic.InstallationState == LauncherLogic.InstallationStates.Downgraded)
			{
				btn_GTA.Content = "Launch GTA V\n   Downgraded";
			}
			else
			{
				btn_GTA.Content = "Launch GTA V\n   Upgraded";
			}

			if (LauncherLogic.GameState == LauncherLogic.GameStates.Running)
			{
				btn_GTA.BorderBrush = Globals.MW_ButtonGTAGameRunningBorderBrush;
			}
			else
			{
				btn_GTA.BorderBrush = Globals.MW_ButtonGTAGameNotRunningBorderBrush;
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
						if (LauncherLogic.AuthState == LauncherLogic.Authstates.Auth)
						{
							SetButtonBackground(myBtn, @"Artwork\lock_closed_mo.png");
						}
						else if (LauncherLogic.AuthState == LauncherLogic.Authstates.NotAuth)
						{
							SetButtonBackground(myBtn, @"Artwork\lock_open_mo.png");
						}
					}
					else
					{
						if (LauncherLogic.AuthState == LauncherLogic.Authstates.Auth)
						{
							SetButtonBackground(myBtn, @"Artwork\lock_closed.png");
						}
						else if (LauncherLogic.AuthState == LauncherLogic.Authstates.NotAuth)
						{
							SetButtonBackground(myBtn, @"Artwork\lock_open.png");
						}
					}
					break;
				default:
					break;
			}
		}

		private void btn_Small_MouseEnter(object sender, MouseEventArgs e)
		{
			SetButtonMouseOverMagic((Button)sender, true);
		}

		private void btn_Small_MouseLeave(object sender, MouseEventArgs e)
		{
			SetButtonMouseOverMagic((Button)sender, false);
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

					string Reply = HelperClasses.FileHandling.GetStringFromURL(Globals.URL_AuthUser);

					if (Reply.Contains(GUID))
					{
						return true;
					}

					if (String.IsNullOrEmpty(Reply))
					{
						// Letting Users in if github is down...
						new Popup(Popup.PopupWindowTypes.PopupOk, "Letting you in since Github appears to be down...").ShowDialog();
						return true;
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion




	} // End of Class
} // End of Namespace
