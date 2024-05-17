﻿using System;
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
        /// <summary>
        /// Kills all Rockstar / GTA / Social Club related processes
        /// </summary>
        public static async void KillRockstarProcessesAsync()
        {
            await SocialClubKillAllProcesses();

            // TODO CTRLF add other ProcessNames
            KillProcessesContains("gta");
            KillProcessesContains("gtastub");
            KillProcessesContains("rockstar");
            KillProcessesContains("play127");
            KillProcessesContains("gtaddl");

            MainWindow.MW.UpdateGUIDispatcherTimer();
        }

        public static void KillRockstarProcesses()
        {
            SocialClubKillAllProcesses().GetAwaiter().GetResult();

            // TODO CTRLF add other ProcessNames
            KillProcessesContains("gta");
            KillProcessesContains("gtastub");
            KillProcessesContains("rockstar");
            KillProcessesContains("play127");
            KillProcessesContains("gtaddl");
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
        /// Killing all Social Club Related Processes
        /// </summary>
        /// <param name="msDelayAfter"></param>
        public async static Task SocialClubKillAllProcesses()
        {
            RegistryKey myRK = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey("SOFTWARE").CreateSubKey("WOW6432Node").CreateSubKey("Microsoft").CreateSubKey("Windows").CreateSubKey("CurrentVersion").CreateSubKey("Uninstall").CreateSubKey("Rockstar Games Launcher");
            string tmpInstallDir = HelperClasses.RegeditHandler.GetValue(myRK, "InstallLocation");

            HelperClasses.Logger.Log("Killing all Social Club Processes", 1);
            Process[] tmp = Process.GetProcesses();

            // Just making sure shit is really closed
            tmp = Process.GetProcesses();
            foreach (Process p in tmp)
            {
                //// Checking if its gtavlauncher or one of the social club executables
                //if ((p.ProcessName.ToLower() == LaunchAlternative.SCL_EXE_ADDON_DOWNGRADED.TrimStart('\\').TrimEnd(".exe").ToLower()) ||
                //	(p.ProcessName.ToLower() == LaunchAlternative.SCL_EXE_ADDON_UPGRADED.TrimStart('\\').TrimEnd(".exe").ToLower()) ||
                //	(p.ProcessName.ToLower() == "gtavlauncher"))
                //{
                // check if its actually a process from SC Install dir or GTA Install dir
                try
                {
                    if (string.IsNullOrWhiteSpace(tmpInstallDir))
                    {
                        if ((p.MainModule.FileName.ToLower().Contains(LauncherLogic.GTAVFilePath.TrimEnd('\\').ToLower())) ||
                        (p.MainModule.FileName.ToLower().Contains(@"C:\Program Files\Rockstar Games".TrimEnd('\\').ToLower())))
                        {
                            ProcessHandler.Kill(p);
                        }
                    }
                    else
                    {
                        if ((p.MainModule.FileName.ToLower().Contains(LauncherLogic.GTAVFilePath.TrimEnd('\\').ToLower())) ||
                        (p.MainModule.FileName.ToLower().Contains(tmpInstallDir.TrimEnd('\\').ToLower())) ||
                        (p.MainModule.FileName.ToLower().Contains(@"C:\Program Files\Rockstar Games".TrimEnd('\\').ToLower())))
                        {
                            ProcessHandler.Kill(p);
                        }
                    }
                }
                catch
                {
                }

                //}
            }
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

                KillProcessAndChildren(pProcess.Id);
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
