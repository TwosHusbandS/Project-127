using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_127.HelperClasses.Keyboard
{
	class KeyboardListener
	{
		// Credits:
		// https://github.com/rvknth043/Global-Low-Level-Key-Board-And-Mouse-Hook

		#region pinvoke details

		// usage: system-wide keyboard hook
		private static KeyboardListener _hook = new KeyboardListener();


		public static bool IsRunning = false;
		public static bool DontStop = false;
		public static bool WantToStop = false;

		private enum HookType : int
		{
			WH_JOURNALRECORD = 0,
			WH_JOURNALPLAYBACK = 1,
			WH_KEYBOARD = 2,
			WH_GETMESSAGE = 3,
			WH_CALLWNDPROC = 4,
			WH_CBT = 5,
			WH_SYSMSGFILTER = 6,
			WH_MOUSE = 7,
			WH_HARDWARE = 8,
			WH_DEBUG = 9,
			WH_SHELL = 10,
			WH_FOREGROUNDIDLE = 11,
			WH_CALLWNDPROCRET = 12,
			WH_KEYBOARD_LL = 13,
			WH_MOUSE_LL = 14
		}

		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;

		public struct KBDLLHOOKSTRUCT
		{
			public UInt32 vkCode;
			public UInt32 scanCode;
			public UInt32 flags;
			public UInt32 time;
			public IntPtr extraInfo;
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowsHookEx(
			HookType code, HookProc func, IntPtr instance, int threadID);

		[DllImport("user32.dll")]
		private static extern int UnhookWindowsHookEx(IntPtr hook);

		[DllImport("user32.dll")]
		private static extern int CallNextHookEx(
			IntPtr hook, int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		#endregion

		HookType _hookType = HookType.WH_KEYBOARD_LL;
		IntPtr _hookHandle = IntPtr.Zero;
		HookProc _hookFunction = null;

		// hook method called by system
		private delegate int HookProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		// events
		//public delegate void HookEventHandler(object sender, HookEventArgs e);
		//public event HookEventHandler KeyDown;
		//public event HookEventHandler KeyUp;

		public KeyboardListener()
		{
			_hookFunction = new HookProc(HookCallback);
		}



		public static void Start()
		{
			WantToStop = false;

			if (!IsRunning)
			{
				_hook._Start();
				IsRunning = true;
				HelperClasses.Logger.Log("Started KeyboardListener");
			}
		}

		public static void Stop()
		{
			if (DontStop)
			{
				WantToStop = true;
				return;
			}
			if (IsRunning)
			{
				_hook._Stop();
				IsRunning = false;
				HelperClasses.Logger.Log("Stopped KeyboardListener");
			}
		}


		private void _Start()
		{
			// make sure not already installed
			if (_hookHandle != IntPtr.Zero)
				return;

			// need instance handle to module to create a system-wide hook
			Module[] list = System.Reflection.Assembly.GetExecutingAssembly().GetModules();
			System.Diagnostics.Debug.Assert(list != null && list.Length > 0);

			// install system-wide hook
			_hookHandle = SetWindowsHookEx(_hookType, _hookFunction, Marshal.GetHINSTANCE(list[0]), 0);
		}

		private void _Stop()
		{
			if (_hookHandle != IntPtr.Zero)
			{
				// uninstall system-wide hook
				UnhookWindowsHookEx(_hookHandle);
				_hookHandle = IntPtr.Zero;
			}
		}

		~KeyboardListener()
		{
			Stop();
		}

		// hook function called by system
		//private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
		//private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
		private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
		{
			//HelperClasses.Logger.Log("DEBUG: Keypress - detected",2);
			bool SurpressKeyEvent = false;
			try
			{
				if (code >= 0)
				{
					uint vkCode = lParam.vkCode;
					if (wParam == (IntPtr)WM_KEYDOWN)
					{
						SurpressKeyEvent = KeyboardHandler.KeyboardDownEvent((Keys)vkCode);
					}
					else if (wParam == (IntPtr)WM_KEYUP)
					{
						SurpressKeyEvent = KeyboardHandler.KeyboardUpEvent((Keys)vkCode);
					}
				}
			}
			catch (Exception e)
			{
				HelperClasses.Logger.Log("Try Catch in KeyboardListener KeyEvent Callback Failed: " + e.ToString());
				return -1;
			}

			// Surpresses the Key Event 
			if (SurpressKeyEvent)
			{
				HelperClasses.Logger.Log("Surpressed: " + ((Keys)lParam.vkCode).ToString());
				return -1;
			}
			else
			{
				return CallNextHookEx(_hookHandle, code, wParam, ref lParam);
			}
		}


	}

	// The callback method converts the low-level keyboard data into something more .NET friendly with the HookEventArgs class.

	public class HookEventArgs : EventArgs
	{
		// using Windows.Forms.Keys instead of Input.Key since the Forms.Keys maps
		// to the Win32 KBDLLHOOKSTRUCT virtual key member, where Input.Key does not
		public Keys Key;
		public bool Alt;
		public bool Control;
		public bool Shift;

		public HookEventArgs(UInt32 keyCode)
		{
			// detect what modifier keys are pressed, using 
			// Windows.Forms.Control.ModifierKeys instead of Keyboard.Modifiers
			// since Keyboard.Modifiers does not correctly get the state of the 
			// modifier keys when the application does not have focus
			this.Key = (Keys)keyCode;
			this.Alt = (System.Windows.Forms.Control.ModifierKeys & Keys.Alt) != 0;
			this.Control = (System.Windows.Forms.Control.ModifierKeys & Keys.Control) != 0;
			this.Shift = (System.Windows.Forms.Control.ModifierKeys & Keys.Shift) != 0;
		}
	}

	//	// usage: system-wide keyboard hook
	//	private KeyboardHook _hook;


	//	// install system-wide keyboard hook
	//	_hook = new KeyboardHook();
	//	_hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);

	//// keyboard hook handler
	//void OnHookKeyDown(object sender, HookEventArgs e)
	//	{
	//	}

	//}
	//}
}
