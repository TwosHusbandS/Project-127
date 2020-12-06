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
			this.Width = Settings.OverlayWidth;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

	}
}
