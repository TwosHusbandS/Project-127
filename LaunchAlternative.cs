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
            // Method is getting called when it should be.
            // Since we dont have the files and file mangement yet, you have to make sure files exist and files are in $GTA_Installation_Path

            // This is only being called when 
            // we are launching downgraded and alternative way of launching is enabled

            // In future / when we have files and file management code this will only be called when Files inside GTA Installation are the correct ones (alternative steam or alternative rockstar)

            // "MySettings.Settings.Retailer" of enum type "MySettings.Settings.Retailers" gives Info which Retailer we have (steam, rockstar, epic)

            // Both not connected to UI, but in effect. Can get and set from registry. Can be changed manually in registry ("True" or "False")
            // "MySettings.Settings.EnableAlternativeLaunch" bool, gets and sets from registry. If we want to launch through social club. Has to be true, since im checking for it when calling this method.
            // "MySettings.Settings.EnableCopyFilesInsteadOfHardlinking_SocialClub" bool, gets and sets from registry. if we want to copy instead of hardlinking for social club stuff

            // Point to the paths (probably non existing and empty at this point)
            // "LauncherLogic.DowngradeAlternativeFilePathSteam"
            // "LauncherLogic.DowngradeAlternativeFilePathRockstar"
            // "LauncherLogic.SocialClubFilePathSteam"
            // "LauncherLogic.SocialClubFilePathRockstar"


            //NOTE: this is just testing example code, not meant for final release (yet)

            var dm = new HelperClasses.DownloadManager("B:\\targets.xml");//MySettings.Settings.XMLLocation); <= can also be a url, just for example
            switch (MySettings.Settings.Retailer)
            {
                case MySettings.Settings.Retailers.Epic:
                    //Error
                    System.Windows.MessageBox.Show("Socialclub mode not supported on Epic!");
                    return;
                case MySettings.Settings.Retailers.Steam:
                    if ((dm.getVersion("AM_124_STEAM") == new Version(0, 0) && MySettings.Settings.Version == "124")
                        ||(dm.getVersion("AM_127_STEAM") == new Version(0, 0) && MySettings.Settings.Version == "127"))
                    {

                        //Error
                        Popups.Popup confirmation = new Popups.Popup(
                            Popups.Popup.PopupWindowTypes.PopupYesNo,
                            "Required subassembly not installed;\n Would you like to install it?"
                            );
                        //Add switch logic
                        confirmation.ShowDialog();
                        if ((bool)confirmation.DialogResult)
                        {
                            var success = false;
                            if (MySettings.Settings.Version == "124")
                            {
                                success = dm.getSubassembly("AM_124_STEAM").Result;
                            }
                            else
                            {
                                success = dm.getSubassembly("AM_127_STEAM").Result;
                            }
                            if (!success)
                            {
                                new Popups.Popup(
                                    Popups.Popup.PopupWindowTypes.PopupOkError,
                                    "Required subassembly failed to install!"
                                    ).ShowDialog();
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        return;
                    }//also check correct files in place
                    break;
                case MySettings.Settings.Retailers.Rockstar:
                    if ((dm.getVersion("AM_124_ROCKSTAR") == new Version(0, 0) && MySettings.Settings.Version == "124")
                        || (dm.getVersion("AM_127_ROCKSTAR") == new Version(0, 0) && MySettings.Settings.Version == "127"))
                    {
                        //Error
                        Popups.Popup confirmation = new Popups.Popup(
                            Popups.Popup.PopupWindowTypes.PopupYesNo,
                            "Required subassembly not installed;\n Would you like to install it?"
                            );
                        //Add switch logic
                        confirmation.ShowDialog();
                        if ((bool)confirmation.DialogResult)
                        {
                            var success = false;
                            if (MySettings.Settings.Version == "124")
                            {
                                success = dm.getSubassembly("AM_124_ROCKSTAR").Result;
                            }
                            else
                            {
                                success = dm.getSubassembly("AM_127_ROCKSTAR").Result;
                            }
                            if (!success)
                            {
                                new Popups.Popup(
                                    Popups.Popup.PopupWindowTypes.PopupOkError,
                                    "Required subassembly failed to install!"
                                    ).ShowDialog();
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    } //also check correct files in place
                    break;
                default:
                    goto case MySettings.Settings.Retailers.Epic;
            }
            //Ensured Copy
            var targetEXE = System.IO.Path.Combine(MySettings.Settings.GTAVInstallationPath, "Play127.exe");
            Process.Start(targetEXE);
        }


        // also let me know when you want me to call the Methods regarding Social club installation repair and checks below
        // Can call them on Game Lauch, on Game exit, on P127 launch, on P127 exit (everything but taskkill and power outtage), pre Downgrade / Upgrade / etc.

    }
}
