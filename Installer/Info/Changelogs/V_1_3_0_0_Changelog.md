### Changelog from 1.2.6.3 to 1.3.0.0:


#### New Stuff:
* Completely overhauled SocialClubLaunch which should now work for everyone. For more information see [here](https://github.com/TwosHusbandS/Project-127/blob/master/Installer/Info/SocialClubLaunch.md)
* New Savefiles (Thanks to @UnNameD for providing savefiles and to @anthonio for coordinating)

#### Changes:
* Savefilehandler
  * Performance improvements due to more efficent way of comparing Files
  * Always displaying "nice name" of SaveFiles inside GTA Directory 
  * Various small UI Improvements and better labeled Buttons.
* Improvements for upgrading / downgrading Social Club
  * General Logic improvements
  * Smart way of handleing update to component
  * Better integration into P127 Spaghetti, like upgrading Social Club for MTL Auth etc.
* Improvements when killing SocialClub or GTA processes.
  * Performance / Speed improvements
  * Smart double checking if process is actually dead
  * Automatically retrying a FileOperation, because File Handle of killed processes stay alive for a few ms
* Anonymized Log and Debugfiles
* Increased Network Timeout to 1000ms (used to be 500ms) before asking User if they want offline mode
* Restored P127 to its original aspect ratio by increasing height after increasing width of main window by 100 pixels last update
* Font Size of Hamburger Buttons increased 
* Removed @Special for taxi long load patch, since its outdated

#### Fixes:
* P127 crashes due to throwing UI Popups from non-UI Thread
* P127 crashes when downgrading/upgrading on "stuck" gta5.exe
* P127 crashes when it receives a "bad" download manager xml from github
* CommandLineArgsOverwrite Window now behaves like you would expect when it comes to saving changes
* BuildVersionTable now compares hardcoded vars and github xml and decides whats newer, instead of always trusting github
* Memleak due to WPF not automatically garbage-collecting old UI Pages
* New Import Button now actually works, Oops
* Small UI fixes here and there

