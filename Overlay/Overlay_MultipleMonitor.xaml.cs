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
using Project_127.MySettings;

namespace Project_127
{
	/// <summary>
	/// Interaction logic for Overlay_MultipleMonitor.xaml
	/// </summary>
	public partial class Overlay_MultipleMonitor : Window
	{
		public Overlay_MultipleMonitor()
		{
			InitializeComponent();
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		public void MyShow()
		{
			this.border_main.Visibility = Visibility.Visible;
		}

		public void MyHide()
		{
			this.border_main.Visibility = Visibility.Hidden;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Settings.OL_MM_Top = this.Top;
			Settings.OL_MM_Left = this.Left;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.Width = Settings.OverlayWidth;

			if (Settings.OL_MM_Left > 0 && Settings.OL_MM_Top > 0)
			{
				this.Top = Settings.OL_MM_Top;
				this.Left = Settings.OL_MM_Left;
			}
		}
	}
}
