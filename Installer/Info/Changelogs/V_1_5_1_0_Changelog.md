### Changelog from 1.5.0.0 to 1.5.1.0:

#### New Feature:
* Support for 1.29
* * This includes Dragon Emu
* * And SocialClubLaunch for Steam/Rockstar
* * (As always, special for did the work)

#### Changes:
* Targeting .NET Framework 4.8 now, apparently helps with proton/wine?
* Updated all nuget packages
* * Small installer changes to accomondate new cefsharp version
* Random small fixes
* * New game version for buildversiontable
* * Removing steam_appid.txt if youre upgrading and not on steam (should have no effect since it should not ever get there)
* * No longer removing patchday4ng dlc file on debloat
