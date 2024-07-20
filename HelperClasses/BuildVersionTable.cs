using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
	/// <summary>
	/// Class for the Logic which build Number is which Version. Gets from github.
	/// </summary>
	class BuildVersionTable
	{
		/// <summary>
		/// Property of the Build Version
		/// </summary>
		Version MyBuildVersion;

		/// <summary>
		/// Property of the corresponding Game Version
		/// </summary>
		Version MyGameVersion;

		/// <summary>
		/// Property of a "nice" String of the GameVersion
		/// </summary>
		string MyGameVersionString
		{
			get
			{
				string rtrn = "";

				rtrn += MyGameVersion.Major + "." + MyGameVersion.Minor;

				return rtrn;
			}
		}

		/// <summary>
		/// Static List of all BuildVersionGameVersion Objects
		/// </summary>
		static List<BuildVersionTable> MyBuildVersionTables = new List<BuildVersionTable>();

		/// <summary>
		/// Constructor of class. Adds it to the static list above.
		/// </summary>
		/// <param name="pMyBuildVersion"></param>
		/// <param name="pMyGameVersion"></param>
		public BuildVersionTable(string pMyBuildVersion, string pMyGameVersion)
		{
			MyBuildVersion = new Version(pMyBuildVersion);
			MyGameVersion = new Version(pMyGameVersion);

			//MyBuildVersionTables.Add(this);
		}


		public static void Init(string online_xml)
		{
            MyBuildVersionTables.Clear();

			List<BuildVersionTable> Local = GetHardcodedOnes();
			List<BuildVersionTable> Github = GetFromGithub(online_xml);

			if (Github.Count > 5)
			{
                if (Github[Github.Count - 1].MyBuildVersion > Local[Local.Count - 1].MyBuildVersion)
				{
                    MyBuildVersionTables = Github;
                }
				else
				{
                    MyBuildVersionTables = Local;
                }
            }
            else
			{
                MyBuildVersionTables = Local;
			}
        }

        public static List<BuildVersionTable> GetHardcodedOnes()
		{
			List<BuildVersionTable> tmp = new List<BuildVersionTable>
            {
                new BuildVersionTable("1.0.323.0", "1.24"),
                new BuildVersionTable("1.0.350.0", "1.25"),
                new BuildVersionTable("1.0.350.1", "1.26"),
                new BuildVersionTable("1.0.350.3", "1.27"),
                new BuildVersionTable("1.0.393.0", "1.28"),
                new BuildVersionTable("1.0.463.0", "1.29"),
                new BuildVersionTable("1.0.505.0", "1.30"),
                new BuildVersionTable("1.0.573.0", "1.31"),
                new BuildVersionTable("1.0.617.0", "1.32"),
                new BuildVersionTable("1.0.678.0", "1.33"),
                new BuildVersionTable("1.0.757.0", "1.34"),
                new BuildVersionTable("1.0.791.0", "1.35"),
                new BuildVersionTable("1.0.877.0", "1.36"),
                new BuildVersionTable("1.0.944.0", "1.37"),
                new BuildVersionTable("1.0.1011.0", "1.38"),
                new BuildVersionTable("1.0.1032.0", "1.39"),
                new BuildVersionTable("1.0.1103.0", "1.40"),
                new BuildVersionTable("1.0.1180.0", "1.41"),
                new BuildVersionTable("1.0.1290.0", "1.42"),
                new BuildVersionTable("1.0.1365.0", "1.43"),
                new BuildVersionTable("1.0.1493.0", "1.44"),
                new BuildVersionTable("1.0.1604.0", "1.46"),
                new BuildVersionTable("1.0.1734.0", "1.47"),
                new BuildVersionTable("1.0.1737.0", "1.48"),
                new BuildVersionTable("1.0.1868.0", "1.50"),
                new BuildVersionTable("1.0.2060.0", "1.51"),
                new BuildVersionTable("1.0.2060.1", "1.52"),
                new BuildVersionTable("1.0.2189.0", "1.52"),
                new BuildVersionTable("1.0.2215.0", "1.53"),
                new BuildVersionTable("1.0.2245.0", "1.54"),
                new BuildVersionTable("1.0.2372.0", "1.57"),
                new BuildVersionTable("1.0.2545.0", "1.58"),
                new BuildVersionTable("1.0.2612.1", "1.59"),
                new BuildVersionTable("1.0.2628.2", "1.60"),
                new BuildVersionTable("1.0.2699.0", "1.61"),
                new BuildVersionTable("1.0.2699.16", "1.63"),
                new BuildVersionTable("1.0.2802.0", "1.64"),
                new BuildVersionTable("1.0.2824.0", "1.66"),
                new BuildVersionTable("1.0.2845.0", "1.66"),
                new BuildVersionTable("1.0.2944.0", "1.67"),
                new BuildVersionTable("1.0.3028.0", "1.67"),
                new BuildVersionTable("1.0.3095.0", "1.68"),
                new BuildVersionTable("1.0.3179.0", "1.68"),
                new BuildVersionTable("1.0.3258.0", "1.69")
            };

			return tmp;
        }


		public static List<BuildVersionTable> GetFromGithub(string xml)
		{
			List<BuildVersionTable> tmp = new List<BuildVersionTable>();

			try
			{
				string uglyVersions = FileHandling.GetXMLTagContent(xml, "buildversiontable");

				foreach (string OnePair in GetStringListFromString(uglyVersions, ':'))
				{
				    List<string> OnePairList = GetStringListFromString(OnePair, ';');
				    if (OnePairList.Count == 2)
				    {
				        tmp.Add(new BuildVersionTable(OnePairList[0], OnePairList[1]));
				    }
				}
			}
			catch (Exception ex)
			{
				HelperClasses.Logger.Log("BuildVersionTable Conversion from Github failed. " + ex.ToString());
			}
			return tmp;
        }







		/// <summary>
		/// Helper Method
		/// </summary>
		/// <param name="pString"></param>
		/// <param name="Deliminiter"></param>
		/// <returns></returns>
		public static List<string> GetStringListFromString(string pString, char Deliminiter)
		{
			List<string> rtrn = new List<string>(pString.Split(Deliminiter));

			if (rtrn.Count == 1)
			{
				if (String.IsNullOrWhiteSpace(rtrn[0]))
				{
					rtrn = new List<string>();
				}
			}

			return rtrn;
		}

		/// <summary>
		/// Returns if GTA5.exe is Downgraded. Takes both path to exe and folder.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool IsDowngradedGTA(string filePath)
		{
			filePath = filePath.ToLower().TrimEnd('\\').TrimEnd("gta5.exe");
			return HelperClasses.BuildVersionTable.GetGameVersionOfBuild(HelperClasses.FileHandling.GetVersionFromFile(filePath + @"\gta5.exe", true)) < new Version(1, 30);
		}

		/// <summary>
		/// Returns if GTA5.exe is Upgraded. Takes both path to exe and folder.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool IsUpgradedGTA(string filePath)
		{
			filePath = filePath.ToLower().TrimEnd('\\').TrimEnd("gta5.exe");
			return HelperClasses.BuildVersionTable.GetGameVersionOfBuild(HelperClasses.FileHandling.GetVersionFromFile(filePath + @"\gta5.exe")) > new Version(1, 30);
		}

		public static Version GetGameVersionOfBuild(Version pBuildVersion)
		{
			Version LastVersionIwasBiggerthan = new Version("1.0");
			for (int i = 0; i <= MyBuildVersionTables.Count - 1; i++)
			{
				if (pBuildVersion >= MyBuildVersionTables[i].MyBuildVersion)
				{
					LastVersionIwasBiggerthan = MyBuildVersionTables[i].MyGameVersion;

					if (i == MyBuildVersionTables.Count - 1 && (pBuildVersion > MyBuildVersionTables[i].MyBuildVersion))
					{
						LastVersionIwasBiggerthan = MyBuildVersionTables[i].MyGameVersion;
					}
				}
			}
			return LastVersionIwasBiggerthan;
		}

		/// <summary>
		/// Returning a very nice string to display in UI based on the build version
		/// </summary>
		/// <param name="pBuildVersion"></param>
		/// <returns></returns>
		public static string GetNiceGameVersionString(Version pBuildVersion)
		{
			string rtrn = "";

			Version LastVersionIwasBiggerthan = new Version("1.0");
			for (int i = 0; i <= MyBuildVersionTables.Count - 1; i++)
			{
				if (pBuildVersion >= MyBuildVersionTables[i].MyBuildVersion)
				{
					LastVersionIwasBiggerthan = MyBuildVersionTables[i].MyGameVersion;

					if (i == MyBuildVersionTables.Count - 1 && (pBuildVersion > MyBuildVersionTables[i].MyBuildVersion))
					{
						rtrn = ">";
					}
				}
			}

			if (LastVersionIwasBiggerthan == new Version("1.0"))
			{
				rtrn = "???";
			}
			else
			{
				rtrn += LastVersionIwasBiggerthan.Major + "." + LastVersionIwasBiggerthan.Minor;
			}


			rtrn = "(" + rtrn + ")";


			return rtrn;
		}
	}
}
