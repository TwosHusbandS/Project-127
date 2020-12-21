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

namespace Project_127.Overlay
{
	/// <summary>
	/// Interaction logic for EmptyPage.xaml
	/// </summary>
	public partial class EmptyPage : Page
	{
		/// <summary>
		/// Empty Page to take load of the CPU so the current Page doesnt get rendered all the time (noteoverlay_preview)
		/// </summary>
		public EmptyPage()
		{
			InitializeComponent();
		}
	}
}
