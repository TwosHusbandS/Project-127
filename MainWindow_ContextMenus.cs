using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Project_127.MySettings;

namespace Project_127
{
	/// <summary>
	/// Partical Class for MainWindow. Contains all ContextMenu related stuff
	/// </summary>
	partial class MainWindow
	{

		// NotifyIcon (Tray Icon) below

		#region NotifyIcon (Tray Icon)

		/// <summary>
		/// Initiating the NotifyIcon (tray icon) with context menu
		/// </summary>
		private void InitNotifyIcon()
		{
			notifyIcon = new System.Windows.Forms.NotifyIcon();
			notifyIcon.Click += new EventHandler(notifyIcon_Click);
			notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
			notifyIcon.Visible = true;

			Uri resourceUri = new Uri(@"Artwork\icon.ico", UriKind.Relative);
			System.Windows.Forms.NotifyIcon icon = new System.Windows.Forms.NotifyIcon();
			using (Stream iconStream = Application.GetResourceStream(resourceUri).Stream)
			{
				icon.Icon = new System.Drawing.Icon(iconStream);
				notifyIcon.Icon = icon.Icon;
				iconStream.Dispose();
			}

			System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();

			System.Windows.Forms.MenuItem mi1 = new System.Windows.Forms.MenuItem();
			mi1.Text = "Show P127";
			mi1.Click += new System.EventHandler(this.menuItem_Show_Click);
			cm.MenuItems.Add(mi1);

			System.Windows.Forms.MenuItem mi2 = new System.Windows.Forms.MenuItem();
			mi2.Text = "Hide P127";
			mi2.Click += new System.EventHandler(this.menuItem_Hide_Click);
			cm.MenuItems.Add(mi2);

			cm.MenuItems.Add("-");

			System.Windows.Forms.MenuItem mi3 = new System.Windows.Forms.MenuItem();
			mi3.Text = "Upgrade";
			mi3.Click += new System.EventHandler(this.menuItem_Upgrade_Click);
			cm.MenuItems.Add(mi3);

			System.Windows.Forms.MenuItem mi4 = new System.Windows.Forms.MenuItem();
			mi4.Text = "Downgrade";
			mi4.Click += new System.EventHandler(this.menuItem_Downgrade_Click);
			cm.MenuItems.Add(mi4);

			System.Windows.Forms.MenuItem mi5 = new System.Windows.Forms.MenuItem();
			mi5.Text = "Launch Game";
			mi5.Click += new System.EventHandler(this.menuItem_LaunchGame_Click);
			cm.MenuItems.Add(mi5);

			cm.MenuItems.Add("-");

			System.Windows.Forms.MenuItem mi6 = new System.Windows.Forms.MenuItem();
			mi6.Text = "SaveFileHandler";
			mi6.Click += new System.EventHandler(this.menuItem_SaveFileHandler_Click);
			cm.MenuItems.Add(mi6);

			System.Windows.Forms.MenuItem mi7 = new System.Windows.Forms.MenuItem();
			mi7.Text = "NoteOverlay";
			mi7.Click += new System.EventHandler(this.menuItem_NoteOverlay_Click);
			cm.MenuItems.Add(mi7);

			System.Windows.Forms.MenuItem mi7B = new System.Windows.Forms.MenuItem();
			mi7B.Text = "ComponentManager";
			mi7B.Click += new System.EventHandler(this.menuItem_ComponentManager_Click);
			cm.MenuItems.Add(mi7B);

			System.Windows.Forms.MenuItem mi8 = new System.Windows.Forms.MenuItem();
			mi8.Text = "Settings";
			mi8.Click += new System.EventHandler(this.menuItem_Settings_Click);
			cm.MenuItems.Add(mi8);

			System.Windows.Forms.MenuItem mi9 = new System.Windows.Forms.MenuItem();
			mi9.Text = "Information";
			mi9.Click += new System.EventHandler(this.menuItem_Information_Click);
			cm.MenuItems.Add(mi9);

			cm.MenuItems.Add("-");

			System.Windows.Forms.MenuItem mi10 = new System.Windows.Forms.MenuItem();
			mi10.Text = "Close P127";
			mi10.Click += new System.EventHandler(this.menuItem_Close_Click);
			cm.MenuItems.Add(mi10);

			notifyIcon.ContextMenu = cm;

			if (Settings.StartWay == Settings.StartWays.Maximized)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
		}

