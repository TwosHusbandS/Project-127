### Changelog from 1.5.2.0 to 1.5.3.0:

#### :
* Handling Windows greatest new feature 'Controlled Folders'
* * Adding Exception to windows via Powershell, so GTA can still write savefiles.
* Stopped relying on C:\Windows because why not
* MTL not queries every 2.5 seconds instead of every 1 second, in hopes to reduce the 429 Errors
* Added new game versions to UI