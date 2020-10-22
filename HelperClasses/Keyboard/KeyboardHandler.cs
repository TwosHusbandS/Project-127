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
			// Dont do the actual Processing inside the NoteOverlay and the Jumpscript, because they might not be init
			NoteOverlay.KeyBoardEvent(pKey);

			if (KeyboardListener.DontStop)
			{
				LastKeyPress = pKey;
			}


			return SurpressEventFurther;
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
