### Changelog from 1.0 to 1.1:
	
#### New Features:

* Note- Overlay:
  * Overlay to display Text Files (notes for long Runs) either on top of GTA (Borderless) or on another Monitor (MultiMonitor)
  * Support for swapping out NoteFiles, and Scrolling Down and Up.
  * NoteOverlay Keybindings
    * Toggle Visibility, Next Note, Prev Note, scoll up, scroll down
  * NoteOverlay Files (which notes to display) and UI to change them
  * NoteOverlay Look Tab to change how the overlay Looks
  * Preview Window how it will look in Borderless Mode
* Jumpscript:
  * Automatically starting the Jumpscript on Game Launch
  * No installed AutoHotkey / Jumpscript File required.
  * Ability to change Hotkeys for Jumpscript
* Backup UpgradeFiles
  * You can now backup (and apply the backup) the Files P127 uses for Upgrading your game back to an Upgraded Version
  * The normal UpgradeFiles folder will always contain the latest GTA Files.
  * You can enable "slow but stable file comparison" for detecting Game Updates. Note: This effects Upgrade / Downgrade Time
  * "Use Backup" and "Create Backup" Methods which creates or applies a Backup of UpgradeFiles.
    * This means if you back up your UpgradeFiles used for 1.52, and 1.53 hits, you can then use your old Backup files to get your "upgraded" GTA back to 1.52
  * Rightclick the "Use Backup" and "Create Backup" Buttons to use / apply a backup with a specific name
* Component Manager with new Deployment System
* Launching through Original SocialClub. 
  * P127 Provides the File for automatic downgrading for you and automatically does it for you when needed.
  * Completely original Launch Process
  * No Captchas
* New way of getting Authentication Information when NOT launching through Social Club
  * Also no Captchas here.
  * "Legacy Auth" can be enabled in Settings

#### Improved Features:

* SaveFileHandler:
  * Folder Support.
  * Rightclick Support.
  * Selecting First SaveFile of a List when something happened.
  * Mouse Over of gives you full name of SaveFile or Folder
  * Refresh Loading Gif
  * Better Import / Export SaveFiles
  * Added Support for Copy & Move (in Ram) and Paste.
  * Make it load async...with loading gif
* Upgrading / Downgrading:
  * Automatically removing our added Files when upgraded (like botan.dll. This fixes Fivem crashing when Upgraded)
  * Improved Comparison Logic
  * P127 now automatically detects if a Game update (from Steam / Rockstar / Epic) hit, and makes sure to use the new Files for Upgrading
  * Throwing Popup when it detected an Update and giving User the option to Backup the files used for Upgrading

#### Improved UI / UX

* Starting P127 when P127 is already open, will bring the first Instance to Foreground
* Improved ReadMe / Information, with new Help Section and updated Credits
* Improved Spelling / Wording on some UI Text
* Throwing only one network error per Instance
* Tray Icon
  * P127 can be minimized to tray
  * Settings Default Action for Exit and Show.
  * Rightclick on Tray Icon
* Popups Startup Location is now nicer.
* "Look for Updates" Button
* "Import GTA V Settings" Button
* "Reset Everything" Button
* Consistent Margins and BorderThicknesses and "Look" for Pages
* ToolTips in a lot of cases. Same with Rightclick. Seriously. Rightclick stuff.
* Displaying your GTA Version in the top left corner
* Added "Force" Downgrade / Upgrade Method
* Using Popups and Loading Bars for every stage of big File Actions.
* Hiding Options which have no effect in UI
* Improved generating DebugFile UX
* Split Settings in 3 SubPages
* Added "Mode" and "GetBuild" Feature to be able to help Users quickly.
* Improved Command Line Interpretation

#### BugFixes:

* Fixed Auth Button Mouse over Bug
* Fixed Not launching after pressing Launch when non-auth
* Auth will no longer crash when not reachable. Well at least we check it on page load
* Auto-Start XYZ on Game Launch working dir fix (for OBS)
* Made sure Process Priority is set correctly (only once and successfully)


		
