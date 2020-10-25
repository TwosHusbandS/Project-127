using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_127.Overlay;
using Project_127;
using Project_127.SettingsStuff;

namespace Project_127.HelperClasses
{
	class KeyboardHandler
	{
		public static bool JumpKey1Down = false;
		public static bool JumpKey2Down = false;

		public static Keys LastKeyPress = Keys.None;

		[STAThread]
		public static bool KeyboardDownEvent(Keys pKey)
		{
			bool SurpressEventFurther = false;

			MainWindow.MW.Dispatcher.Invoke((Action)delegate
			{
				try
				{
					if (KeyboardListener.DontStop)
					{
						LastKeyPress = pKey;
						SurpressEventFurther = true;
					}

					// Those are all if and not else if because users might be stupid and use the same key for multiple things

					//if (Settings.EnableAutoStartJumpScript)
					//{
					//	if (pKey == Settings.JumpScriptKey1)
					//	{
					//		SurpressEventFurther = true;
					//		if (!JumpKey1Down)
					//		{
					//			HelperClasses.KeyboardSender.SendKeyPress(GTAOverlay.targetWindow, Settings.JumpScriptKey2);
					//		}
					//		JumpKey1Down = true;
					//	}
					//	if (pKey == Settings.JumpScriptKey2)
					//	{
					//		SurpressEventFurther = true;
					//		if (!JumpKey2Down)
					//		{
					//			HelperClasses.KeyboardSender.SendKeyPress(GTAOverlay.targetWindow, Settings.JumpScriptKey1);
					//		}
					//		JumpKey2Down = true;
					//	}
					//}


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
							if (pKey == Settings.KeyOverlayScrollLeft)
							{
								NoteOverlay.OverlayNoteChapterPrev();
							}
							if (pKey == Settings.KeyOverlayScrollRight)
							{
								NoteOverlay.OverlayNoteChapterNext();
							}
							if (pKey == Settings.KeyOverlayNotePrev)
							{
								NoteOverlay.OverlayNotePrev();
							}
							if (pKey == Settings.KeyOverlayNoteNext)
							{
								NoteOverlay.OverlayNoteNext();
							}
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





		[STAThread]
		public static void KeyboardUpEvent(Keys pKey)
		{
			if (Settings.EnableAutoStartJumpScript)
			{
				if (pKey == Settings.JumpScriptKey1)
				{
					JumpKey1Down = false;
				}
				else if (pKey == Settings.JumpScriptKey2)
				{
					JumpKey2Down = false;
				}
			}
		}




		public static async Task<Keys> GetNextKeyPress(int pWaitMilliSeconds = 2000)
		{
			Keys RtrnKey = Keys.None;

			KeyboardListener.Start();
			KeyboardListener.DontStop = true;

			int MsPassed = 0;
			while (MsPassed <= pWaitMilliSeconds && LastKeyPress == Keys.None)
			{
				await Task.Delay(50);
				MsPassed += 50;
			}

			KeyboardListener.DontStop = false;
			if (KeyboardListener.WantToStop)
			{
				KeyboardListener.WantToStop = false;
				KeyboardListener.Stop();
			}

			RtrnKey = LastKeyPress;
			LastKeyPress = Keys.None;

			return RtrnKey;
		}


	}
}
