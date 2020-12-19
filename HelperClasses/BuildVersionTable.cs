using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
	class BuildVersionTable
	{
		Version MyBuildVersion;

		Version MyGameVersion;

		string MyGameVersionString
		{
			get
			{
				string rtrn = "";

				rtrn += MyGameVersion.Major + "." + MyGameVersion.Minor;

				return rtrn;
			}
		}


		static List<BuildVersionTable> MyBuildVersionTable = new List<BuildVersionTable>();

		public BuildVersionTable(string pMyBuildVersion, string pMyGameVersion)
		{
			MyBuildVersion = new Version(pMyBuildVersion);
			MyGameVersion = new Version(pMyGameVersion);

			MyBuildVersionTable.Add(this);
		}

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
