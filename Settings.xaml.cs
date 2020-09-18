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
using System.Windows.Shapes;

namespace Project_127
{
    /// <summary>
    /// Class Settings.xaml (Partical class is in SettingsPartial.cs)
    /// </summary>
    public partial class Settings : Window
    {
		/// <summary>
		/// Constructor of Settings Window 
		/// </summary>
		public Settings()
		{
			// Initializing all WPF Elements
			InitializeComponent();

			// Sets this for DataBinding
			this.DataContext = this;
		}


		// There are only Button Clicks and GUI Functions here. Some of the Functionality is in SettingsPartial.cs


		/// <summary>
		/// Button Click to change the Path of GTA V Installation Path
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_GTAVInstallationPath_Click(object sender, RoutedEventArgs e)
		{
			GTAVInstallationPath = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains your GTAV.exe", @"C:\");
			btn_Set_GTAVInstallationPath.Content = GTAVInstallationPath;
		}

		/// <summary>
		/// Button Click to change the Path of FileFolder which we use to use for SaveFiles etc.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_FileFolder_Click(object sender, RoutedEventArgs e)
		{
			FileFolder = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder which contains all sort of Files", @"C:\");
			btn_Set_FileFolder.Content = FileFolder;
		}

		/// <summary>
		/// Button Click to change the Path of LiveSplit Executable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_PathLiveSplit_Click(object sender, RoutedEventArgs e)
		{
			PathLiveSplit = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Select the .exe of the LiveSplit Program", @"C:\");
			btn_Set_FileFolder.Content = PathLiveSplit;
		}

		/// <summary>
		/// Button Click to change the Path of StreamProgram Executable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_PathStreamProgram_Click(object sender, RoutedEventArgs e)
		{
			PathStreamProgram = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Select the .exe of the Stream Program", @"C:\");
			btn_Set_FileFolder.Content = PathStreamProgram;
		}

		/// <summary>
		/// Button Click to change the Path of FPS Limiter Executable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_PathFPSLimiter_Click(object sender, RoutedEventArgs e)
		{
			PathFPSLimiter = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Select the .exe of the FPS Limit Program", @"C:\");
			btn_Set_FileFolder.Content = PathFPSLimiter;
		}

		/// <summary>
		/// Button Click to change the Key1 of the JumpScript
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_JumpScriptKey1_Click(object sender, RoutedEventArgs e)
		{
			// Needs to be implemented I guess
			(new Popup(Popup.PopupWindowTypes.PopupOk, "This does nothing so far")).ShowDialog();
		}

		/// <summary>
		/// Button Click to change the Key2 of the JumpScript
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_JumpScriptKey2_Click(object sender, RoutedEventArgs e)
		{
			// Needs to be implemented I guess
			(new Popup(Popup.PopupWindowTypes.PopupOk, "This does nothing so far")).ShowDialog();
		}

		/// <summary>
		/// Button Click to change the Path of Nohboard executable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_PathNohboard_Click(object sender, RoutedEventArgs e)
		{
			PathLiveSplit = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.File, "Select the .exe of the LiveSplit Program", @"C:\");
			btn_Set_FileFolder.Content = PathLiveSplit;
		}

		/// <summary>
		/// Button Click to select the Folder for the Theme
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Set_Theme_Click(object sender, RoutedEventArgs e)
		{
			(new Popup(Popup.PopupWindowTypes.PopupOk, "This does nothing so far")).ShowDialog();
			Theme = HelperClasses.FileHandling.OpenDialogExplorer(HelperClasses.FileHandling.PathDialogType.Folder, "Pick the Folder of your Theme", @"C:\");
			btn_Set_FileFolder.Content = Theme;
		}

		/// <summary>
		/// Button Click on Close.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Close_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}


		// Below are Methods we need to make the behaviour of this nice.


		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

	} // End of Class
} // End of Namespace
