using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keys = System.Windows.Forms.Keys;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Jumpscript Logic
	/// </summary>
	class Jumpscript
	{
		/// <summary>
		/// Bool if Jumpscript is running
		/// </summary>
		public static bool IsRunning;

		/// <summary>
		/// (Re-)Starting Jumpscript
		/// </summary>
		public static void StartJumpscript()
		{
			StopJumpscript();

			if (MySettings.Settings.EnableJumpscriptUseCustomScript && HelperClasses.FileHandling.doesFileExist(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript_Custom.ahk"))
			{
				Logger.Log("Custom Jumpscript found and settings enabled, lets use it.");

				ProcessHandler.StartProcess(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript.exe", Globals.ProjectInstallationPathBinary, "P127_Jumpscript_Custom.ahk");
			}
			else
			{
				List<string> myList = new List<string>();
				myList.Add("#NoTrayIcon");
				myList.Add("#SingleInstance Force");
				myList.Add("#MaxHotkeysPerInterval 10000");
				myList.Add("#UseHook");
				myList.Add("#IfWinActive " + Overlay.GTAOverlay.targetWindowBorderless);
				myList.Add(KeyToString(MySettings.Settings.JumpScriptKey1) + "::" + KeyToString(MySettings.Settings.JumpScriptKey2));
				myList.Add(KeyToString(MySettings.Settings.JumpScriptKey2) + "::" + KeyToString(MySettings.Settings.JumpScriptKey1));
				myList.Add("#IfWinActive");
				FileHandling.WriteStringToFileOverwrite(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript.ahk", myList.ToArray());

				ProcessHandler.StartProcess(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript.exe");
			}


			Logger.Log("(Re-)Started Jumpscript");

			IsRunning = true;
		}

		/// <summary>
		/// Stopping Jumpscript.
		/// </summary>
		public static void StopJumpscript()
		{
			HelperClasses.ProcessHandler.KillProcessesContains("P127_Jumpscript");
			HelperClasses.FileHandling.deleteFile(Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\P127_Jumpscript.ahk");
			HelperClasses.Logger.Log("Stopped Jumpscript");
			IsRunning = false;

		}




		/// <summary>
		/// Translating from System.Windows.Forms.Keys to AutoHotkey key strings
		/// </summary>
		/// <param name="pKey"></param>
		/// <returns></returns>
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
			else if (pKey == Keys.OemOpenBrackets)
			{
				rtrn = "ß";
			}
			else if (pKey == Keys.Oem6)
			{
				rtrn = "´";
			}
			else if (pKey == Keys.Oemplus)
			{
				rtrn = "+";
			}
			else if (pKey == Keys.OemQuestion)
			{
				rtrn = "#";
			}
			else if (pKey == Keys.Oem5)
			{
				rtrn = "^";
			}
			else if (pKey == Keys.Oemcomma)
			{
				rtrn = ",";
			}
			else if (pKey == Keys.OemPeriod)
			{
				rtrn = ".";
			}
			else if (pKey == Keys.OemMinus)
			{
				rtrn = "^";
			}
			else if (pKey == Keys.OemBackslash)
			{
				rtrn = "<";
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
			else if (pKey == Keys.Space)
			{
				rtrn = "Space";
			}
			else if (166 <= (int)pKey || (int)pKey <= 172)
			{
				rtrn = rtrn.Replace("Browser", "Browser_");
			}
			else if (173 <= (int)pKey || (int)pKey <= 175)
			{
				rtrn = rtrn.Replace("Volume", "Volume_");
			}
			else if (96 <= (int)pKey || (int)pKey <= 105)
			{
				rtrn = rtrn.Replace("NumPad", "Numpad");
			}


			// Translate this:
			// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netcore-3.1

			// into this:
			// https://www.autohotkey.com/docs/KeyList.htm

			return rtrn;
		}
	}
}
