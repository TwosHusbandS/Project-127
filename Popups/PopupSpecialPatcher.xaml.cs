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

namespace Project_127.Popups
{
    /// <summary>
    /// Interaction logic for PopupSpecialPatcher.xaml
    /// </summary>
    public partial class PopupSpecialPatcher : Window
    {
        public PopupSpecialPatcher()
        {
            InitializeComponent();
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            if (MainWindow.MW.IsVisible)
            {
                this.Left = MainWindow.MW.Left + (MainWindow.MW.Width / 2) - (this.Width / 2);
                this.Top = MainWindow.MW.Top + (MainWindow.MW.Height / 2) - (this.Height / 2);
            }
        }

        /// <summary>
		/// Move around...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
