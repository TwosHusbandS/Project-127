using AutoHotkey.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_127.HelperClasses.Keyboard
{
	class LetsDoThis
	{
		public static void TMP()
		{
			var ahk = AutoHotkeyEngine.Instance;
		}


		/*
		static Input myInput;

		public static void Init()
		{
			myInput = new Input();
			myInput.KeyboardFilterMode = KeyboardFilterMode.All;
			myInput.KeyPressDelay = 5;
			myInput.Load();
		}

		public static void Unload()
		{
			myInput.Unload();
		}

		public static void Send() // Keys myKey)
		{
			myInput.SendKey(Keys.W, KeyState.Down);  // Presses the ENTER key down and then up (this constitutes a key press)
			System.Threading.Thread.Sleep(10);
			myInput.SendKey(Keys.W, KeyState.Up);  // Presses the ENTER key down and then up (this constitutes a key press)
		}
		*/
	}
}
