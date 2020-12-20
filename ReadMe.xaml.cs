using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
	/// Interaction logic for ReadMe.xaml
	/// </summary>
	public partial class ReadMe : Page
	{
		/// <summary>
		/// Last ReadMeState saved to load it up correctly
		/// </summary>
		public static ReadMeStates LastReadMeState = ReadMeStates.About;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReadMe()
		{
			// WPF Shit
			InitializeComponent();

			// Setting ReadMeState to the LastReadMeState
			ReadMeState = ReadMe.LastReadMeState;

			SetUpSpeedrun();
			SetUpCredits();
			SetUpAbout();
			SetUpHelp();

		}


		/// <summary>
		/// Enum for all ReadMeStates
		/// </summary>
		public enum ReadMeStates
		{
			SpeedRun,
			About,
			Credits,
			Help
		}

		/// <summary>
		/// Internal Value
		/// </summary>
		private ReadMeStates _ReadMeState = ReadMeStates.About;

		/// <summary>
		/// Value we get and set. Setters are gucci. 
		/// </summary>
		public ReadMeStates ReadMeState
		{
			get
			{
				return _ReadMeState;
			}
			set
			{
				_ReadMeState = value;

				// Saving it in LastReadMeState
				ReadMe.LastReadMeState = value;

				if (value == ReadMeStates.SpeedRun)
				{
					Border_SpeedRun.Visibility = Visibility.Visible;
					btn_SpeedRun.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

					Border_About.Visibility = Visibility.Hidden;
					btn_About.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Border_Credits.Visibility = Visibility.Hidden;
					btn_Credits.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Border_Help.Visibility = Visibility.Hidden;
					btn_Help.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
				}
				else if (value == ReadMeStates.About)
				{
					Border_About.Visibility = Visibility.Visible;
					btn_About.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

					Border_SpeedRun.Visibility = Visibility.Hidden;
					btn_SpeedRun.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Border_Credits.Visibility = Visibility.Hidden;
					btn_Credits.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Border_Help.Visibility = Visibility.Hidden;
					btn_Help.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
				}
				else if (value == ReadMeStates.Credits)
				{
					Border_Credits.Visibility = Visibility.Visible;
					btn_Credits.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

					Border_SpeedRun.Visibility = Visibility.Hidden;
					btn_SpeedRun.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Border_About.Visibility = Visibility.Hidden;
					btn_About.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Border_Help.Visibility = Visibility.Hidden;
					btn_Help.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
				}
				else if (value == ReadMeStates.Help)
				{
					Border_Help.Visibility = Visibility.Visible;
					btn_Help.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

					Border_Credits.Visibility = Visibility.Hidden;
					btn_Credits.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Border_SpeedRun.Visibility = Visibility.Hidden;
					btn_SpeedRun.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Border_About.Visibility = Visibility.Hidden;
					btn_About.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
				}
			}
		}

		private void SetUpSpeedrun()
		{
			AddParagraph(rtb_SpeedRun, "");

			rtb_SpeedRun.Document.Blocks.Remove(rtb_SpeedRun.Document.Blocks.FirstBlock);

			Hyperlink linkD1 = new Hyperlink();
			linkD1.IsEnabled = true;
			linkD1.Inlines.Add("Invite - Link 1");
			linkD1.NavigateUri = new Uri("https://discord.gg/3qjGGBM");
			linkD1.RequestNavigate += Hyperlink_RequestNavigate;

			Hyperlink linkD2 = new Hyperlink();
			linkD2.IsEnabled = true;
			linkD2.Inlines.Add("Invite - Link 2");
			linkD2.NavigateUri = new Uri("https://discord.gg/rRrTGUV");
			linkD2.RequestNavigate += Hyperlink_RequestNavigate;

			Hyperlink linkD3 = new Hyperlink();
			linkD3.IsEnabled = true;
			linkD3.Inlines.Add("Invite - Link 3");
			linkD3.NavigateUri = new Uri("https://discord.com/invite/zQt8wZg");
			linkD3.RequestNavigate += Hyperlink_RequestNavigate;

			Hyperlink linkR1 = new Hyperlink();
			linkR1.IsEnabled = true;
			linkR1.Inlines.Add("this discord message");
			linkR1.NavigateUri = new Uri("https://discordapp.com/channels/501661012844347392/554856196679532544/701171654948028426");
			linkR1.RequestNavigate += Hyperlink_RequestNavigate;

			Hyperlink linkR2 = new Hyperlink();
			linkR2.IsEnabled = true;
			linkR2.Inlines.Add("speedrun.com/gtav");
			linkR2.NavigateUri = new Uri("https://www.speedrun.com/gtav");
			linkR2.RequestNavigate += Hyperlink_RequestNavigate;

			Paragraph para = new Paragraph();
			para.Margin = new Thickness(10); // remove indent between paragraphs
			para.Inlines.Add("General Information\n");
			para.Inlines.Add("GTA V is a Rockstar game Speedrun by many people. The goals change depending on the category. There are many things you ought to know:\n");
			para.Inlines.Add("Id very much recommend joining the GTA V Speedrunning Discord (");
			para.Inlines.Add(linkD1);
			para.Inlines.Add(new Run(", "));
			para.Inlines.Add(linkD2);
			para.Inlines.Add(new Run(", "));
			para.Inlines.Add(linkD3);
			para.Inlines.Add(new Run(").\nThere are a lot of resources in "));
			para.Inlines.Add(linkR1);
			para.Inlines.Add(new Run(", as well as on "));
			para.Inlines.Add(linkR2);
			para.Inlines.Add(new Run("."));
			para.TextAlignment = TextAlignment.Center;
			rtb_SpeedRun.Document.Blocks.Add(para);

			AddParagraph(rtb_SpeedRun, "CATEGORIES\nThe many categories of GTA V speedrunning are sorted into 3 main groups. Any%, 100%, and Misc.\nAny% is about completing the game with any completion percentage, as fast as possible. The categories of Any% are;\n- Any% Classic,\n- Any% No Mission Skips, and\n- Any% Mission Skips.\n\n100% is about completing the game with a full completion percentage, as fast as possible. The categories of 100% are;\n- 100% Classic,\n- 100% No Mission Skips, and\n- 100% Mission Skips.\n\nMisc is everything that can't be put under neither Any%, nor 100%. The categories here are usually not taken seriously, and are only meant as casual fun. (Other than Golf and a certain individual :p) The categories of Misc are;\n- Segments,\n- All Stunt Jumps,\n- A Close Shave,\n- All Letters,\n- All Monkey Mosaics,- All Peyote Plants.\n\nRULES\nClassic: Classic is supposed to mimic the speedruns of earlier GTA games. In IV, and V, many new game mechanics got introduced that weren't present previously. Classic forbids the use of Taxis, Mission Skipping, and choosing the The Third Way ending is mandatory.Timer starts on first frame of the Prologue text.\n\nNo Mission Skips: The only thing that No Mission Skips forbids is, surprisingly, Mission Skipping.Taxis and any ending are allowed here.Timer starts on first frame of the Prologue text.\n\nMission Skips: This ruleset allows everything.Timer starts on first frame of the Prologue text.\n\nSegments: These follow the rules of Classic. Shorter, bite-sized runs for optimal learning.Timer varies from segment to segment, but usually when you gain control.\n\nAll Stunt Jumps: Goal is to hit all 50 Stunt Jumps of the game as fast as you can. The run starts from Franklin and Lamar, which is easiest to reach from the Prologue autosave. Mission Skipping, and Taxis are all allowed.Timer starts on first frame of the Franklin and Lamar cutscene.\n\nA Close Shave: A similar run to All Stunt Jumps, but here you complete all Under The Bridges, and Knife Flights. Mission Skipping, and Taxis are all allowed.Timer starts on first frame of the Franklin and Lamar cutscene.\n\nAll Letters: Pick up all Letter scraps scattered around the map. These become accessible after completing Prologue, Franklin and Lamar, and Repossession. Mission Skipping, and Taxis are all allowed.Timer starts on first frame of the Franklin and Lamar cutscene.\n\nAll Monkey Mosaics: Photograph all Monkey Mosaics scattered around the map. Mission Skipping, and Taxis are all allowed.Timer starts on first frame of the Franklin and Lamar cutscene.\n\nAll Peyote Plants: Pick up all Peyote Plants scattered around the map.Mission Skipping, and Taxis are all allowed.Timer starts on first frame of the Franklin and Lamar cutscene.\n");
		}

		private void SetUpCredits()
		{
			AddParagraph(rtb_Credits, "");

			rtb_Credits.Document.Blocks.Remove(rtb_Credits.Document.Blocks.FirstBlock);

			AddParagraph(rtb_Credits, "Solving the patch 1.27 Downgrade problem has been achieved by a month of hard work by a number of dedicated individuals. This would not have been possible without the time and effort of a number of very talented individuals from all walks of life, who have contributed skills in Reverse Engineering, Programming, Decryption, Project Management, Scripting and Testing. Below is a list of SOME of the main contributors to the project, although our thanks go out to EVERYONE who has helped throughout the process.");

			AddParagraph(rtb_Credits, "Reverse Engineering:\n" +
"@dr490n, @Special For, @zCri");

			AddParagraph(rtb_Credits, "Launcher / Client Programming, Documentating:\n" +
"@thS");

			AddParagraph(rtb_Credits, "Launcher GUI Design & Artwork:\n" +
"@hossel");

			AddParagraph(rtb_Credits, "Special thanks to:\n" +
"@JakeMiester, @Antibones, @Aperture, @MOMO");

			AddParagraph(rtb_Credits, "Special shoutouts to @dr490n, who was responsible for getting the downgraded game to launch, adding patches against in-game triggers, writing the Overlay Backend, authentication backend, decryption and managed to get the preorder entitlement to work.");

			AddParagraph(rtb_Credits, "Special shoutouts to @special for, for being there during the whole development phase, providing valuable insight, being available to bounce ideas off of, being available for brainstorming, and providing valuable help in regards to reverse engineering the GTA V Launch Process. We could not have done this without you.");

			AddParagraph(rtb_Credits, "Shoutout to FiveM and Goldberg, whose Source Code proved to be vital\n" +
"to understand and reverse engineer the GTA V Launch Process");

			AddParagraph(rtb_Credits, "Shoutout to @Fro for providing Hosting to the Files needed for Project 1.27");

			AddParagraph(rtb_Credits, "Shoutout to @yoshi for providing the Information which Build Version corresponds with which Game Version");

			AddParagraph(rtb_Credits, "Shoutout to @Diamondo25 for finding a way to launch GTA without going through LaunchGTAV.exe or PlayGTAV.exe, which eased the development process to launch through Social Club");

			AddHyperlinkText(rtb_Credits, "https://github.com/DaWolf85/GTAVAutoPatcher/", "open-sourcing his Tool", "Shoutout to @DaWolf85 for ", " the Community used to Upgrade / Downgrade previously.It helped us a ton.");

			AddParagraph(rtb_Credits, "Shoutout to @burhac, @Crapideot, @GearsOfW, @rollschuh2282 , @Ollie, @Alfie, @AntherXx for being awesome members of the GTA Speedrunning community, always being nice and respectful, and providing Help / Testing. You guys are much appreciated.");

			AddParagraph(rtb_Credits, "");
		}

		private void SetUpAbout()
		{
			AddParagraph(rtb_About, "");

			rtb_About.Document.Blocks.Remove(rtb_About.Document.Blocks.FirstBlock);

			AddParagraph(rtb_About, "Project 1.27 Version: '" + Globals.ProjectVersion + "'\n" +
"BuildInfo: '" + Globals.BuildInfo + "'\n" +
"ZIP Version: '" + Globals.ZipVersion + "'");


			AddParagraph(rtb_About, "You are running Project 1.27, a tool for the GTA V Speedrunning Community. This was created for the patch 1.27 downgrade problem, which started in August of 2020. This tool has a number of features, including Downgrading, Upgrading and launching the game.");

			AddParagraph(rtb_About, "If you have any issues with this program or ideas for new features,\n" +
"feel free to contact me on Discord: @thS#0305");

			AddHyperlinkText(rtb_About, "https://www.mind.org.uk/donate", "Charity", "If you want to support us, we encourage you to donate to a ", " of your chosing.");

			AddParagraph(rtb_About, "");
		}

		private void SetUpHelp()
		{
			AddParagraph(rtb_Help, "");

			rtb_Help.Document.Blocks.Remove(rtb_Help.Document.Blocks.FirstBlock);

			AddHyperlinkText(rtb_Help, "http://BigZip.com", "here", "When Project 1.27 crashes when Downloading or Importing Files, try to download the ZIP manually from ", ", then go to Settings -> Import ZIP Manually and select the file you just downloaded. If that doesnt work, rightclick the ZIP Extraction Path in Settings, copy your downloaded zip file there, right click -> extract here.");

			AddParagraph(rtb_Help, "When Launching GTA V does not launch the Version it says it is (Text in Top Left Corner), make sure the Path to GTA V is set correctly in the settings of Project 1.27.");

			AddParagraph(rtb_Help, "When the Auth / Login appears to load infinitely, please try to re-start Project 1.27, and wait a few  minutes. If its still not working, Rockstar just might not like your IP. In this case try using a Hotspot from your phone or a VPN or any other internet connection.");

			AddParagraph(rtb_Help, "When P127 crashes just when you are expected to login (on click of Auth Button, or on Game Launch when not logged in already), you might fail to connect to Rockstar Server. Make sure you are connected to the internet.");

			AddParagraph(rtb_Help, "When Upgrading / Downgrading does not work as expected in general, verify Game Files via Steam / Rockstar / Epic (Or re-download GTAV) and click \"Repair GTAV\" inside Settings.");

			AddParagraph(rtb_Help, "When Project 1.27 crashes when you are trying to authenticate (when pressing the Auth Button or when Pressing \"Launch GTAV\" while not being authenticated) Project 1.27 cannot reach Rockstar Servers.");

			AddHyperlinkText(rtb_Help, "http://ResetAll.com", "\"Reset All\"", "If something is still not working, you can always try verifying Files via Steam / Rockstar / Epic and hitting the ", " Button below.This might take a few minutes, and Project 1.27 will quit automatically when its done. Re - Open it and everything should work again.");

			AddParagraph(rtb_Help, "If you still cant get it to work or you wish to contact me, please RIGHT-click the Auth icon (the one with the lock icon in the top left corner) and send me the AAA - Logfile.log and the AAA - Debugfile.txt from the folder which will open (Project 1.27 Installation Directory) and include a detailed Report of what you did and whats not working.");

			AddParagraph(rtb_Help, "Working on Project 1.27 has been incredibly satisfying but also incredibly frustrating. I am glad we were able to give the GTA V Speedrunning Community a permanent solution for their problem and I cheerish the experience I have gained from doing this Project. Shoutouts to everyone involved in Project 1.27 from Day 1. Working with all of you has been a great pleasure.");

			AddParagraph(rtb_Help, "I hope everything works for you and you dont experience any crashes or anything like that. In case you do, i sincerly apoligze for the inconvenience. Feel free to contact me for help :) Discord: @thS#0305.");

			AddParagraph(rtb_Help, "I hope whoever reads this has a great day : )");

			AddParagraph(rtb_Help, "");
		}

		private void AddParagraph(RichTextBox rtb, string Paragraph)
		{
			Paragraph para = new Paragraph();
			para.Margin = new Thickness(10);
			para.Inlines.Add(Paragraph);
			para.TextAlignment = TextAlignment.Center;
			rtb.Document.Blocks.Add(para);
		}

		private void AddStrikethroughParagraph(RichTextBox rtb, string textBefore, string textStrike,
			  string textAfter)
		{
			Paragraph para = new Paragraph();
			para.Margin = new Thickness(10); // remove indent between paragraphs

			Run run1 = new Run(textStrike);
			run1.TextDecorations = TextDecorations.Strikethrough;

			para.Inlines.Add(textBefore);
			para.Inlines.Add(run1);
			para.Inlines.Add(textAfter);

			para.TextAlignment = TextAlignment.Center;

			rtb.Document.Blocks.Add(para);
		}

		private void AddHyperlinkText(RichTextBox rtb, string linkURL, string linkName,
			  string TextBeforeLink, string TextAfterLink)
		{
			Paragraph para = new Paragraph();
			para.Margin = new Thickness(10); // remove indent between paragraphs

			Hyperlink link = new Hyperlink();
			link.IsEnabled = true;
			link.Inlines.Add(linkName);
			link.NavigateUri = new Uri(linkURL);
			link.RequestNavigate += Hyperlink_RequestNavigate;

			para.Inlines.Add(TextBeforeLink);
			para.Inlines.Add(link);
			para.Inlines.Add(new Run(TextAfterLink));

			para.TextAlignment = TextAlignment.Center;

			rtb.Document.Blocks.Add(para);
		}


		private void btn_SpeedRun_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.SpeedRun;
		}

		private void btn_About_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.About;
		}

		private void btn_Credits_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.Credits;
		}


		private void btn_Help_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.Help;
		}


		private void btn_About_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 3)
			{
				new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOk, "Shoutouts to @thedosei for being cool\nYou do matter, dont let someone tell you different\nAlso your cat picture is cute").ShowDialog();
			}
		}


		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			if (e.Uri.ToString().ToLower().Contains("bigzip"))
			{
				TryBigZip();
			}
			else if (e.Uri.ToString().ToLower().Contains("resetall"))
			{
				MySettings.Settings.ResetEverything();
			}
			else
			{
				Process.Start(e.Uri.ToString());
			}
			e.Handled = true;
		}

		private void TryBigZip()
		{
			string TMP_UpdateXML = Globals.XML_AutoUpdate;

			string BigZipDownloadLink = HelperClasses.FileHandling.GetXMLTagContent(TMP_UpdateXML, "zip");
			BigZipDownloadLink = Globals.GetDDL(BigZipDownloadLink);

			Popups.Popup yesno = new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupYesNo, "Do you want Project 1.27 to automatically handle the Download and Import?\n\nIf this keeps crashing / not working, feel free to press NO and try to do it manually.");
			yesno.ShowDialog();
			if (yesno.DialogResult == true)
			{
				string hashNeeded = HelperClasses.FileHandling.GetXMLTagContent(TMP_UpdateXML, "bigzipmd5");

				HelperClasses.Logger.Log("DL Link: '" + BigZipDownloadLink + "'");
				HelperClasses.Logger.Log("HashNeeded: " + hashNeeded);

				// Deleting old ZIPFile
				HelperClasses.FileHandling.deleteFile(Globals.ZipFileDownloadLocation);

				// Downloading the ZIP File
				new Popups.PopupDownload(BigZipDownloadLink, Globals.ZipFileDownloadLocation, "ZIP-File").ShowDialog();

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
			else
			{
				new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOk, "This will open the Download Link in your browser.\nDownload it, open Settings, click \"Import ZIP manually\"\nand select the file you just downloaded.").ShowDialog();

				Process.Start(BigZipDownloadLink);
			}
		}
	}
}


