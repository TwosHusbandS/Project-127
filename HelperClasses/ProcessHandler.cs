using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Helper Class for ProcessHandler 
	/// </summary>
	public static class ProcessHandler
	{
		/// <summary>
		/// Kills all Rockstar / GTA / Social Club related processes
		/// </summary>
		public static void KillRockstarProcesses()
		{
			// TODO CTRLF add other ProcessNames
			string[] ProcessNames = new string[] { "gta", "subprocess" };
			KillProcessContains(ProcessNames);
		}

		/// <summary>
		/// Kills all Steam related processes
		/// </summary>
		public static void KillSteamProcesses()
		{
			// TODO CTRLF add other ProcessNames
			KillProcessContains("steam");
		}

		/// <summary>
		/// Kills all Steam and Rockstar / GTA / Social Club related processes
		/// </summary>
		public static void KillRelevantProcesses()
		{
			KillRockstarProcesses();
			KillSteamProcesses();
		}

		/// <summary>
		/// Checks if One Process is running
		/// </summary>
		/// <param name="pProcessName"></param>
		/// <returns></returns>
		public static bool IsProcessRunning(string pProcessName)
		{
			bool rtrn = false;
			Process[] MyProcesses = Process.GetProcesses();
			foreach (Process MyProcess in MyProcesses)
			{
				if (MyProcess.ProcessName.ToLower() == pProcessName.ToLower().TrimEnd(".exe"))
				{
					return true;
				}
			}
			return rtrn;
		}

		/// <summary>
		/// Checks if steam itself is running
		/// </summary>
		/// <returns></returns>
		public static bool IsSteamRunning()
		{
			return IsProcessRunning("steam");
		}

		/// <summary>
		/// Check if gta itself is running
		/// </summary>
		/// <returns></returns>
		public static bool IsGtaRunning()
		{
			return IsProcessRunning("gta5.exe");
		}

		/// <summary>
		/// Kills all processes with that name
		/// </summary>
		/// <param name="pProcessName"></param>
		public static void KillProcess(string pProcessName)
		{
			Process[] MyProcesses = Process.GetProcesses();
			foreach (Process MyProcess in MyProcesses)
			{
				if (MyProcess.ProcessName.ToLower().TrimEnd(".exe") == pProcessName.ToLower().TrimEnd(".exe"))
				{
					MyProcess.MyKill();
				}
			}
		}

		/// <summary>
		/// Kills all processes with one of those names from the string array
		/// </summary>
		/// <param name="pProcessNames"></param>
		public static void KillProcess(string[] pProcessNames)
		{
			Process[] MyProcesses = Process.GetProcesses();
			foreach (Process MyProcess in MyProcesses)
			{
				foreach (String pProcessName in pProcessNames)
				{
					if (MyProcess.ProcessName.ToLower().TrimEnd(".exe") == pProcessName.ToLower().TrimEnd(".exe"))
					{
						MyProcess.MyKill();
					}
				}
			}
		}

		/// <summary>
		/// Kills all processes whose names contain with that parameter string
		/// </summary>
		/// <param name="pProcessName"></param>
		public static void KillProcessContains(string pProcessName)
		{
			Process[] MyProcesses = Process.GetProcesses();
			foreach (Process MyProcess in MyProcesses)
			{
				if (MyProcess.ProcessName.ToLower().TrimEnd(".exe").Contains(pProcessName.ToLower().TrimEnd(".exe")))
				{
					MyProcess.MyKill();
				}
			}
		}

		/// <summary>
		/// Kills all processes whose names contain one of the parameters (string array)
		/// </summary>
		/// <param name="pProcessNames"></param>
		public static void KillProcessContains(string[] pProcessNames)
		{
			Process[] MyProcesses = Process.GetProcesses();
			foreach (Process MyProcess in MyProcesses)
			{
				foreach (string pProcessName in pProcessNames)
				{
					if (MyProcess.ProcessName.ToLower().TrimEnd(".exe").Contains(pProcessName.ToLower().TrimEnd(".exe")))
					{
						MyProcess.MyKill();
					}
				}
			}
		}

		// Extension Method for Processes to log all Killed processes
		public static void MyKill(this Process pProcess)
		{
			Logger.Log("Trying to kill Process '" + pProcess.ProcessName + "'",1);
			try
			{
				pProcess.Kill();
				Logger.Log("Killed Process '" + pProcess.ProcessName + "'",1);
			}
			catch
			{
				Logger.Log("Failed to kill Process '" + pProcess.ProcessName + "'",1);
			}
		}

		/// <summary>
		/// Starts a Process
		/// </summary>
		/// <param name="pFilepath"></param>
		/// <param name="runAsAdmin"></param>
		/// <param name="waitForexit"></param>
		public static void StartProcess(string pFilepath, string CommandLineArguments, bool runAsAdmin, bool waitForExit)
		{
			if (FileHandling.doesFileExist(pFilepath))
			{
			Process proc = new Process();
			proc.StartInfo.FileName = pFilepath;
			if (string.IsNullOrEmpty(CommandLineArguments))
				{
					proc.StartInfo.Arguments = CommandLineArguments;
				}
			if (runAsAdmin)
				{
				proc.StartInfo.Verb = "runas";
				}
			proc.Start();
			if (waitForExit)
				{
					proc.WaitForExit();
				}
			}
		}

	} // End of Class
} // End of Namespace
