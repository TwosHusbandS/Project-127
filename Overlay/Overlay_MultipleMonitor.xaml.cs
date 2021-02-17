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

namespace Project_127.Overlay
{
	/// <summary>
	/// Interaction logic for Overlay_MultipleMonitor.xaml
	/// </summary>
	public partial class Overlay_MultipleMonitor : Window
	{

		private int _width;

		/// <summary>
		/// 
		/// </summary>
		public int trueWidth
        {
            get
            {
				return _width;
            }
            set
            {
				_width = value;
				this.Width = _width / DpiScaleX();
			}
        }

		/// <summary>
		/// 
		/// </summary>
		public int trueHeight
		{
			get
			{
				return (int)(this.ActualHeight * DpiScaleY());
			}
		}

		/// <summary>
		/// Constructor Of Multi Monitor Overlay WPF Window
		/// </summary>
		public Overlay_MultipleMonitor()
		{

			InitializeComponent();
		}

		/// <summary>
		/// Moving Window around works
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		/// <summary>
		/// Making our WPF Window show
		/// </summary>
		public void MyShow()
		{
			this.border_main.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// If our Window is Visible / Displayed
		/// </summary>
		/// <returns></returns>
		public bool IsDisplayed()
		{
			if (this.border_main.Visibility == Visibility.Visible)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Hiding our WPF Window
		/// </summary>
		public void MyHide()
		{
			this.border_main.Visibility = Visibility.Hidden;
		}

		/// <summary>
		/// Saving our Position when this is being closed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var loc = getWindowPosition();
			Settings.OL_MM_Top = loc.Y;
			Settings.OL_MM_Left = loc.X;
		}


		/// <summary>
		/// Window Styles on Window Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WindowInteropHelper wndHelper = new WindowInteropHelper(this);

			int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

			exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
			SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

		}

		/// <summary>
		/// Re-setting Window Margins, then hiding our Window on Init
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			//this.Top = Settings.OL_MM_Top;
			//this.Left = Settings.OL_MM_Left;
			setWindowPosition((int)Settings.OL_MM_Left, (int)Settings.OL_MM_Top);

			MyHide();
		}

		private void Window_LocationChanged(object sender, EventArgs e)
		{
			try
			{
				var s = getWindowPosition();
				var ps = System.Windows.PresentationSource.FromVisual(this);


				this.Width = _width / DpiScaleX(); ;
			}
			catch { }
		}

		private double DpiScaleX()
		{
			var dpi = VisualTreeHelper.GetDpi(this);
			return dpi.DpiScaleX;
		}

		private double DpiScaleY()
		{
			var dpi = VisualTreeHelper.GetDpi(this);
			return dpi.DpiScaleY;
		}


		public static void ResetPosition()
		{
			if (Settings.EnableOverlay &&
			GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.MultiMonitor &&
			MainWindow.OL_MM != null)
			{

				MainWindow.OL_MM.Left = 0;
				MainWindow.OL_MM.Top = 0;
				NoteOverlay.OverlaySetVisible();
			}
			else
			{
				Settings.OL_MM_Left = 0;
				Settings.OL_MM_Top = 0;
			}
		}

		// Whatever this sorcery is below this shit...

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

		#region nativePosition

