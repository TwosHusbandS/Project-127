using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_127.HelperClasses
{
	class WindowChangeHander
	{
		public static void WindowChangeEvent(string WindowTitle)
		{
			if (WindowTitle == GTAOverlay.targetWindow)
			{

				HelperClasses.Logger.Log("'" + GTAOverlay.targetWindow + "' Foreground Change Event detected. It is now in Foreground.");
				KeyboardListener.Start();
				if (Overlay.NoteOverlay.OverlayWasVisible)
				{
					Overlay.NoteOverlay.OverlaySetVisible();
					Overlay.NoteOverlay.OverlayWasVisible = false;
				}
			}
			else
			{
				if (KeyboardListener.IsRunning)
				{
					// So we dont spam log with that.
					HelperClasses.Logger.Log("'" + GTAOverlay.targetWindow + "' no longer foreground");
					KeyboardListener.Stop();
				}
				if (Overlay.NoteOverlay.IsOverlayVisible())
				{
					Overlay.NoteOverlay.OverlayWasVisible = true;
					Overlay.NoteOverlay.OverlaySetInvisible();
				}
			}
		}

	}
}
