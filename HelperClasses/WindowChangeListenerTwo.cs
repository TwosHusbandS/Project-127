using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Project_127.HelperClasses
{
	class WindowChangeListenerTwo
	{
		public static DispatcherTimer dp = new DispatcherTimer();
		public static bool IsRunning = false;

		public static void start()
		{
			if (!IsRunning)
			{
				dp.Stop();
				dp = new DispatcherTimer();
				dp = new System.Windows.Threading.DispatcherTimer();
				dp.Tick += new EventHandler(TickAndShit);
				dp.Interval = TimeSpan.FromMilliseconds(50);
				dp.Start();
				TickAndShit();
				IsRunning = true;
			}
		}

		public static void stop()
		{
			if (IsRunning)
			{
				dp.Stop();
				TickAndShit();
				IsRunning = false;
			}
		}

		public static void TickAndShit(object sender = null, EventArgs e = null)
		{
			WindowChangeHander.WindowChangeEvent(WindowChangeListener.GetActiveWindowTitle());
		}
	}
}
