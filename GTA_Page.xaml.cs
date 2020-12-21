using System;
using System.Collections.Generic;
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
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Diagnostics;

namespace Project_127
{
	/// <summary>
	/// Interaction logic for GTA_Page.xaml
	/// </summary>
	public partial class GTA_Page : Page
	{
		/// <summary>
		/// Static Property of Control of Instance.
		/// </summary>
		public static Button btn_GTA_static = new Button();

		/// <summary>
		/// Constructor of Main Page. Will be loaded when nothing else is loaded
		/// </summary>
		public GTA_Page()
		{
			InitializeComponent();
			btn_GTA_static = btn_GTA;

			if (LauncherLogic.GameState == LauncherLogic.GameStates.Running)
			{
				GTA_Page.btn_GTA_static.BorderBrush = MyColors.MW_ButtonGTAGameRunningBorderBrush;
				GTA_Page.btn_GTA_static.Content = "Exit GTA V";
			}
			else
			{
				GTA_Page.btn_GTA_static.BorderBrush = MyColors.MW_ButtonGTAGameNotRunningBorderBrush;
				GTA_Page.btn_GTA_static.Content = "Launch GTA V";
			}
		}



		/// <summary>
		/// Method which gets called when the Launch GTA Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.btn_GTA_Click_Static();
		}


		/// <summary>
		/// Method which gets called when the Launch GTA Button is RIGHT - clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_GTA_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			MainWindow.btn_GTA_MouseRightButtonDown_Static();
		}

	
	}
}
