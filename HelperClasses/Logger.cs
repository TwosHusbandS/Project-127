using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Class of our own Logger
	/// </summary>
	public static class Logger
	{
		// We should probably use a logging libary / framework now that I think about it...whatevs
		// Actually implementing this probably took less time than googling "Logging class c#", and we have more control over it

		/// <summary>
		/// Init Function which gets called once at the start.
		/// </summary>
		public static void Init()
		{
			// Since the createFile Method will override an existing file
			if (!FileHandling.doesFileExist(Globals.Logfile))
			{
			HelperClasses.FileHandling.createFile(Globals.Logfile);
			}


			string MyCreationDate = HelperClasses.FileHandling.GetCreationDate(Process.GetCurrentProcess().MainModule.FileName);

			HelperClasses.Logger.Log("", true, 0);
			HelperClasses.Logger.Log("", true, 0);
			HelperClasses.Logger.Log(" === Project - 127 Started (Version: '" + Globals.ProjectVersion + "' BuildInfo: '" + Globals.BuildInfo + "' Built at: '" + MyCreationDate + "' Central European Time). Logging initiated === ", true, 0);
		}

		/// <summary>
		/// Main Method of Logging.cs which is called to log stuff.
		/// </summary>
		/// <param name="pLogMessage"></param>
		public static void Log(string pLogMessage, bool pSkipLogSetting, int pLogLevel)
		{
			if (pSkipLogSetting)
			{
				string LogMessage = "[" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "] - "; 
				
				// Yes this for loop is correct. If Log level 0, we dont add another "- "
				for (int i = 0; i <= pLogLevel - 1; i++)
				{
					LogMessage += "- ";
				}
				
				LogMessage += pLogMessage;

				HelperClasses.FileHandling.AddToLog(Globals.Logfile, LogMessage);
			}
		}

		/// <summary>
		/// Overloaded / Underloaded Logging Method
		/// </summary>
		/// <param name="pLogMessage"></param>
		public static void Log(string pLogMessage)
		{
			Log(pLogMessage, Settings.EnableLogging, 0);
		}

		/// <summary>
		/// Overloaded / Underloaded Logging Method
		/// </summary>
		/// <param name="pLogMessage"></param>
		/// <param name="pLogLevel"></param>
		public static void Log(string pLogMessage, int pLogLevel)
		{
			Log(pLogMessage, Settings.EnableLogging, pLogLevel);
		}
	
	} // End of Class
} // End of NameSpace
