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
    /// Popup Window. Used to custom Yes/No and Ok Dialogs.
    /// </summary>
    public partial class Popup : Window
    {
		/// <summary>
		/// Defines the Enum "PopupWindowTypes"
		/// </summary>
		public enum PopupWindowTypes
		{
			PopupYesNo,
			PopupOk,
			PopupOkError,
		}


		/// <summary>
		/// Constructor for Popup window.
		/// </summary>
		/// <param name="pPopupWindowType"></param>
		/// <param name="pMsg"></param>
		/// <param name="pTitle"></param>
		/// <param name="pFontSize"></param>
		public Popup(Popup.PopupWindowTypes pPopupWindowType, string pMsg, int pFontSize = 18)
        {
			// Initializing all WPF Elements
            InitializeComponent();

			// Set the Parameters as Properties of new Popup Window
			lbl_Main.FontSize = pFontSize;
			lbl_Main.Content = pMsg;

			// Add "Support Text" to bottom if error
			if (pPopupWindowType == PopupWindowTypes.PopupOkError)
			{
			lbl_Main.Content = pMsg + "\n\nIf this happens a lot,\nContact me on Discord:\n@thS#0305";
			}

			// If its a "OK" Window:
			if (pPopupWindowType.ToString().Contains("PopupOk"))
            {
				// Creates Button
                Button myButtonOk = new Button();
                myButtonOk.Content = "Ok";
                myButtonOk.Style = Resources["btn"] as Style;
                myButtonOk.Click += btn_Ok_Click;

				// Adds it to the Grid
                myGrid.Children.Add(myButtonOk);
                Grid.SetColumn(myButtonOk, 0);
                Grid.SetColumnSpan(myButtonOk, 2);
                Grid.SetRow(myButtonOk, 1);

				// Focusing the "Ok" Button so you can just hit "Space" or "Enter" to trigger the Click event
				myButtonOk.Focus();
            }
			// If its a "Yes/No" Window:
            else if (pPopupWindowType == Popup.PopupWindowTypes.PopupYesNo)
            {
				// Creating the "Yes" Button
                Button myButtonYes = new Button();
                myButtonYes.Content = "Yes";
                myButtonYes.Style = Resources["btn"] as Style;
                myButtonYes.Click += btn_Yes_Click;

				// Adding it to the Grid
                myGrid.Children.Add(myButtonYes);
                Grid.SetColumn(myButtonYes, 0);
                Grid.SetRow(myButtonYes, 1);

				// Creating the "No" Button
                Button myButtonNo = new Button();
                myButtonNo.Content = "No";
                myButtonNo.Style = Resources["btn"] as Style;
                myButtonNo.Click += btn_No_Click;

				// Adding it to the Grid
				myGrid.Children.Add(myButtonNo);
                Grid.SetColumn(myButtonNo, 1);
                Grid.SetRow(myButtonNo, 1);

				// Focusing the "Yes" Button so you can just hit "Space" or "Enter" to trigger the Click event
				myButtonYes.Focus();
			}
		}


        /// <summary>
        /// Click on "OK". Closes itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // probably not needed...
            this.Close();
		}


        /// <summary>
        /// Click on "Yes". Sets DialogResult to "Yes" and closes itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }


        /// <summary>
        /// Click on "No". Sets DialogResult to "No" and closes itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_No_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();

        }


		// Below are Methods we need to make the behaviour of this nice.


		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

	} // End of Class
} // End of Namespace
