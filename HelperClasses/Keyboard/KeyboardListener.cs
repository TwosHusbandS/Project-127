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
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Threading;

namespace Project_127.HelperClasses
{
	// Copied and Adapted from: https://docs.microsoft.com/en-us/archive/blogs/toub/low-level-keyboard-hook-in-c

	// This gets Started / Stoppeds based on the foreground Window in WindowChangeListener
	//		(which gets started / stopped on 2.5 Second poll if GTA V is running in GameStates)



	class KeyboardListener
	{
		// Dont need to mess with KeyUp or see how long something is pressed
		// Since Jumpscript should just "translate" keypressed 1:1 and not act like a auto-spam-script


		public static bool DontStop = false;
		public static bool WantToStop = false;

		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		private static LowLevelKeyboardProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;

		public static bool IsRunning = false;

		public static Thread MyThread;

		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
					GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		public static void Start()
		{
			WantToStop = false;

			if (!KeyboardListener.IsRunning)
			{
				HelperClasses.Logger.Log("Started KeyboardListener");
				KeyboardHandler.JumpKey1Down = false;
				KeyboardHandler.JumpKey2Down = false;
				MyThread = new Thread(_Start);
				MyThread.Start();
			}
		}

		private static void _Start()
		{
			_hookID = SetHook(_proc);
			KeyboardListener.IsRunning = true;
			System.Windows.Forms.Application.Run();
		}

		public static void Stop()
		{
			if (DontStop)
			{
				WantToStop = true;
				return;
			}
			if (KeyboardListener.IsRunning)
			{
				UnhookWindowsHookEx(_hookID);
				KeyboardListener.IsRunning = false;
				MyThread.Abort();
				HelperClasses.Logger.Log("Stopped KeyboardListener");
			}
		}

		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		[StructLayout(LayoutKind.Sequential)]
		public class KBDLLHOOKSTRUCT
		{
			public uint vkCode;
			public uint scanCode;
			public KBDLLHOOKSTRUCTFlags flags;
			public uint time;
			public UIntPtr dwExtraInfo;
		}

		[Flags]
		public enum KBDLLHOOKSTRUCTFlags : uint
		{
			LLKHF_EXTENDED = 0x01,
			LLKHF_INJECTED = 0x10,
			LLKHF_ALTDOWN = 0x20,
			LLKHF_UP = 0x80,
		}


		[STAThread]
		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			bool SurpressKeyEvent = false;
			try
			{
				if (nCode >= 0)
				{
					int vkCode = Marshal.ReadInt32(lParam);
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
				return new IntPtr(-1);
			}

			// Surpresses the Key Event 
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
