using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using static Project_127.HelperClasses.SpecialPatchHandler;

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
            dg_patches.ItemsSource = patch.PatchesObservable;
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

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var si = (patch)dg_patches.SelectedItem;
            new PopupPatchEditor(si.Name).ShowDialog();
        }

        private void btn_patch_minus_Click(object sender, RoutedEventArgs e)
        {
            if (dg_patches.SelectedItem != null)
            {
                var si = (patch)dg_patches.SelectedItem;
                bool doDelete = new Popup(Popup.PopupWindowTypes.PopupYesNo, String.Format("Are you sure you would like to delete patch \"{0}\"?", si.Name)).ShowDialog()??false;
                if (doDelete)
                {
                    patch.deletePatch(si.Name);
                }
            }
            
        }

        private void btn_patch_plus_Click(object sender, RoutedEventArgs e)
        {
            new PopupPatchEditor(null).ShowDialog();
        }

        private void btn_DisableAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in patch.PatchesObservable)
            {
                p.Enabled = false;
            }
            dg_patches.Items.Refresh();
        }

        private void btn_EnableAll_Click(object sender, RoutedEventArgs e)
        {
            foreach(var p in patch.PatchesObservable)
            {
                p.Enabled = true;
            }
            dg_patches.Items.Refresh();
        }
    }
}
