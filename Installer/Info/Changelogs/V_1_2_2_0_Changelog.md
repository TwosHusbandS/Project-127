### Changelog from 1.2 to 1.2.2:
	

#### New Features:
* Automatically and silently MTL Authing on Startup if wanted
* Ability to chose what MTL does once its completed.
* Better UI / UX for LaunchWays and Authways, and their respective custom Options
* Ability to give custom command line args for GTA.

#### Improved Features:
* SaveFileHandler now doesnt ask you to give a GTA SaveFile a new name when backing it up (unless File with that name already exists in Backups) (Thanks Piska)
* Ability to import ZIP from Url with rightclick
* Rightclick a loaded NoteFile to open it with Notepad
* PowerUser Instructions
* Decreased P127 Executable Size from 11 MB to 2 MB.

#### Improved UI / UX
* Improved UX on First Launch of P127. (Thanks Hoxi).
* Throwing Helpful Popup on Unsure InstallationState (Thanks Reloe / AntherX)
* Middle Mouse Button on HamburgerButton will Launch GTA. (Thanks Piska)
* Improved UX on Upgrading / Downgrading under special circumstances.
* Generally Improved a lot of Popup texts
* Added MouseOver to a lot of stuff.
* Better NoteOverlay Title support.
* Auth Button is now crossed when not needed.
* Added a bunch of random backgrounds
* Asking if User wants to switch Component Installation Location to Default Locaton when hes changing GTA V Path Installation

#### BugFixes:
* Changed when and how im killing rockstar processes. (Thanks Hoxi).
* Overlay MultiMonitorMode not forcing the position correctly
* Popups always starting in the center of screen instead of center of parent window.
* Downloading Components no longer freezes the UI.
* Automatically installing Downgrading SC when not installed without asking (if SCL Stuff is being installed already)
* Potentially fixed the Remember Me issue on Legacy Auth where it wouldnt re-enter Credentials
* Improved Efficency of SaveFileHandler
* Improved when im showing edge-case Popups.
* Finally fixed P127 Activating itself over and over again. 
* Finally fixed P127 Auth throwing multiple errors instead of one.
* Social Club Downgrade Cache and Backups path are deleted on a Reinstall or verify of Downgraded Social Club Files.
* Killing more (now all) Processes when Downgrading Social Club
* Fixed the ugly "Rockstar Games Launcher has exited unexpectedly" error Message.
* CommandLineInterpretation hopefully fixed for real this time.

#### Internal Improvements
* Better Logging
* Better DebugFile

