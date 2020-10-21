using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{

	// Taken and adapted from: https://stackoverflow.com/a/10280800

	// Gets Started / Stopped from the 2.5 seconds poll of if GTA V is running in GameStates
	// This Starts / Stopps the KeyBoard Listener on Foreground Changed Event

	class WindowChangeListener
	{
		private static WinEventDelegate dele = null;

		public static bool IsRunning = false;

		public static void Stop()
		{
			if (WindowChangeListener.IsRunning)
			{
				Task.Run(() => WindowChangeListener._Stop());
			}
		}

		public static void _Stop()
		{
			IsRunning = false;
		}

		public static void Start()
		{
			if (!WindowChangeListener.IsRunning)
			{
				Task.Run(() => WindowChangeListener._Start());
			}
			WinEventProc((IntPtr)null, 0, (IntPtr)null, 0, 0, 0, 0);
		}

		public static void _Start()
		{
			IsRunning = true;
			dele = new WinEventDelegate(WinEventProc);
			IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
			EventLoop.Run();
		}

		delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

		[DllImport("user32.dll")]
		static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

		private const uint WINEVENT_OUTOFCONTEXT = 0;
		private const uint EVENT_SYSTEM_FOREGROUND = 3;

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		private static string GetActiveWindowTitle()
		{
			const int nChars = 256;
			IntPtr handle = IntPtr.Zero;
			StringBuilder Buff = new StringBuilder(nChars);
			handle = GetForegroundWindow();

			if (GetWindowText(handle, Buff, nChars) > 0)
			{
				return Buff.ToString();
			}
			return null;
		}

		public static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			//if (GetActiveWindowTitle() == "Project - 1.27")
			//if (GetActiveWindowTitle() == "Grand Theft Auto V")
			if (GetActiveWindowTitle() == "WhatsApp")
			{
				MainWindow.MW.Dispatcher.Invoke((Action)delegate
				{
					//KeyboardListener.Start();
				});
			}
			else
			{
				MainWindow.MW.Dispatcher.Invoke((Action)delegate
				{
					//KeyboardListener.Stop();
				});
			}
		}
	}

	public static class EventLoop
	{
		public static void Run()
		{
			MSG msg;

			while (WindowChangeListener.IsRunning)
			{
				if (PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_REMOVE))
				{
					if (msg.Message == WM_QUIT)
						break;

					TranslateMessage(ref msg);
					DispatchMessage(ref msg);
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct MSG
		{
			public IntPtr Hwnd;
			public uint Message;
			public IntPtr WParam;
			public IntPtr LParam;
			public uint Time;
			public System.Drawing.Point Point;
		}

		const uint PM_NOREMOVE = 0;
		const uint PM_REMOVE = 1;

		const uint WM_QUIT = 0x0012;

		[DllImport("user32.dll")]
		private static extern bool PeekMessage(out MSG lpMsg, IntPtr hwnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
		[DllImport("user32.dll")]
		private static extern bool TranslateMessage(ref MSG lpMsg);
		[DllImport("user32.dll")]
		private static extern IntPtr DispatchMessage(ref MSG lpMsg);
	}

}
