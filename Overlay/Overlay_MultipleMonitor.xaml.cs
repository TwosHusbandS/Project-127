using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Project_127.MySettings;
using Project_127.Overlay;

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

		public void TMP()
		{
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
			WindowInteropHelper wndHelper = new WindowInteropHelper(this);

			int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

			exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
			SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

		}


		#region Window styles
		[Flags]
		public enum ExtendedWindowStyles
		{
			// ...
			WS_EX_TOOLWINDOW = 0x00000080,
			// ...
		}

		public enum GetWindowLongFields
		{
			// ...
			GWL_EXSTYLE = (-20),
			// ...
		}

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			int error = 0;
			IntPtr result = IntPtr.Zero;
			// Win32 SetWindowLong doesn't clear error on success
			SetLastError(0);

			if (IntPtr.Size == 4)
			{
				// use SetWindowLong
				Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
				error = Marshal.GetLastWin32Error();
				result = new IntPtr(tempResult);
			}
			else
			{
				// use SetWindowLongPtr
				result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
				error = Marshal.GetLastWin32Error();
			}

			if ((result == IntPtr.Zero) && (error != 0))
			{
				throw new System.ComponentModel.Win32Exception(error);
			}

			return result;
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

		private static int IntPtrToInt32(IntPtr intPtr)
		{
			return unchecked((int)intPtr.ToInt64());
		}

		[DllImport("kernel32.dll", EntryPoint = "SetLastError")]
		public static extern void SetLastError(int dwErrorCode);
		#endregion


		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			this.Width = Settings.OverlayWidth / 2;

			this.Top = Settings.OL_MM_Top;
			this.Left = Settings.OL_MM_Left;

			MyHide();

		}
	}
}
