using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Project_127;

namespace Project_127
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
	{
		/// <summary>
		/// Gets called when PC shuts off.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
		{
			Globals.ProperExit();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			
		}
	}
}
