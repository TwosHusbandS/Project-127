### Changelog from 1.1 to 1.2:
	

#### New Features:
* Note Overlay
  * General performance improvements
  * Addition of dynamic text markers
    * Allows the overlay to display certain in game stats, such as number of stuntjumps or missions completed
  * Addition of a chapter system with optional autoswitching
    * Improves note organization
	* Allows for pertinent notes to be displayed based on a specified states
  * More Information [here](Advanced%20Notefile.md)
* Easter eggs
* SCL Launch
  * Launching through downgraded social club. For more Information see the "Launching Downgraded GTA" Section [here](Help.md#launching-downgraded-gta-additional-information-and-authentication)
    * Social Club is automatically Downgraded / Upgraded
	* Removes the need for P127 to have your Auth information (through MTL or LegacyAuth)
	* Your normal GTA V Installation Settings / Savefiles etc.
	* Automatically detecting if offline auth token is expired.
* Added Reset and Uninstall as command line arguments (-reset and -uninstall).
* Automatic Detection (and Correction) if Rockstar changed our Files in a specific way. This fixes "Downgraded (1.52)"
* Option to enable / disable the loading of ScriptHook on Downgraded GTA

#### Improved Features:
* MTL Auth for Emu improvements. Should be working for everyone. For more Information see the "Launching Downgraded GTA" Section [here](Help.md#launching-downgraded-gta-additional-information-and-authentication)
* Improved Logic of ComponenetManager
  * What you are allowed to uninstall under circumstances
  * When and how it will be recommended to install Components
  * Fixed not all required Components installing automatically
  * Checking if Files are really there, and not just trusting the IsInstalled Information.
* Jumpscript
  * Added more Keys.
  * Added Option to import your own AHK File P127 will save and start (instead of the automatic jumpscript)
* Two different "Repair GTA" Methods. Also saving the name of every file we ever modified or added to the GTA V Installation Folder
* Improved ResetEverything and Uninstall Methods.
* P127 will know recognize when Rockstar fucked our Files and offer a repair.
* Improved Debug-File


#### Improved UI / UX
* Added Option to exclude P127 File-Paths from Windows Defender.
* Improved UI Texts across the board
* Separate "Install Componenet" Popup in case it freezes at the end for 2-3 Seconds
* Various and multiple Improvements of ReadMe, Help, Features, etc.
* Pointing Users to the Discord Channel instead of my PMs.
* Added Dragons Github and Documentation on GTA V Launch Process on About Section
* Throwing Popup if User is trying to Auth but there is no need to, due to SCL
* Full re-set of whats installed on tripple rightclick on Refresh Button on Componenet Manager. 
* Ability to overwrite the Path used for Social Club Downgrading (instead of just getting it from registry)
* Social Club Downgrading now takes Registry Location into account
* ToolTip on Not Installed Componenets fixed.
* Throwing ToolTip to recommend Settings for RockstarGamesLauncher if needed.
* Improved UX in debug-help options


#### BugFixes:
* "Authentication failed, please launch through Project 1.27" fixed.
* Not Deleting our own Uninstaller anymore
* Killing all Processes (even dragons, when needed) now
* Fix P127 not being able to minimize
* Fixed P127 Crashes. So. Many. Crashes. Fixed.
* Fixed Taskbar Icon needing MouseOver to update in SOME circumstances. Windows bug is responsible for the other circumstances.
* Overlapping UI Texts, especially inside Settings
* Fixed DPI Scaling issue on some monitors in Overlay Multi Monitor Mode.
* Fixed Overlay not re-appearing in Multi Monitor Mode, depending on which Monitor it was closed on.
* WPF Window being Spammed into foreground fixed.
* GTA starting 50 times fixed.
* Fixed Jumpscript not being properly closed in some circumstances.
* Fixed "Repair GTA Installation" potentially crashing P127 if "Project_127_Files" does not exist...
* P127 no longer crashes on generating token file for emu, if token file is readonly.


#### Internal Improvements
* Download Popups now only throw one line of log. In all circumstances. Also logs filesize, time it took to download, hash if needed
* Better Methods for getting Version from Files (and corresponding Game Versions)
* Use dr490ns IPC for P127 Starting and Showing
* dr490ns DownloadManager multiple fixes (for crash on startup)
* Translation of ZIP Version to BASE_COMPONENT Version (X to 1.X)
  * Forcing correct installed Version of BASE_COMPONENT on first launch with it
* Sleeping Longer before Upgrading Social Club after game exit
  * Forcing correct installed Version of BASE_COMPONENT on Import ZIP
* Better detection if Upgrade / Downgrade Files exist
* Stopwatch Timer for StartupTime and Time it takes to generate Debug. 
* moved from Social Club Switcheroo from Play127.exe inside P127
* Overlay (and all Listeners and extras) One-Does-All-Logic Method improved.
  * Mainly no longer disposes existing Overlay just to create a new one
* Stopped WindowChangeListener from getting Garbage Collected
* Doing what i do best. Moved code around.
* Add Credential Manager Info on Debug
* Added Debug Command Line Args for often used testing things as part of general Code Improvement.
* Not spamming Users (likely) SSD with 100 MB of SC Files every SC Downgrade
* More efficent and less freezing way of getting dragons github link for SCL and ENV:PATH findings
* Improved Autoupdater of Installer (checking if URL and file exists)
* Correct Logging on Process Priority changing
* Fixed "empty" logging (LogLines) at start.
* Removed DebugPopup Option on LoadGif (for special)
* Removed old debug logstatements from public version, not needed, code runs fine. (For comparing Files)
* ComponentManager can force install all major Componenets.

