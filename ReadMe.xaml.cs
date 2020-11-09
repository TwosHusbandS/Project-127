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

			string msg1 = "" +
				"This text will contain Information about GTAV Speedrunning.\n" +
				"Paragraphs explaining the basics, rules, categories etc.\n" +
				"And some link to resources like the Leaderboard, Guides\n" +
				"Useful Programs, Maps, and whatever else is useful\n\n" +
				"I am not a speedrunner or very involved with the GTA V Community,\n" +
				"if you read this, and could shoot me a PM on Discord with stuff\n" +
				"you want to Read here, that would be great.";
			//Grid_SpeedRun_Lbl.Content = msg1;
			//Grid_SpeedRun_Lbl.FontSize = 18;





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
"@JakeMiester, @Antibones, @Aperture, @Diamondo25, @MOMO");

			AddParagraph(rtb_Credits, "Shoutout to FiveM and Goldberg, whose Source Code proved to be vital\n" +
"to understand and reverse engineer the GTA V Launch Process");

			AddParagraph(rtb_Credits, "Shoutout to @Fro for providing Hosting to the Files needed for Project 1.27");

			AddParagraph(rtb_Credits, "Shoutout to @yoshi for providing the Information which Build Version corresponds with which Game Version");

			AddHyperlinkText(rtb_Credits, "https://github.com/DaWolf85/GTAVAutoPatcher/", "open-sourcing his Tool", "Shoutout to @DaWolf85 for ", "the Community used to Upgrade / Downgrade previously.It helped us a ton.");

			AddParagraph(rtb_Credits, "Shoutout to @burhac, @Crapideot, @GearsOfW, @rollschuh2282 , @Ollie, @Alfie, @redwolf and @Reloe for being awesome members of the GTA Speedrunning community, always being nice and respectful, and providing Help / Testing. You guys are much appreciated.");

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

			AddParagraph(rtb_About, "Special shoutouts to @dr490n who was responsible for getting the downgraded game to launch, adding patches against in-game triggers, writing the Overlay Backend, authentication backend, decryption and managed to get the preorder entitlement to work.");

			AddParagraph(rtb_About, "If you have any issues with this program or ideas for new features,\n" +
"feel free to contact me on Discord: @thS#0305");

			AddHyperlinkText(rtb_About, "https://www.mind.org.uk/donate", "Charity", "If you want to support us, we encourage you to donate to a ", " of your chosing.");

			AddParagraph(rtb_About, "");
		}

		private void SetUpHelp()
		{
			AddParagraph(rtb_Help, "");

			rtb_Help.Document.Blocks.Remove(rtb_Help.Document.Blocks.FirstBlock);

			AddParagraph(rtb_Help, "When Project 1.27 crashes when Downloading or Importing Files, try to download the ZIP manually from [here](), then go to Settings -> Import ZIP Manually and select the file you just downloaded. If that doesnt work, rightclick the ZIP Extraction Path in Settings, copy your downloaded zip file there, right click -> extract here.");

			AddParagraph(rtb_Help, "When Launching GTA V does not launch the Version it says it is (Text in Top Left Corner), make sure the Path to GTA V is set correctly in the settings of Project 1.27.");

			AddParagraph(rtb_Help, "When the Auth / Login appears to load infinitely, please try to re-start Project 1.27, and wait a few  minutes. If its still not working, Rockstar just might not like your IP. In this case try using a Hotspot from your phone or a VPN or any other internet connection.");

			AddParagraph(rtb_Help, "When P127 crashes just when you are expected to login (on click of Auth Button, or on Game Launch when not logged in already), you might fail to connect to Rockstar Server. Make sure you are connected to the internet.");

			AddParagraph(rtb_Help, "When Upgrading / Downgrading does not work as expected in general, verify Game Files via Steam / Rockstar / Epic (Or re-download GTAV) and click \"Repair GTAV\" inside Settings.");

			AddParagraph(rtb_Help, "When Project 1.27 crashes when you are trying to authenticate (when pressing the Auth Button or when Pressing \"Launch GTAV\" while not being authenticated) Project 1.27 cannot reach Rockstar Servers.");

			AddParagraph(rtb_Help, "If something is still not working, you can always try verifying Files via Steam / Rockstar / Epic and hitting the \"Reset all\" Button below. This might take a few minutes, and Project 1.27 will quit automatically when its done. Re-Open it and everything will work.");

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


		private void Grid_SpeedRun_Btn_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(@"https://www.speedrun.com/WhyHasNobodyWrittenATextForThisTheCommunityWantedThisFeatureFFS");
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

		private void btn_SpeedRun_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 3)
			{
				new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOk, "Shoutouts to @crapideot for being awesome and a\ngreat friend and Helper during Project 1.27 :)\nHope you have a great day buddy").ShowDialog();
			}
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(e.Uri.ToString());
			e.Handled = true;
		}
	}
}


