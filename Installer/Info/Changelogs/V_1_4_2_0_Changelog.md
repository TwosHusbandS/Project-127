### Changelog from 1.4.1.0 to 1.4.2.0:

#### Changes:
* Fixed a bug where P127 crashes after successfully updating a component in very rare circumstances
* P127 no longer freezes (stuck in a loop of killing processes) when P127 is installed inside your GTA Install Directory
* New BuildTableVersions (1.70) for UI
* UI Text change of crash-fix options
* Bugfix for p127 binary crashing immediately unless executed as admin (admin relauncher fixed)
* Bugfix for core affinity setting to more than 16 cores, Bugfix for setting steams core affinity.
* Wine Compability Mode
* * Launches GTA in a failsafe way which works on wine, but disregards any core affinity options or custom commandline
