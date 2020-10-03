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
		public static void StartProcess(string pFilepath, string pWorkingDir = null, string pCommandLineArguments = null, bool runAsAdmin = false, bool pUseShellExecute = false, bool waitForExit = false)
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
				if (!runAsAdmin)
				{
					RunAsDesktopUser(pFilepath, proc.StartInfo.Arguments, proc.StartInfo.WorkingDirectory);
					return;
				}
				else
				{
					proc.StartInfo.Verb = "runas";
				}
				proc.StartInfo.UseShellExecute = pUseShellExecute;

				// Lets see if this works
				//proc.ProcessorAffinity = (IntPtr)0xFFFFFFFF;

				proc.Start();
				if (waitForExit)
				{
					proc.WaitForExit();
				}
			}
		}




		// Stolen from: https://stackoverflow.com/a/40501607

		/// <summary>
		/// Starts a process as User
		/// </summary>
		/// <param name="fileName"></param>
		public static void RunAsDesktopUser(string fileName, string pCommandLineArgs, string pStartupLocation = "")
		{
			// Fixing Arg if needed
			if (pStartupLocation == "")
			{
				pStartupLocation =  Path.GetDirectoryName(fileName);
			}


			if (string.IsNullOrWhiteSpace(fileName))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));

			// To start process as shell user you will need to carry out these steps:
			// 1. Enable the SeIncreaseQuotaPrivilege in your current token
			// 2. Get an HWND representing the desktop shell (GetShellWindow)
			// 3. Get the Process ID(PID) of the process associated with that window(GetWindowThreadProcessId)
			// 4. Open that process(OpenProcess)
			// 5. Get the access token from that process (OpenProcessToken)
			// 6. Make a primary token with that token(DuplicateTokenEx)
			// 7. Start the new process with that primary token(CreateProcessWithTokenW)

			var hProcessToken = IntPtr.Zero;
			// Enable SeIncreaseQuotaPrivilege in this process.  (This won't work if current process is not elevated.)
			try
			{
				var process = GetCurrentProcess();
				if (!OpenProcessToken(process, 0x0020, ref hProcessToken))
					return;

				var tkp = new TOKEN_PRIVILEGES
				{
					PrivilegeCount = 1,
					Privileges = new LUID_AND_ATTRIBUTES[1]
				};

				if (!LookupPrivilegeValue(null, "SeIncreaseQuotaPrivilege", ref tkp.Privileges[0].Luid))
					return;

				tkp.Privileges[0].Attributes = 0x00000002;

				if (!AdjustTokenPrivileges(hProcessToken, false, ref tkp, 0, IntPtr.Zero, IntPtr.Zero))
					return;
			}
			finally
			{
				CloseHandle(hProcessToken);
			}

			// Get an HWND representing the desktop shell.
			// CAVEATS:  This will fail if the shell is not running (crashed or terminated), or the default shell has been
			// replaced with a custom shell.  This also won't return what you probably want if Explorer has been terminated and
			// restarted elevated.
			var hwnd = GetShellWindow();
			if (hwnd == IntPtr.Zero)
				return;

			var hShellProcess = IntPtr.Zero;
			var hShellProcessToken = IntPtr.Zero;
			var hPrimaryToken = IntPtr.Zero;
			try
			{
				// Get the PID of the desktop shell process.
				uint dwPID;
				if (GetWindowThreadProcessId(hwnd, out dwPID) == 0)
					return;

				// Open the desktop shell process in order to query it (get the token)
				hShellProcess = OpenProcess(ProcessAccessFlags.QueryInformation, false, dwPID);
				if (hShellProcess == IntPtr.Zero)
					return;

				// Get the process token of the desktop shell.
				if (!OpenProcessToken(hShellProcess, 0x0002, ref hShellProcessToken))
					return;

				var dwTokenRights = 395U;

				// Duplicate the shell's process token to get a primary token.
				// Based on experimentation, this is the minimal set of rights required for CreateProcessWithTokenW (contrary to current documentation).
				if (!DuplicateTokenEx(hShellProcessToken, dwTokenRights, IntPtr.Zero, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out hPrimaryToken))
					return;

				// Start the target process with the new token.
				var si = new STARTUPINFO();
				var pi = new PROCESS_INFORMATION();
				if (!CreateProcessWithTokenW(hPrimaryToken, 0, fileName, pCommandLineArgs, 0, IntPtr.Zero, pStartupLocation, ref si, out pi))
					return;
			}
			finally
			{
				CloseHandle(hShellProcessToken);
				CloseHandle(hPrimaryToken);
				CloseHandle(hShellProcess);
			}

		}

		#region Interop

		private struct TOKEN_PRIVILEGES
		{
			public UInt32 PrivilegeCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public LUID_AND_ATTRIBUTES[] Privileges;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		private struct LUID_AND_ATTRIBUTES
		{
			public LUID Luid;
			public UInt32 Attributes;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct LUID
		{
			public uint LowPart;
			public int HighPart;
		}

		[Flags]
		private enum ProcessAccessFlags : uint
		{
			All = 0x001F0FFF,
			Terminate = 0x00000001,
			CreateThread = 0x00000002,
			VirtualMemoryOperation = 0x00000008,
			VirtualMemoryRead = 0x00000010,
			VirtualMemoryWrite = 0x00000020,
			DuplicateHandle = 0x00000040,
			CreateProcess = 0x000000080,
			SetQuota = 0x00000100,
			SetInformation = 0x00000200,
			QueryInformation = 0x00000400,
			QueryLimitedInformation = 0x00001000,
			Synchronize = 0x00100000
		}

		private enum SECURITY_IMPERSONATION_LEVEL
		{
			SecurityAnonymous,
			SecurityIdentification,
			SecurityImpersonation,
			SecurityDelegation
		}

		private enum TOKEN_TYPE
		{
			TokenPrimary = 1,
			TokenImpersonation
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct STARTUPINFO
		{
			public Int32 cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public Int32 dwX;
			public Int32 dwY;
			public Int32 dwXSize;
			public Int32 dwYSize;
			public Int32 dwXCountChars;
			public Int32 dwYCountChars;
			public Int32 dwFillAttribute;
			public Int32 dwFlags;
			public Int16 wShowWindow;
			public Int16 cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool LookupPrivilegeValue(string host, string name, ref LUID pluid);

		[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TOKEN_PRIVILEGES newst, int len, IntPtr prev, IntPtr relen);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);


		[DllImport("user32.dll")]
		private static extern IntPtr GetShellWindow();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes, SECURITY_IMPERSONATION_LEVEL impersonationLevel, TOKEN_TYPE tokenType, out IntPtr phNewToken);

		[DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern bool CreateProcessWithTokenW(IntPtr hToken, int dwLogonFlags, string lpApplicationName, string lpCommandLine, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

		#endregion

	} // End of Class
} // End of Namespace
