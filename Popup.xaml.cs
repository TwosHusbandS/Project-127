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
			PopupOkTextBox,
			PopupOkComboBox,
		}


		public string MyReturnString = "";


		/// <summary>
		/// Constructor for Popup window.
		/// </summary>
		/// <param name="pPopupWindowType"></param>
		/// <param name="pMsg"></param>
		/// <param name="pTitle"></param>
		/// <param name="pFontSize"></param>
		public Popup(Popup.PopupWindowTypes pPopupWindowType, string pMsg, int pFontSize = 18, string pDefaultTBText = "", Enum pEnum = null)
		{
			this.Owner = MainWindow.MW;
			// Initializing all WPF Elements
			InitializeComponent();

			// Add "Support Text" to bottom if error
			if (pPopupWindowType == PopupWindowTypes.PopupOkError)
			{
				pMsg = pMsg + "\n\nIf this happens a lot,\nContact me on Discord:\n@thS#0305";
				pPopupWindowType = PopupWindowTypes.PopupOk;
			}

			// If its a "Yes/No" Window:
			if (pPopupWindowType == Popup.PopupWindowTypes.PopupYesNo)
			{
				// Creating the "Yes" Button
				Button myButtonYes = new Button();
				myButtonYes.Content = "Yes";
				myButtonYes.Style = Resources["btn"] as Style;
				myButtonYes.Click += btn_Yes_Click;

				// Adding it to the Grid
				myGrid.Children.Add(myButtonYes);
				Grid.SetColumn(myButtonYes, 0);
				Grid.SetRow(myButtonYes, 2);

				// Creating the "No" Button
				Button myButtonNo = new Button();
				myButtonNo.Content = "No";
				myButtonNo.Style = Resources["btn"] as Style;
				myButtonNo.Click += btn_No_Click;

				// Adding it to the Grid
				myGrid.Children.Add(myButtonNo);
				Grid.SetColumn(myButtonNo, 1);
				Grid.SetRow(myButtonNo, 2);
				Grid.SetRowSpan(lbl_Main, 2);

				// Focusing the "Yes" Button so you can just hit "Space" or "Enter" to trigger the Click event
				myButtonYes.Focus();
			}
			// If its a "OK" Window:
			else
			{
				// Creating the "Yes" Button
				Button myButtonYes = new Button();
				myButtonYes.Content = "Ok";
				myButtonYes.Style = Resources["btn"] as Style;
				myButtonYes.Click += btn_Yes_Click;

				// Adding it to the Grid
				myGrid.Children.Add(myButtonYes);
				Grid.SetColumn(myButtonYes, 0);
				Grid.SetRow(myButtonYes, 2);

				// Creating the "No" Button
				Button myButtonNo = new Button();
				myButtonNo.Content = "Cancel";
				myButtonNo.Style = Resources["btn"] as Style;
				myButtonNo.Click += btn_No_Click;

				// Adding it to the Grid
				myGrid.Children.Add(myButtonNo);
				Grid.SetColumn(myButtonNo, 1);
				Grid.SetRow(myButtonNo, 2);
				Grid.SetRowSpan(lbl_Main, 2);

				if (pPopupWindowType == PopupWindowTypes.PopupOk)
				{
					Grid.SetRowSpan(lbl_Main, 2);
				}
				else
				{
					if (pPopupWindowType == PopupWindowTypes.PopupOkTextBox)
					{
						// Creates Button
						TextBox myTextBox = new TextBox();
						myTextBox.Text = pDefaultTBText;
						MyReturnString = myTextBox.Text;
						myTextBox.Style = Resources["tb"] as Style;
						myTextBox.TextChanged += MyTextBox_TextChanged;
						myTextBox.KeyDown += MyTextBox_KeyDown;

						// Adds it to the Grid
						myGrid.Children.Add(myTextBox);
						Grid.SetColumn(myTextBox, 0);
						Grid.SetColumnSpan(myTextBox, 2);
						Grid.SetRow(myTextBox, 1);

						myTextBox.Focus();
					}
					else if (pPopupWindowType == PopupWindowTypes.PopupOkComboBox)
					{
						if (pEnum != null)
						{
							// Creates Button
							ComboBox MyComboBox = new ComboBox();
							MyComboBox.SelectionChanged += MyComboBox_SelectionChanged;

							// Adds it to the Grid
							myGrid.Children.Add(MyComboBox);
							Grid.SetColumn(MyComboBox, 0);
							Grid.SetColumnSpan(MyComboBox, 2);
							Grid.SetRow(MyComboBox, 1);

							MyComboBox.Focus();

							List<string> myEnumValues = new List<string>();
							foreach (string myString in pEnum.GetType().GetEnumNames())
							{
								myEnumValues.Add(myString);
							}

							string enumname = pEnum.GetType().ToString();

							MyComboBox.ItemsSource = myEnumValues;

							if (enumname.Contains("Retailer"))
							{
								MyComboBox.SelectedItem = Settings.Retailer.ToString();
							}
							else if (enumname.Contains("Language"))
							{
								MyComboBox.SelectedItem = Settings.LanguageSelected.ToString();
							}

							MyReturnString = MyComboBox.SelectedItem.ToString();
						}
						else
						{
							HelperClasses.Logger.Log("pEnum is Null, this should not have happened");
							new Popup(PopupWindowTypes.PopupOkError, "pEnum is Null, this should not have happened").ShowDialog();
						}
					}
				}
			}

			// Set the Parameters as Properties of new Popup Window
			lbl_Main.FontSize = pFontSize;
			lbl_Main.Content = pMsg;
		}


		private void MyTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				btn_Ok_Click(null, null);
			}
		}

		private void MyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox tmp = (ComboBox)sender;
			if (tmp != null)
			{
				MyReturnString = tmp.SelectedItem.ToString();
			}
		}

		private void MyTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox tmp = (TextBox)sender;
			if (tmp != null)
			{
				MyReturnString = tmp.Text;
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
