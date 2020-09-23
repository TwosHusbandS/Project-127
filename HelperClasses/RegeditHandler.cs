using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace Project_127.HelperClasses
{
	/// <summary>
	/// Regedit Class. Used for reading and writing config stuff to the windows registry.
	/// </summary>
	static class RegeditHandler
	{
		/*

        Terminiology is kinda confusing here. 

        Regedit:
            Folder = "Key"
            VariableName = "Value"
            VariableData = "Data"

        KeyValuePair:
            Name = "Key"
            Data = "Value"

        So while "Value" is the actual Data in the context of a KVP (KeyValuePair), a "Value" is actual the Name of the place where Data is stored in regedit terms.

		Also:
		- GetValue() returns "" if the key does not exist. This fucked us over in DoesValueExist(), but thats fixed. Just be aware of that.
		- In all these Functions there is just 1 Line which actually Reads and only 1 line which actually writes the the regedit. There is a lot of overloading and calling each other

        */


		/// <summary>
		/// Overloaded. Sets one Value/Data combination of our Key in Registry. AS STRING
		/// </summary>
		/// <param name="pValue"></param>
		/// <param name="pData"></param>
		public static void SetValue(string pValue, string pData)
		{
			SetValue(Globals.MySettingsKey, pValue, pData, RegistryValueKind.String);
		}

		/// <summary>
		/// Overloaded. Sets one Value/Data combination of our Key in Registry. AS STRING
		/// </summary>
		/// <param name="pValue"></param>
		/// <param name="pData"></param>
		public static void SetValue(RegistryKey pRKey, string pValue, string pData)
		{
			SetValue(pRKey, pValue, pData, RegistryValueKind.String);
		}

		/// <summary>
		/// Sets one Value/Data combination of our Key in Registry.
		/// </summary>
		/// <param name="pValue"></param>
		/// <param name="pData"></param>
		/// <param name="pRegistryValueKind"></param>
		public static void SetValue(string pValue, string pData, RegistryValueKind pRegistryValueKind)
		{
			SetValue(Globals.MySettingsKey, pValue, pData, pRegistryValueKind);
		}

		/// <summary>
		/// Sets one Value/Data combination of our Key in Registry.
		/// </summary>
		/// <param name="pValue"></param>
		/// <param name="pData"></param>
		/// <param name="pRegistryValueKind"></param>
		public static void SetValue(RegistryKey pRKey, string pValue, string pData, RegistryValueKind pRegistryValueKind)
		{
			try
			{
				pRKey.SetValue(pValue, pData, pRegistryValueKind);
			}
			catch
			{
				HelperClasses.Logger.Log("haha regedit goes boom while writing");
				HelperClasses.Logger.Log("pRKey: '" + pRKey.ToString() + "', pValue: '" + pValue + "', pData:'" + pData + "', pRegistryValueKind: '" + pRegistryValueKind.ToString() + "'",1);
			}
		}

		/// <summary>
		/// Gets one specific Data from one Value of our MyKey RegeditKey. Will return "" if things go boom. 
		/// </summary>
		/// <param name="pValue"></param>
		/// <returns></returns>
		public static string GetValue(string pValue)
		{
			return GetValue(Globals.MySettingsKey, pValue);
		}

		/// <summary>
		/// Gets one specific Data from one Value of our MyKey RegeditKey. Will return "" if things go boom. 
		/// </summary>
		/// <param name="pValue"></param>
		/// <returns></returns>
		public static string GetValue(RegistryKey pRKey, string pValue)
		{
			string rtrn = "";
			try
			{
				rtrn = pRKey.GetValue(pValue).ToString();
			}
			catch
			{
				HelperClasses.Logger.Log("haha regedit goes boom while reading. Function will return \"\"");
				HelperClasses.Logger.Log("pRKey: '" + pRKey.ToString() + "', pValue: '" + pValue + "'", 1);
			}
			return rtrn;
		}

		/// <summary>
		/// Overloaded. Checks if Value in the Key of Regedit exists. Takes our own MyKey.
		/// </summary>
		/// <param name="pValue"></param>
		/// <returns></returns>
		public static bool DoesValueExists(string pValue)
		{
			return DoesValueExists(Globals.MySettingsKey, pValue);
		}

		/// <summary>
		/// Checks if Value in Key of Regedit exists.
		/// </summary>
		/// <param name="pRKey"></param>
		/// <param name="pValue"></param>
		/// <returns></returns>
		public static bool DoesValueExists(RegistryKey pRKey, string pValue)
		{
			try
			{
				// Not using our custom GetValue Method so that this does not show up in logs...
				string val = pRKey.GetValue(pValue).ToString();
				return (!(String.IsNullOrEmpty(val)));
			}
			catch
			{
				return false;
			}
		}

	} // End of Class
} // End of NameSpace

