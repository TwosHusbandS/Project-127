using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project_127
{
    static class LauncherLogic
    {
        [DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode)]
        public static extern int CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        public static Dictionary<string, string> DefaultSettings { get; private set; } = new Dictionary<string, string>() {
            {"FirstLaunch", "true"},
            {"GTAVInstallationPath", Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%\\Steam\\steamapps\\common\\Grand Theft Auto V")},
            {"FilesFolder", Environment.ExpandEnvironmentVariables("%ALLUSERSPROFILE%\\Project127")},
            {"500KBonus", "true"}
        };

        public static RegeditDictionary Settings { get; private set; } = new RegeditDictionary("Project127", DefaultSettings);

        public static void Init()
        {
            if (bool.Parse(Settings.Get("FirstLaunch")))
            {
                Log.Information("First launch, initializing settings and files.");
                //TODO: initialize settings and files
                Settings settingsWindow = new Settings();
                settingsWindow.ShowDialog();
                Settings.Set("FirstLaunch", "false");
            }
            PerformChecks();
        }

        public static void ResetSettings()
        {
            Settings.Clear();
            Settings.Add(DefaultSettings);
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        #region User error checks
        //TODO: do checks for user errors, if uninitialized or half state, or whatever, and fix them
        //case 1: check against 1.27 if every filename of 1.27 folder is a symlink in gtav root, else uninitialized / half state
        //case 2: check against 1.27 if every filename of 1.27 folder exists in gtav root, else missing symlinks, or nonexistent files
        //case 3: support files not present in support folder inside data folder, well nothing we can do unless downloading from thing but notify users
        //if uninitialized probably just reset settings and restart and let the autosetup do the job, but not for half state
        //also implement autosetup in checks, what we need to do is: get list of file names from downgrade folder, get files with those names in the gtav root and move them to upgrade folder
        public static void PerformChecks()
        {
            Log.Information("Checking for user error.");
            //For now it just checks if it needs the initial setup, wont be handling user errors now i guess those users better be smart and not fuck their files up (unless its my fault, then, actually nevermind its still their fault)
            bool needsUpgrade = true;
            foreach (string filePath in Directory.GetFiles($"{Settings.Get("FilesFolder")}\\Downgrade"))
            {
                string fileName = Path.GetFileName(filePath);
                if (File.Exists($"{Settings.Get("GTAVInstallationPath")}\\{fileName}") && (File.GetAttributes($"{Settings.Get("GTAVInstallationPath")}\\{fileName}") & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    Log.Information("File {fileName} needs to be moved, moving it.", fileName);
                    File.Move($"{Settings.Get("GTAVInstallationPath")}\\{fileName}", $"{Settings.Get("FilesFolder")}\\Upgrade\\{fileName}");
                    needsUpgrade = true;
                }
            }

            foreach (string filePath in Directory.GetFiles($"{Settings.Get("FilesFolder")}\\Downgrade\\update"))
            {
                string fileName = Path.GetFileName(filePath);
                if (File.Exists($"{Settings.Get("GTAVInstallationPath")}\\update\\{fileName}") && (File.GetAttributes($"{Settings.Get("GTAVInstallationPath")}\\update\\{fileName}") & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    Log.Information("File {fileName} needs to be moved, moving it.", fileName);
                    File.Move($"{Settings.Get("GTAVInstallationPath")}\\update\\{fileName}", $"{Settings.Get("FilesFolder")}\\Upgrade\\update\\{fileName}");
                    needsUpgrade = true;
                }
            }
            if (needsUpgrade) Upgrade();
        }
        #endregion

        #region Launcher logic
        public static void Downgrade()
        {
            Log.Information("Downgrading.");

            PerformChecks();

            foreach (string filePath in Directory.GetFiles($"{Settings.Get("FilesFolder")}\\Downgrade"))
            {
                string fileName = Path.GetFileName(filePath);
                Log.Information("Replacing link of {fileName}.", fileName);
                File.Delete($"{Settings.Get("GTAVInstallationPath")}\\{fileName}");
                CreateSymbolicLink($"{Settings.Get("GTAVInstallationPath")}\\{fileName}", filePath, 0);
            }

            foreach (string filePath in Directory.GetFiles($"{Settings.Get("FilesFolder")}\\Downgrade\\update"))
            {
                string fileName = Path.GetFileName(filePath);
                Log.Information("Replacing link of {fileName}.", fileName);
                File.Delete($"{Settings.Get("GTAVInstallationPath")}\\update\\{fileName}");
                CreateSymbolicLink($"{Settings.Get("GTAVInstallationPath")}\\update\\{fileName}", filePath, 0);
            }

            Log.Information("Uninstalling RGSC.");
            Process.Start($"{Settings.Get("FilesFolder")}\\Support\\UninstallRGSC.exe").WaitForExit();

            Log.Information("Installing downgraded RGSC.");
            Process.Start($"{Settings.Get("FilesFolder")}\\Support\\InstallRGSC.exe").WaitForExit();
        }

        public static void Upgrade()
        {
            Log.Information("Upgrading.");
            PerformChecks();

            foreach (string filePath in Directory.GetFiles($"{Settings.Get("FilesFolder")}\\Upgrade"))
            {
                string fileName = Path.GetFileName(filePath);
                Log.Information("Replacing link of {fileName}.", fileName);
                File.Delete($"{Settings.Get("GTAVInstallationPath")}\\{fileName}");
                CreateSymbolicLink($"{Settings.Get("GTAVInstallationPath")}\\{fileName}", filePath, 0);
            }

            foreach (string filePath in Directory.GetFiles($"{Settings.Get("FilesFolder")}\\Upgrade\\update"))
            {
                string fileName = Path.GetFileName(filePath);
                Log.Information("Replacing link of {fileName}.", fileName);
                File.Delete($"{Settings.Get("GTAVInstallationPath")}\\update\\{fileName}");
                CreateSymbolicLink($"{Settings.Get("GTAVInstallationPath")}\\update\\{fileName}", filePath, 0);
            }

            Log.Information("Uninstalling RGSC.");
            Process.Start($"{Settings.Get("FilesFolder")}\\Support\\UninstallRGSC.exe").WaitForExit();
        }
        #endregion
    }
}
