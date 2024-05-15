### Changelog from 1.2.6.2 to 1.2.6.3:


#### New Stuff:
* Fix for MTL (Thanks for offsets Gogsi)
* Fix for crashing P127 on failed SCL while displaying Error message
* UI Changes regarding Importing Components inside the Component Manager
* Dynamic MTL Offsets (allows remote-fixing RGL MTL Changes)


### Dynamic MTL Offsets:
* MTL Auth reads RAM (process memory) of the rockstar games launcher to verify you have a valid session and own the game.
* To achieve this, you have to tell the PC where exactly in RAM you want to read data (and how much data).
* This is called "offset". Any update of Rockstar games Launcher might change where rockstar games launcher stores your session (simplified).
* This results in a different offset and P127 trying to read your game-ownership in the wrong location.
* This is what happened when the "mtl broken" spam begins. This used to require a full P127 to fix, because we needed to change the offset in our code.
* Now there is something called "Dynamic MTL Offsets". 
* P127 still comes with default (working at that time) offsets, but can also read new offsets from github and from a local file.
* This means in the future we SHOULD be able to fix MTL issues remotely by changing files in the github repo, where P127 reads it from, without having to push an entire P127 Update.
* P127 reads them from github [here](https://github.com/TwosHusbandS/Project-127/blob/master/Installer/MTLOffsets.xml)
* The local file used to store offsets is (by default) C:\Program Files (x86)\Project 1.27\MTLOffsets.xml.
* If your local file and github are NOT the same offsets (like in the event of a remote-fix after broken MTL) P127 will throw a popup asking you if you would like to overwrite your local offsets with the github offsets.
* - If the github offsets are the broken ones, and your local ones are correct, this might get annoying quick...
* - Set your local file to read-only and P127 will respect it and not offer you the github offsets anymore.
* If MTL is broken, you may find a file with working offsets in the GTA V Speedrunning discord, even before P127 is officially remote-fixed.