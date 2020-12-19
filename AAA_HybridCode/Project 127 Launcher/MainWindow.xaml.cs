using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_127_Launcher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			string ProjectInstallationPath = Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\'));

			string filePath = ProjectInstallationPath.TrimEnd('\\') + @"\UglyFiles\Project 1.27.exe";

			string[] args = Environment.GetCommandLineArgs();

			for (int i = 0; i <= args.Length - 1; i++)
			{
				if (args[i].ToLower() == "-ImportBuild".ToLower())
				{
					Task.Delay(1000).GetAwaiter().GetResult();

					// i+1 exists
					if (i < args.Length - 1)
					{
						string ImportPath = args[i + 1].TrimStart('"').TrimEnd('"');

						if (File.Exists(ImportPath))
						{
							foreach (Process myP in Process.GetProcessesByName("Project 1.27"))
							{
								myP.Kill();
							}

							if (File.Exists(filePath + ".BACKUP"))
							{
								File.Delete(filePath + ".BACKUP");
							}
							File.Move(filePath, filePath + ".BACKUP");

							if (File.Exists(filePath))
							{
								File.Delete(filePath);
							}
							File.Move(ImportPath, filePath);


							Process pp = new Process();
							pp.StartInfo.FileName = filePath;
							pp.StartInfo.UseShellExecute = true;
							pp.StartInfo.Verb = "runas";
							pp.Start();

							this.Close();
							Environment.Exit(0);
						}
					}
				}
			}



			string arg = string.Join(" ", args.Skip(1).ToArray());

			Process p = new Process();
			p.StartInfo.FileName = filePath;
			p.StartInfo.Arguments = arg;
			p.StartInfo.UseShellExecute = true;
			p.StartInfo.Verb = "runas";
			p.Start();

			this.Close();
		}
	}
}
