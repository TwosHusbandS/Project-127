using Microsoft.Win32;
using Project_127.HelperClasses;
using Project_127.MySettings;
using Project_127.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127
{
	/// <summary>
	/// Static Class used for the new / alternative / cleaner way of Launching
	/// </summary>
	static class LaunchAlternative
	{
		/// <summary>
		/// Launches Downgraded GTA in new / cleaner way.
		/// </summary>
		public static void Launch()
		{
			string filePath = LauncherLogic.GTAVFilePath.TrimEnd('\\') + @"\gtastub.exe";

			HelperClasses.Logger.Log("SCL - Launch");

			if (HelperClasses.FileHandling.doesFileExist(filePath))
			{
				HelperClasses.Logger.Log("SCL - Launch, GTA Stub exists");
				if (!IsCfgDatUpToDate())
				{
					HelperClasses.Logger.Log("SCL - Launch, cfg.dat is NOT up to date.");

                    MainWindow.MW.Dispatcher.Invoke(() =>
                    {
						Popup yesno2 = new Popup(Popup.PopupWindowTypes.PopupYesNo, "The file rockstar uses for offline authentication\nis (probably) expired and wont work.\nUpgrade GTA and launch into main menu\nto generate a new one.\n\nWant me to try to launch anyways?");
						yesno2.ShowDialog();
						if (yesno2.DialogResult == true)
						{
							HelperClasses.Logger.Log("SCL - Launch, cfg.dat is NOT up to date, user wants to try anyways...lets go");
						}
						else
						{
							HelperClasses.Logger.Log("SCL - Launch, cfg.dat is NOT up to date, user does NOT want to try anyways...returning here");
							return;
						}
                    });
                }
				else
				{
					HelperClasses.Logger.Log("SCL - Launch, cfg.dat IS up to date.");
				}

				if (SocialClubDowngrade())
				{
					LauncherLogic.UpgradeSocialClubAfterGame = true;

					HelperClasses.Logger.Log("SCL - Launch, Social Club Downgrade WAS successfull. Launching Game now.");


					ProcessHandler.StartDowngradedGame();
				}
				else
				{
					HelperClasses.Logger.Log("SCL - Launch, Social Club Downgrade was NOT successfull.");
					new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOkError, "Social Club downgrade went wrong.").ShowDialog();
				}
			}
			else
			{
				HelperClasses.Logger.Log("SCL - Launch, GTA Stub does NOT exists");

				new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOkError, "Cant find the required File ('gtastub.exe')\ninside your GTA Installation.\nSomething went wrong").ShowDialog();
			}
		}





		#region SocialClubSwitcheroo


		public static void SetUpSocialClubRegistryThing()
		{
			SCL_SC_Installation = @"C:\Program Files\Rockstar Games\Social Club";

			if (Settings.EnableAlternativeLaunchForceCProgramFiles)
			{
				return;
			}

			RegistryKey myRK = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Rockstar Games\Rockstar Games Social Club");
			string RegeditInstallPath = HelperClasses.RegeditHandler.GetValue(myRK, "InstallFolder");
			if (Get_SCL_InstallationState(RegeditInstallPath) != SCL_InstallationStates.Trash)
			{
				SCL_SC_Installation = RegeditInstallPath;
			}
		}



		/// <summary>
		/// Social Club Installation Path.
		/// </summary>
		public static string SCL_SC_Installation = @"C:\Program Files\Rockstar Games\Social Club";

		/// <summary>
		/// TEMP BACKUP Social Club Directory we use to rename Original Social Club too.
		/// </summary>
		public static string SCL_SC_TEMP_BACKUP { get { return HelperClasses.FileHandling.GetParentFolder(SCL_SC_Installation).TrimEnd('\\') + @"\Social Club_P127_TEMP_BACKUP"; ; } }

		/// <summary>
		/// "CACHED" Downgraded Social Club inside C:\Program Files\Rockstar Games
		/// </summary>
		public static string SCL_SC_DOWNGRADED_CACHE { get { return HelperClasses.FileHandling.GetParentFolder(SCL_SC_Installation).TrimEnd('\\') + @"\Social Club_P127_DOWNGRADED_CACHE"; ; } }

		/// <summary>
		/// Path to Downgraded SC Files inside $P127_Files
		/// </summary>
		public static string SCL_SC_DOWNGRADED
		{
			get
			{
				return LauncherLogic.ZIPFilePath.TrimEnd('\\') + @"\Project_127_Files\SupportFiles\DowngradedSocialClub";
			}
		}

		public static string SCL_DLL_ADDON = @"\socialclub.dll";
		public static string SCL_EXE_ADDON_DOWNGRADED = @"\subprocess.exe";
		public static string SCL_EXE_ADDON_UPGRADED = @"\socialclubhelper.exe";

		public enum SCL_InstallationStates
		{
			Upgraded,
			Downgraded,
			Trash
		}

		/// <summary>
		/// Gets the InstallationState of one Social Club Folder
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static SCL_InstallationStates Get_SCL_InstallationState(string filePath)
		{
			Version vDLL = new Version("0.0.0.1");
			Version vEXE = new Version("0.0.0.1");

			// check if we have more than 10 files
			if (HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(filePath).Length >= 10)
			{
				// check if SC DLL exists
				if (HelperClasses.FileHandling.doesFileExist(filePath + SCL_DLL_ADDON))
				{
					// Grab version of DLL
					vDLL = HelperClasses.FileHandling.GetVersionFromFile(filePath + SCL_DLL_ADDON);

					// Grab correct exe, depending which one the installation uses, read Version from it
					if (HelperClasses.FileHandling.doesFileExist(filePath + SCL_EXE_ADDON_DOWNGRADED) && !HelperClasses.FileHandling.doesFileExist(SCL_EXE_ADDON_UPGRADED))
					{
						vEXE = HelperClasses.FileHandling.GetVersionFromFile(filePath + SCL_EXE_ADDON_DOWNGRADED);
					}
					else
					{
						if (HelperClasses.FileHandling.doesFileExist(filePath + SCL_EXE_ADDON_UPGRADED) && !HelperClasses.FileHandling.doesFileExist(SCL_EXE_ADDON_DOWNGRADED))
						{
							vEXE = HelperClasses.FileHandling.GetVersionFromFile(filePath + SCL_EXE_ADDON_UPGRADED);
						}
					}
				}
			}

			// if any version is default, return trash
			if (vEXE == new Version("0.0.0.1") || vDLL == new Version("0.0.0.1"))
			{
				return SCL_InstallationStates.Trash;
			}
			// etc...
			else if (vEXE <= new Version("1.2") && vDLL <= new Version("1.2"))
			{
				return SCL_InstallationStates.Downgraded;
			}
			else if (vEXE >= new Version("1.2") && vDLL >= new Version("1.2"))
			{
				return SCL_InstallationStates.Upgraded;
			}
			else
			{
				return SCL_InstallationStates.Trash;
			}
		}

		/// <summary>
		/// HelperMethod to make sure we have an Downgrade_Cache inside C:\Program Files. RETURNS IF WE HAVE A DOWNGRADED CACHE IN C:\PROGRAM FILES
		/// </summary>
		/// <returns></returns>
		public static bool SCL_MakeSureDowngradedCacheIsCorrect()
		{
			if (Get_SCL_InstallationState(SCL_SC_DOWNGRADED_CACHE) == SCL_InstallationStates.Downgraded)
			{
				return true;
			}
			else
			{
				if (Get_SCL_InstallationState(SCL_SC_DOWNGRADED) != SCL_InstallationStates.Downgraded)
				{
					// ERROR, RE-INSTALL SOCIAL CLUB DOWNGRADED
					HelperClasses.Logger.Log("SCL - $SC_DOWNGRADE_FILES isnt looking good. Asking User if he wants to re-install", 1);

					string msg = "The Components needed to downgrade Social Club\nare not installed.\nWant to install them now?";
					Popup yesno = new Popup(Popup.PopupWindowTypes.PopupYesNo, msg);
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						HelperClasses.Logger.Log("SCL - User wants to, lets download.", 1);

						if (!ComponentManager.Components.DowngradedSC.ReInstall())
						{
							HelperClasses.Logger.Log("SCL - Install failed. Will abort.", 1);
							return false;
						}
					}
					else
					{
						HelperClasses.Logger.Log("SCL - User does NOT want it. Will abort.", 1);
						return false;
					}
				}


				List<MyFileOperation> tmp = new List<MyFileOperation>();

				// Delete Folder, Create new Folder, copy all files to it.


				tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SCL_SC_DOWNGRADED_CACHE, "", "Deleting SCL_SC_DOWNGRADED_CACHE Folder: '" + SCL_SC_DOWNGRADED_CACHE + "'", 2, MyFileOperation.FileOrFolder.Folder));

				tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Create, SCL_SC_DOWNGRADED_CACHE, "", "Creating SCL_SC_DOWNGRADED_CACHE Folder: '" + SCL_SC_DOWNGRADED_CACHE + "'", 2, MyFileOperation.FileOrFolder.Folder));

				// Those are WITH the "\" at the end
				string[] FilesInSCDowngraded = HelperClasses.FileHandling.GetFilesFromFolderAndSubFolder(SCL_SC_DOWNGRADED);
				string[] CorrespondingFilesInSC_DOWNGRADED_CACHE = new string[FilesInSCDowngraded.Length];

				// Loop through all Files in Downgrade Files Folder
				for (int i = 0; i <= FilesInSCDowngraded.Length - 1; i++)
				{
					CorrespondingFilesInSC_DOWNGRADED_CACHE[i] = SCL_SC_DOWNGRADED_CACHE + FilesInSCDowngraded[i].Substring(SCL_SC_DOWNGRADED.Length);


					if (FilesInSCDowngraded[i].Contains(@"socialclub.dll"))
					{
						tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, FilesInSCDowngraded[i], CorrespondingFilesInSC_DOWNGRADED_CACHE[i], "Copying: '" + FilesInSCDowngraded[i] + "' to '" + CorrespondingFilesInSC_DOWNGRADED_CACHE[i] + "', as part of Downgrading SC. Only one Log so we dont spam.", 2, MyFileOperation.FileOrFolder.File));
					}
					else
					{
						tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Copy, FilesInSCDowngraded[i], CorrespondingFilesInSC_DOWNGRADED_CACHE[i], "", 2, MyFileOperation.FileOrFolder.File));
					}
				}

				// only actually throw pop up when its needed...
				if (tmp.Count > 0)
				{
					new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Creating Social Club Cache", tmp).ShowDialog();
				}

				// Return true when needed
				if (Get_SCL_InstallationState(SCL_SC_DOWNGRADED_CACHE) == SCL_InstallationStates.Downgraded)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Downgrades SocialClub if needed. RETURNS TRUE ONLY IF REALLY DOWNGRADED
		/// </summary>
		/// <param name="msDelay"></param>
		/// <returns></returns>
		public static bool SocialClubDowngrade(int msDelay = 0)
		{
			// exit if we are already correct
			if (Get_SCL_InstallationState(SCL_SC_Installation) == SCL_InstallationStates.Downgraded)
			{
				HelperClasses.Logger.Log("SCL - SC Looks Downgraded already. No need to Downgrade.", 1);
				return true;
			}


			Task.Delay(msDelay).GetAwaiter().GetResult();


			HelperClasses.Logger.Log("SCL - Initiating a Social Club Downgrade after " + msDelay + " ms of Delay", 0);

			// KILL ALL PROCESSES
			HelperClasses.ProcessHandler.SocialClubKillAllProcesses().GetAwaiter().GetResult();

			if (!SCL_MakeSureDowngradedCacheIsCorrect())
			{
				return false;
			}

			// All processes killed, downgradedcache is good. Just rename now.

			List<MyFileOperation> tmp = new List<MyFileOperation>();

			// If Installation is Upgraded
			if (Get_SCL_InstallationState(SCL_SC_Installation) == SCL_InstallationStates.Upgraded)
			{
				// DELETE PREV BACKUP
				tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SCL_SC_TEMP_BACKUP, "", "Deleting previous Backup Folder '" + SCL_SC_TEMP_BACKUP + "'", 2, MyFileOperation.FileOrFolder.Folder));

				// SAVE CURR ONE AS BACKUP VIA RENAMING
				tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, SCL_SC_Installation, SCL_SC_TEMP_BACKUP, "Saving curr Installation as Backup. Renaming '" + SCL_SC_Installation + "' to '" + SCL_SC_TEMP_BACKUP + "'", 2, MyFileOperation.FileOrFolder.Folder));

			}
			// if installation is not upgraded
			else
			{
				// if our temp folder is Upgraded
				if (Get_SCL_InstallationState(SCL_SC_TEMP_BACKUP) == SCL_InstallationStates.Upgraded)
				{
					// DELETE INSTALL FOLDER
					tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SCL_SC_Installation, "", "Deleting Installation Folder: '" + SCL_SC_Installation + "', TEMP_BACKUP is looking good. Keeping it", 2, MyFileOperation.FileOrFolder.Folder));
				}
				// if our Temp folder is downgraded, we can still use it as "upgraded" gta, since it will update on use.
				else if (Get_SCL_InstallationState(SCL_SC_TEMP_BACKUP) == SCL_InstallationStates.Downgraded)
				{
					// DELETE INSTALL FOLDER
					tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SCL_SC_Installation, "", "Deleting Installation Folder: '" + SCL_SC_Installation + "', TEMP_BACKUP is looking Downgrading, still gotta keep it.", 2, MyFileOperation.FileOrFolder.Folder));
				}
				// our temp folder is trash
				else
				{
					// DELETE INSTALL FOLDER
					tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SCL_SC_TEMP_BACKUP, "", "Deleting TEMP BACKUP Folder: '" + SCL_SC_TEMP_BACKUP + "'. Since its trash", 2, MyFileOperation.FileOrFolder.Folder));

					// if actual installation is downgraded
					if (Get_SCL_InstallationState(SCL_SC_Installation) == SCL_InstallationStates.Downgraded)
					{
						// SAVE CURR ONE AS BACKUP VIA RENAMING
						tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, SCL_SC_Installation, SCL_SC_TEMP_BACKUP, "Saving curr Installation as Backup. Even tho its Downgraded Renaming '" + SCL_SC_Installation + "' to '" + SCL_SC_TEMP_BACKUP + "'", 2, MyFileOperation.FileOrFolder.Folder));
					}
				}
			}

			// Renaming cached downgrade to Installation
			tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, SCL_SC_DOWNGRADED_CACHE, SCL_SC_Installation, "Applying Cache Downgraded to Installationpath. Renaming '" + SCL_SC_DOWNGRADED_CACHE + "' to '" + SCL_SC_Installation + "'", 2, MyFileOperation.FileOrFolder.Folder));

			// only actually throw pop up when its needed...
			if (tmp.Count > 0)
			{
				new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Downgrading Social Club", tmp).ShowDialog();
			}

			// returning based on actual folder contents, not what we think should be in there.
			if (Get_SCL_InstallationState(SCL_SC_Installation) == SCL_InstallationStates.Downgraded)
			{
				HelperClasses.Logger.Log("SCL - SC Downgrade was sucessfull. Will return true.", 1);
				return true;
			}
			else
			{
				HelperClasses.Logger.Log("SCL - SC Downgrade was NOT sucessfull. Will return FALSE.", 1);
				return false;
			}
		}



		public static void SocialClubReset(int msDelay = 0)
		{
			HelperClasses.Logger.Log("SCL - RESETTING SC FOLDERS. DELETING EVERYTHING CUSTOM", 1);
			SocialClubUpgrade(msDelay);
			HelperClasses.FileHandling.DeleteFolder(SCL_SC_DOWNGRADED);
			HelperClasses.FileHandling.DeleteFolder(SCL_SC_DOWNGRADED_CACHE);
			HelperClasses.FileHandling.DeleteFolder(SCL_SC_TEMP_BACKUP);
		}


		/// <summary>
		/// Upgrades Social Club if needed. RETURNS TRUE IF INSTALLATION OF SC IS USABLE (Upgraded OR Downgraded)
		/// </summary>
		/// <param name="msDelay"></param>
		/// <returns></returns>
		public static bool SocialClubUpgrade(int msDelay = 0)
		{
			if (Get_SCL_InstallationState(SCL_SC_Installation) == SCL_InstallationStates.Upgraded)
			{
				HelperClasses.Logger.Log("SCL - SC Looks Upgraded already. No need to Upgrade.", 1);
				return true;
			}

			// Waiting msDelay if wanted (after GTAClosed)
			Task.Delay(msDelay).GetAwaiter().GetResult();

			HelperClasses.Logger.Log("SCL - Initiating a Social Club Upgrade after " + msDelay + " ms of Delay", 0);

			// KILL ALL PROCESSES
			HelperClasses.ProcessHandler.SocialClubKillAllProcesses().GetAwaiter().GetResult();

			List<MyFileOperation> tmp = new List<MyFileOperation>();

			// If backup is correct
			if (Get_SCL_InstallationState(SCL_SC_TEMP_BACKUP) == SCL_InstallationStates.Upgraded)
			{
				HelperClasses.Logger.Log("SCL - Temp / Backup Files are good. Normal Upgrade Procedure.", 1);

				// Save "Downgraded_CACHE" if we can
				if (Get_SCL_InstallationState(SCL_SC_DOWNGRADED_CACHE) != SCL_InstallationStates.Downgraded && Get_SCL_InstallationState(SCL_SC_Installation) == SCL_InstallationStates.Downgraded)
				{
					// Rename Install to Downgrade_Cache
					tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, SCL_SC_Installation, SCL_SC_DOWNGRADED_CACHE, "Renaming Installation ('" + SCL_SC_Installation + "') to Downgraded Cache Folder ('" + SCL_SC_DOWNGRADED_CACHE + "')", 2, MyFileOperation.FileOrFolder.Folder));
				}
				// just delete install dir if we cant
				else
				{
					tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SCL_SC_Installation, "", "Deleting Installation Folder: '" + SCL_SC_Installation + "'", 2, MyFileOperation.FileOrFolder.Folder));

				}

				// rename temp to install dir
				tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, SCL_SC_TEMP_BACKUP, SCL_SC_Installation, "Renaming Temp ('" + SCL_SC_TEMP_BACKUP + "') to Installation Folder ('" + SCL_SC_Installation + "')", 2, MyFileOperation.FileOrFolder.Folder));
			}
			// if backup is not correct
			else
			{
				// install folder and temp folder are both not upgraded
				HelperClasses.Logger.Log("SCL - Neither the Installation nor the Temp Folder are upgraded. Lets see if any of them are Downgraded", 1);

				// if install dir is Downgraded
				if (Get_SCL_InstallationState(SCL_SC_Installation) == SCL_InstallationStates.Downgraded)
				{
					// keeping it since its usable and will auto-upgrade
					HelperClasses.Logger.Log("SCL - Installation Folder is Downgraded, lets keep it.", 2);
				}
				else if (Get_SCL_InstallationState(SCL_SC_TEMP_BACKUP) == SCL_InstallationStates.Downgraded)
				{
					HelperClasses.Logger.Log("SCL - Installation Folder is not Downgraded (nor Updated), Temp Folder is tho.", 1);
					HelperClasses.Logger.Log("SCL - Will apply Temp / Backup anyways, to have a working Social Club Installation.", 1);

					tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SCL_SC_Installation, "", "Deleting Installation Folder: '" + SCL_SC_Installation + "'", 2, MyFileOperation.FileOrFolder.Folder));

					tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, SCL_SC_TEMP_BACKUP, SCL_SC_Installation, "Renaming TEMP BACKUP ('" + SCL_SC_TEMP_BACKUP + "') to Installation Folder ('" + SCL_SC_Installation + "')", 2, MyFileOperation.FileOrFolder.Folder));
				}
				else
				{
					// Install and TEMP Backup path both are trash. Maybe we can reuse downgrade cache as new upgraded

					HelperClasses.Logger.Log("SCL - Installation Folder is not Downgraded (nor Updated), Temp Folder is not Downgraded (nor Updated) either.", 1);
					HelperClasses.Logger.Log("SCL - Lets see if we can save ourselves with the Downgraded Cache folder.", 1);
					if (Get_SCL_InstallationState(SCL_SC_DOWNGRADED_CACHE) != SCL_InstallationStates.Trash)
					{
						HelperClasses.Logger.Log("SCL - SCL_SC_DOWNGRADED_CACHE Folder is not Trash. Yay.", 2);

						tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Delete, SCL_SC_Installation, "", "Deleting Installation Folder: '" + SCL_SC_Installation + "'", 2, MyFileOperation.FileOrFolder.Folder));
						tmp.Add(new MyFileOperation(MyFileOperation.FileOperations.Move, SCL_SC_DOWNGRADED_CACHE, SCL_SC_Installation, "Renaming DowngradedCache ('" + SCL_SC_DOWNGRADED_CACHE + "') to Installation Folder ('" + SCL_SC_Installation + "')", 2, MyFileOperation.FileOrFolder.Folder));
					}
					else
					{
						HelperClasses.Logger.Log("SCL - Welp looks like everything is trash User gotta deal with it I guess.", 1);

					}
				}
			}

			// only actually throw pop up when its needed...
			if (tmp.Count > 0)
			{
				new PopupProgress(PopupProgress.ProgressTypes.FileOperation, "Upgrading Social Club", tmp).ShowDialog();
			}

			// returning based on actual folder contents, not what we think should be in there.
			if (Get_SCL_InstallationState(SCL_SC_Installation) != SCL_InstallationStates.Trash)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		#endregion

		public static bool IsCfgDatUpToDate()
		{
			HelperClasses.Logger.Log("SCL - Checking if cfg.dat is up to date.", 1);

			string cfgdat = HelperClasses.FileHandling.MostLikelyProfileFolder().TrimEnd('\\') + @"\cfg.dat";

			if (HelperClasses.FileHandling.doesFileExist(cfgdat))
			{
				DateTime tmp = HelperClasses.FileHandling.GetLastWriteDate(cfgdat);
				if (tmp != DateTime.MinValue)
				{
					HelperClasses.Logger.Log("SCL - most likely cfg.dat exists (LastWriteDate returns good value).", 2);
					TimeSpan asdf = DateTime.Now - tmp;
					if (asdf.TotalDays <= 27)
					{
						HelperClasses.Logger.Log("SCL - most likely cfg.dat is less than 27 days old. Will not throw popup", 2);
						return true;
					}
					else
					{
						HelperClasses.Logger.Log("SCL - most likely cfg.dat is older than 27 days. Will throw popup", 2);
						return false;
					}
				}
			}
			HelperClasses.Logger.Log("SCL - most likely cfg.dat does NOT exists (or LastWriteDate returns garbage). Will throw popup", 2);
			return false;
		}




	}
}
