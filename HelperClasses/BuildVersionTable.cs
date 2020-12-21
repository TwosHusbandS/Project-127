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