		/// <summary>
		///     Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered
		///     according to their appearance on the screen. The topmost window receives the highest rank and is the first window
		///     in the Z order.
		///     <para>See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545%28v=vs.85%29.aspx for more information.</para>
		/// </summary>
		/// <param name="hWnd">C++ ( hWnd [in]. Type: HWND )<br />A handle to the window.</param>
		/// <param name="hWndInsertAfter">
		///     C++ ( hWndInsertAfter [in, optional]. Type: HWND )<br />A handle to the window to precede the positioned window in
		///     the Z order. This parameter must be a window handle or one of the following values.
		///     <list type="table">
		///     <itemheader>
		///         <term>HWND placement</term><description>Window to precede placement</description>
		///     </itemheader>
		///     <item>
		///         <term>HWND_BOTTOM ((HWND)1)</term>
		///         <description>
		///         Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost
		///         window, the window loses its topmost status and is placed at the bottom of all other windows.
		///         </description>
		///     </item>
		///     <item>
		///         <term>HWND_NOTOPMOST ((HWND)-2)</term>
		///         <description>
		///         Places the window above all non-topmost windows (that is, behind all topmost windows). This
		///         flag has no effect if the window is already a non-topmost window.
		///         </description>
		///     </item>
		///     <item>
		///         <term>HWND_TOP ((HWND)0)</term><description>Places the window at the top of the Z order.</description>
		///     </item>
		///     <item>
		///         <term>HWND_TOPMOST ((HWND)-1)</term>
		///         <description>
		///         Places the window above all non-topmost windows. The window maintains its topmost position
		///         even when it is deactivated.
		///         </description>
		///     </item>
		///     </list>
		///     <para>For more information about how this parameter is used, see the following Remarks section.</para>
		/// </param>
		/// <param name="X">C++ ( X [in]. Type: int )<br />The new position of the left side of the window, in client coordinates.</param>
		/// <param name="Y">C++ ( Y [in]. Type: int )<br />The new position of the top of the window, in client coordinates.</param>
		/// <param name="cx">C++ ( cx [in]. Type: int )<br />The new width of the window, in pixels.</param>
		/// <param name="cy">C++ ( cy [in]. Type: int )<br />The new height of the window, in pixels.</param>
		/// <param name="uFlags">
		///     C++ ( uFlags [in]. Type: UINT )<br />The window sizing and positioning flags. This parameter can be a combination
		///     of the following values.
		///     <list type="table">
		///     <itemheader>
		///         <term>HWND sizing and positioning flags</term>
		///         <description>Where to place and size window. Can be a combination of any</description>
		///     </itemheader>
		///     <item>
		///         <term>SWP_ASYNCWINDOWPOS (0x4000)</term>
		///         <description>
		///         If the calling thread and the thread that owns the window are attached to different input
		///         queues, the system posts the request to the thread that owns the window. This prevents the calling
		///         thread from blocking its execution while other threads process the request.
		///         </description>
		///     </item>
		///     <item>
		///         <term>SWP_DEFERERASE (0x2000)</term>
		///         <description>Prevents generation of the WM_SYNCPAINT message. </description>
		///     </item>
		///     <item>
		///         <term>SWP_DRAWFRAME (0x0020)</term>
		///         <description>Draws a frame (defined in the window's class description) around the window.</description>
		///     </item>
		///     <item>
		///         <term>SWP_FRAMECHANGED (0x0020)</term>
		///         <description>
		///         Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message
		///         to the window, even if the window's size is not being changed. If this flag is not specified,
		///         WM_NCCALCSIZE is sent only when the window's size is being changed
		///         </description>
		///     </item>
		///     <item>
		///         <term>SWP_HIDEWINDOW (0x0080)</term><description>Hides the window.</description>
		///     </item>
		///     <item>
		///         <term>SWP_NOACTIVATE (0x0010)</term>
		///         <description>
		///         Does not activate the window. If this flag is not set, the window is activated and moved to
		///         the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
		///         parameter).
		///         </description>
		///     </item>
		///     <item>
		///         <term>SWP_NOCOPYBITS (0x0100)</term>
		///         <description>
		///         Discards the entire contents of the client area. If this flag is not specified, the valid
		///         contents of the client area are saved and copied back into the client area after the window is sized or
		///         repositioned.
		///         </description>
		///     </item>
		///     <item>
		///         <term>SWP_NOMOVE (0x0002)</term>
		///         <description>Retains the current position (ignores X and Y parameters).</description>
		///     </item>
		///     <item>
		///         <term>SWP_NOOWNERZORDER (0x0200)</term>
		///         <description>Does not change the owner window's position in the Z order.</description>
		///     </item>
		///     <item>
		///         <term>SWP_NOREDRAW (0x0008)</term>
		///         <description>
		///         Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies
		///         to the client area, the nonclient area (including the title bar and scroll bars), and any part of the
		///         parent window uncovered as a result of the window being moved. When this flag is set, the application
		///         must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
		///         </description>
		///     </item>
		///     <item>
		///         <term>SWP_NOREPOSITION (0x0200)</term><description>Same as the SWP_NOOWNERZORDER flag.</description>
		///     </item>
		///     <item>
		///         <term>SWP_NOSENDCHANGING (0x0400)</term>
		///         <description>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</description>
		///     </item>
		///     <item>
		///         <term>SWP_NOSIZE (0x0001)</term>
		///         <description>Retains the current size (ignores the cx and cy parameters).</description>
		///     </item>
		///     <item>
		///         <term>SWP_NOZORDER (0x0004)</term>
		///         <description>Retains the current Z order (ignores the hWndInsertAfter parameter).</description>
		///     </item>
		///     <item>
		///         <term>SWP_SHOWWINDOW (0x0040)</term><description>Displays the window.</description>
		///     </item>
		///     </list>
		/// </param>
		/// <returns><c>true</c> or nonzero if the function succeeds, <c>false</c> or zero otherwise or if function fails.</returns>
		/// <remarks>
		///     <para>
		///     As part of the Vista re-architecture, all services were moved off the interactive desktop into Session 0.
		///     hwnd and window manager operations are only effective inside a session and cross-session attempts to manipulate
		///     the hwnd will fail. For more information, see The Windows Vista Developer Story: Application Compatibility
		///     Cookbook.
		///     </para>
		///     <para>
		///     If you have changed certain window data using SetWindowLong, you must call SetWindowPos for the changes to
		///     take effect. Use the following combination for uFlags: SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER |
		///     SWP_FRAMECHANGED.
		///     </para>
		///     <para>
		///     A window can be made a topmost window either by setting the hWndInsertAfter parameter to HWND_TOPMOST and
		///     ensuring that the SWP_NOZORDER flag is not set, or by setting a window's position in the Z order so that it is
		///     above any existing topmost windows. When a non-topmost window is made topmost, its owned windows are also made
		///     topmost. Its owners, however, are not changed.
		///     </para>
		///     <para>
		///     If neither the SWP_NOACTIVATE nor SWP_NOZORDER flag is specified (that is, when the application requests that
		///     a window be simultaneously activated and its position in the Z order changed), the value specified in
		///     hWndInsertAfter is used only in the following circumstances.
		///     </para>
		///     <list type="bullet">
		///     <item>Neither the HWND_TOPMOST nor HWND_NOTOPMOST flag is specified in hWndInsertAfter. </item>
		///     <item>The window identified by hWnd is not the active window. </item>
		///     </list>
		///     <para>
		///     An application cannot activate an inactive window without also bringing it to the top of the Z order.
		///     Applications can change an activated window's position in the Z order without restrictions, or it can activate
		///     a window and then move it to the top of the topmost or non-topmost windows.
		///     </para>
		///     <para>
		///     If a topmost window is repositioned to the bottom (HWND_BOTTOM) of the Z order or after any non-topmost
		///     window, it is no longer topmost. When a topmost window is made non-topmost, its owners and its owned windows
		///     are also made non-topmost windows.
		///     </para>
		///     <para>
		///     A non-topmost window can own a topmost window, but the reverse cannot occur. Any window (for example, a
		///     dialog box) owned by a topmost window is itself made a topmost window, to ensure that all owned windows stay
		///     above their owner.
		///     </para>
		///     <para>
		///     If an application is not in the foreground, and should be in the foreground, it must call the
		///     SetForegroundWindow function.
		///     </para>
		///     <para>
		///     To use SetWindowPos to bring a window to the top, the process that owns the window must have
		///     SetForegroundWindow permission.
		///     </para>
		/// </remarks>
		[DllImport("user32.dll", SetLastError = true)]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

