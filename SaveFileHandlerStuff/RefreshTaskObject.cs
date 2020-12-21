using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.SaveFileHandlerStuff
{
	/// <summary>
	/// RefreshTaskObject. Is an Object with the 2 Observable Collections
	/// </summary>
	class RefreshTaskObject
	{
		public ObservableCollection<MySaveFile> MyBackupSaves = new ObservableCollection<MySaveFile>();
		public ObservableCollection<MySaveFile> MyGTASaves = new ObservableCollection<MySaveFile>();

		/// <summary>
		/// Constuctor
		/// </summary>
		/// <param name="_MyBackupSaves"></param>
		/// <param name="_MyGTASaves"></param>
		public RefreshTaskObject(ObservableCollection<MySaveFile> _MyBackupSaves, ObservableCollection<MySaveFile> _MyGTASaves)
		{
			this.MyBackupSaves = _MyBackupSaves;
			this.MyGTASaves = _MyGTASaves;
		}

	}
}
