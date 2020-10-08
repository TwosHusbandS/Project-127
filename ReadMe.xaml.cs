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
using System.Windows.Shapes;

namespace Project_127
{
	/// <summary>
	/// Interaction logic for ReadMe.xaml
	/// </summary>
	public partial class ReadMe : Page
	{
		// /////////////////////////////////////////////////////////////
		// CLASS FOR PAGE READ_ME, CONTAINING INFO FOR: "SPEEDRUN"; "ABOUT"; "CREDITS"
		// WORKS BY TOGGLING VISIBILITY OF GRIDS FOR EACH POSSIBLE STATE
		// MOSTLY UNDOCUMENTED AND IM-PERFECT IMPLEMENTATIONS
		// WORKING POC THO
		// /////////////////////////////////////////////////////////////



		public static ReadMeStates LastReadMeState = ReadMeStates.About;

		public ReadMe()
		{
			InitializeComponent();

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

			string msg2 = "" +
				 "You are running Project 1.27, a tool for the GTA V Speedrunning Community.\n" +
				 "This was created for the patch 1.27 downgrade problem, which started in August of 2020\n" +
				 "This tool has a number of features, including Downgrading, Upgrading and launching the game,\n" +
				 "\nSpecial shoutouts to @dr490n who was responsible for getting the downgraded game\n" +
				 "to launch, added patches against in-game triggers, wrote the authentication backend,\n" +
				 "decryption and got the preorder entitlement to work.\n\n" +
				 "If you have any issues with this program or ideas for new features,\n" +
				 "feel free to contact me on Discord: @thS#0305\n\n" +
				 "Project 1.27 Version: '" + Globals.ProjectVersion + "', BuildInfo: '" + Globals.BuildInfo + "', ZIP Version: '" + Globals.ZipVersion + "'";
			Grid_About_Lbl.Content = msg2;

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
		}

		public enum ReadMeStates
		{
			Speedrun,
			About,
			Credits
		}

		private ReadMeStates _ReadMeState = ReadMeStates.About;

		public ReadMeStates ReadMeState
		{
			get
			{
				return _ReadMeState;
			}
			set
			{
				_ReadMeState = value;
				ReadMe.LastReadMeState = value;
				if (value == ReadMeStates.Speedrun)
				{
					Grid_SpeedRun.Visibility = Visibility.Visible;
					Grid_About.Visibility = Visibility.Hidden;
					Grid_Credits.Visibility = Visibility.Hidden;

					btn_SpeedRun.Background = Globals.MW_ButtonMOBackground;
					btn_SpeedRun.Foreground = Globals.MW_ButtonMOForeground;

					btn_About.Background = Globals.MW_ButtonBackground;
					btn_About.Foreground = Globals.MW_ButtonForeground;
					btn_Credits.Background = Globals.MW_ButtonBackground;
					btn_Credits.Foreground = Globals.MW_ButtonForeground;
				}
				else if (value == ReadMeStates.About)
				{
					Grid_About.Visibility = Visibility.Visible;
					Grid_SpeedRun.Visibility = Visibility.Hidden;
					Grid_Credits.Visibility = Visibility.Hidden;

					btn_About.Background = Globals.MW_ButtonMOBackground;
					btn_About.Foreground = Globals.MW_ButtonMOForeground;

					btn_SpeedRun.Background = Globals.MW_ButtonBackground;
					btn_SpeedRun.Foreground = Globals.MW_ButtonForeground;
					btn_Credits.Background = Globals.MW_ButtonBackground;
					btn_Credits.Foreground = Globals.MW_ButtonForeground;
				}
				else if (value == ReadMeStates.Credits)
				{
					Grid_Credits.Visibility = Visibility.Visible;
					Grid_SpeedRun.Visibility = Visibility.Hidden;
					Grid_About.Visibility = Visibility.Hidden;

					btn_Credits.Background = Globals.MW_ButtonMOBackground;
					btn_Credits.Foreground = Globals.MW_ButtonMOForeground;

					btn_About.Background = Globals.MW_ButtonBackground;
					btn_About.Foreground = Globals.MW_ButtonForeground;
					btn_SpeedRun.Background = Globals.MW_ButtonBackground;
					btn_SpeedRun.Foreground = Globals.MW_ButtonForeground;
				}
				else
				{
					// This should never happen
				}
			}
		}




		private void btn_SpeedRun_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.Speedrun;
		}

		private void btn_About_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.About;
		}

		private void btn_Credits_Click(object sender, RoutedEventArgs e)
		{
			ReadMeState = ReadMeStates.Credits;
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			Button myBtn = (Button)sender;
			switch (myBtn.Name)
			{
				case "btn_Speedrun":
					if (ReadMeState == ReadMeStates.Speedrun)
					{
						myBtn.Background = Globals.MW_ButtonBackground;
						myBtn.Foreground = Globals.MW_ButtonForeground;
					}
					else
					{
						myBtn.Background = Globals.MW_ButtonMOBackground;
						myBtn.Foreground = Globals.MW_ButtonMOForeground;
					}
					break;
				case "btn_About":
					if (ReadMeState == ReadMeStates.About)
					{
						myBtn.Background = Globals.MW_ButtonBackground;
						myBtn.Foreground = Globals.MW_ButtonForeground;
					}
					else
					{
						myBtn.Background = Globals.MW_ButtonMOBackground;
						myBtn.Foreground = Globals.MW_ButtonMOForeground;
					}
					break;
				case "btn_Credits":
					if (ReadMeState == ReadMeStates.Credits)
					{
						myBtn.Background = Globals.MW_ButtonBackground;
						myBtn.Foreground = Globals.MW_ButtonForeground;
					}
					else
					{
						myBtn.Background = Globals.MW_ButtonMOBackground;
						myBtn.Foreground = Globals.MW_ButtonMOForeground;
					}
					break;
			}
		}

		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			Button myBtn = (Button)sender;
			switch (myBtn.Name)
			{
				case "btn_Speedrun":
					if (ReadMeState == ReadMeStates.Speedrun)
					{
						myBtn.Background = Globals.MW_ButtonMOBackground;
						myBtn.Foreground = Globals.MW_ButtonMOForeground;
					}
					else
					{
						myBtn.Background = Globals.MW_ButtonBackground;
						myBtn.Foreground = Globals.MW_ButtonForeground;
					}
					break;
				case "btn_About":
					if (ReadMeState == ReadMeStates.About)
					{
						myBtn.Background = Globals.MW_ButtonMOBackground;
						myBtn.Foreground = Globals.MW_ButtonMOForeground;
					}
					else
					{
						myBtn.Background = Globals.MW_ButtonBackground;
						myBtn.Foreground = Globals.MW_ButtonForeground;
					}
					break;
				case "btn_Credits":
					if (ReadMeState == ReadMeStates.Credits)
					{
						myBtn.Background = Globals.MW_ButtonMOBackground;
						myBtn.Foreground = Globals.MW_ButtonMOForeground;
					}
					else
					{
						myBtn.Background = Globals.MW_ButtonBackground;
						myBtn.Foreground = Globals.MW_ButtonForeground;
					}
					break;
			}
		}

		private void UpdateText()
		{
			
		}


		private void Grid_SpeedRun_Btn_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(@"https://www.speedrun.com/Why/Has/Nobody/Written/A/Text/For/This/The/Community/Wanted/This/Feature/FFS");
		}

		private void Grid_About_Btn_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Grid_Credits_Btn_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}


