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
* Added Reset and Uninstall as command line arguments (-reset and -uninstall).
* Automatic Detection (and Correction) if Rockstar changed our Files in a specific way. This fixes "Downgraded (1.53)"

#### Improved Features:
* MTL Auth for Emu improvements. Should be working for everyone. For more Information see the "Launching Downgraded GTA" Section [here](Help.md#launching-downgraded-gta-additional-information-and-authentication)
* Improved Logic of ComponenetManager
  * What you are allowed to uninstall under circumstances
  * When and how it will be recommended to install Components
  * Fixed not all required Components installing automatically
* Jumpscript
  * Added more Keys.
  * Added Option to import your own AHK File P127 will save and start (instead of the automatic jumpscript)
* Two different "Repair GTA" Methods. Also saving the name of every file we ever modified or added to the GTA V Installation Folder
* Improved ResetEverything and Uninstall Methods.
* P127 will know recognize when Rockstar fucked our Files and offer a repair.

#### Improved UI / UX
* Improved UI Texts across the board
* Separate "Install Componenet" Popup in case it freezes at the end for 2-3 Seconds
* Improved ReadMe, Help, Features, etc.
* Pointing Users to the Discord Channel instead of my PMs.
* Added Dragons Github and Documentation on GTA V Launch Process on About Section

#### BugFixes:
* "Authentication failed, please launch through Project 1.27" fixed.
* Not Deleting our own Uninstaller anymore
* Killing all Processes (even dragons, when needed) now
* Fix P127 not being able to minimize
* Fixed P127 Crashes

#### Internal Improvements
* Download Popups now only throw one line of log. In all circumstances. Also logs filesize, time it took to download, hash if needed
* Better Methods for getting Version from Files (and corresponding Game Versions)
* Use dr490ns IPC for P127 Starting and Showing
* dr490ns DownloadManager multiple fixes (for crash on startup)
* Translation of ZIP Version to BASE_COMPONENT Version (X to 1.X)
  * Forcing correct installed Version of BASE_COMPONENT on first launch with it
  * Forcing correct installed Version of BASE_COMPONENT on Import ZIP
* Better detection if Upgrade / Downgrade Files exist
* Stopwatch Timer for StartupTime and Time it takes to generate Debug. 
* moved from Social Club Switcheroo from Play127.exe inside P127
* Overlay (and all Listeners and extras) One-Does-All-Logic Method improved.
  * Mainly no longer disposes existing Overlay just to create a new one
* Stopped WindowChangeListener from getting Garbage Collected
* Add Credential Manager Info on Debug
* Not spamming Users (likely) SSD with 100 MB of SC Files every SC Downgrade
* More efficent and less freezing way of getting dragons github link for SCL and ENV:PATH findings
* Improved Autoupdater of Installer (checking if URL and file exists)
