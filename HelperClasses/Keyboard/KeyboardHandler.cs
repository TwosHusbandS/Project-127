using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_127.Overlay;

namespace Project_127.HelperClasses
{
	class KeyboardHandler
	{
		public static Keys LastKeyPress = Keys.None;

		[STAThread]
		public static bool KeyboardEvent(Keys pKey)
		{
			// Need this for some Jumpscript thing later
			bool SurpressEventFurther = false;

			// Process Keyboard event here and then call the correct Methods in NoteOverlay and also in the Jumpscript
			//NoteOverlay.KeyBoardEvent(pKey);
			HelperClasses.Logger.Log("Keypress detected: " + pKey);

			if (KeyboardListener.DontStop)
			{
				LastKeyPress = pKey;
			}

			NoteOverlay.KeyBoardEvent(pKey);

			return SurpressEventFurther;
			//return SurpressEventFurther;
		}

		// This fails for 2 Reasons
		// While 2 seconds loop is doing dumb shit
		// Keyboard Event Callbug is failing try/catch because of some STAThread
		// Have a few ideas of how to fix
		public static Keys GetNextKeyPress()
		{
			Keys RtrnKey = Keys.None;

			KeyboardListener.Start();
			KeyboardListener.DontStop = true;

			// Loop While
			// Sleep 10 ms inside the while loop
			// Exit Loop when Key got pressed, or when 2 seconds are over
			HelperClasses.Logger.Log("A");
			int MsPassed = 0;
			while (MsPassed <= 5000) // && LastKeyPress == Keys.None)
			{
				HelperClasses.Logger.Log("B");
				ActualWait(100);
				MsPassed += 100;
			}
			HelperClasses.Logger.Log("C");

			KeyboardListener.DontStop = false;
			if (KeyboardListener.WantToStop)
			{
				KeyboardListener.WantToStop = false;
				KeyboardListener.Stop();
			}

			return RtrnKey;
		}

		public static async void ActualWait(int pMS)
		{
			await Task.Delay(pMS);
		}
	}
}
