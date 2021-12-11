using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_127.Overlay;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Class which handles our WindowChanceListener
	/// </summary>
	class WindowChangeHander
	{
		// Using this class to determine when to start / stop the Keyboard Listener if the Overlay is enabled and in Borderless Mode.
		// Keyboard Listener should only be running when GTA is in foreground

		// to actually detect a change
		private static string LastWindowTitle = "";

		/// <summary>
		/// Method when a WindowChange occured
		/// </summary>
		/// <param name="WindowTitle"></param>
		public static void WindowChangeEvent(string WindowTitle)
		{
			//HelperClasses.Logger.Log("Window Change Event: '" + WindowTitle + "'");

			if ((MySettings.Settings.EnableOverlay || MySettings.Settings.SpecialPatcherEnabled) && GTAOverlay.OverlayMode == GTAOverlay.OverlayModes.Borderless)
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
