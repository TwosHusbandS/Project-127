using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.KeyStuff;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.SettingsStuff;

namespace Project_127.HelperClasses
{
	// Copied and Adapted from: https://docs.microsoft.com/en-us/archive/blogs/toub/low-level-keyboard-hook-in-c

	// This gets Started / Stopped based on the foreground Window in WindowChangeListener
	//		(which gets started / stopped on 2.5 Second poll if GTA V is running in GameStates)


	class KeyboardListener
	{
		// Dont need to mess with KeyUp or see how long something is pressed
		// Since Jumpscript should just "translate" keypressed 1:1 and not act like a auto-spam-script

		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;
		private static LowLevelKeyboardProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;

		public static bool IsRunning = false;

		public static void Start()
		{
			if (!KeyboardListener.IsRunning)
			{
				Task.Run(() => KeyboardListener._Start());
			}
		}

		public static void Stop()
		{
			if (KeyboardListener.IsRunning)
			{
				Task.Run(() => KeyboardListener._Stop());
			}
		}

		public static void _Start()
		{
			_hookID = SetHook(_proc);
			KeyboardListener.IsRunning = true;
			System.Windows.Forms.Application.Run();
			UnhookWindowsHookEx(_hookID);
		}

		public static void _Stop()
		{
			System.Windows.Forms.Application.Exit();
			KeyboardListener.IsRunning = false;
		}

		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
					GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			bool SurpressKeyEvent = false;

			try
			{
				if (nCode >= 0)
				{
					if (wParam == (IntPtr)WM_KEYDOWN)
					{
						int vkCode = Marshal.ReadInt32(lParam);
						SurpressKeyEvent = KeyboardHandler.KeyboardEvent((Keys)vkCode);
					}
				}
			}
			catch (Exception e)
			{
				Globals.DebugPopup(e.ToString());
				HelperClasses.Logger.Log("TRY CATCH IN KEYBOARD CALLBACK: " + e.ToString());
				return new IntPtr(-1);
			}

			if (SurpressKeyEvent)
			{
				return new IntPtr(-1);
			}
			else
			{
				return CallNextHookEx(_hookID, nCode, wParam, lParam);
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);
	}
}
