using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127
{
	/// <summary>
	/// Class for "MyFile" Objects. Used in the DataGrids for the SaveFileManagement
	/// </summary>
    public class MyFile
    {
		/// <summary>
		/// FileName Property.
		/// </summary>
        public string Filename { private set; get; }
        
		/// <summary>
		/// PathName Property
		/// </summary>
		public string Pathname { private set; get; }

		/// <summary>
		/// Standart Constructor. Dont need to forbid default Constructor since it wont be generated.
		/// </summary>
		/// <param name="pFilename"></param>
		/// <param name="pPathname"></param>
        public MyFile(string pFilename, string pPathname)
        {
            this.Filename = pFilename;
            this.Pathname = pPathname;
        }
	
    } // End of Class
} // End of Namespace
