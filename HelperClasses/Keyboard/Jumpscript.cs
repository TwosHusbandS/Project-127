using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
	class Jumpscript
	{
		public static void StartJumpscript()
		{
			StopJumpscript();

			List<string> myList = new List<string>();

			myList.Add("#NoTrayIcon");
			myList.Add("#SingleInstance Force");
			myList.Add("#MaxHotkeysPerInterval 10000");
			myList.Add("#UseHook");
			myList.Add("#IfWinActive " + GTAOverlay.targetWindow);
			myList.Add(KeyToString(MySettings.Settings.JumpScriptKey1) + "::" + KeyToString(MySettings.Settings.JumpScriptKey2));
			myList.Add(KeyToString(MySettings.Settings.JumpScriptKey2) + "::" + KeyToString(MySettings.Settings.JumpScriptKey1));
			myList.Add("#IfWinActive");

			HelperClasses.FileHandling.WriteToFile(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\P127_Jumpscript.ahk", myList.ToArray());

			HelperClasses.ProcessHandler.StartProcess(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\P127_Jumpscript.exe");

			HelperClasses.Logger.Log("(Re-)Started Jumpscript");
		}

		public static void StopJumpscript()
		{
			HelperClasses.ProcessHandler.KillProcesses("P127_Jumpscript");

			HelperClasses.FileHandling.deleteFile(Globals.ProjectInstallationPath.TrimEnd('\\') + @"\P127_Jumpscript.ahk");

			HelperClasses.Logger.Log("Stopped Jumpscript");
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
