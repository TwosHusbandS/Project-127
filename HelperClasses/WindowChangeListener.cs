using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_127.HelperClasses
{

	// Taken and adapted from: https://stackoverflow.com/a/10280800

	// Gets Started / Stopped from the 2.5 seconds poll of if GTA V is running in GameStates
	// This Starts / Stopps the KeyBoard Listener on Foreground Changed Event
	// Using this class to determine when to start / stop the Keyboard Listener if the Overlay is enabled and in Borderless Mode.
	// Keyboard Listener should only be running when GTA is in foreground

	/// <summary>
	/// Window Change Listener class
	/// </summary>
	class WindowChangeListener
	{
		public static bool IsRunning = false;

		/// <summary>
		/// Acual Start Method
		/// </summary>
		public static void _Start()
		{
			dele = new WinEventDelegate(WinEventProc);
			IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
			IsRunning = true;
			Application.Run();
		}

		/// <summary>
		/// Public Start. Checks if its already running before starting
		/// </summary>
		public static void Start()
		{
			if (!WindowChangeListener.IsRunning)
			{
				Task.Run(() => WindowChangeListener._Start());
				WindowChangeHander.WindowChangeEvent(GetActiveWindowTitle());
				HelperClasses.Logger.Log("Started WindowChangeListener");
				//myThread = new Thread(_Start);
				//myThread.Start();
			}
		}

		/// <summary>
		/// Public Stop. Checks if its stopped before trying to stop.
		/// </summary>
		public static void Stop()
		{
			if (WindowChangeListener.IsRunning)
			{
				IsRunning = false;
				//myThread.Abort();
				_Stop();
				HelperClasses.Logger.Log("Stopped WindowChangeListener");
			}
		}

		/// <summary>
		/// Actual Stop Method
		/// </summary>
		public static void _Stop()
		{
			Application.Exit();
		}



		static WinEventDelegate dele = null; //STATIC

		delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

		[DllImport("user32.dll")]
		static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

		private const uint WINEVENT_OUTOFCONTEXT = 0;
		private const uint EVENT_SYSTEM_FOREGROUND = 3;

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		/// <summary>
		/// Gets the most foreground window
		/// </summary>
		/// <returns></returns>
		public static string GetActiveWindowTitle() //STATIC
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

		[STAThread]
		public static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) //STATIC
		{
			//HelperClasses.Logger.Log("DEBUG WINDOW: '" + GetActiveWindowTitle() + "'",2);
			WindowChangeHander.WindowChangeEvent(GetActiveWindowTitle());
		}
	}
}
