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
		public enum ComponentActions
		{
			Installing,
			ReInstalling,
			Updating
		}


		ComponentManager.Components MyComponent;
		ComponentActions MyComponentAction;

		public bool rtrn;

		public PopupInstallComponent(ComponentManager.Components pComponent, ComponentActions pComponentAction)
		{
			InitializeComponent();

			MyComponent = pComponent;
			MyComponentAction = pComponentAction;

			lbl_Main.Content = MyComponentAction.ToString() + "\n'" + MyComponent.GetNiceName() + "'" + "\nPlease Stand By"; ;

			StartWork();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		//	Task.Delay(100).GetAwaiter().GetResult();
		//	LetsGo();
		}

		async void LetsGo()
		{
		//	lbl_Main.Content += "\nPlease Stand By";
		//	await Task.Run(() => ActualWork());
		//	this.Close();
		}


		/// <summary>
		/// Starting the Task 
		/// </summary>
		[STAThread]
		public async void StartWork()
		{
			await Task.Delay(250);

			// Awaiting the Task of the Actual Work
			await Task.Run(new Action(ActualWork));

			// Close this
			this.Close();
		}

		/// <summary>
		/// Task of the actual work being done
		/// </summary>
		[STAThread]
		public void ActualWork()
		{
			this.Dispatcher.Invoke(() =>
			{
				switch (MyComponentAction)
				{
					case ComponentActions.Installing:
						rtrn = Globals.MyDM.getSubassembly(MyComponent.GetAssemblyName()).GetAwaiter().GetResult();
						break;
					case ComponentActions.ReInstalling:
						rtrn = Globals.MyDM.getSubassembly(MyComponent.GetAssemblyName(), true).GetAwaiter().GetResult();
						break;
					case ComponentActions.Updating:
						rtrn = Globals.MyDM.updateSubssembly(MyComponent.GetAssemblyName(), true).GetAwaiter().GetResult();
						break;
				}
			});
			
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}
	}
}
