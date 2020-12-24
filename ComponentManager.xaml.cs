using Project_127.Popups;
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

namespace Project_127
{
	/// <summary>
	/// Interaction logic for ComponentManager.xaml
	/// </summary>
	public partial class ComponentManager : Page
	{
		public enum Components
		{
			Base,
			SCLSteam127,
			SCLSteam124,
			SCLRockstar127,
			SCLRockstar124,
			SCLDowngradedSC,
			AdditionalSaveFiles
		}


		public static Array AllComponents
		{
			get
			{
				return Enum.GetValues(typeof(Components));
			}
		}

		public static List<Components> InstalledComponents
		{
			get
			{
				List<Components> tmp = new List<Components>();
				foreach (Components myComponent in AllComponents)
				{
					if (myComponent.IsInstalled())
					{
						tmp.Add(myComponent);
					}
				}
				return tmp;
			}
		}


		public static List<Components> RequiredComponentsBasedOnSettings
		{
			get
			{
				List<Components> tmp = new List<Components>();
				tmp.Add(Components.Base);
				if (MySettings.Settings.EnableAlternativeLaunch && MySettings.Settings.Retailer != MySettings.Settings.Retailers.Epic)
				{
					tmp.Add(Components.SCLDowngradedSC);
					if (MySettings.Settings.SocialClubLaunchGameVersion == "124")
					{
						if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Steam)
						{
							tmp.Add(Components.SCLSteam124);
						}
						else if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Rockstar)
						{
							tmp.Add(Components.SCLRockstar124);
						}
					}
					else if (MySettings.Settings.SocialClubLaunchGameVersion == "127")
					{
						if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Steam)
						{
							tmp.Add(Components.SCLSteam127);
						}
						else if (MySettings.Settings.Retailer == MySettings.Settings.Retailers.Rockstar)
						{
							tmp.Add(Components.SCLRockstar127);
						}
					}
				}
				return tmp;
			}
		}

		public static bool CheckIfRequiredComponentsAreInstalled(bool AskUser = false)
		{
			bool rtrn = true;
			foreach (Components myComponent in RequiredComponentsBasedOnSettings)
			{
				if (!myComponent.IsInstalled())
				{
					if (AskUser)
					{
						Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Component:\n" + myComponent.GetNiceName() + "\nmissing but needed.\nDo you want to install it?\n(Clicking no might result in Upgrading / Downgrading / Launching being disabled.)");
						yesno.ShowDialog();
						if (yesno.DialogResult == true)
						{
							myComponent.Install();
						}
						else
						{
							rtrn = false;
						}
					}
					else
					{
						new Popup(Popup.PopupWindowTypes.PopupOk, "Component:\n" + myComponent.GetNiceName() + "\nmissing but needed.\nIt will be downloaded and installed now.").ShowDialog();
						myComponent.Install();
					}
				}
				else
				{
					if (myComponent == Components.SCLDowngradedSC)
					{
						string Path1 = LauncherLogic.DowngradedSocialClub.TrimEnd('\\') + @"\subprocess.exe";
						string Path2 = LauncherLogic.DowngradedSocialClub.TrimEnd('\\') + @"\socialclub.dll";
						if (!HelperClasses.FileHandling.doesFileExist(Path1) || !HelperClasses.FileHandling.doesFileExist(Path2))
						{
							if (AskUser)
							{
								Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Component:\n" + myComponent.GetNiceName() + "\nmissing but needed.\nDo you want to install it?\n(Clicking no might result in Upgrading / Downgrading / Launching being disabled.)");
								yesno.ShowDialog();
								if (yesno.DialogResult == true)
								{
									myComponent.ReInstall();
								}
								else
								{
									rtrn = false;
								}
							}
							else
							{
								new Popup(Popup.PopupWindowTypes.PopupOk, "Component:\n" + myComponent.GetNiceName() + "\nmissing but needed.\nIt will be downloaded and installed now.").ShowDialog();
								myComponent.ReInstall();
							}
						}
					}
				}
			}
			return rtrn;
		}

		public static bool isCSLSocialClubRequired
		{
			get
			{
				foreach (Components myComponent in RequireCSLSocialClub)
				{
					if (myComponent.IsInstalled())
					{
						return true;
					}
				}
				return false;
			}
		}

		public static Components[] RequireCSLSocialClub = new Components[] { Components.SCLRockstar124, Components.SCLRockstar127, Components.SCLSteam124, Components.SCLSteam127 };



		public ComponentManager()
		{
			InitializeComponent();
			ButtonMouseOverMagic(btn_Refresh);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Refresh();
		}


		private void btn_Install_Click(object sender, RoutedEventArgs e)
		{
			string RealTag = ((Button)sender).Tag.ToString().TrimStart("Files".ToCharArray());
			Components MyComponent = (Components)System.Enum.Parse(typeof(Components), RealTag);
			if (MyComponent.IsInstalled())
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you want to Re-Install the following Component:\n" + MyComponent.GetNiceName());
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					MyComponent.ReInstall();
					Refresh();
					new Popup(Popup.PopupWindowTypes.PopupOk, "Done ReInstalling:\n" + MyComponent.GetNiceName()).ShowDialog();
					if (MyComponent == Components.AdditionalSaveFiles)
					{
						ThrowShoutout();
					}
				}
			}
			else
			{
				MyComponent.Install();
				Refresh();
				new Popup(Popup.PopupWindowTypes.PopupOk, "Done Installing:\n" + MyComponent.GetNiceName()).ShowDialog();
				if (MyComponent == Components.AdditionalSaveFiles)
				{
					ThrowShoutout();
				}
			}
		}

		public static void ZIPVersionSwitcheroo()
		{
			int _ZipVersion = 0;
			string VersionTxt = LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\Version.txt";
			if (HelperClasses.FileHandling.doesFileExist(VersionTxt))
			{
				Int32.TryParse(HelperClasses.FileHandling.ReadContentOfFile(VersionTxt), out _ZipVersion);
				if (_ZipVersion > 0)
				{
					HelperClasses.FileHandling.deleteFile(VersionTxt);
					Components.Base.ForceSetInstalled(new Version(1, _ZipVersion));
				}
			}

		}

		public static void ThrowShoutout()
		{
			string msg = "";
			msg += "Editors Note: Massive Shoutout to @AntherXx for going through the trouble and manual labor" + "\n";
			msg += "of renaming the SaveFiles and collecting them.Thanks a lot mate." + "\n";
			msg += "" + "\n";
			msg += "This is a compilation of save files to practice each mission as you would in a full run, instead of in a Mission Replay." + "\n";
			msg += "All save files are made immediately following the end of the previous mission, so that you can practice the drive to the mission" + "\n";
			msg += "The mission order is based on the current Classic % route." + "\n";
			msg += "" + "\n";
			msg += "Some save files have a duplicate, for example Lamar Down, where you finish The Wrap Up as Michael at the gas station(save file #1)," + "\n";
			msg += "but switch to Franklin and save warp to start Lamar Down. (save file #2 is made right after the switch to Franklin)" + "\n";
			msg += "" + "\n";
			msg += "Some mission save files are missing, such as the tow truck and garbage truck for Blitz Play. This is simply because there are routes" + "\n";
			msg += "that differ so much from run to run that it would be unfair to decide one particular way to make a save file for it." + "\n";
			msg += "" + "\n";
			msg += "This should NOT be used to start segment runs, there should be save files already within your Project 1.27 launcher that has those," + "\n";
			msg += "if not you can download them from speedrun.com/ gtav" + "\n";
			msg += "" + "\n";
			msg += "Although I tried to make them as accurate to the Classic % strats and mission routing as possible," + "\n";
			msg += "there might be some points where the characters do not have the correct weapons, and I am sorry for that." + "\n";
			msg += "" + "\n";
			msg += "In that case, instead of making a new save file yourself or correcting it yourself, (which you can do as well)," + "\n";
			msg += "I would request that you reach out to me on discord so that I can correct it." + "\n";
			msg += "" + "\n";
			msg += "If there are any other questions/ comments / concerns, please let me know on discord at @AntherXx#5392";

			new Popup(Popup.PopupWindowTypes.PopupOk, msg, 16).ShowDialog();
		}

		private void btn_Uninstall_Click(object sender, RoutedEventArgs e)
		{
			string RealTag = ((Button)sender).Tag.ToString().TrimStart("Files".ToCharArray());
			Components MyComponent = (Components)System.Enum.Parse(typeof(Components), RealTag);

			if (MyComponent == Components.Base)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "Cant delete our Base Component:\n" + Components.Base.GetNiceName()).ShowDialog();
			}
			else if (MyComponent == Components.SCLDowngradedSC && isCSLSocialClubRequired)
			{
				new Popup(Popup.PopupWindowTypes.PopupOk, "Cant delete that, because Components requiring this are installed.").ShowDialog();
			}
			else if (MyComponent.IsInstalled())
			{
				MyComponent.Uninstall();
				Refresh();
				new Popup(Popup.PopupWindowTypes.PopupOk, "Done deleting:\n" + MyComponent.GetNiceName()).ShowDialog();
			}
		}

		private void btn_Install_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			string RealTag = ((Button)sender).Tag.ToString().TrimStart("Files".ToCharArray());
			Components MyComponent = (Components)System.Enum.Parse(typeof(Components), RealTag);
			if (MyComponent.IsInstalled())
			{
				MyComponent.Verify();
				Refresh();
				new Popup(Popup.PopupWindowTypes.PopupOk, "Done verifying:\n" + MyComponent.GetNiceName()).ShowDialog();
			}
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			ButtonMouseOverMagic((Button)sender);
		}

		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			ButtonMouseOverMagic((Button)sender);
		}

		/// <summary>
		/// Logic behind all MouseOver Stuff. Checkboxes and Refresh Button
		/// </summary>
		/// <param name="myBtn"></param>
		private void ButtonMouseOverMagic(Button myBtn)
		{
			switch (myBtn.Name)
			{
				case "btn_Refresh":
					if (myBtn.IsMouseOver)
					{
						MainWindow.MW.SetControlBackground(btn_Refresh, "Artwork/refresh_mo.png");
					}
					else
					{
						MainWindow.MW.SetControlBackground(btn_Refresh, "Artwork/refresh.png");
					}
					break;
			}
		}

		private void btn_Refresh_Click(object sender, RoutedEventArgs e)
		{
			Refresh(true);
		}

		public static void CheckForUpdates()
		{
			foreach (Components myComponent in InstalledComponents)
			{
				myComponent.UpdateLogic();
			}
		}

		public static void StartupCheck()
		{
			CheckForUpdates();
			CheckIfRequiredComponentsAreInstalled(true);
		}

		private void Refresh(bool CheckForUpdatesPls = false)
		{
			if (CheckForUpdatesPls)
			{
				Globals.SetUpDownloadManager(false);
				CheckForUpdates();
			}

			Components.Base.UpdateStatus(lbl_FilesMain_Status);
			Components.SCLRockstar124.UpdateStatus(lbl_FilesSCLRockstar124_Status);
			Components.SCLRockstar127.UpdateStatus(lbl_FilesSCLRockstar127_Status);
			Components.SCLSteam124.UpdateStatus(lbl_FilesSCLSteam124_Status);
			Components.SCLSteam127.UpdateStatus(lbl_FilesSCLSteam127_Status);
			Components.SCLDowngradedSC.UpdateStatus(lbl_FilesSCLDowngradedSC_Status);
			Components.AdditionalSaveFiles.UpdateStatus(lbl_FilesAdditionalSF_Status);

			btn_lbl_FilesMain_Name.ToolTip = Components.Base.GetToolTip();
			lbl_FilesSCLRockstar124_Name.ToolTip = Components.SCLRockstar124.GetToolTip();
			lbl_FilesSCLRockstar127_Name.ToolTip = Components.SCLRockstar127.GetToolTip();
			lbl_FilesSCLSteam124_Name.ToolTip = Components.SCLSteam124.GetToolTip();
			lbl_FilesSCLSteam127_Name.ToolTip = Components.SCLSteam127.GetToolTip();
			lbl_FilesSCLDowngradedSC_Name.ToolTip = Components.SCLDowngradedSC.GetToolTip();
			lbl_FilesAdditionalSF_Name.ToolTip = Components.AdditionalSaveFiles.GetToolTip();



			Version tmp = Components.Base.GetInstalledVersion();
			if (tmp != new Version("0.0.0.1"))
			{
				btn_lbl_FilesMain_Name.Content = "Required Files (v." + tmp.Minor + ")";
			}
			else
			{
				btn_lbl_FilesMain_Name.Content = "Required Files";
			}


		}

		private void btn_lbl_FilesMain_Name_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount >= 3)
			{
				if (Components.Base.IsInstalled())
				{
					Popups.PopupTextbox tmp = new PopupTextbox("Enter forced Version.\nClick cancel,\nif you dont know what youre doing.", "1.0.0.0");
					tmp.ShowDialog();
					if (tmp.DialogResult == true)
					{
						Version tmpV = new Version("0.0.0.1");
						try
						{
							tmpV = new Version(tmp.MyReturnString);
						}
						catch { }
						if (tmpV != new Version("0.0.0.1"))
						{
							Components.Base.ForceSetInstalled(tmpV);
							Refresh();
						}
					}
				}
			}
		}

		private void btn_Refresh_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount >= 3)
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Do you want to reset the Information if Components are installed?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					HelperClasses.RegeditHandler.SetValue("DownloadManagerInstalledSubassemblies", "");
					Globals.SetUpDownloadManager(true);
				}
			}
		}
	}


	static class ComponentsExtensions
	{
		public static string GetAssemblyName(this ComponentManager.Components Component)
		{
			string rtrn = "";
			switch (Component)
			{
				case ComponentManager.Components.Base:
					rtrn = "STANDARD_BASE";
					break;
				case ComponentManager.Components.SCLRockstar124:
					rtrn = "AM_124_ROCKSTAR";
					break;
				case ComponentManager.Components.SCLRockstar127:
					rtrn = "AM_127_ROCKSTAR";
					break;
				case ComponentManager.Components.SCLSteam124:
					rtrn = "AM_124_STEAM";
					break;
				case ComponentManager.Components.SCLSteam127:
					rtrn = "AM_127_STEAM";
					break;
				case ComponentManager.Components.SCLDowngradedSC:
					rtrn = "SOCIALCLUB_1178";
					break;
				case ComponentManager.Components.AdditionalSaveFiles:
					rtrn = "ADDITIONAL_SAVEFILES";
					break;
			}
			return rtrn;
		}

		public static string GetNiceName(this ComponentManager.Components Component)
		{
			string rtrn = "";
			switch (Component)
			{
				case ComponentManager.Components.Base:
					rtrn = "Needed Files for P127 and Downgraded GTA";
					break;
				case ComponentManager.Components.SCLRockstar124:
					rtrn = "Launching through Social Club for Rockstar 1.24";
					break;
				case ComponentManager.Components.SCLRockstar127:
					rtrn = "Launching through Social Club for Rockstar 1.27";
					break;
				case ComponentManager.Components.SCLSteam124:
					rtrn = "Launching through Social Club for Steam 1.24";
					break;
				case ComponentManager.Components.SCLSteam127:
					rtrn = "Launching through Social Club for Steam 1.27";
					break;
				case ComponentManager.Components.SCLDowngradedSC:
					rtrn = "Launching through Downgraded Social Club.";
					break;
				case ComponentManager.Components.AdditionalSaveFiles:
					rtrn = "Additional SaveFiles";
					break;
			}
			return rtrn;
		}

		public static bool IsInstalled(this ComponentManager.Components Component)
		{
			return Globals.MyDM.isInstalled(Component.GetAssemblyName());
		}

		public static void ForceSetInstalled(this ComponentManager.Components Component, Version myVersion)
		{
			Globals.MyDM.setVersion(Component.GetAssemblyName(), myVersion);
		}

		public static bool UpdateLogic(this ComponentManager.Components Component)
		{
			if (Globals.MyDM.isUpdateAvalailable(Component.GetAssemblyName()))
			{
				Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, "Update for: '" + Component.GetNiceName() + "' available.\nDo you want to Download it?");
				yesno.ShowDialog();
				if (yesno.DialogResult == true)
				{
					Globals.MyDM.updateSubssembly(Component.GetAssemblyName(), true).GetAwaiter().GetResult();
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Installs a Specific Assembly. Returns Success bool.
		/// </summary>
		/// <param name="Component"></param>
		/// <returns></returns>
		public static bool Install(this ComponentManager.Components Component)
		{
			bool tmp;
			tmp = Globals.MyDM.getSubassembly(Component.GetAssemblyName()).GetAwaiter().GetResult();
			Globals.DebugPopup(tmp.ToString());
			return tmp;
		}

		/// <summary>
		/// Re-Installs a specific Assembly. Returns Success bool.
		/// </summary>
		/// <param name="Component"></param>
		/// <returns></returns>
		public static bool ReInstall(this ComponentManager.Components Component)
		{
			return Globals.MyDM.getSubassembly(Component.GetAssemblyName(), true).GetAwaiter().GetResult();
		}


		/// <summary>
		/// Uninstall a Specific assembly.
		/// </summary>
		/// <param name="Component"></param>
		public static void Uninstall(this ComponentManager.Components Component)
		{
			Globals.MyDM.delSubassembly(Component.GetAssemblyName());
		}

		/// <summary>
		/// Checks if a Subassembly is actuall installed (matches Hashes). Doesnt repair. Returns Success bool.
		/// </summary>
		/// <param name="Component"></param>
		/// <returns></returns>
		public static bool Verify(this ComponentManager.Components Component)
		{
			return Globals.MyDM.verifySubassembly(Component.GetAssemblyName()).GetAwaiter().GetResult();
		}

		/// <summary>
		/// GUI Method to Update the label
		/// </summary>
		/// <param name="Component"></param>
		/// <param name="myLbl"></param>
		public static void UpdateStatus(this ComponentManager.Components Component, Label myLbl)
		{
			if (Component.IsInstalled())
			{
				myLbl.Content = "Installed";
				myLbl.Foreground = MyColors.MyColorGreen;
				myLbl.ToolTip = "Installed Version: " + Component.GetInstalledVersion().ToString();
			}
			else
			{
				myLbl.Content = "Not Installed";
				myLbl.Foreground = MyColors.MyColorOrange;
				myLbl.ToolTip = "";
			}
		}

		/// <summary>
		/// GetInstallVersionUIText
		/// </summary>
		/// <param name="Component"></param>
		public static Version GetInstalledVersion(this ComponentManager.Components Component)
		{
			Version rtrn = new Version("0.0.0.1");
			if (Component.IsInstalled())
			{
				rtrn = Globals.MyDM.getVersion(Component.GetAssemblyName());
			}
			return rtrn;
		}

		/// <summary>
		/// GetToolTip
		/// </summary>
		/// <param name="Component"></param>
		/// <param name="myLbl"></param>
		public static string GetToolTip(this ComponentManager.Components Component)
		{
			string toolTip = Component.GetNiceName();
			if (Component.IsInstalled())
			{
				toolTip = toolTip + ". Installed Version: " + Component.GetInstalledVersion().ToString();
			}
			return toolTip;
		}
	}
}
