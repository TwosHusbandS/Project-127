using AutoHotkey.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
	class Jumpscript
	{
		static AutoHotkeyEngine AHEI;

		static bool IsRunning = false;
		static bool IsInit = false;

		public static void InitJumpscript()
		{
			if (!IsInit)
			{
				AHEI = AutoHotkeyEngine.Instance;
				IsInit = true;
				HelperClasses.Logger.Log("Inited Jumpscript");

				if (LauncherLogic.GameState == LauncherLogic.GameStates.Running || GTAOverlay.DebugMode)
				{
					StartJumpscript();
				}
			}
		}

		public static void DisposeJumpscript()
		{
			if (IsInit)
			{
				StopJumpscript();
				AHEI.Terminate();
				AHEI = null;
				IsInit = false;
				HelperClasses.Logger.Log("Disposed Jumpscript");
			}
		}

		public static void StartJumpscript()
		{
			if (IsInit)
			{
				StopJumpscript();

				AHEI.UnSuspend();

				AHEI.Reset();

				string Command = "";
				Command += "#SingleInstance Force";
				Command += "\n\r#MaxHotkeysPerInterval 10000";
				Command += "\n\r#UseHook";
				Command += "\n\r#IfWinActive " + GTAOverlay.targetWindow;
				Command += "\n\r" + KeyToString(MySettings.Settings.JumpScriptKey1) + "::" + KeyToString(MySettings.Settings.JumpScriptKey2);
				Command += "\n\r" + KeyToString(MySettings.Settings.JumpScriptKey2) + "::" + KeyToString(MySettings.Settings.JumpScriptKey1);
				Command += "\n\r#IfWinActive";

				AHEI.ExecRaw(Command);

				IsRunning = true;

				HelperClasses.Logger.Log("(Re-)Started Jumpscript");
			}
		}

		public static void StopJumpscript()
		{
			if (IsInit)
			{
				AHEI.Suspend();
				IsRunning = false;
				HelperClasses.Logger.Log("Stopped Jumpscript");
			}
		}

		public static string KeyToString(System.Windows.Forms.Keys pKey)
		{
			string rtrn = "";

			rtrn = pKey.ToString().ToLower();

			// TO DO FOR ALL KEYS...this breaks with numpad keys, num keys in general i think

			// Translate this:
			// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netcore-3.1

			// into this:
			// https://www.autohotkey.com/docs/KeyList.htm

			return rtrn;
		}
	}
}
