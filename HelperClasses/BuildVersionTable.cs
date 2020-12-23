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
		static List<BuildVersionTable> MyBuildVersionTable = new List<BuildVersionTable>();

		/// <summary>
		/// Constructor of class. Adds it to the static list above.
		/// </summary>
		/// <param name="pMyBuildVersion"></param>
		/// <param name="pMyGameVersion"></param>
		public BuildVersionTable(string pMyBuildVersion, string pMyGameVersion)
		{
			MyBuildVersion = new Version(pMyBuildVersion);
			MyGameVersion = new Version(pMyGameVersion);

			MyBuildVersionTable.Add(this);
		}

		/// <summary>
		/// Reading in the Information from Github Update.xml. Needs to be called so this calss is able to do stuff.
		/// </summary>
		public static void ReadFromGithub()
		{
			MyBuildVersionTable.Clear();

			string uglyVersions = FileHandling.GetXMLTagContent(Globals.XML_AutoUpdate, "buildversiontable");

			foreach (string OnePair in GetStringListFromString(uglyVersions, ':'))
			{
				List<string> OnePairList = GetStringListFromString(OnePair, ';');
				if (OnePairList.Count == 2)
				{
					new BuildVersionTable(OnePairList[0], OnePairList[1]);
				}
			}

			if (MyBuildVersionTable.Count < 2)
			{
				new BuildVersionTable("1.0.323.0", "1.24");
				new BuildVersionTable("1.0.350.0", "1.26");
				new BuildVersionTable("1.0.372.0", "1.27");
				new BuildVersionTable("1.0.393.0", "1.28");
				new BuildVersionTable("1.0.463.0", "1.29");
				new BuildVersionTable("1.0.505.0", "1.30");
				new BuildVersionTable("1.0.573.0", "1.31");
				new BuildVersionTable("1.0.617.0", "1.32");
				new BuildVersionTable("1.0.678.0", "1.33");
				new BuildVersionTable("1.0.757.0", "1.34");
				new BuildVersionTable("1.0.757.0", "1.34");
				new BuildVersionTable("1.0.791.0", "1.35");
				new BuildVersionTable("1.0.877.0", "1.36");
				new BuildVersionTable("1.0.944.0", "1.37");
				new BuildVersionTable("1.0.1011.0", "1.38");
				new BuildVersionTable("1.0.1032.0", "1.39");
				new BuildVersionTable("1.0.1103.0", "1.40");
				new BuildVersionTable("1.0.1180.0", "1.41");
				new BuildVersionTable("1.0.1290.0", "1.42");
				new BuildVersionTable("1.0.1365.0", "1.43");
				new BuildVersionTable("1.0.1493.0", "1.44");
				new BuildVersionTable("1.0.1604.0", "1.46");
				new BuildVersionTable("1.0.1734.0", "1.47");
				new BuildVersionTable("1.0.1737.0", "1.48");
				new BuildVersionTable("1.0.1868.0", "1.50");
				new BuildVersionTable("1.0.2060.0", "1.51");
				new BuildVersionTable("1.0.2060.1", "1.52");
				new BuildVersionTable("1.0.2189.0", "1.53");
			}

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

		public static Version GetGameVersionOfBuild(Version pBuildVersion)
		{
			Version LastVersionIwasBiggerthan = new Version("1.0");
			for (int i = 0; i <= MyBuildVersionTable.Count - 1; i++)
			{
				if (pBuildVersion >= MyBuildVersionTable[i].MyBuildVersion)
				{
					LastVersionIwasBiggerthan = MyBuildVersionTable[i].MyGameVersion;

					if (i == MyBuildVersionTable.Count - 1 && (pBuildVersion > MyBuildVersionTable[i].MyBuildVersion))
					{
						LastVersionIwasBiggerthan = MyBuildVersionTable[i].MyGameVersion;
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
			for (int i = 0; i <= MyBuildVersionTable.Count - 1; i++)
			{
				if (pBuildVersion >= MyBuildVersionTable[i].MyBuildVersion)
				{
					LastVersionIwasBiggerthan = MyBuildVersionTable[i].MyGameVersion;

					if (i == MyBuildVersionTable.Count - 1 && (pBuildVersion > MyBuildVersionTable[i].MyBuildVersion))
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
