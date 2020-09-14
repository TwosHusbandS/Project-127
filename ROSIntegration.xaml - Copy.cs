using CefSharp;
using CefSharp.Wpf;
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

namespace Project_127
{
    /// <summary>
    /// Interaction logic for ROSIntegration.xaml
    /// </summary>
    public partial class ROSIntegration : Window
    {
        public ROSIntegration()
        {
            InitializeComponent();
        }
        private void LoadingStateChange(object sender, LoadingStateChangedEventArgs args)
        {
            if (!args.IsLoading)
            {
                browser.ExecuteScriptAsync("alert('All Resources Have Loaded');");
            }
        }
    }
}
