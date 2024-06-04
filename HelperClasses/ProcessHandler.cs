using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Management;
using Microsoft.Win32;

namespace Project_127.HelperClasses
{
    /// <summary>
    /// Helper Class for ProcessHandler 
    /// </summary>
    public static class ProcessHandler
    {
        static private int ProcessKillDelayMS = 50;

        /// <summary>
        /// Kills all Rockstar / GTA / Social Club related processes
        /// </summary>
        public static async void KillRockstarProcessesAsync()
        {
            SocialClubKillAllProcesses();
            GTAKillAllProcesses();

            MainWindow.MW.UpdateGUIDispatcherTimer();
        }

        public static void KillRockstarProcesses()
        {
            SocialClubKillAllProcesses();
            GTAKillAllProcesses();
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



        public static string GetCommandLine(this Process process)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                {
                    using (ManagementObjectCollection objects = searcher.Get())
                    {
                        return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
                    }
                }
            }
            catch
            {
                return "";
            }
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



        [DllImport("Kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        public static string GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            try
            {
            var fileNameBuilder = new StringBuilder(buffer);
            uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
            return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
                fileNameBuilder.ToString() :
                "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }



        public static Process[] GetGTAProcesses()
        {
            List<Process> rtrn = new List<Process>();

            // Kill all processes with these names
            List<string> ProcNames = new List<string>
            {
                "gta",
                "gtavlauncher",
                "playgtav",
                "gtastub",
                "gtaddl",
                "play127"
            };

            // Only kill them if their filepath contains these
            string Pathname = LauncherLogic.GTAVFilePath.TrimEnd('\\').ToLower();

            // Loop through all processes
            Process[] Processes = Process.GetProcesses();
            for (int i = 0; i <= Processes.Length - 1; i++)
            {
                // Loop through all processnames
                foreach (string ProcName in ProcNames)
                {
                    // If processname hits
                    if (Processes[i].ProcessName.ToLower().Contains(ProcName.ToLower().TrimEnd(".exe")))
                    {
                        // If Pathnames hit
                        if (Processes[i].GetMainModuleFileName().ToLower().Contains(Pathname.ToLower()))
                        {
                            rtrn.Add(Processes[i]);
                        }
                    }
                }
            }

            return rtrn.ToArray();
        }


        /// <summary>
        /// Killing all GTA Processes
        /// </summary>
        public static void GTAKillAllProcesses()
        {
            foreach (Process p in GetGTAProcesses())
            {
                Kill(p);
            }

            HelperClasses.Logger.Log("Check if GTA processes are still running...");
            while (GetSocialClubProcesses().Length > 0)
            {
                HelperClasses.Logger.Log("Still running GTA process...");
            }
            HelperClasses.Logger.Log("Check if GTA processes are still running, aparently thats not the case");
            Task.Delay(ProcessKillDelayMS).GetAwaiter().GetResult();
        }


        public static Process[] GetSocialClubProcesses()
        {
            List<Process> rtrn = new List<Process>();

            // Kill all processes with these names
            List<string> ProcNames = new List<string>
            {
                "rockstar",
                "socialclub",
                "launcher",
                "subprocess"
            };


            // Only kill them if their filepath contains these
            List<string> PathNames = new List<string>
            {
                LauncherLogic.GTAVFilePath.TrimEnd('\\').ToLower(),
                //@"C:\Program Files\Rockstar Games"
                "Rockstar Games"
            };
            RegistryKey myRK = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("WOW6432Node").CreateSubKey("Microsoft").CreateSubKey("Windows").CreateSubKey("CurrentVersion").CreateSubKey("Uninstall").CreateSubKey("Rockstar Games Launcher");
            string tmpInstallDir = HelperClasses.RegeditHandler.GetValue(myRK, "InstallLocation");
            if (!String.IsNullOrWhiteSpace(tmpInstallDir))
            {
                PathNames.Add(tmpInstallDir);
            }



            // Loop through all processes
            Process[] Processes = Process.GetProcesses();
            for (int i = 0; i <= Processes.Length - 1; i++)
            {
                // Loop through all processnames
                foreach (string ProcName in ProcNames)
                {
                    // If processname hits
                    if (Processes[i].ProcessName.ToLower().Contains(ProcName.ToLower().TrimEnd(".exe")))
                    {
                        // Loop through pathnames
                        foreach (string Pathname in PathNames)
                        {
                            // If Pathnames hit
                            if (Processes[i].GetMainModuleFileName().ToLower().Contains(Pathname.ToLower()))
                            {
                                rtrn.Add(Processes[i]);
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            return rtrn.ToArray();
        }

        /// <summary>
        /// Killing all Social Club Related Processes
        /// </summary>
        /// <param name="msDelayAfter"></param>
        public static void SocialClubKillAllProcesses()
        {
            foreach (Process p in GetSocialClubProcesses())
            {
                Kill(p);
            }

            HelperClasses.Logger.Log("Check if SocialClub processes are still running...");
            while (GetSocialClubProcesses().Length > 0)
            {
                HelperClasses.Logger.Log("Still running SocialClub process...");
            }
            HelperClasses.Logger.Log("Check if SocialClub processes are still running, aparently thats not the case");
            Task.Delay(ProcessKillDelayMS).GetAwaiter().GetResult();
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
        public static void KillProcessesContains(string pProccessName, bool MakeSureIsGTARelated = false)
        {
            foreach (Process myP in GetProcessesContains(pProccessName))
            {
                if (MakeSureIsGTARelated)
                {
                    string tmp = "Rockstar Games";
                    if (myP.GetMainModuleFileName().ToLower().Contains(tmp.ToLower()))
                    {
                        Kill(myP);
                    }
                }
                else
                {
                    HelperClasses.Logger.Log("AAAAAA - Killing: " + myP.GetMainModuleFileName());
                    Kill(myP);
                }
            }
        }


        /// <summary>
        /// Extension Method for Processes to log all Killed processes
        /// </summary>
        /// <param name="pProcess"></param>
        public static void Kill(this Process pProcess)
        {
            if (!pProcess.HasExited)
            {
                Logger.Log("Trying to kill Process (and Children)'" + pProcess.ProcessName + "'", 1);

                //Logger.Log("Trying to kill Process '" + pProcess.ProcessName + "'", 1);
                //try
                //{
                //	pProcess.Kill();
                //	Logger.Log("Killed Process '" + pProcess.ProcessName + "'", 1);
                //}
                //catch
                //{
                //	Logger.Log("Failed to kill Process '" + pProcess.ProcessName + "'", 1);
                //}



                //KillProcessAndChildren(pProcess.Id);


                try
                {
                    pProcess.Kill();
                }
                catch (Exception ex)
                {
                    Logger.Log("Failed to kill Process '" + pProcess.ProcessName + "' :" + ex.ToString(), 1);
                }
            }
        }




        /// <summary>
        /// Killing Processes and their Children
        /// </summary>
        /// <param name="pid"></param>
        private static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                string procname = proc.ProcessName;
                proc.Kill();
                Logger.Log("Killed Process '" + procname + "'", 1);
            }
            catch
            {
                Logger.Log("Failed to kill Process", 1);
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
        public async static void StartProcess(string pFilepath, string pWorkingDir = null, string pCommandLineArguments = null, bool useShellExecute = false, bool runAsAdmin = false)
        {
            _ = Task.Run(async () =>
            {
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

                    return proc;
                }
                return null;
            });
        }

        /// <summary>
        /// Starting Game as Non Retail
        /// </summary>
        public static async void StartDowngradedGame()
        {
            _ = Task.Run(async () =>
            {
            if (Settings.EnableRunAsAdminDowngraded)
            {
                HelperClasses.Logger.Log("Running downgraded game AS ADMIN");
                Process tmp = Process.Start(@"cmd.exe", LauncherLogic.GetFullCommandLineArgsForStarting());
            }
            else
            {
                HelperClasses.Logger.Log("Running downgraded game not as admin");
                Process tmp = GSF.Identity.UserAccountControl.CreateProcessAsStandardUser(@"cmd.exe", LauncherLogic.GetFullCommandLineArgsForStarting());
            }
            });
        }

    } // End of Class
} // End of Namespace
