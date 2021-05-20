using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_127.Overlay;
using Project_127.MySettings;

namespace Project_127.HelperClasses.Keyboard
{
	/// <summary>
	/// Handling most Keyboard related stuff
	/// </summary>
	class KeyboardHandler
	{
		/// <summary>
		/// Saving last Keypress for Settings Keybinding changes
		/// </summary>
		public static Keys LastKeyPress = Keys.None;


		/// <summary>
		/// When a KeyDown is detected this will get called. Can surpress a Keypress.
		/// </summary>
		/// <param name="pKey"></param>
		/// <returns></returns>
		[STAThread]
		public static bool KeyboardDownEvent(Keys pKey)
		{
			// normally not surpressing Keyboard Input
			bool SurpressEventFurther = false;

			MainWindow.MW.Dispatcher.Invoke((Action)delegate
			{
				try
				{
					// when changing Keybindings
					if (KeyboardListener.DontStop)
					{
						LastKeyPress = pKey;
						SurpressEventFurther = true;
						return;
					}

					// Overlay related stuff
					if (Settings.EnableOverlay)
					{
						if (pKey == Settings.KeyOverlayToggle)
						{
							NoteOverlay.OverlayToggle();
						}

						if (NoteOverlay.IsOverlayVisible())
						{
							if (pKey == Settings.KeyOverlayScrollUp)
							{
								NoteOverlay.OverlayScrollUp();
							}
							if (pKey == Settings.KeyOverlayScrollDown)
							{
								NoteOverlay.OverlayScrollDown();
							}
							if (pKey == Settings.KeyOverlayNotePrev)
							{
								NoteOverlay.OverlayNotePrev();
							}
							if (pKey == Settings.KeyOverlayNoteNext)
							{
								NoteOverlay.OverlayNoteNext();
							}
							if (pKey == Settings.KeyOverlayScrollLeft)
							{
								NoteOverlay.OverlayNoteChapterPrev();
							}
							if (pKey == Settings.KeyOverlayScrollRight)
							{
								NoteOverlay.OverlayNoteChapterNext();
							}
						}
					}
					if (Settings.SpecialPatcherEnabled)
					{
						if (pKey.ToString() == Settings.SpecialPatcherKey) //For now
                        {
							SpecialPatchHandler.patcherEnabled = !SpecialPatchHandler.patcherEnabled;
                        }
                    }
				}
				catch (Exception e)
				{
					HelperClasses.Logger.Log("Try Catch in KeyboardHandler KeyEvent Callback Failed: " + e.ToString());
				}

			});

			return SurpressEventFurther;
		}



		/// <summary>
		/// When a KeyUp Event happens, this gets called
		/// </summary>
		/// <param name="pKey"></param>
		/// <returns></returns>
		[STAThread]
		public static bool KeyboardUpEvent(Keys pKey)
		{
			return false;
		}



		/// <summary>
		/// Async Task to get the next Keypress within a specific Time.
		/// </summary>
		/// <param name="pWaitMilliSeconds"></param>
		/// <returns></returns>
		public static async Task<Keys> GetNextKeyPress(int pWaitMilliSeconds = 2000)
		{
			Keys RtrnKey = Keys.None;

			// start keyboard listener when not already running
			KeyboardListener.Start();
			KeyboardListener.DontStop = true;


			// Checking if time has passed yet or we have a keypress
			int MsPassed = 0;
			while (MsPassed <= pWaitMilliSeconds && LastKeyPress == Keys.None)
			{
				await Task.Delay(50);
				MsPassed += 50;
			}

			// enabling the Keyboard listener to stop again
			KeyboardListener.DontStop = false;

			// if it was meant to stop while we waited for a keypress
			if (KeyboardListener.WantToStop)
			{
				// stop now
				KeyboardListener.WantToStop = false;
				KeyboardListener.Stop();
			}

			// return the LastKeyPress
			RtrnKey = LastKeyPress;
			LastKeyPress = Keys.None;
			return RtrnKey;
		}


	}
}
