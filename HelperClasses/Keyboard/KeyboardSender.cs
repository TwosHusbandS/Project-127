using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_127.HelperClasses
{
	class KeyboardSender
	{

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

		public static void SendKeyPress(string pWindowName, Keys pKey)
		{
			const int WM_SYSKEYDOWN = 0x0104;
			int VK_SPACE = (int)pKey;

			IntPtr WindowToFind = FindWindow(null, pWindowName);

			PostMessage(WindowToFind, WM_SYSKEYDOWN, VK_SPACE, 0);
		}

		public static void SendKeyBoard(Keys pKey, string pProcessName)
		{
			// We gotta send a keyPress to GTA V for the speedrun shit

		}
	}
}
