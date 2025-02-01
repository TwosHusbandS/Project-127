### Changelog from 1.4.1.0 to 1.5.0.0:

#### Changes:
* New SCL Files, different Hooking Method to fake date to bypass expiring cert
* Wine Compability Mode
* * Launches GTA in a failsafe way which works on wine, but disregards any core affinity options or core affinity stuff in custom commandline
* Support for ThirdParty NonP127 Authentication for DragonEmu Users
* P127 can now be properly started without having to rely on the P127 Launcher 
* * This means you can pin to taskbar without issues
* UI Text change of crash-fix options
* UI Fix for cut off text due to too small column width
* P127 will now remove assemblypatcher.dll and launc.dat on game upgrade.
* New BuildTableVersions (1.70) for UI
* Fixed a bug where P127 crashes after successfully updating a component in very rare circumstances
* P127 no longer freezes (stuck in a loop of killing processes) when P127 is installed inside your GTA Install Directory
* Bugfix for core affinity setting to more than 16 cores, Bugfix for setting steams core affinity.
* Bugfix for logging gta command line args and setting its priority twice
