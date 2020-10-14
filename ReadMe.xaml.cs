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
				"This Popup will contain Information about GTAV Speedrunning.\n" +
				"Paragraphs explaining the basics, rules, categories etc.\n" +
				"And some link to resources like the Leaderboard, Guides\n" +
				"Useful Programs, Maps, and whatever else is useful\n\n" +
				"I am not a speedrunner or very involved with the GTA V Community,\n" +
				"if you read this, and could shoot me a PM on Discord with stuff\n" +
				"you want to Read here, that would be great.";
			Grid_SpeedRun_Lbl.Content = msg1;
			Grid_SpeedRun_Lbl.FontSize = 16;

			string msg2 = "" +
				 "You are running Project 1.27, a tool for the GTA V Speedrunning\n" +
				 "Community. This was created for the patch 1.27 downgrade problem,\n" +
				 "which started in August of 2020. This tool has a number of features,\n" +
				 "including Downgrading, Upgrading and launching the game.\n" +
				 "\nSpecial shoutouts to @dr490n who was responsible for getting the downgraded game\n" +
				 "to launch, adding patches against in-game triggers, the authentication backend,\n" +
				 "decryption and got the preorder entitlement to work.\n\n" +
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
			Grid_Credits_Lbl.FontSize = 12;
		}

		/// <summary>
		/// Enum for all ReadMeStates
		/// </summary>
		public enum ReadMeStates
		{
			SpeedRun,
			About,
			Credits
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
					Grid_SpeedRun.Visibility = Visibility.Visible;
					btn_SpeedRun.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

					Grid_About.Visibility = Visibility.Hidden;
					btn_About.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Grid_Credits.Visibility = Visibility.Hidden;
					btn_Credits.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
				}
				else if (value == ReadMeStates.About)
				{
					Grid_About.Visibility = Visibility.Visible;
					btn_About.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

					Grid_SpeedRun.Visibility = Visibility.Hidden;
					btn_SpeedRun.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Grid_Credits.Visibility = Visibility.Hidden;
					btn_Credits.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;
				}
				else if (value == ReadMeStates.Credits)
				{
					Grid_Credits.Visibility = Visibility.Visible;
					btn_Credits.Style = Application.Current.FindResource("btn_hamburgeritem_selected") as Style;

					Grid_SpeedRun.Visibility = Visibility.Hidden;
					btn_SpeedRun.Style = Application.Current.FindResource("btn_hamburgeritem") as Style;

					Grid_About.Visibility = Visibility.Hidden;
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



	}
}


