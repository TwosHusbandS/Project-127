using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Project_127
{
	/// <summary>
	/// Interaction logic for CopyFileProgress.xaml
	/// </summary>
	public partial class CopyFileProgress : Window
	{
		BackgroundWorker Worker = new BackgroundWorker();
		
		public object Filestream { get; private set; }
	

		public CopyFileProgress(string pSourceFile, string pDestinationFile)
		{
			InitializeComponent();

			Worker.WorkerSupportsCancellation = true;
			Worker.WorkerReportsProgress = true;

			Worker.ProgressChanged += Worker_ProgressChanged;
			Worker.DoWork += Worker_DoWork;
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

			throw new NotImplementedException();
		}
	}
}