		/// <summary>
		/// Double click on Tray Icon. Nothing happens.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Single click on Tray Icon. Toggles Visibility
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIcon_Click(object sender, EventArgs e)
		{
			if (this.Visibility == Visibility.Visible)
			{
				menuItem_Hide_Click(null, null);
			}
			else
			{
				menuItem_Show_Click(null, null);
			}
		}
		
		/// <summary>
		/// Tray Icon - Context Menu - Method (Show P127)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		public void menuItem_Show_Click(object Sender, EventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Normal;
			this.Show();
			this.Activate();
		}

		/// <summary>
		/// Tray Icon - Context Menu - Method (Hide P127)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_Hide_Click(object Sender, EventArgs e)
		{
			try
			{
				this.Hide();
			}
			catch
			{
				//Globals.DebugPopup(ex.ToString());
			}
		}

		/// <summary>
		/// Tray Icon - Context Menu - Method (Upgrade)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_Upgrade_Click(object Sender, EventArgs e)
		{
			this.btn_Upgrade_Click(null, null);
		}


		/// <summary>
		/// Tray Icon - Context Menu - Method (Downgrade)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_Downgrade_Click(object Sender, EventArgs e)
		{
			this.btn_Downgrade_Click(null, null);
		}


		/// <summary>
		/// Tray Icon - Context Menu - Method (LaunchGame)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_LaunchGame_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.GTA;
			MainWindow.btn_GTA_Click_Static();
		}

		/// <summary>
		/// Tray Icon - Context Menu - Method (SaveFileHandler)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_SaveFileHandler_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.SaveFileHandler;
		}

		/// <summary>
		/// Tray Icon - Context Menu - Method (NoteOverlay)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_NoteOverlay_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.NoteOverlay;
		}

		/// <summary>
		/// Tray Icon - Context Menu - Method (ComponentManager)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_ComponentManager_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.ComponentManager;
		}

		/// <summary>
		/// Tray Icon - Context Menu - Method (Settings)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_Settings_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.Settings;
		}

		/// <summary>
		/// Tray Icon - Context Menu - Method (Information)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_Information_Click(object Sender, EventArgs e)
		{
			menuItem_Show_Click(null, null);
			Globals.PageState = Globals.PageStates.ReadMe;
		}

		/// <summary>
		/// Tray Icon - Context Menu - Method (Close)
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		private void menuItem_Close_Click(object Sender, EventArgs e)
		{
			Globals.ProperExit();
		}


		#endregion

		// NotifyIcon (Tray Icon) above

		// Exit Button Rightclick Below

		#region ExitButtonRightclick


		private void GenerateExitRightClickContextMenu()
		{
			ContextMenu cm = new ContextMenu();

			MenuItem mi = new MenuItem();
			mi.Header = "Minimize";
			mi.Click += MI_Minimize_Click;
			cm.Items.Add(mi);

			MenuItem mi2 = new MenuItem();
			mi2.Header = "Hide in Tray";
			mi2.Click += MI_ExitToTray_Click;
			cm.Items.Add(mi2);

			MenuItem mi3 = new MenuItem();
			mi3.Header = "Close P127";
			mi3.Click += MI_Close_Click;
			cm.Items.Add(mi3);

			cm.IsOpen = true;
		}


		private void MI_Minimize_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void MI_ExitToTray_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}

		private void MI_Close_Click(object sender, RoutedEventArgs e)
		{
			Globals.ProperExit();
		}

		#endregion

		// Exit Button Rightclick above

	}
}
