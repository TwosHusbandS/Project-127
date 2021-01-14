
* Upgrading / Downgrading GTAV
  * Can be done quick (Hardlinking) or slow but stable (Copying Files)
  * File Comparison for Information Gathering which files are new, which we overwrite etc. can be quick (just checking FileSize) or slow (Checking the entire File)
  * Automatic Detection a GTA Upgrade
* Launching Upgraded and Downgraded GTAV
  * Can "Hide from Steam" as well as Enable / Disable the 500k PreOrder Bonus at will and change your InGameName (only when NOT launching downgraded GTA through Social Club)
  * Can choose between "New" or "Legacy" Method for getting Authenticated. (only when NOT launching downgraded GTA through Social Club)
* Launching through original SocialClub installation if wanted
  * Not Possible for users on Epic
  * no more logging into social club and dealing with captchas inside P127
  * Ability to launch 1.27 and 1.24 Rockstar and Steam
* SaveFileHandler with special GTA V Speedrun Categories as well as a 100% Savefile for Practicing
  * Basic Functionality you would expect from a SaveFileHandler. Folder support, Rightclick Support, Keyboard Support.
* Automatically doing things on Game Launch:
  * Setting gta5.exe Process Priority to "high"
  * Starting GTA V with the affinity process core fix
  * Start LiveSplit
  * Start OBS / Other Stream Programs
  * Start FPS Limiter
  * Start Nohboard 
* Automatic Jumpscript
  * No need of having an AutoHotkey Installation
  * No need of having a ScriptFile
  * Ability to change Hotkeys inside P127
  * Option to provide own ScriptFile in case the integrated Jumpscript does not work.
* NoteOverlay
  * Overlay which draws a textfile on top of either GTA to read Notes during Long runs
  * can render on top of GTA (if in Borderless Mode) or on top of our own Window on other screens (Multi Monitor Mode)
  * Ability to scroll a notefile up and down and cycle through NoteFiles via Hotkeys.
  * Lots of Options regarding Files, Look and Hotkeys
  * Ability display certain in game stats, such as number of stuntjumps or missions completed (See included notefiles for examples)
  * More info [here](Advanced%20Notefile.md)
* Download Manager / Component Manager
  * Download Manager for additional needed Files (depending on Features you want to use)
  * Basic Install, Re-Install and Delete Functionality.
  * Installed Version can be found when hovering your Mouse over the Installed / Not Installed Text.
  * Verify of a Component can be triggered by rightclicking the Install Button. If this does not work, hit "ReInstall"
* Tray Icon and all expected Options / Settings
  * Start Minimized if Wanted
  * Default Exit / Close Action
  * Hide P127 completely
 * Backup Feature (for UpgradeFiles)
  * Ability to back up the Folder P127 uses to Upgrade your Game. 
  * Ability to apply / use a Backup of your UpgradeFiles.
  * This means if 1.52 is latest, you back up 1.52 files, 1.53 hits, you then sometime apply the 1.52 backup, clicking "Upgraded" will get you to 1.52




## To  expand on the Backup Feature (for Modding and "Saving" older but new Game Versions

* P127 comes with a bunch of files needed to downgrade. Lets shall refer to them as "DowngradeFiles"

* On first downgrade, P127 saves the files it replaces in your GTA Installation when Downgrading to back them up and use them when clicking Upgrade later. We shall refer to them as "UpgradeFiles".

* Inside P127 settings (under General P127 Settings) you can create a backup and use / apply the backup of your UpgradeFiles.

* When upgrading and downgrading, P127 compares the files it would be replacing inside your GTA Installation to the one it has backed up as "UpgradeFiles". 
If the file we would be replacing inside your GTA Installation is not the same as the one inside your DowngradeFiles and not the same as in UpgradeFiles, P127 assumes these files come from a GTA Upgrade through rockstar / steam / epic.
It will then ask you if you want to create a Backup (this will delete the previous "normal" backup of UpgradeFiles). 
and it will then replace the files inside UpgradeFiles with the new, never seen before, file inside your GTA Installation.

* This means in theory P127 can automatically detect and handle game updates by steam / rockstar / epic and clicking "Upgrade" will always upgrade you back to the latest version.

* Now this is where rightclick comes into play. You can RIGHTCLICK the "create backup" and "use backup" Buttons inside P127 settings to give a backup (of UpgradeFiles) a specific name (and load / apply / use a specific backup).

* Example:
  * 1.53 is latest version and mods work. RIGHTCLICK "create backup", name it "working 1.53". 
  * Game update (lets say 1.54) will hit, this replaces the files inside UpgradeFiles and Upgrading through P127 will always bring you back to 1.54.
  * You can now RIGHTCLICK the "use backup" button, select the one named "working 1.53" and upgrading through P127 should get you back on 1.53.