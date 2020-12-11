using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keys = System.Windows.Forms.Keys;

namespace Project_127.HelperClasses
{
	class Jumpscript
	{
		static Process myJumpscript;

		public static bool IsRunning;

		public static void StartJumpscript()
		{
			StopJumpscript();

			List<string> myList = new List<string>();

			myList.Add("#NoTrayIcon");
			myList.Add("#SingleInstance Force");
			myList.Add("#MaxHotkeysPerInterval 10000");
			myList.Add("#UseHook");
			myList.Add("#IfWinActive " + GTAOverlay.targetWindowFullscreen);
			myList.Add(KeyToString(MySettings.Settings.JumpScriptKey1) + "::" + KeyToString(MySettings.Settings.JumpScriptKey2));
			myList.Add(KeyToString(MySettings.Settings.JumpScriptKey2) + "::" + KeyToString(MySettings.Settings.JumpScriptKey1));
			myList.Add("#IfWinActive");

			HelperClasses.FileHandling.WriteToFile(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript.ahk", myList.ToArray());

			myJumpscript = HelperClasses.ProcessHandler.StartProcess(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript.exe");

			HelperClasses.Logger.Log("(Re-)Started Jumpscript");

			IsRunning = true;
		}

		public static void StopJumpscript()
		{
			if (myJumpscript != null)
			{
				HelperClasses.ProcessHandler.Kill(myJumpscript);
			}

			HelperClasses.FileHandling.deleteFile(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript.ahk");

			HelperClasses.Logger.Log("Stopped Jumpscript");

			IsRunning = false;
		}

		public static string KeyToString(System.Windows.Forms.Keys pKey)
		{
			string rtrn = "";

			rtrn = pKey.ToString().ToLower();

			// TO DO FOR ALL KEYS...this breaks with numpad keys, num keys in general i think

			if (pKey == Keys.Back)
			{
				rtrn = "BackSpace";
			}
			else if (pKey == Keys.Scroll)
			{
				rtrn = "ScrollLock";
			}
			else if (pKey == Keys.PageUp)
			{
				rtrn = "PgUp";
			}
			else if (pKey == Keys.PageDown)
			{
				rtrn = "PgDn";
			}
			else if (96 <= (int)pKey || (int)pKey <= 105)
			{
				rtrn = rtrn.Replace("NumPad", "Numpad");
			}
			else if (pKey == Keys.Multiply)
			{
				rtrn = "NumpadMult";
			}
			else if (pKey == Keys.Divide)
			{
				rtrn = "NumpadDiv";
			}
			else if (pKey == Keys.Add)
			{
				rtrn = "NumpadAdd";
			}
			else if (pKey == Keys.Subtract)
			{
				rtrn = "NumpadSub";
			}
			else if (pKey == Keys.ControlKey)
			{
				rtrn = "Control";
			}
			else if (pKey == Keys.ShiftKey)
			{
				rtrn = "Shift";
			}
			else if (pKey == Keys.RShiftKey)
			{
				rtrn = "RShift";
			}
			else if (pKey == Keys.LShiftKey)
			{
				rtrn = "LShift";
			}
			else if (pKey == Keys.RControlKey)
			{
				rtrn = "RControl";
			}
			else if (pKey == Keys.LControlKey)
			{
				rtrn = "LControl";
			}
			else if (pKey == Keys.RMenu)
			{
				rtrn = "RAlt";
			}
			else if (pKey == Keys.LMenu)
			{
				rtrn = "LAlt";
			}
			else if (166 <= (int)pKey || (int)pKey <= 172)
			{
				rtrn = rtrn.Replace("Browser", "Browser_");
			}
			else if (173 <= (int)pKey || (int)pKey <= 175)
			{
				rtrn = rtrn.Replace("Volume", "Volume_");
			}
			else if (pKey == Keys.MediaNextTrack)
			{
				rtrn = "Media_Next";
			}
			else if (pKey == Keys.MediaPlayPause)
			{
				rtrn = "Media_Play_Pause";
			}
			else if (pKey == Keys.MediaPreviousTrack)
			{
				rtrn = "Media_Prev";
			}
			else if (pKey == Keys.MediaStop)
			{
				rtrn = "Media_Stop";
			}

			// Translate this:
			// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netcore-3.1

			// into this:
			// https://www.autohotkey.com/docs/KeyList.htm

			return rtrn;
		}
	}
}
