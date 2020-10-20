using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project_127.Overlay;

namespace Project_127.KeyStuff
{
	class KeyboardHandler
	{
		public static bool KeyboardEvent(Keys pKey)
		{
			// Need this for some Jumpscript thing later
			bool SurpressEventFurther = false;

			// Process Keyboard event here and then call the correct Methods in NoteOverlay and also in the Jumpscript
			NoteOverlay.KeyBoardEvent(pKey);

			return NoteOverlay.NO.RunKeyboardListenerSurpress;
			//return SurpressEventFurther;
		}
	}
}
