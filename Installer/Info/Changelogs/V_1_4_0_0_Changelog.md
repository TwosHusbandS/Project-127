### Changelog from 1.3.1.2 to 1.4.0.0:

#### Changes:
* Fixed Bug where P127 would close itself before fully starting the new installer (during auto update)
* StutterFix
  * New socialclub.dll files on ftp
  * StutterFix is now an option for all available downgrades.
  * Adding "-StutterFix" to default CustomCommandLine value in UI, so user knows how it should look
  * Undo "stutterfix" mode (top left corner) for users who were on that beta
* ReturningPlayerBonus now has a dedicated SCL Setting and has reworked logic
* FileOperationWrapper no longer logs 4 lines per try.
* Trying (one last time) to kill gta when user clicks on "stuck gta" button
* DownloadManager
  * Double Backslash Fix
  * Duplicates in file.paths Fix
  * Check Hashes before Re-Downloading a file (less downloads and faster updates)
  * Replaced Sync methods with async UI methods for file operations (copying..) to not freeze UI
  * generall fixes, as well as update & verify improvements