<!--
Shamelessly stolen from: https://github.com/othneildrew/Best-README-Template
-->


<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->





[![Discord][discord-shield]][discord-url]
[![Twitter][twitter-shield]][twitter-url]
[![MIT License][license-shield]][license-url]
[![Maintained][maintained-shield]][maintained-url]
[![Installer][version-shield]][installer-latest-url]
[![Help][help-shield]][help-url]
[![Features][features-shield]][features-url]



<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/TwosHusbandS/Project-127">
    <img src="Artwork/icon.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Project 1.27 aka. P127</h3>

  <p align="center">
    Custom Client / Launcher for Speedrunning GTA.
    <br />
    <a href="https://www.youtube.com/watch?v=dQw4w9WgXcQ&t=PLACEHOLDER">View Demo</a>
	.
    <a href="#contact">Contact me</a>
	.
	  <a href="Installer/Info/Changelogs/V_1_5_3_0_Changelog.md">Changelog</a>
  </p>
</p>



<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About Project 1.27</a>
      <ul>
        <li><a href="#main-features">Main Features</a></li>
        <li><a href="#Help">Help and how to get Started</a></li>
        <li><a href="#built-with">Built With</a></li>
        <li><a href="#installation">Installation</a></li>
        <li><a href="#user-instructions">User Instructions</a></li>
        <li><a href="#advanced-user-instructions">Advanced User Instructions</a></li>
      </ul>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#shoutouts-and-credits">Shoutouts and Credits</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>

-----

<!-- ABOUT THE PROJECT -->
## About The Project

Hi! This is a custom Client which was originally created to give all Speedrunners the capability of launching the Downgraded GTA Version 1.27. 

Over time it grew with Features and Launch - Capabilities

Note:

Project 1.27 now has the stability, user experience and core features we envisioned at the start of this project. While @dr490n, @Special For and me (@thS / TwosHusbandS) both have ideas for potentially improvements and we may get around to working on them at some point, Project 1.27 is not under active development as of this very second (November 2024). Big bugfixes etc. will still happen if needed, but we are making no promises either way. We want to thank everyone who has been a part of this incredible journey.

### Help, How to get started, Support & Contact

* [Help, How to get started, Support & Contact][help-url]


### Main Features

* [List of Main Features here][features-url]


### Built With

