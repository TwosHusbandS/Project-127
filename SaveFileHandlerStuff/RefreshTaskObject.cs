using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.SaveFileHandlerStuff
{
	class RefreshTaskObject
	{
		public ObservableCollection<MySaveFile> MyBackupSaves = new ObservableCollection<MySaveFile>();
		public ObservableCollection<MySaveFile> MyGTASaves = new ObservableCollection<MySaveFile>();
		public string DisplayPath = "";
		public int FontSize = 20;

		public RefreshTaskObject(ObservableCollection<MySaveFile> _MyBackupSaves, ObservableCollection<MySaveFile> _MyGTASaves, string _DisplayPath, int _FontSize)
		{
			this.MyBackupSaves = _MyBackupSaves;
			this.MyGTASaves = _MyGTASaves;
			this.DisplayPath = _DisplayPath;
			this.FontSize = _FontSize;
		}

	}
}
