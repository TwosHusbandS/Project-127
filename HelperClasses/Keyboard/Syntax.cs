using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace Project_127.HelperClasses.Keyboard
{
	class Syntax
	{
		private enum switchMode
		{
			NoSwitch,
			Copy,
			Rename,
			Symlink
		}
		private static switchMode mode = switchMode.NoSwitch;

		private const switchMode symlinkBackupMode = switchMode.Rename;
		public const string VERSION = "1.8 beta";

		public const string SCDIR = "C:\\Program Files\\Rockstar Games\\Social Club\\";
		public const string DEFAULTSCBACKUPDIR = "C:\\Program Files\\Rockstar Games\\Social Club.back\\";
		public const string DEFAULTSC1178DIR = "C:\\Program Files\\Rockstar Games\\SC1178\\";
		public const string LAUNCHSTUBNAME = "GTAStub.exe";
		private static bool isSteam
		{
			get
			{
				return System.IO.File.Exists("steam_api64.dll");
			}
		}

		public static string SCBACKUPDIR, SC1178DIR;
		public static void log(string s)
		{
			Console.Write(s);
			//Todo: actual file logging
		}

		public static void emessage(string s)
		{
			log(s);
			MessageBox.Show(s);
		}
		private static bool IsRunAsAdmin()
		{
			try
			{
				WindowsIdentity id = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(id);
				return principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch (Exception)
			{
				return false;
			}
		}
		private static void AdminRelauncher(string commandlineargs)
		{
			// If not run as Admin
			if (!IsRunAsAdmin())
			{
				try
				{
					// CTRLF TODO // THIS MIGHT BE BROKEN WITH COMMAND LINE ARGS THAT CONTAIN SPACES
					ProcessStartInfo info = new ProcessStartInfo();
					info.FileName = Assembly.GetEntryAssembly().CodeBase;
					info.WorkingDirectory = Environment.CurrentDirectory;
					info.Arguments = commandlineargs;
					Process.Start(info);
					System.Environment.Exit(1);
				}
				catch (Exception)
				{
					MessageBox.Show("This program must be run as an administrator!");
					System.Environment.Exit(-1);
				}
			}
		}







		static void Main(string[] args)
		{
			var exe = Assembly.GetEntryAssembly().Location;
			var loc = System.IO.Path.GetDirectoryName(exe);
			System.IO.Directory.SetCurrentDirectory(loc);
			char[] commandLineRaw = Environment.CommandLine.ToCharArray();
			int ccindex = 0;
			if (commandLineRaw[ccindex] == '"')
			{
				ccindex++;
				while (ccindex < commandLineRaw.Length)
				{
					if (commandLineRaw[ccindex++] == '"')
					{
						break;
					}
				}
			}
			else
			{
				while ((ccindex < commandLineRaw.Length)
					&& commandLineRaw[ccindex] != ' '
					&& commandLineRaw[ccindex] != '\t')
				{
					ccindex++;
				}
			}
			string commandLineArgs = Environment.CommandLine.Substring(ccindex);
			//Elevation Stuffs
			if (!IsRunAsAdmin())
			{
				AdminRelauncher(commandLineArgs);
			}
			//
			log(String.Format("dr490n automated downgrade launcher v{0}\r\n", VERSION));
			if (getExecutableName() == "Play127.exe")
			{
				log("PROJECT 1.27 MODE ENABLED\r\n"); //Stores SC in project127 files/read p127 settings
				readConfig(true);
			}
			else
			{
				readConfig(false);
			}
			string launchercommandline = " -scofflineonly ";
			launchercommandline += commandLineArgs;

			log(String.Format("Retailer detected as {0}\r\n", isSteam ? "steam" : "rockstar"));
			if (isSteam)
			{// Sets up for steam with appid
			 //Todo (appid)

				//Ensures steam is running and logged in
				Process[] pname = Process.GetProcessesByName("steam");
				if (pname.Length == 0)
				{
					Process.Start("steam://");
				}
				while ((int)Registry.GetValue(
					"HKEY_CURRENT_USER\\SOFTWARE\\Valve\\Steam\\ActiveProcess",
					"ActiveUser",
					0) == 0)
				{
					Thread.Sleep(100);
				}

			}


			log("Reading file versions...\r\n");


			// Retrieve all relevant versions
			var ver = getFileVersion("GTA5.exe");
			var glver = getFileVersion("GTAVLauncher.exe");
			var scver = getFileVersion("C:\\Program Files\\Rockstar Games\\Social Club\\socialclub.dll");


			// Log each of them
			log("GTA Version: ");
			log(string.Format("v{0}\r\n", ver));


			log("GTAVLauncher Version: ");
			log(string.Format("v{0}\r\n", glver));

			log("Socialclub Version: ");
			log(string.Format("v{0}\r\n", scver));

			if (likelyExpired())
			{
				log("Warning: Offline auth data likely expired\r\n");
			}

			bool isNonModern = (ver.CompareTo(new Version(1, 0, 372, 2)) <= 0);

			bool restoreflag = false;
			switch (mode) //Determine appropriate action based on mode
			{
				case (switchMode.NoSwitch):
					if (isNonModern && isSteam && (scver.CompareTo(new Version(1, 1, 7, 8)) > 0))
					{ //V1.1.7.8
						emessage("Error: Invalid socialclub for 1.27");
						System.Environment.Exit(-1);
					}
					else if (scver.CompareTo(new Version(0, 0, 0, 0)) == 0)
					{
						emessage("Error: Socialclub missing");
						System.Environment.Exit(-1);
					}
					break;
				case (switchMode.Copy):
					if (isNonModern && (scver.CompareTo(new Version(1, 1, 7, 8)) > 0))
					{ //V1.1.7.8
						log("Switching socialclub to 1.1.7.8\r\n");
						dswitchCopy(SCDIR, SCBACKUPDIR, SC1178DIR);
						restoreflag = true;
					}
					else if (scver.CompareTo(new Version(0, 0, 0, 0)) == 0)
					{
						log("No socialclub; attempting to restore...\r\n");
						if (isNonModern)
						{

							if (!dswitchCopy(SCDIR, SC1178DIR))
							{
								emessage("Error: Missing socialclub for STEAM 1.27 (SC v1.1.7.8)\r\n");
								System.Environment.Exit(-1);
							}
							restoreflag = true;
						}
						else
						{
							if (!dswitchCopy(SCDIR, SCBACKUPDIR))
							{
								emessage("Error: Missing socialclub backup\r\n");
								System.Environment.Exit(-1);
							}
						}
					}
					else if ((scver.CompareTo(getFileVersion(SCDIR + "socialclub.dll")) > 0) && !isNonModern)
					{
						dswitchCopy(SCDIR, SCBACKUPDIR);
					}
					break;
				case (switchMode.Symlink):
					if (isNonModern && (scver.CompareTo(new Version(1, 1, 7, 8)) > 0))
					{ //V1.1.7.8
						log("Switching socialclub to 1.1.7.8\r\n");
						dswitchSymlink(SCDIR, SCBACKUPDIR, SC1178DIR);
						restoreflag = true;
					}
					else if (scver.CompareTo(new Version(0, 0, 0, 0)) == 0)
					{
						if (isNonModern)
						{
							if (!dswitchSymlink(SCDIR, SC1178DIR))
							{
								emessage("Error: Missing socialclub for STEAM 1.27 (SC v1.1.7.8)\r\n");
								System.Environment.Exit(-1);
							}
							restoreflag = true;
						}
						else
						{
							if (!dswitchSymlink(SCDIR, SCBACKUPDIR))
							{
								emessage("Error: Missing socialclub backup\r\n");
								System.Environment.Exit(-1);
							}
						}
					}
					break;
				case (switchMode.Rename):
					if (isNonModern && (scver.CompareTo(new Version(1, 1, 7, 8)) > 0))
					{ //V1.1.7.8
						log("Switching socialclub to 1.1.7.8\r\n");
						dswitchRename(SCDIR, SCBACKUPDIR, SC1178DIR);
						restoreflag = true;
					}
					else if (scver.CompareTo(new Version(0, 0, 0, 0)) == 0)
					{
						if (isNonModern)
						{
							if (!dswitchRename(SCDIR, SC1178DIR))
							{
								emessage("Error: Missing socialclub for STEAM 1.27 (SC v1.1.7.8)\r\n");
								System.Environment.Exit(-1);
							}
							restoreflag = true;
						}
						else
						{
							if (!dswitchRename(SCDIR, SCBACKUPDIR))
							{
								emessage("Error: Missing socialclub backup\r\n");
								System.Environment.Exit(-1);
							}
						}
					}
					break;
				default:
					goto case switchMode.NoSwitch;
			}
			if (isNonModern && glver.Build > 372)
			{
				emessage("Error: Possbile GTAV Launcher missmatch\r\n");
				System.Environment.Exit(-1);
			}
			else if (glver.CompareTo(new Version(0, 0, 0, 0)) == 0)
			{ // Or errors if its missing
				emessage("Error: GTAV Launcher missing");
				System.Environment.Exit(-1);
			}

			if (restoreflag)
			{
				scver = getFileVersion("C:\\Program Files\\Rockstar Games\\Social Club\\socialclub.dll");
				log("Socialclub changed, version is now ");
				log(String.Format("v{0}\r\n", scver));
			}
			if (mode == switchMode.Rename &&
				System.IO.Directory.Exists(SCBACKUPDIR))
			{
				restoreflag = true;
			}
			else if (mode == switchMode.Symlink &&
				isSymLink("C:\\Program Files\\Rockstar Games\\Social Club\\socialclub.dll"))
			{
				restoreflag = true;
			}
			string launchCommand = System.IO.Path.GetFullPath(LAUNCHSTUBNAME);
			launchCommand += " ";
			launchCommand += launchercommandline;
			log(string.Format("Launching: {0}\r\n", launchCommand));
			spawnAsStandardUser(System.IO.Path.GetFullPath(LAUNCHSTUBNAME), launchercommandline);

			Thread.Sleep(30000);
			Process GTA;
			try //Wait for GTA to exit
			{
				GTA = Process.GetProcessesByName("GTA5")[0];
				log("Waiting for GTA to close...\r\n");
				while (!GTA.HasExited)
				{
					Thread.Sleep(5000);
				}
			}
			catch
			{
				emessage("GTA not running\r\n");
			}

			//Something something wait for stub to exit
			try
			{
				var launcher = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(LAUNCHSTUBNAME))[0];
				while (!launcher.HasExited)
				{
					Thread.Sleep(500);
				}
			}
			catch { }



			Thread.Sleep(10000);

			log("Killing residual processes...\r\n"); //After delay, kill all subproccess
			var gtasubprocesses = Process.GetProcessesByName("subprocess");
			foreach (Process p in gtasubprocesses)
			{
				p.Kill();
			}

			if (restoreflag)
			{
				log("Restoring socialclub\r\n");
				try
				{
					switch (mode)
					{
						case switchMode.Copy:
							dswitchCopy(SCDIR, SCBACKUPDIR);
							break;
						case switchMode.Symlink:
							switch (symlinkBackupMode)
							{
								case switchMode.Copy:
									dswitchCopy(SCDIR, SCBACKUPDIR);
									break;
								case switchMode.Rename:
									dswitchRename(SCDIR, SCBACKUPDIR);
									break;
								case switchMode.Symlink:
									dswitchSymlink(SCDIR, SCBACKUPDIR);
									break;
								default:
									goto case switchMode.Copy;

							}

							break;
						case switchMode.Rename:
							dswitchRename(SCDIR, SC1178DIR, SCBACKUPDIR);
							break;
						default:
							emessage("Error: Non-Restorable Mode\r\n");
							System.Environment.Exit(-1);
							break;
					}
				}
				catch
				{
					emessage("Error: socialclub restore failed!\r\n");
					if (!System.IO.Directory.Exists(SCBACKUPDIR))
					{
						emessage("Backup dir not found!\r\n");
					}
					System.Environment.Exit(-1);
				}
			}
			log("Done\r\n");
			//Console.WriteLine("Hello World!");
		}

		private static void dirCopy(System.IO.DirectoryInfo source, System.IO.DirectoryInfo target)
		{
			System.IO.Directory.CreateDirectory(target.FullName);
			foreach (var f in source.GetFiles())
			{
				f.CopyTo(System.IO.Path.Combine(target.FullName, f.Name));
			}
			foreach (var d in source.GetDirectories())
			{
				var nt = target.CreateSubdirectory(d.Name);
				dirCopy(d, nt);
			}
		}
		private static void dirCopy(string src, string dest)
		{
			dirCopy(
				new System.IO.DirectoryInfo(src),
				new System.IO.DirectoryInfo(dest)
				);
		}
		private static bool isSymLink(string path)
		{
			if (!System.IO.Directory.Exists(path))
			{
				return false;
			}
			var di = new System.IO.DirectoryInfo(path);
			return di.Attributes.HasFlag(System.IO.FileAttributes.ReparsePoint);
		}

		private static void attemptDelete(string target, int retrys, int interval, bool recurse = true)
		{
			for (int i = 0; i < retrys + 1; i++)
			{
				try
				{
					System.IO.Directory.Delete(target, recurse);
					break;
				}
				catch
				{
					Thread.Sleep(interval);
				}
			}
		}

		private static bool dswitchCopy(string original, string replacement)
		{
			if (isSymLink(original))
			{
				//RemoveDirectoryW(original);
				attemptDelete(original, 10, 10000, false);
			}
			else if (System.IO.Directory.Exists(original))
			{
				attemptDelete(original, 10, 10000);
			}
			dirCopy(replacement, original);
			return true;
		}

		private static bool dswitchCopy(string original, string backup, string replacement)
		{
			if (isSymLink(original))
			{
				attemptDelete(original, 10, 10000, false);
			}
			else if (System.IO.Directory.Exists(original))
			{
				if (System.IO.Directory.Exists(backup))
				{
					log("Existing Backup Detected\r\n");
					var osc = System.IO.Path.Combine(original, "socialclub.dll");
					var bsc = System.IO.Path.Combine(backup, "socialclub.dll");

					if (getFileVersion(osc).CompareTo(getFileVersion(bsc)) > 0)
					{
						System.IO.Directory.Delete(backup, true);
						dirCopy(original, backup);
					}
				}
				else
				{
					dirCopy(original, backup);
				}
				attemptDelete(original, 10, 10000);
			}
			dirCopy(replacement, original);
			return true;
		}

		private static bool dswitchSymlink(string original, string replacement)
		{
			if (isSymLink(original))
			{
				attemptDelete(original, 10, 10000, false);
			}
			else if (System.IO.Directory.Exists(original))
			{
				attemptDelete(original, 10, 10000);
			}
			return CreateSymbolicLink(original, replacement, SymbolicLink.Directory);
		}

		private static bool dswitchSymlink(string original, string backup, string replacement)
		{
			if (isSymLink(original))
			{
				return dswitchSymlink(original, replacement);
			}
			else
			{
				if (symlinkBackupMode == switchMode.Copy || symlinkBackupMode == switchMode.Symlink)
				{
					if (System.IO.Directory.Exists(backup))
					{
						var osc = System.IO.Path.Combine(original, "socialclub.dll");
						var bsc = System.IO.Path.Combine(backup, "socialclub.dll");

						if (getFileVersion(osc).CompareTo(getFileVersion(bsc)) > 0)
						{
							System.IO.Directory.Delete(backup, true);
							dirCopy(original, backup);
						}
					}
					else
					{
						dirCopy(original, backup);
					}
					attemptDelete(original, 10, 10000);
				}
				else
				{
					if (System.IO.Directory.Exists(backup))
					{
						log("Backup dir exists\r\n");
						var osc = System.IO.Path.Combine(original, "socialclub.dll");
						var bsc = System.IO.Path.Combine(backup, "socialclub.dll");

						if (getFileVersion(osc).CompareTo(getFileVersion(bsc)) > 0)
						{
							System.IO.Directory.Delete(backup, true);
							dswitchRename(backup, original);
						}
					}
					else
					{
						log("Backup dir does not exist\r\n");
						dswitchRename(backup, original);
						//attemptDelete(original, 10, 10000);
					}
					//System.IO.Directory.Move(original, backup);

				}
			}
			return dswitchSymlink(original, replacement);
		}


		private static string getExecutableName()
		{
			string filepath = Assembly.GetEntryAssembly().Location;
			return System.IO.Path.GetFileName(filepath);
		}
		private static void readConfig(bool p127mode)
		{
			if (p127mode)
			{
				var p127rpath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Project_127";
				SC1178DIR = (string)Registry.GetValue(p127rpath, "ZIPExtractionPath", null);
				SC1178DIR = System.IO.Path.Combine(SC1178DIR, @"Project_127_Files\SupportFiles\DowngradedSocialClub\");
				var copyMode = bool.Parse((string)Registry.GetValue(p127rpath, "EnableCopyFilesInsteadOfSyslinking_SocialClub", "false"));

				if (SC1178DIR == null)
				{
					SC1178DIR = DEFAULTSC1178DIR;
				}
				SCBACKUPDIR = DEFAULTSCBACKUPDIR;
				if (!copyMode)
				{
					mode = switchMode.Symlink;
				}
				else
				{
					mode = switchMode.Copy;
				}
			}
			else
			{
				SC1178DIR = DEFAULTSC1178DIR;
				SCBACKUPDIR = DEFAULTSCBACKUPDIR;
				if (!System.IO.File.Exists("GTAADL.cfg"))
				{
					//Todo
					/*
						LauncherExecutable=GTAVLauncher.exe
						SocialClubSwitchMode=NoSwitch
						SocialClubBackupDir=
						SocialClub1178Dir=
						SocialClub1160Dir=
					*/
					using (var f = new System.IO.StreamWriter("GTAADL.cfg"))
					{
						f.WriteLine(string.Format("socialclubswitchmode={0}", mode.ToString()));
						f.WriteLine(string.Format("socialclubbackupdir={0}", DEFAULTSCBACKUPDIR));
						f.WriteLine(string.Format("socialclub1178dir={0}", DEFAULTSC1178DIR));
					}
				}
				else
				{
					string[] CFGLines = System.IO.File.ReadAllLines("GTAADL.cfg");
					foreach (string line in CFGLines)
					{
						char[] equals = { '=' };
						string[] kvpair = line.Split(equals, 2);
						//line.Split('=', 2, StringSplitOptions.None);
						if (kvpair.Length < 2)
						{
							continue;
						}
						char[] whitespace = { ' ', '\t' };
						kvpair[0] = kvpair[0].Trim(whitespace);
						kvpair[1] = kvpair[1].Trim(whitespace);

						switch (kvpair[0].ToLower())
						{
							case "socialclubswitchmode":
								switch (kvpair[1].ToLower())
								{
									case "noswitch":
										log("Switchmode set to noswitch\r\n");
										mode = switchMode.NoSwitch;
										break;
									case "copy":
										log("Switchmode set to copy\r\n");
										mode = switchMode.Copy;
										break;
									case "symlink":
										log("Switchmode set to symlink\r\n");
										mode = switchMode.Symlink;
										break;
									case "rename":
										log("Switchmode set to rename\r\n");
										mode = switchMode.Rename;
										break;
								}
								break;
							case "socialclubbackupdir":
								SCBACKUPDIR = kvpair[1];
								break;
							case "socialclub1178dir":
								SC1178DIR = kvpair[1];
								break;
						}

					}
				}
			}
		}
		private static bool dswitchRename(string original, string replacement)
		{
			if (isSymLink(original))
			{
				//RemoveDirectoryW(original);
				attemptDelete(original, 10, 10000, false);
			}
			else if (System.IO.Directory.Exists(original))
			{
				attemptDelete(original, 10, 10000);
			}
			for (int i = 0; i < 10; i++)
			{
				try
				{
					System.IO.Directory.Move(replacement, original);
					break;
				}
				catch
				{
					Thread.Sleep(10000);
				}
			}
			return true;
		}

		private static bool dswitchRename(string original, string backup, string replacement)
		{
			if (isSymLink(original))
			{
				attemptDelete(original, 10, 10000, false);
			}
			else if (System.IO.Directory.Exists(original))
			{
				if (System.IO.Directory.Exists(backup))
				{
					log("Existing Backup Detected\r\n");
					var osc = System.IO.Path.Combine(original, "socialclub.dll");
					var bsc = System.IO.Path.Combine(backup, "socialclub.dll");

					if (getFileVersion(osc).CompareTo(getFileVersion(bsc)) > 0)
					{
						dswitchRename(backup, original);
					}
				}
				else
				{
					dswitchRename(backup, original);
				}
			}
			dswitchRename(original, replacement);
			return true;
		}
		private static Version getFileVersion(string target)
		{
			try
			{
				return new Version(FileVersionInfo.GetVersionInfo(target).FileVersion);
			}
			catch
			{
				return new Version(0, 0, 0, 0);
			}
		}

		[DllImport("kernel32.dll")]
		static extern bool CreateSymbolicLink(
		string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

		enum SymbolicLink
		{
			File = 0,
			Directory = 1
		}
		/*
		[DllImport("StandardUserRunner.dll", ExactSpelling = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		public static extern bool spawnAsStandardUser(string exe, string commandline);
		private static bool spawnAsStandardUserArgs(string target, string cmdLineArgs)
		{
			return spawnAsStandardUser(
				target, 
				System.IO.Path.GetFileName(target) + " " + cmdLineArgs
			);

		}
		*/
		public static bool likelyExpired()
		{
			string profilesDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			profilesDir = System.IO.Path.Combine(profilesDir, @"Rockstar Games\GTA V\Profiles");
			var di = new System.IO.DirectoryInfo(profilesDir);
			DateTime mostRecent = new DateTime(2000, 1, 1);
			foreach (var profile in di.GetDirectories())
			{
				if (Regex.IsMatch(profile.Name, "[0-9A-F]{8}"))
				{
					foreach (var file in profile.GetFiles())
					{
						if (file.Name.ToLower() == "cfg.dat")
						{
							var lwt = file.LastWriteTime;
							if (DateTime.Compare(lwt, mostRecent) > 0)
							{
								mostRecent = lwt;
							}
							break;
						}
					}
				}
			}
			log(string.Format("Most recent cfg.dat date: {0}\r\n", mostRecent));
			return (DateTime.Compare(mostRecent.AddDays(30), DateTime.Now) < 0);
		}
		public static void spawnAsStandardUser(string target, string cmdLineArgs)
		{
			try
			{
				GSF.Identity.UserAccountControl.CreateProcessAsStandardUser(target, cmdLineArgs);
			}
			catch
			{
				emessage("Unable to launch stub!\r\n");
				System.Environment.Exit(-1);
			}
		}
	}
}
