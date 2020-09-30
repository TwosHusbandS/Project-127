using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProcessSpawner127
{
	/// <summary>
	/// This will be used as a ProcessSpawner.
	/// If this doesnt work, I will use this to spawn project 1.27 itself and then to sapwn gtav and stuff
	/// </summary>
	class Program
	{
		/// <summary>
		/// Main Method
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			// Fully finished, documented, tested, integrated, and finalized Code below

			string FilePath = "";
			string Arguments = "";
			string WorkingDir = "";
			bool runAsAdmin = false;

			//for (int i = 0; i <= args.Length - 1; i++)
			//{
			//	Console.WriteLine("Arg: {0} = '{1}'", i, args[i]);
			//}
			//Console.WriteLine("Done with that ");
			//Console.ReadKey();

			if (args.Length >= 2)
			{
				FilePath = args[0];
				WorkingDir = args[1];

				Process p = new Process();
				p.StartInfo.FileName = FilePath;
				p.StartInfo.WorkingDirectory = WorkingDir;

				p.Start();

				Console.WriteLine("Done...");
				Console.ReadKey();
			}


		}
	}
}