		/// <summary>
		///     Special window handles
		/// </summary>
		public enum SpecialWindowHandles
		{
			// ReSharper disable InconsistentNaming
			/// <summary>
			///     Places the window at the top of the Z order.
			/// </summary>
			HWND_TOP = 0,
			/// <summary>
			///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
			/// </summary>
			HWND_BOTTOM = 1,
			/// <summary>
			///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
			/// </summary>
			HWND_TOPMOST = -1,
			/// <summary>
			///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
			/// </summary>
			HWND_NOTOPMOST = -2
			// ReSharper restore InconsistentNaming
		}

		[Flags]
		public enum SetWindowPosFlags : uint
		{
			// ReSharper disable InconsistentNaming

			/// <summary>
			///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
			/// </summary>
			SWP_ASYNCWINDOWPOS = 0x4000,

			/// <summary>
			///     Prevents generation of the WM_SYNCPAINT message.
			/// </summary>
			SWP_DEFERERASE = 0x2000,

			/// <summary>
			///     Draws a frame (defined in the window's class description) around the window.
			/// </summary>
			SWP_DRAWFRAME = 0x0020,

			/// <summary>
			///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
			/// </summary>
			SWP_FRAMECHANGED = 0x0020,

			/// <summary>
			///     Hides the window.
			/// </summary>
			SWP_HIDEWINDOW = 0x0080,

			/// <summary>
			///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
			/// </summary>
			SWP_NOACTIVATE = 0x0010,

			/// <summary>
			///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
			/// </summary>
			SWP_NOCOPYBITS = 0x0100,

			/// <summary>
			///     Retains the current position (ignores X and Y parameters).
			/// </summary>
			SWP_NOMOVE = 0x0002,

			/// <summary>
			///     Does not change the owner window's position in the Z order.
			/// </summary>
			SWP_NOOWNERZORDER = 0x0200,

			/// <summary>
			///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
			/// </summary>
			SWP_NOREDRAW = 0x0008,

			/// <summary>
			///     Same as the SWP_NOOWNERZORDER flag.
			/// </summary>
			SWP_NOREPOSITION = 0x0200,

			/// <summary>
			///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
			/// </summary>
			SWP_NOSENDCHANGING = 0x0400,

			/// <summary>
			///     Retains the current size (ignores the cx and cy parameters).
			/// </summary>
			SWP_NOSIZE = 0x0001,

			/// <summary>
			///     Retains the current Z order (ignores the hWndInsertAfter parameter).
			/// </summary>
			SWP_NOZORDER = 0x0004,

			/// <summary>
			///     Displays the window.
			/// </summary>
			SWP_SHOWWINDOW = 0x0040,

			// ReSharper restore InconsistentNaming
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}

		private bool setWindowPosition(int x, int y)
        {
			return SetWindowPos(new WindowInteropHelper(this).Handle, IntPtr.Zero, x, y, 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER);
        }

		private Point getWindowPosition()
        {
			RECT r;
			if (!GetWindowRect(new WindowInteropHelper(this).Handle, out r))
            {
				return new Point(-1,-1);
            }
			return new Point(r.Left, r.Top);

        }
		#endregion
	}
}