Pretty much built with straight C# visualized with WPF
* Installer built using [InnoSetup](https://jrsoftware.org/isinfo.php)
* Some componments, mostly in regards to the Game Launch are closed source and written in C / C++
* The logging into your Social Club account is achieved by:
  * JavaScript
  * [Chrome Embedded Framework](https://github.com/cefsharp/CefSharp)


### Installation

* Grab the [latest Installer][installer-latest-url] from [the Installer folder][installer-folder-url]
* Execute said Installer
* Open Project 127
* Legacy installers can be found in the [legacy-installers branch](https://github.com/TwosHusbandS/Project-127/tree/legacy_installers/Installer).

### User Instructions:

* **Get your GTA V Installation to an Up-To-Date State and launch the latest Version online to confirm that its working.**
* Grab the [latest Installer][installer-latest-url] from [the Installer folder][installer-folder-url]
* This Program does automatically detect the current State of the Installation (Downgraded or Upgraded) and launches the Game accordingly.
* Windows 10 Checks all Files for Viruses if they are run for the first time. If you open a file (the Installer or the Program) for the first time, please give it some time (up to 15 seconds) to do so, and just wait.
* This Program only supports 64 Bit at this Point and probably will never support 32 Bit. Seeing as GTA V only supports 64 Bit (AFAIK), and I doubt you can have a good time playing GTA V on less than 4 GB of RAM, this will probably stay this way.
* This Program also requires Admin-Rights for File Operations and Accessing the Registry for Settings. You do not need to start it as Admin, you will get the UserAccessControl Popup regardless of how this was started.
* Please actually Read the Popups the Program gives you
* In order to fully remove this Program and all of its files and settings click the Uninstall Button inside the Settings Window.
* [Changelogs can be found here][changelogs-url]
* If something is not working and you are contact me, I would appreciate the LogFile and the DebugFile, which are both in the InstallationFolder of this Program
* Read the Help Section if something is not working.

-----

### Advanced User Instructions:

* Settings are in: Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Project_127 in the Registry
* The Files this Program needs (apart from the Client itself) are saved in a folder called "Project_127_Files" inside the Folder which you selected to use for ZIP File Extracting.
  * By default it recommends you to use your GTA Installation for that.
  * Again: Use your GTA Installation Folder for this.
* Theres a lot of stuff you can rightclick. I mean. A lot.
* A lot of "advanced user" stuff is explained [here](https://github.com/TwosHusbandS/Project-127/blob/master/Installer/Info/AdvancedUser.md)
* There are also [explanations for the Note Overlay](https://github.com/TwosHusbandS/Project-127/blob/master/Installer/Info/AdvancedNotefile.md) as well as an [Advanced Patcher Guide](https://github.com/TwosHusbandS/Project-127/blob/master/Installer/Info/AdvancedPatcher.md)

-----


## Roadmap

Project 1.27 is not being actively developed at this stage.

It will continue to receive BugFixes and Hotfixes if needed. 

-----

## Contributing

To get a local copy up and running follow these simple example steps.

* Clone the Github Repo.
* Pray Nuget does its magic.
* Press F5.
* In Order to check how stuff is connected id just recommend following User Actions and seeing what Methods are called.
* The whole Project ...historically grew a lot, and it shows.
* Some of the Code (especially XAML / GUI related) is not the best looking and far from best practice.
* If you can think of Improvements or new Features feel free to make a Pull Request or [contact me](#contact). 
* Can always use an extra pair of eyes to make sure I dont do anything stupid.
* **You need [Github LFS (Large File Storage](https://git-lfs.com/)** due to me being lazy and having installer binaries in code.

-----

## License

Distributed under the MIT License. See `LICENSE` for more information.

-----

## Shoutouts and Credits

* Project 1.27 Launcher / Client Programming, Documentating: @thS
* Reverse Engineering: @dr490n, @Special For, @zCri, @Gogsi
* Launcher GUI Design & Artwork: @hossel
* Special thanks to: @JakeMiester, @Antibones, @Aperture, @MOMO
* GOGSI THE GOAT. Very special shoutouts to @Gogsi not only for being awesome and a pleasure to work with but also for continuing to reverse-engineer, help with information, provide fixes and PRs. Very much appreciated.
* Special shoutouts to @dr490n, who was responsible for getting the downgraded game to launch, adding patches against in-game triggers, writing the Overlay Backend, writing the Download Manager Backend, Writing the Launch-Through-Socialclub launch process, Authentication backend, decryption and managed to get the preorder entitlement to work.
* Special shoutouts to @special for, for being there during the whole development phase, providing valuable insight, being available to bounce ideas off of, being available for brainstorming, and providing valuable help in regards to reverse engineering the GTA V Launch Process. We could not have done this without you.
* Also thanks to @special for, for implementing the ability to downgrade GTA to Version 1.24 via the DragonEmu into P127 by figuring out how P127 GUI and backend logic works and how my component manager and dr490ns download manager handles things.
* Also thanks to @special for, for reverse-engineering and implementing StutterFix, AudioFix, CrashFix for both DragonEmu and SocialClubLaunch, on 1.24 and 1.27 and all retailers.
* Shoutout to FiveM and Goldberg, whose Source Code proved to be vital to understand and reverse engineer the GTA V Launch Process
* Shoutout to @Fro for providing Hosting to the Files needed for Project 1.27. Youre an absolute legend
* Shoutout to @yoshi for providing the Information which Build Version corresponds with which Game Version
* Shoutout to @Gogsi for being awesome and a pleasure to work with
* Shoutout to @Diamondo25 for finding a way to launch GTA without going through LaunchGTAV.exe or PlayGTAV.exe, which eased the development process to launch through Social Club
* Shoutout to @AntherXx for ~~making the incredible P127 Demo + Help Video, as well as~~ going through the trouble of tracking down and renaming SaveFiles for every single Mission in Classic% and to @Hossel for providing the SaveFiles for the main Categories          
* Shoutout to @DaWolf85 for [open sourcing his GTAVAutoPatcher Tool](https://github.com/DaWolf85/GTAVAutoPatcher/) the Community used to Upgrade / Downgrade previously.It helped us a ton.
* Shoutout to @Hoxi and @Special For for implementing and integrating the ReturningPlayerBonus toggle on dr490n emu.
* Shoutout to @cuzbabytonight on community.pcgamingwiki.com (and hoxi I think) for figuring out which DLC Folders are safe to delete when on 1.27.
* Shoutout to @Rayope for finding a Bug, fixing it himself, and providing us with the solution. Thanks mate. (P127 crashing on MTL Token Gen when losing internet after P127 Startup)
* Shoutout to @burhac, @Crapideot, @GearsOfW, @rollschuh2282, @Ollie, @Alfie, and the aforementioned people for being awesome members of the GTA Speedrunning community, always being nice and respectful, and providing Help / Testing. You guys are much appreciated.

-----

## Contact

Twitter - [@thsBizz][twitter-url]

Project Link - [github.com/TwosHusbandS/Project-127][p127-url]

Discord - [@ths_was_taken][discord-url]


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[discord-url]: https://discordapp.com/users/612259615291342861
[twitter-url]: https://twitter.com/thSbizz
[features-url]: https://github.com/TwosHusbandS/Project-127/blob/master/Installer/Info/Features.md
[help-url]: https://github.com/TwosHusbandS/Project-127/blob/master/Installer/Info/Help.md
[p127-url]: https://github.com/TwosHusbandS/Project-127/
[twitter-shield]: https://img.shields.io/badge/Twitter-@thSbizz-1DA1F2?style=plastic&logo=Twitter
[discord-shield]: https://img.shields.io/badge/Discord-@ths__was__taken-7289DA?style=plastic&logo=Discord
[changelogs-url]: https://github.com/TwosHusbandS/Project-127/tree/master/Installer/Info/Changelogs
[installer-folder-url]: https://github.com/TwosHusbandS/Project-127/tree/master/Installer
[installer-latest-url]: https://github.com/TwosHusbandS/Project-127/raw/master/Installer/Project_127_Installer_Latest.exe
[license-shield]: https://img.shields.io/badge/License-MIT-4DC71F?style=plastic
[license-url]: https://github.com/TwosHusbandS/Project-127/blob/master/LICENSE
[maintained-shield]: https://img.shields.io/badge/Maintained-No-FFDB3A?style=plastic
[maintained-url]: #about-the-project
[version-shield]: https://img.shields.io/badge/Version-1.5.3.0_Installer-4DC71F?style=plastic
[help-shield]: https://img.shields.io/badge/Help-Here-F48041?style=plastic
[features-shield]: https://img.shields.io/badge/Features-Click_Me-802BCF?style=plastic






