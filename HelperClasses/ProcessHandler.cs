using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
			string ProcessNames = "gta";
			KillProcessesContains(ProcessNames);
		}

		/// <summary>
		/// Kills all Steam related processes
		/// </summary>
		public static void KillSteamProcesses()
		{
			// TODO CTRLF add other ProcessNames
			KillProcessesContains("steam");
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
			if ((GetProcesses(pProcessName)).Length > 0)
			{
				return true;
			}
			return false;
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
		/// Gets all Processes with a specific name
		/// </summary>
		/// <param name="pProcessName"></param>
		/// <returns></returns>
		public static Process[] GetProcesses(string pProcessName)
		{
			List<Process> ProcessList = new List<Process>();
			Process[] Processes = Process.GetProcesses();
			for (int i = 0; i <= Processes.Length - 1; i++)
			{
				if (Processes[i].ProcessName.ToLower() == pProcessName.ToLower().TrimEnd(".exe"))
				{
					ProcessList.Add(Processes[i]);
				}
			}
			return ProcessList.ToArray();
		}

		/// <summary>
		/// Gets all Processes which contain a specific name
		/// </summary>
		/// <param name="pProcessName"></param>
		/// <returns></returns>
		public static Process[] GetProcessesContains(string pProcessName)
		{
			List<Process> ProcessList = new List<Process>();
			Process[] Processes = Process.GetProcesses();
			for (int i = 0; i <= Processes.Length - 1; i++)
			{
				if (Processes[i].ProcessName.ToLower().Contains(pProcessName.ToLower().TrimEnd(".exe")))
				{
					ProcessList.Add(Processes[i]);
				}
			}
			return ProcessList.ToArray();
		}



		/// <summary>
		/// Kills all processes with that name
		/// </summary>
		/// <param name="pProccessName"></param>
		public static void KillProcesses(string pProccessName)
		{
			foreach (Process myP in GetProcesses(pProccessName))
			{
				Kill(myP);
			}
		}


		/// <summary>
		/// Kills all processes which contain that string
		/// </summary>
		/// <param name="pProccessName"></param>
		public static void KillProcessesContains(string pProccessName)
		{
			foreach (Process myP in GetProcessesContains(pProccessName))
			{
				Kill(myP);
			}
		}


		/// <summary>
		/// Extension Method for Processes to log all Killed processes
		/// </summary>
		/// <param name="pProcess"></param>
		public static void Kill(this Process pProcess)
		{
			Logger.Log("Trying to kill Process '" + pProcess.ProcessName + "'", 1);
			try
			{
				pProcess.Kill();
				Logger.Log("Killed Process '" + pProcess.ProcessName + "'", 1);
			}
			catch
			{
				Logger.Log("Failed to kill Process '" + pProcess.ProcessName + "'", 1);
			}
		}

		/// <summary>
		/// Starts a process
		/// </summary>
		/// <param name="pFilepath"></param>
		/// <param name="pWorkingDir"></param>
		/// <param name="pCommandLineArguments"></param>
		/// <param name="runAsAdmin"></param>
		/// <param name="waitForExit"></param>
		public static void StartProcess(string pFilepath, string pWorkingDir = null, string pCommandLineArguments = null, bool useShellExecute = false, bool runAsAdmin = false, bool waitForExit = false)
		{
			// TO DO...THIS MIGHT BE BROKEN WITH CMDL ARGS WITH CONTAIN SPACES...NEED TO DO THIS MANUALLY IN THE METHOD WHICH CALLS IT FOR NOW
			if (FileHandling.doesFileExist(pFilepath))
			{
				Process proc = new Process();
				proc.StartInfo.FileName = pFilepath;
				if (!string.IsNullOrEmpty(pCommandLineArguments))
				{
					proc.StartInfo.Arguments = pCommandLineArguments;
				}
				if (!string.IsNullOrEmpty(pWorkingDir))
				{
					proc.StartInfo.WorkingDirectory = pWorkingDir;
				}
				if (runAsAdmin)
				{
					proc.StartInfo.Verb = "runas";
				}

				proc.StartInfo.UseShellExecute = useShellExecute;
				// Lets see if this works
				//proc.ProcessorAffinity = (IntPtr)0xFFFFFFFF;

				proc.Start();
				if (waitForExit)
				{
					proc.WaitForExit();
				}
			}
		}


		/// <summary>
		/// Starting Game as Non Retail
		/// </summary>
		public static void StartGameNonRetail()
		{

		string cmdLineArgs = @"/c cd /d " + "\"" + LauncherLogic.GTAVFilePath + "\"" + @" && start playgtav.exe -uilanguage " + Settings.ToMyLanguageString(Settings.LanguageSelected).ToLower() + " && exit";

		//cmdLineArgs = @"/c cd / d "F:\SteamLibrary\steamapps\common\Grand Theft Auto V" && playgtav.exe -uilanguage french && exit";
		Process tmp = GSF.Identity.UserAccountControl.CreateProcessAsStandardUser(@"cmd.exe", cmdLineArgs);
		}

	} // End of Class
} // End of Namespace
