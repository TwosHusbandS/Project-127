using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_127.Overlay;

namespace Project_127.HelperClasses
{
	class WindowChangeHander
	{
		private static string LastWindowTitle = "";

		public static void WindowChangeEvent(string WindowTitle)
		{
			//HelperClasses.Logger.Log("DEBUG: '" + WindowTitle + "'", 2);

			if (MySettings.Settings.EnableOverlay && GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.Borderless)
			{
				if (WindowTitle == GTAOverlay.targetWindowBorderless && LastWindowTitle != GTAOverlay.targetWindowBorderless)
				{
					HelperClasses.Logger.Log("'" + GTAOverlay.targetWindowBorderless + "' Foreground Change Event detected. It is now in Foreground.");
					HelperClasses.Keyboard.KeyboardListener.Start();
					if (Overlay.NoteOverlay.OverlayWasVisible)
					{
						Overlay.NoteOverlay.OverlaySetVisible();
						Overlay.NoteOverlay.OverlayWasVisible = false;
					}
				}
				else if (WindowTitle != GTAOverlay.targetWindowBorderless)
				{
					if (HelperClasses.Keyboard.KeyboardListener.IsRunning)
					{
						// So we dont spam log with that.
						HelperClasses.Logger.Log("'" + GTAOverlay.targetWindowBorderless + "' no longer foreground");
						HelperClasses.Keyboard.KeyboardListener.Stop();
					}
					if (Overlay.NoteOverlay.IsOverlayVisible())
					{
						Overlay.NoteOverlay.OverlayWasVisible = true;
						Overlay.NoteOverlay.OverlaySetInvisible();
					}
				}
			}

			LastWindowTitle = WindowTitle;
		}

	}
}
