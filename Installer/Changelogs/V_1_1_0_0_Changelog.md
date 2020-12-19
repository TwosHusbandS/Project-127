### Changelog from 1.0 to 1.1:
		
* New Feature: Overlay:
  * Its own Page
  * Multi Monitor as well as Borderless Mode
  * Keybindings and Options to change them
  * NoteOverlay Files (which notes to display) and UI to change them
  * NoteOverlay Look Tab to change how the overlay Looks
  * Preview Window how it will look in Borderless Mode
  * Needed Backend like WindowChange Listener and Keyboard Hook for Hotkeys
* New Feature: Jumpscript:
  * Automatically starting the Jumpscript on Game Launch
  * Ability to change Hotkeys for Jumpscript
* Improved Feature SaveFileHandler:
  * Folder Support.
  * Rightclick Support.
  * Selecting First SaveFile of a List when something happened.
  * Mouse Over of gives you full name of SaveFile or Folder
  * Refresh Loading Gif
* Improved Feature: Upgrading Downgrading:
  * Automatically removing our added Files when upgraded (like botan.dll. This fixes Fivem crashing when Upgraded)
  * Improved Comparison Logic
  * P127 now automatically detects if a Game update (from Steam / Rockstar / Epic) hit, and makes sure to use the new Files for Upgrading
  * Throwing Popup when it detected an Update and giving User the option to Backup the files used for Upgrading
* New Feature Backup UpgradeFiles
  * P127 uses $UpgradeFiles for Upgrading your game back to Upgraded.
  * The normal UpgradeFiles folder will always contain the latest GTA Files.
  * "Use Backup" and "Create Backup" Methods which create or apply a Backup of UpgradeFiles.
    * This means if you back up your UpgradeFiles used for 1.52, and 1.53 hits, you can then use your old Backup files to get your "upgraded" GTA back to 1.52
  * Rightclick the "Use Backup" and "Create Backup" Buttons to use / apply a backup with a specific name
* Improved UX / UI:

