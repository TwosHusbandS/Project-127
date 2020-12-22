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

		/*
			Questions:
			Had it break when calling getSubassembly with (true)
			Had it crash on verify
			delSubassembly does not return bool
			What happens if SC and SCL_Steam_127 is installed, and SC gets verified
			Need to make this work with current ZIP and Importing ZIP...Check for ZIP Version?
			Not checking for Success Bools...
			Crashing here and there. When SC + One thing which relys in SC is installed, and other thing which relys on SC gets installed.
		
			Integrate everywhere else...instead of update check
			DL shit when its needed (so on settings changed)
			Verify whats installed on startup
			Tripple Rightclick - change ZIP number
		*/


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
					new Popup(Popup.PopupWindowTypes.PopupOk, "Done ReInstalling:\n" + MyComponent.GetNiceName()).ShowDialog();
					Refresh();
				}
			}
			else
			{
				MyComponent.Install();
				new Popup(Popup.PopupWindowTypes.PopupOk, "Done Installing:\n" + MyComponent.GetNiceName()).ShowDialog();
				Refresh();
			}
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
				new Popup(Popup.PopupWindowTypes.PopupOk, "Cant delete that, we because Components requiring this are installed.").ShowDialog();
			}
			else if (MyComponent.IsInstalled())
			{
				MyComponent.Uninstall();
				new Popup(Popup.PopupWindowTypes.PopupOk, "Done deleting:\n" + MyComponent.GetNiceName()).ShowDialog();
				Refresh();
			}
		}

		private void btn_Install_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			string RealTag = ((Button)sender).Tag.ToString().TrimStart("Files".ToCharArray());
			Components MyComponent = (Components)System.Enum.Parse(typeof(Components), RealTag);
			if (MyComponent.IsInstalled())
			{
				MyComponent.Verify();
				new Popup(Popup.PopupWindowTypes.PopupOk, "Done verifying:\n" + MyComponent.GetNiceName()).ShowDialog();
				Refresh();
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
			Refresh();
		}

		private void Refresh(bool VerifyFromDisk = false)
		{
			Globals.SetUpDownloadManager(VerifyFromDisk);

			Components.Base.UpdateStatus(lbl_FilesMain_Status);
			Components.SCLRockstar124.UpdateStatus(lbl_FilesSCLRockstar124_Status);
			Components.SCLRockstar127.UpdateStatus(lbl_FilesSCLRockstar127_Status);
			Components.SCLSteam124.UpdateStatus(lbl_FilesSCLSteam124_Status);
			Components.SCLSteam127.UpdateStatus(lbl_FilesSCLSteam127_Status);
			Components.SCLDowngradedSC.UpdateStatus(lbl_FilesSCLDowngradedSC_Status);
			Components.AdditionalSaveFiles.UpdateStatus(lbl_FilesAdditionalSF_Status);

			btn_lbl_FilesMain_Name.Content = "Required Files (v." + Globals.ZipVersion + ")";
		}

		private void btn_lbl_FilesMain_Name_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{

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
					rtrn = "ADDITIONAL_SF";
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

		public static bool ForceSetInstalled(this ComponentManager.Components Component, Version myVersion)
		{
			//return Globals.MyDM.setInstalled(Component.GetAssemblyName(), myVersion);
			return true;
		}

		public static bool Update(this ComponentManager.Components Component)
		{
			// return Globals.MyDM.Update(Component.GetAssemblyName()).GetAwaiter().GetResult();
			return true;
		}

		/// <summary>
		/// Installs a Specific Assembly. Returns Success bool.
		/// </summary>
		/// <param name="Component"></param>
		/// <returns></returns>
		public static bool Install(this ComponentManager.Components Component)
		{
			return Globals.MyDM.getSubassembly(Component.GetAssemblyName()).GetAwaiter().GetResult();
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
			}
			else
			{
				myLbl.Content = "Not Installed";
				myLbl.Foreground = MyColors.MyColorOrange;
			}
		}
	}
}
