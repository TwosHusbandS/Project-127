
## Get me Started

* Get your GTA in a current Up-To-Date State by Repairing / Verifying Game Files via your choosen Retailer (Steam, Rockstar, Epic)
* Launch your GTA to make sure its working and in an Upgraded State.
* Download the [latest Installer](https://github.com/TwosHusbandS/Project-127/raw/master/Installer/Project_127_Installer_Latest.exe)
* Execute it. Windows will check it for Viruses. This will take 5-10 Seconds.
* Let Project 1.27 Install.
* Open Project 1.27
* Go through Initial Setup.
* I recommend Reading through all the Settings right now, and setting P127 up to your Liking


## Launching Downgraded GTA Additional Information and Authentication

* You have two Options of launching the game.
  * Dr490n Launcher (default / standard)
    * For this you only need the "Required Files"
	* For this you will need "Auth"
  * Launching through downgraded Socialclub (SCL).
    * For this you will NOT need "Auth"
	* Removes the need for P127 to have your Auth information (through MTL or LegacyAuth). NO CAPTCHAS
    * Social Club is automatically Downgraded / Upgraded
	* Your normal GTA V Installation Settings / Savefiles etc.
    * Only available if you own GTA through Steam or Rockstar. Sorry Epic.
    * For this you need the Downgraded Social Club Files, and the SCL - GameFiles for the Game Version (1.24 or 1.27) and for your Retailer (Steam or Rockstar)
	  * They will be downloaded when needed changing Settings, so you dont need to manually Download them.


Paragraph below is **NON EFFECTIVE WHEN LAUNCHING THROUGH SOCIAL CLUB**

When you are launching through the **Dr490n Launcher**, P127 needs to **verify** that you own GTA through Rockstars Server (for legal Reasons).

It can do this in two ways:
* So called **MTL Auth**. This opens Rockstar Laucher, you sign in in the Rockstar Launcher, P127 gets your Ownership Information via that.
  * NO CAPTCHAS THIS WAY.
* So called **Legacy Auth**. (Can be enabled in P127 Settings). You log into your social Club account inside P127.
  * YOU WILL GET CAPTCHAS, AND YOU WILL HAVE TO LOG IN EVERY TIME YOU OPEN P127

## Help / Common Issues

* When Project 1.27 crashes when Downloading or Importing Files, try to download the ZIP manually from the Help Section of P127 then go to Settings -> Import ZIP Manually and select the file you just downloaded. If that doesnt work, rightclick the ZIP Extraction Path in Settings, copy your downloaded zip file there, right click -> extract here.

* When Launching GTA V does not launch the Version it says it is (Text in Top Left Corner), make sure the Path to GTA V is set correctly in the settings of Project 1.27.

* When Upgrading / Downgrading does not work as expected in general, clicking \"Repair GTA\" inside P127 Generel Settings and re-install the components.

* Re-Installing or manually deleting and Installing a Component, as well as re-doing an Upgrade / Downgrade is always worth a try.

* Game Updates from Rockstar might break things. This usually shows itself as P127 saying: "Downgraded (1.52)", which makes no sense. Re-Installing the Components inside the ComponentManager you are using for Downgrading and Re-Applying a Downgrade should fix this. Steam / Epic is not affected by this. P127 should automatically detect and handle this.

* If GTA (mainly Downgraded) does not launch AT ALL, UNCHECK "Overwrite GTA CommandLineArgs" inside "Additional GTA Commandline Options" inside "GTA & Launch Settings".

* If one Lauch - Method (for downgraded GTA) is not working for you, it is worth trying to switch the way P127 Launches the downgraded Game, by doing so inside "GTA & Launch Settings"

* If the current Authentication Method for the dr490n emu is not working, it is worth trying to switch the way P127 tries to authenticate your GTA ownership by doing so inside the Orange Border inside "GTA & Launch Settings".

* If the Dr490n Launcher doesnt seem to take some options (InGameName, PreOrderBonus) into account, try launching P127 with the command line args: "-useemudebugfile true"

* On Legacy Auth: When the Auth / Login appears to load infinitely, re-start the auth-process by hitting the "x" inside the top right corner once, and then trying again. If that doesnt work, you can try to re-start Project 1.27, and wait a few  minutes. If its still not working, Rockstar just might not like your IP. In this case try using a Hotspot from your phone or a VPN or any other internet connection.

* On Legacy Auth: When P127 crashes just when you are expected to login (on click of Auth Button, or on Game Launch when not logged in already), you might fail to connect to Rockstar Server. Make sure you are connected to the internet.

* On MTL Auth, if Rockstar Games Launcher opens and immediately tries to update your Game, you need to disable that option. Inside Rockstar Games Launcher, head into Settings -> My Installed Games -> Grand Theft Auto V -> and uncheck the "Enable automatic updates" box at the very top.

* You can reset the information which Components are installed and in which Version by tripple right-clicking the "Refresh" Button inside the ComponenetManager.

* If you cant see the MultiMonitor Overlay, RIGHT-click the \"Enable Multi Monitor Mode\" Checkbox to Reset its Position
			
* If launching through Social Club does not work, and you believe the Social Club Downgrade to be the cause of this, you can try the following: Head into P127 Settings, inside "GTA & Launch Settings", enable "Force SocialClub Installation Path C:"

* For external HDDs and NAS / SAN / Network Storage I recommend having "Slow but stable Method for Upgrading / Downgrading" checked.

* If something is still not working, you can always try hitting the "RESET ALL" Button inside P127 Settings. This might take a few minutes, and Project 1.27 will quit automatically when its done. Re - Open it and everything should work again. You will need to verifying Files via Steam / Rockstar / Epic afterwards.

## Support

* **If you still cant get it to work or you wish to get Help, please RIGHT-click the Auth icon (the one with the lock icon in the top left corner) and send me the AAA - Logfile.log and the AAA - Debugfile.txt from the folder which will open (Project 1.27 Installation Directory) and include a detailed Report of what you did and whats not working. Note: These Files may contain Filepaths which may contain your Name.**

* Please post a detailed description of what you did and what is not working as expected, in the GTA V Speedrun Discord - tech-support Channel. Invite links [here](https://discord.gg/3qjGGBM), and [here](https://discord.gg/rRrTGUV), as well as [here](https://discord.com/invite/zQt8wZg)

* I do not recommend uploading these 2 Files inside a public channel. They do contain things like your email adresses, and Path / Folder - Names which can contain usernames.

* I hope everything works for you and you dont experience any crashes or anything like that.

* I hope whoever reads this has a great day : )