* BugFixes:





		=> Changed Polling Rate of GTA V Running or Not from 5 to 2.5 seconds
		=> Gave Reloe the new Installer Link
		=> Fixed Spelling Mistakes
		=> Throwing only one nice Network error now, instead of all of them with exception string
		=> Removed the language selection from firstlaunch and Reset Settings
		=> Improved Popup Startup Location Code to make it look nicer
		=> Moved Code around
		=> Improved GTA V Path detection Logging
		=> Added Import GTA V Settings Button back
		=> Moved SourceCodeFiles around to make it easier to find stuff
		=> Fixed Not launching after pressing Launch when non-auth
		=> Dragon Implemented the GameOverlay (GTAOverlay)
		=> KeyboardListener (conntected to Backend)
		=> WindowChangeForegroundEventListener (connected to Backend)
		=> "Look for Updates" Button
		=> Made it generate debugfile and open explorer window of project 1.27 on rightclick of auth button
		=> Fixed Auth Mouse Over
		=> Fixed Consistent Margins and BorderThicknesses for Pages
		=> Made sure Process Priority is set correctly
		=> Implemented GTA Overlay debugmode which means the rest of that stuff is properly connected to backend
		=> Hotkeys only work when Overlay is visible
		=> Made sure GTA Overlay will turn off when not ingame
		=> Running Core stuff when launching through steam. This might not have any effect
		=> Design of UX for NoteOverlay.xaml
		=> Updated Settings with JumpScript and NoteOverlay stuff
		=> Rolling Log (fixed potential off by one very elegantly)
		=> ToolTips on all Icon-Buttons
		=> Annoucement Feature
		=> FailSafe Backup System for Upgrade Files
		=> Auth will no longer crash when not reachable. Well at least we check it on page load...
		=> Overlay for notes and UI for it
		=> Jumpscript
		=> Auto-Start XYZ on Game Launch working dir fix
		=> Downgrade/Upgrade/Repair improvements:
			- Detecting Updates automatically (checking for it on start, upgrade, downgrade), throwing one popup per P127 Launch
			- Throwing Popup with potential Fixes for non-changing InstallationState (upgraded, downgraded, unsure)
			- Not having own files in GTA V Folder when upgraded
		=> FIXED command line args internal once and for all
		=> Implemented Jumpscript via Autohotkey
		=> Integrate Title from Dragon both as in content and as in customizability
		=> Fixed Both Listeners for the hopefully final time
		=> Fixed Update Detection
		=> Commented out new SaveFileHandler Code
		=> Fixed Process Priority setting too often
		=> Added GameLaunched and GameExited methods based on the polling
		=> Fixed ForegroundChangeListener not setting on fullscreen by polling every 2.5 seconds
		=> Fix Command line args crashing it...
		=> Make "DetectUpgrade" more efficent
		=> Integrate Latest working branch. 
		=> Integrate Dragons Fixes for Rockstar Endpoint change
		=> Took care of all Listeners. Using and keeping track of Threads for it as of right now. Seems to work
		=> Split upgrading downgrading into 17 progress popups
		=> Finish Readme (Speedrun text + Reset Button + DL of big zip)
		=> ZIP Hash for big ZIP
		=> Webscraping for DDLs from anonfiles
		=> ALL Styles moved to App.xml
		=> Bring back functionality from which were forgetten in new GUI
		=> Selection after deletion (datagrid) fixxed
		=> Yoshis Info regarding Versions
		=> NoteOverlay Null Reference Fix + CPU Improvements
		=> Cache works, there are other cache files tho...argh. ~~Investigate CEF Cache~~
		=> Added Logging for AutoStart stuff
		=> Added Force Option to Downgrade / Upgrade when GTA V Path is detected to be false on Upgrade / Downgrade
		=> Removed Delay on Downgrade / Upgrade, throwing 3 separate ProgressBar popups for it.
		=> Using Portable AHK now with script written to desk
		=> Released under MIT
		=> Improved UX overall. Lots of small things.
		=> Lots of SaveFileHandler Improvments. Really shitty code, really shitty performance, but UX is great.
		=> Overlay (Borderless + MultiMonitor) done.
		=> Jumpscrip done
		=> Fixed Starting other programs with P127

	Release 1.1

		- Internal Testing Reports Bugs:
			
			=> [DONE] Automatic Update of Files detected broken (when update.rpf missing. Maybe check other file attributes instead of size? Mhm. Or different faster method to detect if files are the same
			=> [DONE] More efficent isEqual method for checking if gta update hit
			=> [DONE] popup that path is wrong and you have to force downgrade
			=> [DONE] long freeze on check if update hit...actually as efficent as can be
			=> [DONE] Using Backup broken (folder locked...Fixed when explorer closed. Kinda weird-ish)
			=> [DONE] No "new files blabla popup when upgrade_files is empty
			=> [DONE] Make settings not write enums to settins on startup. Maybe check on Settings property if its the same as current before setting?
			=> [DONE] Change Popup Text from "if Update hit" to something better
			=> [DONE] Change Popup Text from "AutostartBelow" to something better
			=> [DONE] Create Backup method
			=> [DONE] Re-Downmload ZIP Popup on Check for updates
			=> [DONE] Do actual Modes (internal, beta, master etc.) on some hidden UI shit, "default", textbox, "set new", cancel
			=> [DONE] Add "internal mode" and "buildinfo" and "buildtime" to debug info
			=> [DONE] DebugFile async task,  check if what we are overwriting isnt larger than our message, popup then
			=> [DONE] Ugly startup
			=> [DONE] Release installer for a few people to test update on 2020-12-15
			=> [DONE] Hide options when launching through socialclub (GTA V ingame name, pre order bonus, hide from steam)
			=> [DONE] Hide options (launch through social club and shit) when on epic retailer.
			=> [DONE] Add blue face guy to credits. (AntherXx)
			=> [DONE] Scroll faster
			=> [DONE] Launching retailer steam, hide from steam enabled, when upgraded, launches into rockstar launcher...argh
			=> [DONE] Reset settings is wonky UX
			=> [DONE] (Download Manager popup gonna replace that Check for update button) Button to "reset" and get $DowngradeFiles new, since Rockstar fucks us..
			=> [DONE] Better ProgressBar on CreatingBackup...
			=> [DONE] Fix grammar from dragons screen + Other Text Improvements.
			=> [DONE] Rightclick on create and use backup to give options to name it in a specific way. For mods and shit
				>> Create: Custom Control Popup (not new Window)
						Header
						Textbox name popup,
						2 buttons "Create", "Cancel"
				>> Use: Custom Control Popup (not new Window)
						Header
						Select available Backuos from Dropdown / Combobox, delete empty back ups when reading in the info
						Think of rename and exit functions...
						Buttons "Use, "Exit".
			=> [DONE] Check if rolling log works.
			=> [DONE] Less accurate but faster Method for detecting upgrades
			=> [DONE] New SafeFile Export / Import
			=> [Working for me] Launching dragon Emu through steam broken?
			=> [APPARENTLY DONE???] May not need DidUpdateHit Method...Its not called anywhere...
			=> [DONE] Save WPF MM Overlay startup location somehow...
			=> [DIRTFIXED] Overlay cant be toggled when multi monitor mode set before GTA started.
			=> [DONE] Hide WPF MM Overlay from Alt + Tab
			=> [DONE] Reset Window Location of OverlayMonitor
			=> [DONE] Hide Settings when not on Steam
			=> [DONE] Hide Settings when overlay Mode not enabled
			=> [DONE / WILL BE HANDLED IN DL MANAGER] Back up DowngradeFiles because of rockstar update
			=> [DONE] On OverlaySettingsChange call when Game started due to weird edge case, make overlay be in same state (visible or not as it was before we had to call the method)
			=> [DONE] Overlay not fixed...
			=> [DONE] Check if Backup Methods (use / create) need to call Upgrade or Downgrade after or before
			=> [DONE] Get BuildVersion Table from Github, dont supply the last . and just hope...if that makes sense?
			=> [DONE] Deployment system with modes / branches like above
				--> XML Tag for link / name to specific build.
					>> Finish Logic of getting a build from github.
					>> Add UI to have textbox with a built. Or decide on syntax for pushing builds. Maybe both?
					>> Currently on rightclick check for updates. Might throw a simple textbox.
				--> Download the build, then call Launcher.exe with command line args to swap the files out correctly, so we have the new build.
			=> Investigate Jumpscript with Logs for crapideot

			=> [FUCK THAT] More efficent compare of files
			=> [NOT CONNECTED TO ANY FILE RELATED LOGIC] Dragons stuff. Both paths, all settings
			=> Remember to not only check if alternative launch, but also check out if epic...
			=> Download Manager keeping track of componments
			=> Support for 1.24
			=> Safe File Handler path switch because of social club switch
			=> Think about integrating new lauch version
					- what files we need, how we get them, with Optional stuff
					- where do we keep social club files? How are we messing with them.
					- what do we need to do if user checks the checkmark and wants new way of launching. Etc.

		Quick and Dirty notes:
			- Clean up Code / Readme / Patchnotes
			- [DONE] Release new ZIP
			- [DONE] Binary Folder and stuff
			- [DONE] Make Launcher Built on Main Built
			- [DONE] Copy (Build event) License File to Proper directory
			- [DONE] Copy (Build event) Jumpscript Exe
			- [DONE] Make it create Folder and Savefile for new release...for SFH Demo
			- [DONE] Translate Keys... 
			- [DONE] Delete Internal File for everyone.
			- [DONE] Clean up "big three" method. Make users click no, check for size > 0 instead of file exists...
			- [DONE] Add Fullscreen mode to settings. Added other stuff to settings. Fixed settings bugs.
			- [DONE] Split settings into 3 subpages
			- [DONE] Command Line args...pass from Launcher to main executable. Check code on main executable.
			- [DONE] Add Jumpscript and Overlay to "only when downgraded". Just check in start methods of those things, check on Setting to true OfSettings what should be done based on settings
			- [DONE] Cant do Overlay "only when downgraded" since we dont tie it to game running or game window when multi monitor mode...
		
	- [DONE] Fullscreen mode for overlay
				--> [DONE] Window with just fixed height bar. Fixed Color. Offblack and white boarder and text.
				--> [DONE] Fullscreen / Multi monitor mode Checkmark on top (under enable) with tooltip
				--> [DONE] implement settings backend
				--> [DONE] Margin and Location greyed out and disabled. With popup. On Enable / Disable overlaw. Method to refresh if those are greyed out or not. Or hook on top UI..
				--> [DONE] Implement overlay stuff...thinking of if check inside constructor where to draw on top on, and call it a day.
						- Enum param on Overlay object which gets checked on changing stuff
				--> [DONE] Y and X Margin sepperate settings. 
				--> [DONE] Options scrollable there...

				--> [DONE] Debug tests POC of showing / stopping showing Overlay hooked to our WPF Window
				--> [DONE] check if we need to add Y Margin to it because of WPF Overlay "Titlebar"
				--> [DONE] Check if it works with our hidden WPF Window...
				--> [DONE] Semi-Connected to backend. With all settings correct on P127 launch, shit works.
				--> [DONE] ReWrite Looks stuff...it should update the actual overlay, but just write to settings on mouseLeftUp
				--> [DONE] make WPF WIndow size width accordingly
				--> [DONE] When we click into the monitor to the side of our WPF Window, it will get back to background, but overlay will stay
				--> [DONE] Theres this thing where you force stuff to be in foreground...that could help. (WPF.Window.Instance.TopMost)
				--> [DONE] WPF Window + Overlay gets init with correct target windows on P127 launch.
				--> [DONE] WPF Window Closes on Hotkey globally
				--> [DONE] WPF Window Close on P127 close
				--> [DONE] WPF Window opens on Hotkey
				--> [DONE] WPF Window Size changes with settings change...
				--> [DONE] Changing Settings of OverlayMode and OverlayEnable work while P127 is running and while GTA is running and non running
				--> [DONE] Display Logic on Look tab. 
				--> [DONE] Bevor messing with stuff below, check how we hide / show currently...and how to untangle that logic
						=> Maybe use OverlayMode for that?
						=> we may be referencing DebugMode of GTAOverlay for that...how can we use that.
						=> Rethink KeyboardListener. Might have to run it 24/7. Maybe already tied to debugmode? 
				--> [DONE] WPF Window Closes on Settings Disable
				--> [DONE] WPF Window openes on Settings Enable
				--> [DONE] WPF Window opens on Look Tab
				--> [DONE] WPF Window closes on Look Tab (unless it was shown before open)
				--> [DONE] Game Overlay doesnt disappear when alt tabbing and in tabbing
				--> [DONE] Make sure shit works when changing settings with GTA Running...
		- [DONE] SFH Improvements
			=> [DONE] Add Folder Support
				>> ReWrite of SaveFileHandler class with enum for File or Folder
				>> Folders as clickable items in list at the very top with a "[FolderName]
				>> Top Folder being "[..]" like in WinRar
				>> Connect to Backend in terms of rename, move around, etc.
			=> [DONE] Add Support for Copy & Move (in Ram) and Paste.
				>> Backend Properites
				>> Taking care of when we show the contextmenus..
				>> Copy / Cut Methods
				>> Pasting Methods
			=> [DONE] Make it load async...with loading gif
			=> [SCRATCHED] Search Bar
			=> [DONE] Rename left file doesnt update text in brackets on right side...
			=> [DONE] MouseOver displays full fillename
			=> [DONE] Make selected File act like NoteOverlay_NoteFiles
			=> [DONE] Better Hotkey support
			=> [SCRATCHED] Multiselect
			=> [DONE] RightClick on File (Copy, Rename, Delete)
			=> [DONE] RightClick on Files (Copy, Delete, Delete)
			=> [DONE] RightClick on Background (new Folder, Paste)
			=> [DONE] RightClick on Folder
			=> [SCRATCHED] Horizontal Scroll bar
			
		
		- Update ReadMe, to reflect that its not being actively developed and read through it in general. 
			=> Add credits to new people (hosting, version info, legends of Community)
		- NameSpace clean up...
		- Code clean up
		- Code documentation
		- Comments clean up
		- Add Logging