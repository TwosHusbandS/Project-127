### Information for Advanced Users:

These Infos do NOT contain advanced-user-use-cases which are on a normal Leftclick on a normal Buttons. Read through the damn Settings.
	
#### General Client:
* If you see any new Texts about "Mode", you can rightclick those, go get into that menu.
  * Speaking about that. There are 2-3 Additional (tripple) rightclicks, not documented here. 
  * Even advanced Users do not know what these are, and they only have an effect if there is something server side.
* Rightclick "Launch GTA V" Button will close it. 
* Middle Mouse Button HamburgerMenu (very top left) will always Launch GTA
* Default StartWay in Settings
* Default ExitWay in Settings (works)
* Rightclick Exit Button (top right)
* Rightclick Tray Icon
* Click / Double click Taskbar Icon as well as Tray Icon.
* If you open P127 and there already is a running instance (in normal Window, Minimized, or Hidden in tray), it will bring up the existing instance instead of starting a new one

#### Command Line Options:
* "-reset" -> Resets P127 completely.
* "-uninstall" -> Fully uninstall P127 (including downloaded files and settings)
* "-useemudebugfile" -> To be used when stuff like PreOrderBonus and InGameName is not working on Dragon Launcher.
* "-background CustomBackgroundName" -> Available Custom Backups: "Default", "FourTwenty", "Winter", "Spooky", "Valentine", "Murica", "Germania", "Cat"
* There are more, but these are staying internal. If you really want them and need them, you probably know where to look...

#### SaveFileHandler:
* Keyboard support (arrow keys, delete, F2, F5, stuff like that)
* Rightclick support + Mouse Over support
* Importing can either be a single savefile or a ZIP of savefiles, which will be extracted.
* Mouse Over Titles of both Sections or a SaveFile to have its Full Path / Name visible on a ToolTip / MouseOver
* The button at the top switches the GTA V SaveFile Location from Dragon Launcher to SocialClubLaunch and back and forth. It will load the appropiate one based on Settings when opening the SaveFileHandler.
* You can Rightclick said Button to load a custom Folder.
* You can Rightclick the Titles of both sections for extra options.

#### NoteOverlay:
* Rightclick a loaded Note to open it on Notepad.
* Rightclick the "MultiMonitorCheckbox" when Multi Monitor Overlay is active to reset its Location (in case its not visible due to removed Monitors).
* Stuff for advanced NoteOverlay Usage can be found [here](AdvancedNotefile.md)

#### ComponentManager:
* Tripple rightclick the text of a Componenet to mark is as installed in a version of your choosing. Use with caution. Further down it may check if the files are actually on disk.
* Tripple rightclick the Refresh Button to reset the Information of what Components are installed in which Version. You will have to re-download Components. Easiest way to do that tho.
* Rightclick the (Re-Install) Button to verify local files.
* Rightclick Uninstall Button to enter a DDL to a ZIP which will get extracted at the appropiate folder. Note: "RequiredFiles" ZIP will need to follow normal P127_ZIP naming / folder convention.

#### Settings:
* Settings in general
  * You can Mouse-Over Path-Buttons to get the Full Path in case it cuts off. 
  * You can RightClick Path-Buttons to open the Path in Windows Explorer.
* General P127 Settings
  * Rightclick the "Create Backup" as well as "Use Backup" Buttons. Explanation is further down.
  * Importing a zip you find floating around (named: "Project_127_Files_VX_Big.zip") could help. Especially if you read this in years from now (January 2021).
    * Do so by either Downloading it, and leftclicking "Import ZIP", or rightclicking "Import ZIP" to paste a DIRECT download link to a zip File.
* GTA & Launch Settings
  * Rightclick the hidden Submenu (of the other LaunchMethod) still edit those, even tho they are not in effect right now.
  * Rightclick "GTA & Launch Settings" to change your GTA 5 Window title to "Stealy Wheely Automobiley 5". This may break custom Jumpscripts and Stream setups.
* Extra Features
  * The Right-Clicking "Enable Multi Monitor Mode" checkbox of the Overlay from above, also applies here.
  * Leftclick "Additional Jumpscript Options" to import and have P127 start a custom Jumpscript instead of its own Jumpscript.

#### Advanced "Create Backup" and "Use Backup"

To expand on the Backup Feature (for Modding and "Saving" older but new Game Versions): 

* P127 comes with a bunch of files needed to downgrade. Lets refer to them as "DowngradeFiles"
* On first downgrade, P127 saves the files it replaces in your GTA Installation when Downgrading to back them up and use them when clicking Upgrade later. We shall refer to them as "UpgradeFiles".
* Inside P127 settings (under General P127 Settings) you can create a backup and use / apply the backup of your UpgradeFiles.
* When upgrading and downgrading, P127 compares the files it would be replacing inside your GTA Installation to the one it has backed up as "UpgradeFiles". If the file we would be replacing inside your GTA Installation is not the same as the one inside your DowngradeFiles and not the same as in UpgradeFiles, P127 assumes these files come from a GTA Upgrade through rockstar / steam / epic. It will then ask you if you want to create a Backup (this will delete the previous "normal" backup of UpgradeFiles). and it will then replace the files inside UpgradeFiles with the new, never seen before, file inside your GTA Installation.
* This means P127 can automatically detect and handle game updates by steam / rockstar / epic and clicking "Upgrade" will always upgrade you back to the latest version.
* Now this is where rightclick comes into play. You can RIGHTCLICK the "create backup" and "use backup" Buttons inside P127 settings to give a backup (of UpgradeFiles) a specific name (and load / apply / use a specific backup).
* Example:
  * 1.53 is latest version and mods work. RIGHTCLICK "create backup", name it "working 1.53".
  * Game update (lets say 1.54) will hit, this replaces the files inside UpgradeFiles and Upgrading through P127 will always bring you back to 1.54.
  * You can now RIGHTCLICK the "use backup" button, select the one named "working 1.53" and upgrading through P127 should get you back on 1.53.
  * Should be useful mainly for mods. 
  * If you are a content creater and NEED this to work, id recommend making a backup the manual way. As always, I / We are not responsible in any way for anything.


