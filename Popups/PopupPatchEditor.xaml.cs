using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for PopupPatchEditor.xaml
    /// </summary>
    public partial class PopupPatchEditor : Window
    {
		private editPatch ep;
        public PopupPatchEditor(string target)
        {
			if (target != null)
            {
				ep = new editPatch(target);
            }
            else
            {
				ep = new editPatch();
			}
            InitializeComponent();
			this.DataContext = ep;
        }

		

		private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			if (MainWindow.MW.IsVisible)
			{
				this.Left = MainWindow.MW.Left + (MainWindow.MW.Width / 2) - (this.Width / 2);
				this.Top = MainWindow.MW.Top + (MainWindow.MW.Height / 2) - (this.Height / 2);
			}
		}

        private void cb_btn_click(object sender, RoutedEventArgs e)
        {
			var btn = (Button)sender;
			BindingExpression be = btn.GetBindingExpression(Button.TagProperty);
			string bindingPath = be.ParentBinding.Path.Path;
			object dataObject = btn.DataContext;

			Type t = dataObject.GetType();
			var propInfo = t.GetProperty(bindingPath);
			propInfo.SetValue(dataObject, (!bool.Parse((string)propInfo.GetValue(dataObject))).ToString());
			//= (!bool.Parse((string)((Button)sender).Tag)).ToString();

		}


        private void btn_toggle_keybind_Click(object sender, RoutedEventArgs e)
        {
			ep.updateKeybind();
		}

		private void HexTextPreview(object sender, TextCompositionEventArgs e)
		{
			Regex hex = new Regex("^[0-9a-f]*$");
			e.Handled = !hex.IsMatch(e.Text.ToLower());
		}

		private void btn_Apply_Click(object sender, RoutedEventArgs e)
		{
			if (ep.Validate())
            {
				ep.contentUpdate();
				ep.update();
            }

		}


		private class editPatch : HelperClasses.SpecialPatchHandler.patch, INotifyPropertyChanged
		{
			private bool isEditing = false;
			public editPatch() : base() { }
			public editPatch(string name)
            {
				isEditing = true;
				var p = GetPatch(name);
				Name = p.Name;
				RVA = p.RVA;
				Content = p.Content;
				DefaultEnabled = p.DefaultEnabled;
				KeyBind = p.KeyBind;
				content_string = p.hexContent;
			}


			public string DefEnabled
			{
				get
				{
					return DefaultEnabled.ToString();
				}
				set
				{
					DefaultEnabled = bool.Parse(value);
					NotifyPropertyChanged();
				}
			}

			internal System.Windows.Forms.Keys keybind
			{
				get
				{
					return KeyBind;
				}
				set
				{
					KeyBind = value;
					NotifyPropertyChanged("KeyBinding");
				}
			}

			private bool _keybindSetting = false;

			private bool keybindSetting
            {
                get
                {
					return _keybindSetting;
                }
                set
                {
					_keybindSetting = value;
					NotifyPropertyChanged("KeyBinding");
                }
            }

			public string KeyBinding
			{
				get
				{
					if (!keybindSetting)
					{
						return keybind.ToString();
					}
					else
					{
						return "[...]";

					}
				}
			}

			public string patchName
			{
				get
				{
					return Name;
				}
				set
				{
					Name = value;
					NotifyPropertyChanged();
				}
			}

			public string RVA_string
			{
				get
				{
					return RVA.ToString("X").ToLower();
				}
				set
				{
					RVA = UInt32.Parse(value, System.Globalization.NumberStyles.HexNumber);
					NotifyPropertyChanged();
				}
			}

			private string content_string = "";

			public string Content_string
			{
				get
				{
					return content_string;
				}
				set
				{
					content_string = value;
					NotifyPropertyChanged();
				}
			}

			public bool Validate()
            {
				if (content_string == "")
                {
					PopupWrapper.PopupOk("Patch cannot be empty!");
					return false;
				}
				else if (Content_string.Length % 2 != 0)
                {
					PopupWrapper.PopupOk("Patch must consist of full bytes!");
					return false;
                }
				else if (Name == "" || Name == null)
                {
					PopupWrapper.PopupOk("Patch must have name!");
					return false;
				}
				else if (nameExists(Name) && !isEditing)
                {
					PopupWrapper.PopupOk("A patch by that name already exists!");
					return false;
                }
				return true;
            }

			public void contentUpdate()
            {
				Content = StringToByteArray(Content_string);
			}
			private static byte[] StringToByteArray(string hex)
			{
				return Enumerable.Range(0, hex.Length)
								 .Where(x => x % 2 == 0)
								 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
								 .ToArray();
			}

			internal async void updateKeybind()
			{
				keybindSetting = true;
				NotifyPropertyChanged("KeyBinding");
				//((Button)sender).Content = "[Press new Key]";
				System.Windows.Forms.Keys MyNewKey = await HelperClasses.Keyboard.KeyboardHandler.GetNextKeyPress();
				if (MyNewKey != System.Windows.Forms.Keys.None && MyNewKey != System.Windows.Forms.Keys.Escape)
				{
					keybind = MyNewKey;
				}
				else if (MyNewKey == System.Windows.Forms.Keys.Escape)
				{
					keybind = System.Windows.Forms.Keys.None;
				}
				keybindSetting = false;
				NotifyPropertyChanged("KeyBinding");
			}

			public event PropertyChangedEventHandler PropertyChanged;

			internal void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string pName = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(pName));
				}
			}

            
        }

    }

	
}
