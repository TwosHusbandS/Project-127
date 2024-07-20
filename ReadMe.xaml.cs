using GSF.Units;
using Project_127.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

		public static bool HideVideo = true;
		public static string DragonsLink = "https://github.com/jaredtb";
		public static string AnthersDemoVideoLink = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&t=PLACEHOLDER";


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

		/// <summary>
		/// Method to set up the Speedrun Info Text
		/// </summary>
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

		/// <summary>
		/// Method to set up the Credits Text
		/// </summary>
		private void SetUpCredits()
		{
			AddParagraph(rtb_Credits, "");

			rtb_Credits.Document.Blocks.Remove(rtb_Credits.Document.Blocks.FirstBlock);

			AddParagraph(rtb_Credits, "Solving the patch 1.27 Downgrade problem has been achieved by a month of hard work by a number of dedicated individuals. This would not have been possible without the time and effort of a number of very talented individuals from all walks of life, who have contributed skills in Reverse Engineering, Programming, Decryption, Project Management, Scripting and Testing. Below is a list of SOME of the main contributors to the project, although our thanks go out to EVERYONE who has helped throughout the process.");

			AddParagraph(rtb_Credits, "Reverse Engineering:\n" +
"@dr490n, @Special For, @zCri, @Gogsi");

			AddParagraph(rtb_Credits, "Launcher / Client Programming, Documenting:\n" +
"@thS");

			AddParagraph(rtb_Credits, "Launcher GUI Design & Artwork:\n" +
"@hossel");

			AddParagraph(rtb_Credits, "Special thanks to:\n" +
"@JakeMiester, @Antibones, @Aperture, @MOMO");


			AddParagraph(rtb_Credits, "GOGSI THE GOAT. Very special shoutouts to @Gogsi not only for being awesome and a pleasure to work with but also for continuing to reverse-engineer, help with information, provide fixes and PRs. Very much appreciated.");

			AddParagraph(rtb_Credits, "Special shoutouts to @dr490n, who was responsible for getting the downgraded game to launch, adding patches against in-game triggers, writing the Overlay Backend, downloadmanager backend, authentication backend, decryption and managed to get the preorder entitlement to work.");

			AddParagraph(rtb_Credits, "Special shoutouts to @special for, for being there during the whole development phase, providing valuable insight, being available to bounce ideas off of, being available for brainstorming, and providing valuable help in regards to reverse engineering the GTA V Launch Process. We could not have done this without you.");

			AddParagraph(rtb_Credits, "Also thanks to @special for, for implementing the ability to downgrade GTA to Version 1.24 via the DragonEmu into P127 by figuring out how P127 GUI and backend logic works and how my component manager and dr490ns download manager handles things.");

			AddParagraph(rtb_Credits, "Shoutout to FiveM and Goldberg, whose Source Code proved to be vital\n" +
"to understand and reverse engineer the GTA V Launch Process");

			AddParagraph(rtb_Credits, "Shoutout to @Fro for providing Hosting to the Files needed for Project 1.27");

			AddParagraph(rtb_Credits, "Shoutout to @yoshi for providing the Information which Build Version corresponds with which Game Version and Testing our Social-Club Launch");

			AddParagraph(rtb_Credits, "Shoutout to @Diamondo25 for finding a way to launch GTA without going through LaunchGTAV.exe or PlayGTAV.exe, which eased the development process to launch through Social Club");

			// AddParagraph(rtb_Credits, "Shoutout to @AntherXx for making the incredible P127 Demo + Help Video, as well as going through the trouble of tracking down and renaming SaveFiles for every single Mission in Classic% and to @Hossel for providing the SaveFiles for the main Categories");
            
			AddStrikethroughParagraph(rtb_Credits, "Shoutout to @AntherXx for ", "making the incredible P127 Demo & Help Video, as well as", " going through the trouble of tracking down and renaming SaveFiles for every single Mission in Classic % and to @Hossel for providing the SaveFiles for the main Categories");


            if (!HideVideo)
                        {
				AddHyperlinkText(rtb_Credits, AnthersDemoVideoLink, "P127 Demo & Help Video", "Shoutout to @AntherXx for making the incredible ", ", as well as going through the trouble of tracking down and renaming SaveFiles for every single Mission in Classic% and to @Hossel for providing the SaveFiles for the main Categories.");
			}

			AddHyperlinkText(rtb_Credits, "https://github.com/DaWolf85/GTAVAutoPatcher/", "open-sourcing his Tool", "Shoutout to @DaWolf85 for ", " the Community used to Upgrade / Downgrade previously. It helped us a ton.");

			AddParagraph(rtb_Credits, "Shoutout to @Hoxi and @Special For for implementing and integrating the ReturningPlayerBonus toggle on dr490n emu.");

			AddParagraph(rtb_Credits, "Shoutout to @cuzbabytonight on community.pcgamingwiki.com (and hoxi I think) for figuring out which DLC Folders are safe to delete when on 1.27");

			AddParagraph(rtb_Credits, "Shoutout to @Rayope for finding a Bug, fixing it himself, and providing us with the solution.Thanks mate. (P127 crashing on Token Gen when losing internet after P127 Startup)");

			AddParagraph(rtb_Credits, "Shoutout to @burhac, @Crapideot, @rollschuh2282, @GearsOfW, @Ollie, @Alfie, and the aforementioned people for being awesome members of the GTA Speedrunning community, always being nice and respectful, and providing Help / Testing. You guys are much appreciated.");

			AddParagraph(rtb_Credits, "");
		}

		/// <summary>
		/// Message about the "About" Text
		/// </summary>
		private void SetUpAbout()
		{
			AddParagraph(rtb_About, "");

			rtb_About.Document.Blocks.Remove(rtb_About.Document.Blocks.FirstBlock);

			AddParagraph(rtb_About, "Project 1.27 Version: '" + Globals.ProjectVersion + "'\n" + "BuildInfo: '" + Globals.BuildInfo + "'\n" + "ZIP Version: '" + Globals.ZipVersion + "'");

			if (!HideVideo)
			{
				AddHyperlinkText(rtb_About, AnthersDemoVideoLink, "P127 Demo & Help Video", "", "");
			}

			AddParagraph(rtb_About, "You are running Project 1.27, a tool for the GTA V Speedrunning Community. This was created for the patch 1.27 downgrade problem, which started in August of 2020. This tool has a number of features, including Downgrading, Upgrading and launching the game.");

			AddParagraph(rtb_About, "This whole Project would not have been possible without the hard work of @dr490n, @Special For, @thS who worked on this for months in their free time, going above and beyond what was required.");

			AddHyperlinkText(rtb_About, "https://www.mind.org.uk/donate", "Charity", "If you want to support us, we encourage you to donate to a ", " of your chosing.");

			AddHyperlinkText(rtb_About, DragonsLink, "here", "@dr490n, who did all of the game-launch related work which made P127 possible, uploaded his work, sourcecode and documented his findings on his Github ", ".");

			AddHyperlinkText(rtb_About, "https://github.com/TwosHusbandS/Project-127/blob/master/README.md", "here", "A more lengthy and detailed version of this can be found on github ", "");

			AddParagraph(rtb_About, "If you are having trouble with Project 1.27 or are running into any issues, please read the \"Help-Section\" and visit the GTA V Speedrun Discord and post it in the \"Project-1-27-Chat\" Channel.");

			AddHyperlinkText(rtb_About, "https://discordapp.com/users/612259615291342861", "Discord (@ths_was_taken)", "For anything regarding this client, feel free to contact me on ", "");

			AddParagraph(rtb_About, "");

		}

		/// <summary>
		/// Setting up the "Help" Text
		/// </summary>
		private void SetUpHelp()
		{
			AddParagraph(rtb_Help, "");

			rtb_Help.Document.Blocks.Remove(rtb_Help.Document.Blocks.FirstBlock);

			if (!HideVideo)
			{
				AddHyperlinkText(rtb_Help, AnthersDemoVideoLink, "P127 Demo & Help Video", "", "");
			}

			// AddHyperlinkText(rtb_Help, "http://BigZip.com", "here", "When Project 1.27 crashes when Downloading or Importing Files, try to download the ZIP manually from ", ", then go to Settings -> Import ZIP Manually and select the file you just downloaded. If that doesnt work, rightclick the ZIP Extraction Path in Settings, copy your downloaded zip file there, right click -> extract here.");

			AddParagraph(rtb_Help, "When Launching GTA V does not launch the Version it says it is (Text in Top Left Corner), make sure the Path to GTA V is set correctly in the settings of Project 1.27.");

			AddHyperlinkText(rtb_Help, "http://RepairGTA.com", "\"Repair GTA\"", "When Upgrading / Downgrading does not work as expected in general, clicking ", " inside P127 Generel Settings and re-install the components.");

			AddParagraph(rtb_Help, "Re - Installing or manually deleting and Installing a Component, as well as re - doing an Upgrade / Downgrade is always worth a try.");

			AddParagraph(rtb_Help, "Game Updates from Rockstar might break things.This usually shows itself as P127 saying: \"Downgraded(1.52)\", which makes no sense. Re - Installing the Components inside the ComponentManager you are using for Downgrading and Re - Applying a Downgrade should fix this. Steam / Epic is not affected by this.P127 should automatically detect and handle this.");

			AddParagraph(rtb_Help, "If GTA (mainly Downgraded) does not launch AT ALL, UNCHECK \"Overwrite GTA CommandLineArgs\" inside \"Additional GTA Commandline Options\" inside \"GTA & Launch Settings\".");

			AddParagraph(rtb_Help, "If one Launch - Method(for downgraded GTA) is not working for you, it is worth trying to switch the way P127 Launches the downgraded Game, by doing so inside \"GTA & Launch Settings\".");

			AddParagraph(rtb_Help, "If the current Authentication Method for the dr490n emu is not working, it is worth trying to switch the way P127 tries to authenticate your GTA ownership by doing so inside the Orange Border inside \"GTA & Launch Settings\".");

			AddParagraph(rtb_Help, "If the Dr490n Launcher doesnt seem to take some options(InGameName, PreOrderBonus) into account, try launching P127 with the command line args: \"-useemudebugfile true\"");

			AddParagraph(rtb_Help, "On Legacy Auth: When the Auth / Login appears to load infinitely, re - start the auth - process by hitting the \"X\" inside the top right corner once, and then trying again.If that doesnt work, you can try to re-start Project 1.27, and wait a few  minutes.If its still not working, Rockstar just might not like your IP.In this case try using a Hotspot from your phone or a VPN or any other internet connection.");

			AddParagraph(rtb_Help, "On Legacy Auth: When P127 crashes just when you are expected to login(on click of Auth Button, or on Game Launch when not logged in already), you might fail to connect to Rockstar Server. Make sure you are connected to the internet.");

			AddParagraph(rtb_Help, "On MTL Auth, if Rockstar Games Launcher opens and immediately tries to update your Game, you need to disable that option.Inside Rockstar Games Launcher, head into Settings -> My Installed Games->Grand Theft Auto V->and uncheck the \"Enable automatic updates\" box at the very top.");

			AddParagraph(rtb_Help, "You can reset the information which Components are installed and in which Version by tripple right - clicking the \"Refresh\" Button inside the ComponenetManager.");

			AddParagraph(rtb_Help, "If you cant see the MultiMonitor Overlay, RIGHT-click the \"Enable Multi Monitor Mode\" Checkbox to Reset its Position.");

			AddParagraph(rtb_Help, "If launching through Social Club does not work, and you believe the Social Club Downgrade to be the cause of this, you can try the following: Head into P127 Settings, inside  \"GTA & Launch Settings\", enable \"Force SocialClub Installation Path C:\"");

			AddParagraph(rtb_Help, "For external HDDs and NAS / SAN / Network Storage I recommend having \"Slow but stable Method for Upgrading / Downgrading\" checked.");

			AddHyperlinkText(rtb_Help, "http://ResetAll.com", "\"RESET ALL\"", "If something is still not working, you can always try hitting the ", "Button inside P127 Settings.This might take a few minutes, and Project 1.27 will quit automatically when its done.Re - Open it and everything should work again.You will need to verifying Files via Steam / Rockstar / Epic afterwards.");

			AddHyperlinkText(rtb_Help, "http://RightClickAuthIcon.com", "RIGHT-click the Auth icon", "If you still cant get it to work or you wish to get Help, please ", " (the one with the lock icon in the top left corner) and send me the AAA - Logfile.log and the AAA - Debugfile.txt from the folder which will open (Project 1.27 Installation Directory) and include a detailed Report of what you did and whats not working. Note: These Files may contain Filepaths which may contain your Name.");

			AddHyperlinkText(rtb_Help, "http://SpeedrunReadMe.com", "\"Speedrun\"", "Please post a detailed description of what you did and what is not working as expected, in the GTA V Speedrun Discord - tech-support - chat Channel. Invite links are in the ", " Section of this Page.");

			AddHyperlinkText(rtb_Help, "https://github.com/TwosHusbandS/Project-127/blob/master/Installer/Info/Help.md", "here", "A more lengthy and detailed version of this can be found on github ", "");

			AddParagraph(rtb_Help, "I do not recommend uploading these 2 Files inside a public channel. They may contain things like your email adress, and Path / Folder - Names which can contain usernames.");

			AddParagraph(rtb_Help, "I hope everything works for you and you dont experience any crashes or anything like that.");

			AddHyperlinkText(rtb_Help, "https://github.com/TwosHusbandS/Project-127/blob/master/Installer/Info/Help.md#help--common-issues", "here", "A more lengthy and detailed version of this help-page can be found on github ", "");

			AddParagraph(rtb_Help, "I hope whoever reads this has a great day : )");

			AddParagraph(rtb_Help, "");

		}

		/// <summary>
		/// Adding a Paragraph to a RichTextBox
		/// </summary>
		/// <param name="rtb"></param>
		/// <param name="Paragraph"></param>
		private void AddParagraph(RichTextBox rtb, string Paragraph)
		{
			Paragraph para = new Paragraph();
			para.Margin = new Thickness(10);
			para.Inlines.Add(Paragraph);
			para.TextAlignment = TextAlignment.Center;
			rtb.Document.Blocks.Add(para);
		}

		/// <summary>
		/// Adding StrikeThrough Paragraph to a RichTextBox
		/// </summary>
		/// <param name="rtb"></param>
		/// <param name="textBefore"></param>
		/// <param name="textStrike"></param>
		/// <param name="textAfter"></param>
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

		/// <summary>
		/// Adding a Paragraph with a Hyperlink to a RichTextBox
		/// </summary>
		/// <param name="rtb"></param>
		/// <param name="linkURL"></param>
		/// <param name="linkName"></param>
		/// <param name="TextBeforeLink"></param>
		/// <param name="TextAfterLink"></param>
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


		/// <summary>
		/// Loading the Speedrun "Page"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_SpeedRun_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.SpeedRun;
		}

		/// <summary>
		/// Loading the About "Page"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void btn_About_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.About;
		}
		/// <summary>
		/// Loading the Credits "Page"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void btn_Credits_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.Credits;
		}

		/// <summary>
		/// Loading the Help "Page"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Help_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.Help;
		}

		/// <summary>
		/// Special Shoutouts to a special someone
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_About_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 3)
			{
                PopupWrapper.PopupOk("Shoutouts to @thedosei for being cool\nYou do matter, dont let someone tell you different\nAlso your cat picture is cute");
			}
		}

		/// <summary>
		/// Some Hyperlink logic we need
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			if (e.Uri.ToString().ToLower().Contains("repairgta"))
			{
				MySettings.Settings.RepairGTA_UserInteraction();
			}
			else if (e.Uri.ToString().ToLower().Contains("resetall"))
			{
				MySettings.Settings.ResetEverything();
			}
			else if (e.Uri.ToString().ToLower().Contains("rightclickauthicon"))
			{
				HelperClasses.Logger.GenerateDebug();
			}
			else if (e.Uri.ToString().ToLower().Contains("speedrunreadme"))
			{
				ReadMeState = ReadMeStates.SpeedRun;
			}
			else
			{
				Process.Start(e.Uri.ToString());
			}
			e.Handled = true;
		}

	}
}


