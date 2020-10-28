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
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        private void browser_JavascriptMessageReceived(object sender, CefSharp.JavascriptMessageReceivedEventArgs e)
        {
            var message = e.Message.ToString();
            message = message.Substring(1, message.Length - 2);
            var vals = message.Split(',');
            var R = int.Parse(vals[0]);
            var G = int.Parse(vals[1]);
            var B = int.Parse(vals[2]);
            var A = double.Parse(vals[3]);
            //Insert Whatever Here

        }
    }
}
