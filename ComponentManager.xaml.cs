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
		public ComponentManager()
		{
			InitializeComponent();
			ButtonMouseOverMagic(btn_Refresh);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{

		}


		private void btn_Install_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btn_Uninstall_Click(object sender, RoutedEventArgs e)
		{

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

		}
	}
}
