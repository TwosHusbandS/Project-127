### Changelog from 1.4.1.0 to 1.5.0.0:

#### Changes:
* UI Text change of crash-fix options
* UI Fix for cut off text due to too small column width
* Wine Compability Mode
* * Launches GTA in a failsafe way which works on wine, but disregards any core affinity options or core affinity stuff in custom commandline
* New SCL Files, different Hooking Method to fake date to bypass expiring cert
* Support for ThirdParty NonP127 Authentication for DragonEmu Users
* P127 will now remove assemblypatcher.dll and launc.dat on game upgrade.
* New BuildTableVersions (1.70) for UI
* Fixed a bug where P127 crashes after successfully updating a component in very rare circumstances
* P127 no longer freezes (stuck in a loop of killing processes) when P127 is installed inside your GTA Install Directory
* Bugfix for p127 binary crashing immediately unless executed as admin (admin relauncher fixed)
* Bugfix for core affinity setting to more than 16 cores, Bugfix for setting steams core affinity.
* Bugfix for logging gta command line args and setting its priority twice
