# Welcome to Project - 1.27

Hi! This is a custom Client which is capable of Downgrading and Upgrading the PC Version of GTA-V, and provides a few other Features which help the GTA V Speedrunning Community has Use-Cases for.

The Client is also able to launch the Downgraded GTA V while authenticating through Rockstar Services, made possible by the hard work of @dr490n.

List of Main Features:

* Upgrading / Downgrading GTAV (both via File-Copying and via Hardlinking)
* Launching Upgraded and Downgraded GTAV
* SaveFileHandler for special GTA V Speedrun Categories as well as a 100% Savefile for Practicing
* Enable / Disable the 500k PreOrder Bonus as you please
* Automatically doing things on Game Launch:
	* Setting gta5.exe Process Priority to "high"
	* Starting GTA V with the affinity process core fix
	* Start LiveSplit
	* Start OBS / Other Stream Programs
	* Start FPS Limiter
	* Start JumpScript (plus editing your keybindings)
	* Start Nohboard (if wanted, with Burhacs Setup)

Not all Features are fully implemented at this point. If you can think of any other Features or things this Program could do, please do not hesitate to contact me.

---

### User Instructions:

* Get your GTA V Installation to an Up-To-Date State and launch the latest Version online to confirm that its working.
* Download the latest Installer from the [Installer Folder](https://github.com/TwosHusbandS/Project-127/tree/master/Installer)
* This Program does automatically detect the current State of the Installation (Downgraded or Upgraded) and launches the Game accordingly.
* Windows 10 Checks all Files for Viruses if they are run for the first time. If you open a file (the Installer or the Program) for the first time, please give it some time (up to 15 seconds) to do so, and just wait.
* This Program only supports 64 Bit at this Point and probably will never support 32 Bit. Seeing as GTA V only supports 64 Bit (AFAIK), and I doubt you can have a good time playing GTA V on less than 4 GB of RAM, this will probably stay this way.
* This Program also requires Admin-Rights for File Operations and Accessing the Registry for Settings. You do not need to start it as Admin, you will get the UserAccessControl Popup regardless of how this was started.
* Please actually Read the Popups the Program gives you
* In order to fully remove this Program and all of its files and settings, you need to run the [Cleanup.exe](https://github.com/TwosHusbandS/Project-127/raw/master/Installer/Cleanup.exe) from the same folder, after uninstalling the Program via Control Panel
* [Changelogs can be found here](https://github.com/TwosHusbandS/Project-127/tree/master/Installer/Changelogs)
* Rightclick the Authentification Button (Lock Symbol, Top Left Corner) to generate a DebugFile with some informations.
* If something is not working and you are contact me, I would appreciate the LogFile and the DebugFile, which are both in the InstallationFolder of this Program
* **If a Steam Upgrade Hits, make sure you are fully UPGRADED, and click "Repair" in the Client.**

---

### Beta Instructions:

The Program **currently** does not get the latest Files for Downgrading, Launching, SaveFiles etc. automatically for you. It already has the technical capabilities, but there are reasons we are distributing those files manually at this Point in the Beta. If the Program asks you to Download a ZIP File, click "No". 

Get the latest ZIP Files from the Pinned Messages in the Beta Channel of the GTAV Speedrunning Discord. Download it (click the long link, not the thumbnail), save it somewhere. Open the Project 127, click "Import ZIP" and select the ZIP File you just downloaded.

---

### Advanced User Instructions:

* Settings are in: Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Project_127 in the Registry
* The Files this Program needs (apart from the Client itself) are Deployed with a ZIP File which gets extracted.
* They are saved in a folder called "Project_127_Files" inside the Folder which you selected to use for ZIP File Extracting.
*  You should use a Folder on the same Partition and Physical Drive as your GTA V Installation Folder. **I recommend selecting the GTAV Installation Path as your Location for ZIP File Extraction**
* You can chose if you want to use hardlinking (creating a kind of shortcut to another file) or copy / pasting Files for Downgrading. I recommend using Copying if your ZIP File Path is not on the same drive as your GTA V Installation Path

---

### Programer Instructions:

* This was mainly developed using C# and WPF.
* Check out the Main Documentation on the very top of the MainWindow.xaml.cs File.
* Some of the Code (especially XAML / GUI related) is not the best looking.
* If you can think of Improvements or new Features feel free to make a Pull Request of contact me on Discord. 
* Can always use an extra pair of eyes to make sure I dont do anything stupid.
* Do whatever the F you want with this Code, as long as you dont straight up charge money for this exact client, we gucci

---


This was made possibly for the GTA V Speedrunning community through almost a month of hard work by a number of dedicated individuals. This utility serves to not only allow downgrade of the existing GTA V patch, but subsequent patches as well down to the widely accepted speedrunning version of 1.27. This would not have been possible without the hard work of a number of very talented individuals from all walks of life who have contributed skills in Reverse Engineering, Programming, Decryption, Project Management, Scripting and Testing. Below is a list of some of the main contributors to the project although our thanks go out to everyone who has helped throughout the process. dr490n, Special For, thS, zCri, hossel, JakeMiester, MOMO, Daniel Kinau, Antibones, Aperture, Diamondo25

----------

Contact me for anything related to this Client on Discord. @ths#0305



