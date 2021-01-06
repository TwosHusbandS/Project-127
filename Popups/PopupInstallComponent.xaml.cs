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

namespace Project_127.Popups
{
	/// <summary>
	/// Interaction logic for PopupInstallComponent.xaml
	/// </summary>
	public partial class PopupInstallComponent : Window
	{
		ComponentManager.Components MyComponent;
		bool AssemblyReinstall;
		public bool rtrn;

		public PopupInstallComponent(ComponentManager.Components pComponent, bool pAssemblyReInstall = false)
		{
			InitializeComponent();

			MyComponent = pComponent;
			AssemblyReinstall = pAssemblyReInstall;

			string msg = "";

			if (pAssemblyReInstall)
			{
				msg += "Re-";
			}

			msg += "Installing:\n'" + MyComponent.GetNiceName() + "'\nPlease wait until this Window closes";

			lbl_Main.Content = msg;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Task.Delay(100).GetAwaiter().GetResult();

			rtrn = Globals.MyDM.getSubassembly(MyComponent.GetAssemblyName(), AssemblyReinstall).GetAwaiter().GetResult();

			this.Close();
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}
	}
}
