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

			string msg2 = "" +
				 "You are running Project 1.27, a tool for the GTA V Speedrunning\n" +
				 "Community. This was created for the patch 1.27 downgrade problem,\n" +
				 "which started in August of 2020. This tool has a number of features,\n" +
				 "including Downgrading, Upgrading and launching the game.\n" +
				 "\nSpecial shoutouts to @dr490n who was responsible for getting the\n" +
				 "downgraded game to launch, adding patches against in-game triggers,\n" +
				 "writing the authentication backend, decryption and managed to get\n" +
				 "the preorder entitlement to work.\n\n" +
				 "If you have any issues with this program or ideas for new features,\n" +
				 "feel free to contact me on Discord: @thS#0305\n\n" +
				 "Project 1.27 Version: '" + Globals.ProjectVersion + "'\n" +
				 "BuildInfo: '" + Globals.BuildInfo + "'\n" +
				 "ZIP Version: '" + Globals.ZipVersion + "'";
			Grid_About_Lbl.Content = msg2;
			Grid_About_Lbl.FontSize = 14;

			string msg3 = "" +
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
				"to understand and reverse engineer the GTA V Launch Process";
			Grid_Credits_Lbl.Content = msg3;
			Grid_Credits_Lbl.FontSize = 14;

			string msg4 = "" +
		"BlaBlaBla\nCommonErrors\nBlablabla\nAnd how to fix them\nBlablabla\n\nIf you see this after installing or updating Project 1.27 we forgot to change this Text.\nUpsi.";

			// Change Error Popup to link here

			// Issues: Path, File Permissions, fucked up GTAV, Wrong version booting,installation state not changing

			//Grid_Help_Lbl.Content = msg4;
			//Grid_Help_Lbl.FontSize = 14;

			/*


			When Project 1.27 crashes when Downloading or Importing Files, try to download the ZIP manually from [here](), then go to Settings -> Import ZIP Manually and select the file you just downloaded. If that doesnt work, rightclick the ZIP Extraction Path in Settings, copy your downloaded zip file there, right click -> extract here.

When Launching GTA V does not launch the Version it says it is (Text in Top Left Corner), make sure the Path to GTA V is set correctly in the settings of Project 1.27

When the Auth / Login appears to load infinitely, please try to re-start Project 1.27, and wait a few  minutes. If its still not working, Rockstar just might not like your IP. In this case try using a Hotspot from your phone or a VPN or any other internet connection.

When P127 crashes just when you are expected to login (on click of Auth Button, or on Game Launch when not logged in already), you might fail to connect to Rockstar Server. Make sure you are connected to the internet.

When Upgrading / Downgrading does not work as expected in general, verify Game Files via Steam / Rockstar / Epic (Or re-download GTAV) and click "Repair GTAV" inside Settings.

If something is still not working, you can always try verifying Files via Steam / Rockstar / Epic and hitting the "Reset all" Button below. This might take a few minutes, and Project 1.27 will quit automatically when its done. Re-Open it and everything will work.

If you still cant get it to work or you wish to contact me, please RIGHT-click the Auth icon (the one with the lock icon in the top left corner) and send me the AAA - Logfile.log and the AAA - Debugfile.txt from the folder which will open (Project 1.27 Installation Directory) and include a detailed Report of what you did and whats not working.

Working on Project 1.27 has been incredibly satisfying but also incredibly frustrating. I am glad we were able to give the GTA V Speedrunning Community a permanent solution for their problem and I cheerish the experience I have gained from doing this Project. Shoutouts to everyone involved in Project 1.27 from Day 1. Working with all of you has been a great pleasure.

I hope everything works for you and you dont experience any crashes or anything like that. In case you do, i sincerly apoligze for the inconvenience. Feel free to contact me for help :) Discord: @thS#0305.

I hope whoever reads this has a great day : )

			*/
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

		private void Grid_About_Btn_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(@"https://www.mind.org.uk/donate");
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
			Process.Start("http://google.com");
			e.Handled = true;
		}
	}
}


